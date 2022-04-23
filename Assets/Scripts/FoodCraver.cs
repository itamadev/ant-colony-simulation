using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCraver : MonoBehaviour
{
    [SerializeField]
    public float maxSpeed;
    [SerializeField]
    public float steerStrength;
    public float wanderStrength;

    [SerializeField]
    LayerMask foodLayer;
    [SerializeField]
    LayerMask pheromoneLayer;
    [SerializeField]
    LayerMask foodPheromoneLayer;
    [SerializeField]
    float viewRadius;
    [SerializeField]
    float viewAngle;

    [SerializeField]
    GameObject NestPheromone;
    [SerializeField]
    GameObject FoodPheromone;


    bool hasFood;
    float time = 0.0f;
    LayerMask myLayer;
    Vector2 position;
    Vector2 velocity;
    Vector2 desiredDirection;
    Transform targetFood;
    Transform targetPheromone;
    Transform head;
    Vector2 forward;

    private void Start()
    {
        myLayer = transform.gameObject.layer;
        hasFood = false;
    }

    void Update()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        var desiredVelocity = desiredDirection * maxSpeed;
        var desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        var acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle));

        head = transform;
        forward = head.right;
        if (hasFood)
            BackHome();
        else
            HandleFood();
        LayPheromones();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Nest") && hasFood)
        {
            hasFood = false;
            head.DetachChildren();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Wall"))
            desiredDirection = -desiredDirection;
    }

    void BackHome()
    {
        if (targetPheromone == null)
        {
            Collider2D[] allPheromones = Physics2D.OverlapCircleAll(position, viewRadius, pheromoneLayer);

            if (allPheromones.Length > 0)
            {
                Transform pheromone = allPheromones[Random.Range(0, allPheromones.Length)].transform;
                Vector2 dirToPheromone = (pheromone.position - head.position).normalized;

                if (Vector2.Angle(forward, dirToPheromone) < viewAngle / 2)
                {
                    targetPheromone = pheromone;
                }
            }
        }
        else
        {
            desiredDirection = (targetPheromone.position - head.position).normalized;

            const float pheromoneRange = 0.7f;
            if (Vector2.Distance(targetPheromone.position, head.position) < pheromoneRange)
            {
                targetPheromone = null;
                desiredDirection = -desiredDirection;
            }
        }
    }

    void FoodPheromones()
    {
        if (targetPheromone == null)
        {
            Collider2D[] allFoodPheromones = Physics2D.OverlapCircleAll(position, viewRadius, foodPheromoneLayer);

            if (allFoodPheromones.Length > 0)
            {
                Transform pheromone = allFoodPheromones[Random.Range(0, allFoodPheromones.Length)].transform;
                Vector2 dirToPheromone = (pheromone.position - head.position).normalized;

                if (Vector2.Angle(forward, dirToPheromone) < viewAngle / 2)
                {
                    targetPheromone = pheromone;
                }
            }
        }
        else
        {
            desiredDirection = (targetPheromone.position - head.position).normalized;

            const float pheromoneRange = 0.7f;
            if (Vector2.Distance(targetPheromone.position, head.position) < pheromoneRange)
            {
                targetPheromone = null;
            }
        }
    }

    void HandleFood()
    {
        if (targetFood == null)
        {
            // Get all food objects within the perception radius
            Collider2D[] allFood = Physics2D.OverlapCircleAll(position, viewRadius, foodLayer);

            if (allFood.Length > 0)
            {
                // Select one of the food objects at random
                Transform food = allFood[Random.Range(0, allFood.Length)].transform;
                Vector2 dirToFood = (food.position - head.position).normalized;

                // Start targeting the food if it is within the view angle
                if (Vector2.Angle(forward, dirToFood) < viewAngle / 2)
                {
                    food.gameObject.layer = myLayer;
                    targetFood = food;
                }
            }
            else
            {
                FoodPheromones();
            }

        }
        else if (head.childCount == 0)
        {
            // Try move towards the target food
            desiredDirection = (targetFood.position - head.position).normalized;

            // Pick up the food if it is close enough
            const float foodPickupRadius = 0.5f;
            if (Vector2.Distance(targetFood.position, head.position) < foodPickupRadius)
            {
                targetFood.position = head.position;
                targetFood.parent = head;
                targetFood = null;
                hasFood = true;
                desiredDirection = -desiredDirection;
            }
        }
    }


    void LayPheromones()
    {
        time += Time.deltaTime;

        GameObject pheromone = hasFood ? FoodPheromone : NestPheromone;
        if (time >= 0.2f)
        {
            Vector3 currentPosition = head.position;
            GameObject tmpObj = Instantiate(pheromone, currentPosition, Quaternion.identity);
            time -= 0.2f;
        }
    }

}

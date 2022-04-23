using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodExplorerAnt : MonoBehaviour
{
    [SerializeField]
    public float maxSpeed = 2;
    [SerializeField]
    public float steerStrength = 2;
    public float wanderStrength = 1;

    Vector2 position;
    Vector2 velocity;
    Vector2 desiredDirection;

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
    }
}

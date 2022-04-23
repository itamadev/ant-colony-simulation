using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceFood : MonoBehaviour
{

    [SerializeField]
    GameObject food;

    private void OnMouseDown()
    {
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        GameObject tmpObj = Instantiate(food, currentPosition, Quaternion.identity);
    }
}

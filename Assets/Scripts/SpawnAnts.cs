using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnts : MonoBehaviour
{
    [SerializeField]
    GameObject ant;

    [SerializeField]
    uint amount;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(4);

        for (int i = 0; i < amount; i++)
        {
            Vector3 currentPosition = transform.position;
            GameObject tmpObj = Instantiate(ant, currentPosition, Quaternion.identity);
        }
    }
}

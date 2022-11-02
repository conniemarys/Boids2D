using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public Vector3 location;

    public float radius { get; set; }

    private void Start()
    {
        location = transform.position;

        radius = Random.Range(0.25f, 4);

        transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    //position of boid
    public float X { get; set; }
    public float Y { get; set; }
    //velocity of boid
    [SerializeField]
    public float Xvel { get; set; }
    [SerializeField]
    public float Yvel { get; set; }
    private Rigidbody2D rb;

    private void Start()
    {
        transform.position = new Vector2(Random.Range(0, 100), Random.Range(0, 100));
        Xvel = Random.Range(-10, 10);
        Yvel = Random.Range(-10, 10);

        rb = GetComponent<Rigidbody2D>();

    }

    private void FixedUpdate()
    {
        Vector2 velocity = new Vector2(Xvel, Yvel);
        var movement = rb.position + velocity * Time.fixedDeltaTime;
        //rb.position = movement;
        rb.MovePosition(movement);
        //rb.AddForce(new Vector2(Xvel, Yvel), ForceMode2D.Force);

        if (Xvel > 0 || Yvel > 0)
        {
            float angle = Mathf.Atan2(Xvel, Yvel) * Mathf.Rad2Deg;
            rb.MoveRotation(rb.rotation + angle/* * Time.fixedDeltaTime*/);
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        Xvel = 0;
        Yvel = 0;
    }

}

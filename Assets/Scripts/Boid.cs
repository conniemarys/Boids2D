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

    public float ogX;
    public float ogY;

    public float maxSpeed = 1;
    public float minSpeed = 10;

    // private Rigidbody2D rb;

    private void Start()
    {
        transform.position = new Vector2(Random.Range(0, 100), Random.Range(0, 100));
        ogX = Random.Range(-1, 1);
        ogY = Random.Range(-1, 1);

        //rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        

        Vector3 movement = new Vector3(ogX, ogY, 0) + new Vector3(Xvel, Yvel, 0);

        //rb.position = movement;

        //rb.AddForce(new Vector2(Xvel, Yvel), ForceMode2D.Force);

        if(movement.x == 0 && movement.y == 0)
        {
            movement.x = minSpeed;
            movement.y = minSpeed;
        }
        else
        {
           MoveFoward(movement);
        }

        transform.position += movement;

        if (Xvel > 0 || Yvel > 0)
        {
            float angle = Mathf.Atan2(Xvel, Yvel) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        Xvel = 0;
        Yvel = 0;

    }

    public void MoveFoward(Vector3 movement)
    {
        float speed = Mathf.Sqrt(Mathf.Pow(movement.x, 2) + Mathf.Pow(movement.y, 2));

        if (speed > maxSpeed)
        {
            movement.x = (movement.x / speed) * maxSpeed;
            movement.y = (movement.y / speed) * maxSpeed;
        }
        else if (speed < minSpeed)
        {
            movement.x = (movement.x / speed) * minSpeed;
            movement.y = (movement.y / speed) * minSpeed;
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public float SpeedX { get; set; }
    public float SpeedY { get; set; }
    public float SteeringSpeed { get; set; }

    private float minSpeed = 1;
    private float maxSpeed = 4;

    public enum Team
    {
        green,
        orange,
    }

    public Team team;

    public void FixedUpdate()
    {
        (SpeedX, SpeedY) = MinMax(SpeedX, SpeedY);

        Vector3 movementDirection = new Vector3(SpeedX, SpeedY, 0);

        transform.position += movementDirection * Time.deltaTime;

        if(movementDirection != Vector3.zero)
        {

            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        }

    }

    private (float newSpeedX, float newSpeedY) MinMax(float speedX, float speedY)
    {
        float speed = GetSpeed(speedX, speedY);

        if (speed > maxSpeed)
        {
            speedX = (speedX / speed) * maxSpeed;
            speedY = (speedY / speed) * maxSpeed;
        }
        else if (speed < minSpeed)
        {
            speedX = (speedX / speed) * minSpeed;
            speedY = (speedY / speed) * minSpeed;
        }

        return (speedX, speedY);
    }

    private float GetSpeed(float speedX, float speedY)
    {
        float speed = Mathf.Sqrt(Mathf.Pow(speedX, 2) + Mathf.Pow(speedY, 2));

        return speed;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSecondAttempt : MonoBehaviour
{
    [SerializeField]
    public BoidController boidPrefab;

    private float width = 26.625f;
    private float height = 16;

    public int spawnBoids = 100;

    private List<BoidController> boids;

    [SerializeField]
    [Header("Options")]
    private bool useFlock = true;
    [SerializeField]
    private bool useAlignment = true;
    [SerializeField]
    private bool useSeparation = true;
    [SerializeField]
    private bool useWiggle = true;

    [SerializeField]
    [Header("Separation")]
    private float separationRadius = 5f;
    [SerializeField]
    private float separationWeighting = 0.05f;

    [SerializeField]
    [Header("Alignment")]
    private float alignmentRadius = 10f;
    [SerializeField]
    private float alignmentWeighting;

    [SerializeField]
    [Header("Flock")]
    private float flockRadius = 10f;
    [SerializeField]
    private float flockWeighting;

    [SerializeField]
    [Header("Bouncing")]
    private float pad;
    [SerializeField]
    private float turn;
    [SerializeField]
    private float bounceWeighting;

    [SerializeField]
    [Header("Other")]
    private float steeringSpeed = 1000f;
    [SerializeField]
    private float totalSpeedWeighting = 1f;
    [SerializeField]
    private float maxWiggle = 1f;
    [SerializeField]
    private int wiggleFrame = 10;

    private float sepX;
    private float sepY;
    private float aliX;
    private float aliY;
    private float flockX;
    private float flockY;
    private float bounceX;
    private float bounceY;
    private float wiggleX;
    private float wiggleY;

    private int currentFrame;

    private void Start()
    {
        boids = new List<BoidController>();

        for (int i = 0; i < spawnBoids; i++)
        {
            SpawnBoid(boidPrefab.gameObject);
        }
    }

    private void FixedUpdate()
    {
       Advance();
    }

    private void Advance()
    {

        foreach(BoidController boid in boids)
        {
            if (useSeparation)
            {
                (sepX, sepY) = Separation(boid);
            }
            else
            {
                sepX = 0;
                sepY = 0;
            }

            if(useAlignment)
            {
                (aliX, aliY) = Alignment(boid);
            }
            else
            {
                aliX = 0;
                aliY = 0;
            }

            if(useFlock)
            {
                (flockX, flockY) = Flock(boid);
            }
            else
            {
                flockX = 0;
                flockY = 0;
            }

            if(useWiggle)
            {
                (wiggleX, wiggleY) = Wiggle();
            }
            else
            {
                wiggleX = 0;
                wiggleY = 0;
            }

            (bounceX, bounceY) = BounceOffWalls(boid);

            boid.SpeedX += (sepX + aliX + flockX + bounceX) / totalSpeedWeighting + wiggleX;
            boid.SpeedY += (sepY + aliY + flockY + bounceY) / totalSpeedWeighting + wiggleY;
        }
    }

    private void SpawnBoid(GameObject prefab)
    {
        var boidInstance = Instantiate(prefab);
        boidInstance.transform.localPosition += new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0);
        BoidController boidController = boidInstance.GetComponent<BoidController>();
        boidController.SpeedX = Random.Range(-2, 2);
        boidController.SpeedY = Random.Range(-2, 2);
        boidController.SteeringSpeed = steeringSpeed;
        boids.Add(boidController);
    }

    private (float sepX, float sepY) Separation(BoidController boid)
    {
        //separation vars
        Vector3 separationDirection = Vector3.zero;
        int separationCount = 0;

        foreach (BoidController otherBoid in boids)
        {
            //skip self
            if (otherBoid == boid)
                continue;

            var distance = Vector3.Distance(boid.transform.position, otherBoid.transform.position);

            //identify local neighbour
            if (distance < separationRadius)
            {
                separationDirection += otherBoid.transform.position - boid.transform.position;
                separationCount++;
            }
        }

        //calculate average
        if (separationCount > 0)
            separationDirection /= separationCount;

        //flip and normalize
        separationDirection = -separationDirection.normalized * separationWeighting;

        //apply to steering
        return (separationDirection.x, separationDirection.y);
    }

    private (float aliX, float aliY) Alignment(BoidController boid)
    {
        Vector3 alignmentDirection = Vector3.zero;
        int alignmentCount = 0;

        foreach (BoidController otherBoid in boids)
        {
            if (otherBoid == boid)
                continue;

            var distance = Vector3.Distance(boid.transform.position, otherBoid.transform.position);

            if (distance < alignmentRadius)
            {
                alignmentDirection += new Vector3(otherBoid.SpeedX, otherBoid.SpeedY, 0);
                alignmentCount++;
            }
        }

        if (alignmentCount > 0)
            alignmentDirection /= alignmentCount;

        alignmentDirection = alignmentDirection.normalized * alignmentWeighting;

        return (alignmentDirection.x, alignmentDirection.y);
    }

    private (float flockX, float flockY) Flock(BoidController boid)
    {
        Vector3 flockDirection = Vector3.zero;
        int flockCount = 0;
        foreach (BoidController otherBoid in boids)
        {
            if (otherBoid == boid)
                continue;

            var distance = Vector3.Distance(boid.transform.position, otherBoid.transform.position);

            if (distance < flockRadius)
            {
                flockDirection += otherBoid.transform.position - boid.transform.position;
                flockCount++;

            }
        }

        if (flockCount > 0)
            flockDirection /= flockCount;

        flockDirection = flockDirection.normalized * flockWeighting;

        return (flockDirection.x, flockDirection.y);
    }

    private (float bounceX, float bounceY) BounceOffWalls(BoidController boid)
    {
        Vector3 bounceDirection = Vector3.zero;

        if (boid.transform.position.x < -width + pad)
        {
            bounceDirection.x += turn;
        }
        if (boid.transform.position.x > width - pad)
        {
            bounceDirection.x -= turn;
        }
        if (boid.transform.position.y > height - pad)
        {
            bounceDirection.y -= turn;
        }
        if (boid.transform.position.y < -height + pad)
        {
            bounceDirection.y += turn;
        }

        bounceDirection = bounceDirection.normalized * bounceWeighting;

        return (bounceDirection.x, bounceDirection.y);
    }

    private (float wiggleX, float wiggleY) Wiggle()
    {
        wiggleX = 0;
        wiggleY = 0;

        if (currentFrame == wiggleFrame)
        {
            wiggleX = Random.Range(-maxWiggle, maxWiggle);
            wiggleY = Random.Range(-maxWiggle, maxWiggle);
            currentFrame = 0;
        }

        currentFrame++;
        return (wiggleX, wiggleY);
    }

}

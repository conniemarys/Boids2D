using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Field : MonoBehaviour
{
    //Contains a list of Boid objects, responsible for applying the rules to each Boid.

    //instantiated with a set of dimensions and a number of initial bois, and random boids are placed upon instantiation

    public int Width;
    public int Height;
    [SerializeField]
    public GameObject boidPrefab;
    public readonly List<Boid> boids = new List<Boid>();

    [Range(0, 60)]
    public float flockDistance;
    [Range(0, 2)]
    public float flockPower;
    [Range(0, 60)]
    public float alignDistance;
    [Range(0, 5)]
    public float alignPower;
    [Range(0, 60)]
    public float avoidDistance;
    [Range(0, 2)]
    public float avoidPower;
    [Range(0.0001f, 400)]
    public float speedModifier;
    public float pad;
    public float turn;

    public float maxSpeed;

    public void MakeField(int width = 400, int height = 400, int boidCount = 100)
    {
        (Width, Height) = (width, height);


        for (int i = 0; i < boidCount; i++)
        {
            GameObject boidObject = Instantiate(boidPrefab, transform);
            boids.Add(boidObject.GetComponent<Boid>());
        }

    }

    private void Start()
    {
        MakeField();
    }

    private void Update()
    {
        Advance();
    }


    public void Advance()
    {
        //Update void speed and direction based on rules

        foreach (var boid in boids)
        {
            (float flockXvel, float flockYvel) = Flock(boid, flockDistance, flockPower);
            (float alignXvel, float alignYvel) = Align(boid, alignDistance, alignPower);
            (float avoidXvel, float avoidYvel) = Avoid(boid, avoidDistance, avoidPower);
            (float bounceX, float bounceY) = BounceOffWalls(boid);

            List<float> values = new List<float>() { flockXvel, flockYvel, alignXvel, alignYvel, avoidXvel, avoidYvel, bounceX, bounceY };

            for(int i = 0; i < values.Count; i++)
            {
                if(values[i] > maxSpeed)
                {
                    values[i] = maxSpeed;
                }
            }

            boid.Xvel += (flockXvel + avoidXvel + alignXvel) / speedModifier + bounceX;
            boid.Yvel += (flockYvel + avoidYvel + alignYvel) / speedModifier + bounceY;
        }

    }
    
    //Rule 1: Steer Toward Center of Mass of Nearby Boids
    private (float xVel, float yVel) Flock(Boid boid, float distance, float power)
    {
        var neighbours = boids.Where(x => GetDistance(x, boid) < distance);
        float meanX = neighbours.Sum(x => x.transform.position.x) / neighbours.Count();
        float meanY = neighbours.Sum(x => x.transform.position.y) / neighbours.Count();
        float deltaCenterX = meanX - boid.transform.position.x;
        float deltaCenterY = meanY - boid.transform.position.y;
        return (deltaCenterX * power, deltaCenterY * power);

    }

    //Rule 2: Mimic Direction and Speed of Nearby Boids
    private (float xVel, float yVel) Align(Boid boid, float distance, float power)
    {
        var neighbours = boids.Where(x => GetDistance(x, boid) < distance);
        float meanXvel = neighbours.Sum(x => x.Xvel) / neighbours.Count();
        float meanYvel = neighbours.Sum(x => x.Yvel) / neighbours.Count();

        float dXvel = meanXvel - boid.Xvel;
        float dYvel = meanYvel - boid.Yvel;

        return (dXvel * power, dYvel * power);
    }

    //Rule 3: Steer away from extremely close boids
    private (float xVel, float yVel) Avoid(Boid boid, float distance, float power)
    {
        var neighbours = boids.Where(x => GetDistance(x, boid) < distance);
        (float sumClosenessX, float sumClosenessY) = (0, 0);
        foreach (var neighbour in neighbours)
        {
            float closeness = distance - GetDistance(boid, neighbour);
            sumClosenessX += (boid.transform.position.x - neighbour.transform.position.x) * closeness;
            sumClosenessY += (boid.transform.position.y - neighbour.transform.position.y) * closeness;
        }

        return (sumClosenessX * power, sumClosenessY * power);

    }

    //Rule 4:

    //Rule 5: Avoid Edges

    private (float xVel, float yVel) BounceOffWalls(Boid boid)
    {


        if (boid.transform.position.x < pad)
        {
            boid.Xvel += turn;
        }
        else if (boid.transform.position.x > Width - pad)
        {
            boid.Xvel -= turn;
        }
        else if (boid.transform.position.y < pad)
        {
            boid.Yvel += turn;
        }
        else if (boid.transform.position.y > Height - pad)
        {
            boid.Yvel -= turn;
        } 

        return (boid.Xvel, boid.Yvel);
    }

    private float GetDistance(Boid x, Boid boid)
    {
        float diffX = x.transform.position.x - boid.transform.position.x;
        float diffY = x.transform.position.y - boid.transform.position.y;

        return Mathf.Sqrt(Mathf.Pow(diffX, 2) + Mathf.Pow(diffY, 2));
    }
}

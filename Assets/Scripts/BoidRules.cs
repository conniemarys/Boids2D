using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidRules
{

    public static (float aliX, float aliY) Alignment(BoidController boid, float alignmentRadius, float alignmentWeighting)
    {
        Vector3 alignmentDirection = Vector3.zero;
        int alignmentCount = 0;

        foreach (BoidController otherBoid in FieldSecondAttempt.Boids)
        {
            if (otherBoid == boid)
                continue;
            else if (otherBoid.team != boid.team)
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

    public static (float sepX, float sepY) Separation(BoidController boid, float separationRadius, float separationWeighting)
    {
        //separation vars
        Vector3 separationDirection = Vector3.zero;
        int separationCount = 0;

        foreach (BoidController otherBoid in FieldSecondAttempt.Boids)
        {
            //skip self
            if (otherBoid == boid)
                continue;
            if (otherBoid.team != boid.team)
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

    public static (float flockX, float flockY) Flock(BoidController boid, float flockRadius, float flockWeighting)
    {
        Vector3 flockDirection = Vector3.zero;
        int flockCount = 0;
        foreach (BoidController otherBoid in FieldSecondAttempt.Boids)
        {
            if (otherBoid == boid)
                continue;
            else if (otherBoid.team != boid.team)
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

    public static (float avoidTeamX, float avoidTeamY) AvoidTeams(BoidController boid, float avoidTeamRadius, float avoidTeamWeighting)
    {
        Vector3 avoidTeamDirection = Vector3.zero;
        int otherTeamCount = 0;

        foreach (BoidController otherBoid in FieldSecondAttempt.Boids)
        {
            if (otherBoid == boid)
                continue;
            if (otherBoid.team == boid.team)
                continue;

            var distance = Vector3.Distance(boid.transform.position, otherBoid.transform.position);

            //identify local neighbour
            if (distance < avoidTeamRadius)
            {
                avoidTeamDirection += otherBoid.transform.position - boid.transform.position;
                otherTeamCount++;
            }
        }

        //calculate average
        if (otherTeamCount > 0)
            avoidTeamDirection /= otherTeamCount;

        //flip and normalize
        avoidTeamDirection = -avoidTeamDirection.normalized * avoidTeamWeighting;

        //apply to steering
        return (avoidTeamDirection.x, avoidTeamDirection.y);

    }

    public static (float obstacleX, float obstacleY) AvoidObstacles(BoidController boid, List<Obstacle> obstacles, float avoidObstaclesWeighting)
    {
        Vector3 avoidObstacleDirection = Vector2.zero;
        int obstacleCount = 0;

        if(obstacles.Count > 0)
        {
            foreach(Obstacle obstacle in obstacles)
            {
                var distance = Vector3.Distance(boid.transform.position, obstacle.transform.position);

                if (distance < obstacle.radius + 5)
                {
                    avoidObstacleDirection += (obstacle.transform.position - boid.transform.position) * (10 / distance);
                }
            }

        }

        if(obstacleCount > 0)
        {
            avoidObstacleDirection /= obstacleCount;
        }

        avoidObstacleDirection = -avoidObstacleDirection.normalized * avoidObstaclesWeighting;

        return (avoidObstacleDirection.x, avoidObstacleDirection.y);
            
        
    }

}

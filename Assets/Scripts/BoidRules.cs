using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidRules
{
    /// <summary>
    /// This class holds only rules: Alignment, Separation and Flock to create the simulation. 
    /// </summary>
    /// <param name="boid"></param>
    /// <param name="alignmentRadius"></param>
    /// <param name="alignmentWeighting"></param>
    /// <returns></returns>

    public static (float aliX, float aliY) Alignment(BoidController boid, float alignmentRadius, float alignmentWeighting)
    {
        Vector3 alignmentDirection = Vector3.zero;
        int alignmentCount = 0;

        foreach (BoidController otherBoid in FieldSecondAttempt.boids)
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

        foreach (BoidController otherBoid in FieldSecondAttempt.boids)
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
        foreach (BoidController otherBoid in FieldSecondAttempt.boids)
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

}

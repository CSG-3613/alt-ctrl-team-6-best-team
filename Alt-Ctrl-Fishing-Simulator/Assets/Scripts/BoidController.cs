using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public int SwarmIndex { get; set; }
    public float NoClumpingRadius { get; set; }
    public float LocalAreaRadius { get; set; }
    public float Speed { get; set; }
    public float SteeringSpeed { get; set; }

    public void SimulateMovement(List<BoidController> other)
    {
        //default vars
        var steering = Vector3.zero;
        var separationDirection = Vector3.zero;
        var separationCount = 0;
        var alignmentDirection = Vector3.zero;
        var alignmentCount = 0;
        var cohesionDirection = Vector3.zero;
        var cohesionCount = 0;

        var leaderBoid = other[0];
        var leaderAngle = 180f;

        foreach (BoidController boid in other)
        {
            if (boid == this) continue;

            var distance = Vector3.Distance(boid.transform.position, this.transform.position);

            if (distance < NoClumpingRadius)
            {
                separationDirection += boid.transform.position - transform.position;
                separationCount++;
            }

            if (distance < LocalAreaRadius && boid.SwarmIndex == this.SwarmIndex)
            {
                alignmentDirection += boid.transform.forward;
                alignmentCount++;

                cohesionDirection += boid.transform.position - transform.position;
                cohesionCount++;

                //identify leader
                var angle = Vector3.Angle(boid.transform.position - transform.position, transform.forward);
                if (angle < leaderAngle && angle < 90f)
                {
                    leaderBoid = boid;
                    leaderAngle = angle;
                }
            }

        }

        if(separationCount > 0)
        {
            separationDirection/=separationCount;
        }

        separationDirection = -separationDirection;

        if(alignmentCount > 0)
        {
            alignmentDirection /= alignmentCount;
        }

        if(cohesionCount > 0)
        {
            cohesionDirection /= cohesionCount;
        }

        cohesionDirection -= transform.position;

        steering += separationDirection.normalized;
        steering += alignmentDirection.normalized;
        steering += cohesionDirection.normalized;

        if(leaderBoid != null)
        {
            steering += (leaderBoid.transform.position - transform.position).normalized;
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, LocalAreaRadius, LayerMask.GetMask("Default")))
        {
            steering = ((hitInfo.point + hitInfo.normal) - transform.position).normalized;
        }

        if(steering != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), SteeringSpeed * Time.deltaTime);
        }

        transform.position += transform.TransformDirection(new Vector3(0, 0, Speed) * Time.deltaTime);



    }
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction = new Vector2(1,0);
    public float moveToCenterStrength = 4f;
    public float localFishDistance;
    public GameObject[] fishInScene;
    float avoidOtherStrength = 0.5f;
    float collisionAvoidCheckDistance = 4f;
    float alignWithOthersStrength = 0.5f;
    float alignmentCheckDistance = 4f;

    private void Awake()
    {
        fishInScene = GameObject.FindGameObjectsWithTag("Fish");
    }

    void Update()
    {
        AlignWithOthers();
        MoveToCenter();
        AvoidOtherFish();
        MoveToCenter();
        this.transform.Translate(direction * (speed * Time.deltaTime));

        foreach (var fish in fishInScene)
        {
            if (fish.transform.localPosition.y < 0)
            {
                fish.transform.SetPositionAndRotation(new Vector3(fish.transform.localPosition.x, 0, fish.transform.localPosition.z), fish.transform.localRotation);
            }
        }

    }

    void MoveToCenter()
    {
        Vector2 positionSum = transform.position;
        int count = 0;
        foreach(var fish in fishInScene)
        {
            float distance = Vector2.Distance(fish.transform.position, transform.position);
            if (distance <= localFishDistance)
            {
                positionSum += (Vector2)fish.transform.position;
                count++;
            }
        }

        if (count == 0)
        {
            return;
        }

        Vector2 positionAverage = positionSum / count;
        positionAverage = positionAverage.normalized;
        Vector2 faceDirection = (positionAverage - (Vector2)transform.position).normalized;

        float deltaTimeStrength = moveToCenterStrength * Time.deltaTime;
        direction = direction + deltaTimeStrength * faceDirection / (deltaTimeStrength + 1);
        direction = direction.normalized;
        Debug.Log("centering");

    }

    void AvoidOtherFish()
    {
        Vector2 faceAwayDirection = Vector2.zero;

        
        foreach (var fish in fishInScene)
        {
            float distance = Vector2.Distance(fish.transform.position, transform.position);

            
            if (distance <= collisionAvoidCheckDistance)
            {
                faceAwayDirection = faceAwayDirection + (Vector2)(transform.position - fish.transform.position);
            }
        }

        faceAwayDirection = faceAwayDirection.normalized;

        direction = direction + avoidOtherStrength * faceAwayDirection / (avoidOtherStrength + 1);
        direction = direction.normalized;
        Debug.Log("avoiding");
    }

    void AlignWithOthers()
    {
        Vector2 directionSum = Vector2.zero;
        int count = 0;

        foreach (var fish in fishInScene)
        {
            float distance = Vector2.Distance(fish.transform.position, transform.position);
            if (distance <= alignmentCheckDistance)
            {
                directionSum += (Vector2) fish.transform.rotation.eulerAngles;
                count++;
            }
        }

        Vector2 directionAverage = directionSum / count;
        directionAverage = directionAverage.normalized;

        //now add this direction to direction vector to steer towards it
        float deltaTimeStrength = alignWithOthersStrength * Time.deltaTime;
        direction = direction + deltaTimeStrength * directionAverage / (deltaTimeStrength + 1);
        direction = direction.normalized;
        Debug.Log("aligning");

    }
}

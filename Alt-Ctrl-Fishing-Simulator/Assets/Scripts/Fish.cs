using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public float speed = 10f;
    public Vector3 direction = new Vector2(1,0);
    public List<Fish> fishInScene;
    public float moveToCenterStrength;
    public float localFishDistance;

    

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(direction * (speed * Time.deltaTime));
    }

    void MoveToCenter()
    {
        Vector3 positionSum = transform.position;

    }
}

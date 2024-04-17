using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BobberManager : MonoBehaviour
{
    public Transform homePosition;

    [Header("Floating Fields")]
    public float floatHeight = 0f;
    public float floatBobWavelength = 1f;
    public float floatBobAmplitude = .2f;

    [Header("UnderwaterFields")]
    public float underwaterHeight = -1f;
    public float underwaterDiveSpeed = 3f;

    [Header("Reeling Fields")]
    public float reelSpeed;

    private GameObject target = null;

    private enum BobberState
    {
        uncast,
        flying,
        floating,
        underwater,
        reeling,
        retrieving
    }

    [Header("Debug Fields")]
    [SerializeField] private BobberState state;
    [SerializeField] private bool DEBUG = false;

    private void Start()
    {
        EventManager.Instance.reelButtonPressedEvent.AddListener(ReelPressed);
        EventManager.Instance.reelButtonReleasedEvent.AddListener(ReelReleased);
    }

    private void Update()
    {
        switch (state)
        {
            case BobberState.uncast:
                break;
            case BobberState.flying:
                if(gameObject.transform.position.y <= floatHeight)
                {
                    state = BobberState.floating;
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

                    // TODO: Splash sound and particle here
                }
                break;
            case BobberState.floating:
                // If floating do a bobbing animation
                float yOffset = Mathf.Sin(Time.time / floatBobWavelength) * floatBobAmplitude;

                gameObject.transform.position = new Vector3(gameObject.transform.position.x, floatHeight + yOffset, gameObject.transform.position.z);
                break;
            case BobberState.underwater:
                if (gameObject.transform.position.y <= underwaterHeight)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                break;
            case BobberState.reeling:
                break;
            case BobberState.retrieving:
                break;
        }
    }

    public void Cast()
    {
        state = BobberState.flying;
    }

    public void AddTarget(GameObject newTarget)
    {
        if(target == null)
        {
            target = newTarget;
            state = BobberState.underwater;
            Vector3 velocity = new Vector3();
            float distToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);

            velocity.x = (distToTarget / (transform.position.x - target.transform.position.x)) * underwaterDiveSpeed;
            velocity.x = (distToTarget / (transform.position.y - target.transform.position.y)) * underwaterDiveSpeed;
            velocity.x = (distToTarget / (transform.position.z - target.transform.position.z)) * underwaterDiveSpeed;

            gameObject.GetComponent<Rigidbody>().velocity = velocity;
        }
    }

    private void ReelPressed()
    {
        // TODO: Fish related stuff

        state = BobberState.reeling;
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -reelSpeed);
    }
    private void ReelReleased()
    {
        state = BobberState.floating;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}

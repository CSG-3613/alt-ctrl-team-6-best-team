using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WiimoteApi;

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
    public float reelInitialSpeed;
    public float reelSpeedDecreaseRate;
    public float reelMaxNegativeSpeed;

    private float lastReelPress = 0f;

    [SerializeField] private GameObject target = null;
    public GameObject tempTarget;


    private Rigidbody rb;

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
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // TEMPORARY
        if(DEBUG)
        {
            if (Input.GetKey(KeyCode.T))
            {
                AddTarget(tempTarget);
            }
            Debug.Log("Bobber state: " + state + "\nVelocity " + rb.velocity + "\tPosition " + gameObject.transform.position);
        }

        switch (state)
        {
            case BobberState.uncast:
                break;
            case BobberState.flying:
                if(gameObject.transform.position.y <= floatHeight)
                {
                    state = BobberState.floating;
                    rb.velocity = Vector3.zero;
                    rb.useGravity = false;
                    // TODO: Splash sound and particle here (optional)
                }
                break;
            case BobberState.floating:
                // If floating do a bobbing animation
                float yOffset = Mathf.Sin(Time.time / floatBobWavelength) * floatBobAmplitude;
                rb.velocity = Vector3.zero;

                gameObject.transform.position = new Vector3(gameObject.transform.position.x, floatHeight + yOffset, gameObject.transform.position.z);
                break;
            case BobberState.underwater:
                if (gameObject.transform.position.y <= underwaterHeight)
                {
                    rb.velocity = Vector3.zero;
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, underwaterHeight, gameObject.transform.position.z);
                }

                break;
            case BobberState.reeling:
                // Handle decceleration
                Vector3 velocity = rb.velocity;
                velocity.Normalize();
                velocity *= Mathf.Max(reelInitialSpeed - (reelSpeedDecreaseRate * (Time.time - lastReelPress)), -reelMaxNegativeSpeed);
                rb.velocity = velocity;

                break;
            case BobberState.retrieving:
                break;
        }
    }

    public void Cast()
    {
        AccelData accel = EventManager.wiimote.Accel;
        if (accel != null)
        {
            float[] accelData = accel.GetCalibratedAccelData();       

            float accel_x = accelData[0];
            float accel_y = -accelData[2];
            float accel_z = -accelData[1];

            if(DEBUG)
            {
                Debug.Log("Wiimote accel: " + accel_x + ", " + accel_y + ", " + accel_z);
            }
        }
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

            velocity.x = (target.transform.position.x - transform.position.x);
            velocity.y = (target.transform.position.y - transform.position.y);
            velocity.z = (target.transform.position.z - transform.position.z);

            velocity.Normalize();
            velocity *= underwaterDiveSpeed;
            rb.velocity = velocity;

            if (DEBUG) { Debug.Log("Diving"); }


        }
    }

    private void ReelPressed()
    {
        // TODO: Fish related stuff
        lastReelPress = Time.time;
        state = BobberState.reeling;
        Vector3 velocity = Vector3.zero;

        velocity.x = homePosition.position.x - transform.position.x;
        velocity.y = homePosition.position.y - transform.position.y;
        velocity.z = homePosition.position.z - transform.position.z;

        velocity.Normalize();
        velocity *= reelInitialSpeed;

        rb.velocity = velocity;

        if (DEBUG)
        {
            Debug.Log("Reel pressed");
        }
    }
}

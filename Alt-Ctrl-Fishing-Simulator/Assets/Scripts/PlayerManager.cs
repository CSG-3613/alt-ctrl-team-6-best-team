/*
 * Does player stuff
 * yay.
 */ 
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    private static PlayerManager instance;
    public static PlayerManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Duplicate EventManager found in scene");
            Destroy(this);
        }
    }
    #endregion

    public float castTimeLimit = 2f;
    public BobberManager bobber;
    public Transform bobberStartingPosition;

    private bool isCast = false;
    private bool isReeling = false;

    // TEMPORARY
    private float timeCastPressed = 0f;
    public float bobberHorizontalForceMultiplier = 3f;
    public float bobberVerticalForceMultiplier = 2f;

    private EventManager eventManager;

    private void Start()
    {
        eventManager = EventManager.Instance;
        eventManager.castButtonPressedEvent.AddListener(CastPressed);
        eventManager.castButtonReleasedEvent.AddListener(CastReleased);
        bobber.gameObject.SetActive(false);
        isCast = false;
    }

    private void CastPressed()
    {
        if(isCast) { return; }

        Debug.Log("pressed cast button");
        timeCastPressed = Time.time;
        bobber.gameObject.SetActive(true);
        bobber.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    private void CastReleased()
    {
        if(isCast) { return; }
        isCast = true;

        // TODO: Get acceleration vector from wiimote here and use it to generate bobber velocity
        float timeSinceCastPressed = math.max(Time.time - timeCastPressed, castTimeLimit);
        Rigidbody rb = bobber.gameObject.GetComponent<Rigidbody>();

        Vector3 bobberVelocity = new Vector3(0f, timeSinceCastPressed * bobberVerticalForceMultiplier, timeSinceCastPressed * bobberHorizontalForceMultiplier);
        Debug.Log("Launching bobber with velocity vector " + bobberVelocity);
        rb.velocity = bobberVelocity;
        rb.useGravity = true;
        bobber.Cast();
    }
}

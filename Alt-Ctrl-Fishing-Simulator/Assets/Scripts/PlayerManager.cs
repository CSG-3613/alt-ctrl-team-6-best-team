/*
 * Does player stuff
 * yay.
 */ 
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    [Header("Fish Display Menu")]
    public Canvas fishDisplayCanvas;
    public Transform fishDisplayTransform;
    public float fishTurnSpeed;
    private bool isInFishMenu = false;

    private bool isCast = false;
    // private bool isReeling = false;

    private GameObject fish = null;

    private EventManager eventManager;



    private void Start()
    {
        eventManager = EventManager.Instance;
        eventManager.castButtonPressedEvent.AddListener(CastPressed);
        eventManager.castButtonReleasedEvent.AddListener(CastReleased);
        bobber.gameObject.SetActive(false);
        isCast = false;
        fishDisplayCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isInFishMenu)
        {
            fish.transform.Rotate(0, fishTurnSpeed, 0);
        }
    }

    private void CastPressed()
    {
        if(isCast) { return; }

        Debug.Log("pressed cast button");
        bobber.gameObject.SetActive(true);
        bobber.gameObject.GetComponent<Rigidbody>().useGravity = false;
        EventManager.wiimote.RumbleOn = true;
    }

    private void CastReleased()
    {
        if(isCast) { return; }
        isCast = true;
        EventManager.wiimote.RumbleOn = false;

        bobber.Cast();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bobber")
        {
            Debug.Log("Bobber " + other.gameObject.name + " entered retrieval zone");
            BobberManager bm = other.gameObject.GetComponent<BobberManager>();
            if(bm != null && bm.State == BobberManager.BobberState.reeling)
            {
                fish = bm.Retrieve();
                fishDisplayCanvas.gameObject.SetActive(true);
                if(fish == null)
                {
                    bm.Reset();
                    ResetGamestate();
                }
                else
                {
                    EnterFishDisplay();
                }
            }
        }
    }

    private void EnterFishDisplay()
    {
        isInFishMenu = true;
        fish.transform.position = fishDisplayTransform.position;
        fish.transform.rotation = fishDisplayTransform.rotation;
        Collider col = fish.GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = false;
        }
        bobber.gameObject.SetActive(false);
        EventManager.Instance.castButtonReleasedEvent.AddListener(ResetGamestate);
    }

    private void ResetGamestate()
    {
        fishDisplayCanvas.gameObject.SetActive(false);
        isCast = false;
        // isReeling = false;
        if(fish != null)
        {
            fish.SetActive(false);
            fish = null;
        }
        if (isInFishMenu)
        {
            EventManager.Instance.castButtonReleasedEvent.RemoveListener(ResetGamestate);
            isInFishMenu = false;
        }
        bobber.Reset();
        bobber.gameObject.SetActive(false);
    }

}

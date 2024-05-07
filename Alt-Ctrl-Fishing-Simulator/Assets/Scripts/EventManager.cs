/*
 * Handles all input events
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.tvOS;
using WiimoteApi;

public class EventManager : MonoBehaviour
{
    #region Singleton
    private static EventManager instance;
    public static EventManager Instance { get { return instance; } }
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

    // Casting Fields
    public KeyCode castKeycode = KeyCode.Mouse0;
    public UnityEvent castButtonPressedEvent = new UnityEvent();
    public UnityEvent castButtonReleasedEvent = new UnityEvent();
    private bool isCasting = false;

    // Reeling Fields
    public KeyCode reelKeycode = KeyCode.Mouse1;
    public UnityEvent reelButtonPressedEvent = new UnityEvent();

    public bool DEBUG = false;

    [HideInInspector] public static Wiimote wiimote;

    private bool wasOnePressedLast = false;

    private void Start()
    {
        isCasting = false;
        InitWiimotes();
    }

    private void OnDestroy()
    {
        Debug.Log("Calling FinishedWithWiimote()");
        FinishedWithWiimotes();
    }

    
    #region Wiimote stuff
    void InitWiimotes()
    {
        WiimoteManager.FindWiimotes(); // Poll native bluetooth drivers to find Wiimotes
        wiimote = null;
        foreach (Wiimote remote in WiimoteManager.Wiimotes)
        {
            if (wiimote == null) { wiimote = remote; }
            Debug.Log("Found Wiimote: " + remote.ToString());
            remote.SendPlayerLED(true, false, false, false);

            wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);

            wiimote.ReadWiimoteData();
            Debug.LogWarning(wiimote.Status.battery_level);
        }

        Debug.Log("Wiimote = " + wiimote.ToString());
    }

    void FinishedWithWiimotes()
    {
        WiimoteManager.Cleanup(wiimote);
    }
    #endregion
    

    // Update is called once per frame
    void Update()
    {

        
        // this loop taken from the wiimote API docs
        int ret;
        do
        {
            ret = wiimote.ReadWiimoteData();
        } while (ret > 0);  //ReadWiimoteData() returns 0 when nothing is left to read.  So by doing this we continue to
                           // update the Wiimote until it is "up to date."

        if (DEBUG)
        {
            float[] accelData = wiimote.Accel.GetCalibratedAccelData();

            float accel_x = accelData[0];
            float accel_y = accelData[2];
            float accel_z = accelData[1];

            if (wiimote.Button.minus)
            {
                Debug.Log("Wiimote accel: " + accel_x + ", " + accel_y + ", " + accel_z);
            }
        }
        


        if (!isCasting && (Input.GetKeyDown(castKeycode) || wiimote.Button.a))
        {
            Debug.Log("CastButtonPressedEvent invoked");
            castButtonPressedEvent.Invoke();
            isCasting = true;
            
            wiimote.SendStatusInfoRequest();
        }
        if (isCasting && (Input.GetKeyUp(castKeycode) || !wiimote.Button.a))
        {
            Debug.Log("CastButtonReleasedEvent invoked");
            castButtonReleasedEvent.Invoke();
            isCasting = false;
            
            wiimote.SendStatusInfoRequest();
        }

        if (Input.GetKeyDown(reelKeycode) || (wasOnePressedLast && wiimote.Button.two) || (!wasOnePressedLast && wiimote.Button.one))
        {
            if(wiimote.Button.one) { wasOnePressedLast = true; } else { wasOnePressedLast = false; }
            reelButtonPressedEvent.Invoke();
        } 
    }
}

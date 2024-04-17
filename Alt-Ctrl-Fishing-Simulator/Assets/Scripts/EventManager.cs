/*
 * Handles all input events
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    #region Singleton
    private static EventManager instance;
    public static EventManager Instance {  get { return instance; } }
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
    public UnityEvent reelButtonReleasedEvent = new UnityEvent();
    private bool isReeling = false;

    private void Start()
    {
        isCasting = false;
        isReeling = false;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Edit this to be based on wiimote accelerometers (may be handled in the PlayerManager)

        if(!isCasting && Input.GetKeyDown(castKeycode))
        {
            Debug.Log("CastButtonPressedEvent invoked");
            castButtonPressedEvent.Invoke();
            isCasting = true;
        }
        if (isCasting && Input.GetKeyUp(castKeycode))
        {
            Debug.Log("CastButtonReleasedEvent invoked");
            castButtonReleasedEvent.Invoke();
            isCasting = false;
        }
        
        if (!isReeling && Input.GetKeyDown(reelKeycode))
        {
            reelButtonPressedEvent.Invoke();
            isReeling = true;
        } 
        if (isReeling && Input.GetKeyUp(reelKeycode))
        {
            reelButtonReleasedEvent.Invoke();
            isReeling = false;
        }
    }
}

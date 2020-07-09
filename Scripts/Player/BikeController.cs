using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BikeController : MonoBehaviour
{

    public float releaseForgiveness = 0.1f;
    public float forcePerCycle = 100.0f;
    public float degreesPerCycle = 45.0f;
    public float rumbleThreshold = 10.0f;
    public float breakSpeed = 1.0f;

    public bool showDebugMenu = false;

    private float pedalLeft;
    private float pedalRight;
    private float pedalDelta = 0.0f;

    private bool commitLeft = false;
    private bool commitRight = false;

    private bool hitMax = false;

    private bool previousDirectionWasLeft = false;

    private float pedalCycle = 0; // 0 - Left // 0.5 - Right // 1 - Left

    private int numRevolutions = 0;


    private Rigidbody rb;
    private float momentum = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0.0f, -1.0f, 0.0f);
        
    }

    // Update is called once per frame
    void Update()
    {
        float rumble = Mathf.Min(rumbleThreshold, rb.velocity.magnitude) / rumbleThreshold;
        Gamepad.current.SetMotorSpeeds(rumble / 10.0f, rumble / 10.0f);

        if (pedalLeft == 0.0f && pedalRight == 0.0f && pedalCycle > 0.0f) {
            // The player is not holding any triggers
            hitMax = false;
            pedalCycle = 0.0f;
            previousDirectionWasLeft = commitLeft;
            commitLeft = false;
            commitRight = false;
        }

        if(pedalLeft > releaseForgiveness && pedalRight > releaseForgiveness) {
            // The player is holding both triggers, break
            rb.velocity = rb.velocity.normalized * Mathf.Max(0.0f,(rb.velocity.magnitude - breakSpeed * Time.deltaTime));
            return;
        }

        // Commit to a direction if we don't have one already
        if(commitLeft == false && commitRight == false) {
            commitLeft = pedalLeft > pedalRight;
            commitRight = pedalRight > pedalLeft;
        }
        

        float triggerPull = commitLeft ? pedalLeft : pedalRight;
        float previousPedalCycle = pedalCycle;
        pedalDelta = 0.0f;

        /*if (pedalCycle >= 0.0f && pedalCycle < 0.5f) {
            pedalCycle = Mathf.Max(pedalRight / 2.0f, pedalCycle);
            pedalDelta = pedalCycle - previousPedalCycle;
        } else if (pedalCycle >= 0.5f && pedalCycle < 1.0f) {
            pedalCycle = Mathf.Max((0.5f + pedalLeft / 2.0f), pedalCycle);
            pedalDelta = pedalCycle - previousPedalCycle;
        } else if (pedalCycle == 1.0f) {
            pedalCycle = 0.0f;
            numRevolutions++;
        }*/

        pedalCycle = Mathf.Max(triggerPull, pedalCycle);
        pedalDelta = pedalCycle - previousPedalCycle;

        if(pedalCycle == 1.0f) {
            hitMax = true;
        }

        // Add momentum to the bike
        momentum = forcePerCycle * pedalDelta * 300.0f;
        Vector3 laterialForward = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
        laterialForward.Normalize();
        rb.AddForce(laterialForward * momentum);

        if (pedalDelta > 0.0f) {
            if(previousDirectionWasLeft && commitLeft || !previousDirectionWasLeft && commitRight) {
                //Gamepad.current.SetMotorSpeeds(0.25f, 0.75f);
                // Bike Tilt
                transform.localRotation = Quaternion.AngleAxis(10.0f * pedalDelta * (commitRight ? -1.0f : 1.0f), transform.forward) * transform.localRotation;
                // Bike Rotation
                transform.rotation = Quaternion.AngleAxis(degreesPerCycle * pedalDelta * (commitLeft ? -1.0f : 1.0f), Vector3.up) * transform.rotation;
                Vector3 v = transform.forward * rb.velocity.magnitude;
                rb.velocity = new Vector3(v.x, rb.velocity.y, v.z);


            } else {
                //Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
            }
        } else {
            //Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
        }
    }

    private void FixedUpdate() {
        //Debug.DrawRay(transform.position + Vector3.up, desiredDirection);
        if(transform.position.y < -10.0f)
        {
            GameObject respawnPoint = GameObject.Find("WorldClipRespawn");
            if(respawnPoint != null)
            {
                transform.position = respawnPoint.transform.position;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void OnDestroy()
    {
        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
    }

    private void OnPedalLeft(InputValue value) {
        pedalLeft = value.Get<float>();
        if(pedalLeft < releaseForgiveness)
        {
            pedalLeft = 0.0f;
        }
    }

    private void OnPedalRight(InputValue value) {
        pedalRight = value.Get<float>();
        if (pedalRight < releaseForgiveness)
        {
            pedalRight = 0.0f;
        }
    }

    bool showOnGUI = true;
    private void OnGUI() {
        if(showDebugMenu == false)
        {
            return;
        }
        Rect windowRect0 = new Rect(20, 20, 150, 200);
        if (showOnGUI) {
            windowRect0 = GUI.Window(0, windowRect0, DoMyWindow, "Bike Debug Menu");
        }
    }
    void DoMyWindow(int windowID) {
        
        if (GUI.Button(new Rect(0, 0, 20, 20), "X")) {
            showOnGUI = false;
        }

        GUI.Label(new Rect(5, 18, 400, 20), "Cycle:" + pedalCycle.ToString("n2"));
        GUI.Label(new Rect(5, 18 + 15 * 1, 400, 20), "pedalLeft:" + pedalLeft.ToString("n2"));
        GUI.Label(new Rect(5, 18 + 15 * 2, 400, 20), "pedalRight:" + pedalRight.ToString("n2"));
        GUI.Label(new Rect(5, 18 + 15 * 3, 400, 20), "numRevolutions:" + numRevolutions);
        GUI.Label(new Rect(5, 18 + 15 * 4, 400, 20), "momentum:" + momentum);
        GUI.Label(new Rect(5, 18 + 15 * 5, 400, 20), "velocity:" + rb.velocity.magnitude.ToString("n2"));
        GUI.Label(new Rect(5, 18 + 15 * 6, 400, 20), "commitLeft:" + commitLeft);
        GUI.Label(new Rect(5, 18 + 15 * 7, 400, 20), "commitRight:" + commitRight);
        GUI.Label(new Rect(5, 18 + 15 * 8, 400, 20), "prevLeft:" + previousDirectionWasLeft);
        GUI.Label(new Rect(5, 18 + 15 * 9, 400, 20), "hitMax:" + hitMax);

        // Make the windows be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeWheelHandler : MonoBehaviour
{

    public float wheelDiameter = 1.0f;
    public GameObject bikeRoot;

    private Vector3 previous_location;

    // Update is called once per frame
    void FixedUpdate()
    {
        bool forward = true;
        float distance = Vector3.Distance(bikeRoot.transform.position, previous_location);

        if (Vector3.Angle(bikeRoot.transform.position - previous_location, bikeRoot.transform.forward) < 90.0f) {
            // We're moving forward
            forward = true;
        } else {
            // We're moving backward
            forward = false;
        }

        previous_location = bikeRoot.transform.position;

        float rotation = (distance / (wheelDiameter / 2.0f)) * (180.0f / Mathf.PI);

        if(forward == false) {
            rotation *= -1;
        }
        
        
        transform.Rotate(new Vector3(0, -1, 0), rotation);
    }
}

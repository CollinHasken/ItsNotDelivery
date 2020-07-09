using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    public float stopBuffer = 1.5f;

    public int priority = -1;

    public BoxCollider collider;

    private float splineWalkerDuration;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(collider != null, "A car was setup without a Box Collider pointing to the stopBuffer");
        splineWalkerDuration = GetComponent<SplineWalker>().duration;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        bool nearVehicle = false;
        RaycastHit[] hit;
        hit = Physics.BoxCastAll(collider.center, transform.localScale * stopBuffer, transform.forward);
        if(hit.Length > 0)
        {
            for(int i = 0; i < hit.Length; i++)
            {
                if(hit[i].transform.tag != "NonPlayerVehicle")
                {
                    continue;
                }
                
                nearVehicle = priority < hit[i].transform.gameObject.GetComponentInParent<CarManager>().priority;
            }
        }
        if(nearVehicle == true)
        {
            GetComponent<SplineWalker>().duration = splineWalkerDuration * 10.0f;
        }
        else
        {
            GetComponent<SplineWalker>().duration = splineWalkerDuration;
        }*/
    }
}

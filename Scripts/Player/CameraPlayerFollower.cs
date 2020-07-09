using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerFollower : MonoBehaviour
{
    public GameObject playerObject;

    public float lookAtSpeed = 5.0f;
    public float cameraMoveSpeed = 100.0f;
    public Vector3 offset = new Vector3(0, 0, 0);
    public Vector3 lookAtOffset = new Vector3(0, 0, 0);

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        //initOffset = transform.position - playerObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lateralForward = new Vector3(playerObject.transform.forward.x, 0.0f, playerObject.transform.forward.z);
        lateralForward.Normalize();
        Vector3 lateralRight = new Vector3(playerObject.transform.right.x, 0.0f, playerObject.transform.right.z);
        lateralRight.Normalize();

        Vector3 targetPostition = playerObject.transform.position +
                lateralRight * offset.x +
                Vector3.up * offset.y +
                lateralForward * offset.z;

        //transform.position = Vector3.Lerp(transform.position, targetPostition, cameraMoveSpeed * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, targetPostition, ref velocity, 0.3f);

        Quaternion targetRotation = Quaternion.LookRotation(playerObject.transform.position + lookAtOffset - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookAtSpeed * Time.unscaledDeltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputController : MonoBehaviour
{

    private Vector2 inputMovement;
    float moveSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(inputMovement.x, 0, inputMovement.y) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    private void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }
}

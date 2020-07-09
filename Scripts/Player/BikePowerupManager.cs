using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BikePowerupManager : MonoBehaviour
{

    public int m_DropBlocks = 0;
    public GameObject m_DropBlocksPrefab;

    public bool hasPowerup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_DropBlocks > 0)
        {
            hasPowerup = true;
        }
        else
        {
            hasPowerup = false;
        }
        
    }

    private void OnPowerDropBlocks(InputValue value)
    {
        if(m_DropBlocks <= 0)
        {
            return;
        }
        m_DropBlocks--;

        // Spawn the prefab 
        GameObject power = Instantiate(m_DropBlocksPrefab, gameObject.transform.TransformPoint(new Vector3(0.0f,1.0f,-2.0f)), gameObject.transform.rotation);
        power.GetComponent<PowerDropBlocks>().m_Owner = gameObject;
    }
}

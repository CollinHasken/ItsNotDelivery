using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestruction : MonoBehaviour
{
    public int m_LifetimeSec = 10;

    private float m_BirthTime;

    // Start is called before the first frame update
    void Start()
    {
        m_BirthTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > m_BirthTime + m_LifetimeSec)
        {
            Destroy(gameObject);
        }
    }
}

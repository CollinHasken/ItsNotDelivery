using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Explosion : MonoBehaviour
{
    public UnityEvent particlesStopped;

    // Start is called before the first frame update
    void Start()
    {
        if(particlesStopped == null)
        {
            particlesStopped = new UnityEvent();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnParticleSystemStopped()
    {
        particlesStopped.Invoke();
    }
}

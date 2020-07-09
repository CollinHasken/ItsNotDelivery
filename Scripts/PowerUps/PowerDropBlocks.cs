using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDropBlocks : MonoBehaviour
{
    public int m_BlocksToDropMin = 5;
    public int m_BlocksToDropMax = 7;
    public GameObject m_Block;
    public int m_force;

    public GameObject m_Owner;

    // Start is called before the first frame update
    void Start()
    {
        int blocksToDrop = Random.Range(m_BlocksToDropMin, m_BlocksToDropMax);
        for(int i = 0; i < blocksToDrop; i++)
        {
            GameObject projectile = Instantiate(m_Block, transform.position, Random.rotation);
            //projectile.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * -1.0f * Random.Range(0.9f, 0.1f) * m_force);
            projectile.GetComponent<Rigidbody>().velocity = (gameObject.transform.forward * -1.0f * m_force);

            // Disable collisions between the block and the owner
            /*Collider blockColider = GetComponent<Collider>();
            Collider[] colList = m_Owner.GetComponentsInChildren<Collider>();
            foreach (Collider col in colList) {
                Physics.IgnoreCollision(blockColider, col, true);
            }*/
        }
        Destroy(gameObject);
    }

}

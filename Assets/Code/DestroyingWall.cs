using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyingWall : MonoBehaviour
{
    ParticleSystem m_LavaParticle;

    private void Start()
    {
        m_LavaParticle = Resources.Load<ParticleSystem>("Particles/Lava");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<ICube>() != null)
        {
            Instantiate(m_LavaParticle, collision.collider.transform.position, Quaternion.identity);

            collision.collider.GetComponent<ICube>().DestroyCube();
        }
        if(collision.collider.GetComponent<Turret>()!=null)
        {
            Instantiate(m_LavaParticle, collision.collider.transform.position, Quaternion.identity);

            collision.collider.GetComponent<Turret>().TurretDie();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Instantiate(m_LavaParticle, other.transform.position, Quaternion.identity);
    }
}

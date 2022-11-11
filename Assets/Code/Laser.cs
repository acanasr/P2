using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer m_Laser;
    public LayerMask m_LaserLayerMask;
    public float m_MaxLaserDistance = 250.0f;


    private void Update()
    {
        
    }
    public void Shoot()
    {
        Ray l_Ray = new Ray(m_Laser.transform.position, m_Laser.transform.forward);
        float l_LaserDistance = m_MaxLaserDistance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, l_LaserDistance, m_LaserLayerMask))
        {
            l_LaserDistance = Vector3.Distance(m_Laser.transform.position, l_RaycastHit.point);
            switch (l_RaycastHit.collider.tag)
            {
                case "RefractionCube":
                    l_RaycastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
                    break;
                case "CompanionCube":
                    l_RaycastHit.collider.GetComponent<Companion>().DestroyCube();
                    break;
                case "Player":
                    l_RaycastHit.collider.GetComponent<PlayerCheckpointSystem>().PlayerDie();
                    break;
                case "Turret":
                    l_RaycastHit.collider.GetComponent<Turret>().TurretDie();
                    break;
                case "Button":
                    l_RaycastHit.collider.GetComponent<ButtonEvent>().UseButton();
                    break;
                case "Portal":
                    l_RaycastHit.collider.GetComponent<Portal>().TeleportLaser(l_RaycastHit.point, m_Laser);
                    break;
                default:
                    break;
            }

        }

        m_Laser.SetPosition(1, new Vector3(0.0f, 0.0f, l_LaserDistance));
        
    }

}

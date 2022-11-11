using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Laser m_Laser;
    public float m_AlifeAngleInDegress = 30.0f;
    bool m_IsTurretDead;

    AudioClip m_DeathAudio;

    void Start()
    {
        m_IsTurretDead = false;
        m_DeathAudio = Resources.Load<AudioClip>("Sounds/DeathRobot");
    }

    void Update()
    {
        if (m_IsTurretDead) return;
        bool l_LaserAlife = Vector3.Dot(transform.up, Vector3.up) > Mathf.Cos(m_AlifeAngleInDegress * Mathf.Deg2Rad);
            
        m_Laser.gameObject.SetActive(l_LaserAlife);

        if (l_LaserAlife) {

            m_Laser.Shoot();
        }
    }

    public void TurretDie()
    {
        if (m_IsTurretDead) return;
        m_IsTurretDead = true;

        Destroy(m_Laser.gameObject);
        Transform[] Objects = ObjectUtilities.GetObjectUtilities().MakeItRagdoll(transform);
        SoundController.GetSoundController().PlayOneShot3D(gameObject, m_DeathAudio);
        ObjectUtilities.GetObjectUtilities().DestroyTransformArray(Objects, m_DeathAudio.length);
    }
}

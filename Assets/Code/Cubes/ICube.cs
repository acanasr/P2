using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICube : MonoBehaviour
{
    // ICUBE is an abstract class that does everything than ALL cubes can do.

    protected bool m_CanBeTeleportable; 
    Rigidbody m_Rigidbody;
    float m_TeleportOffset = 2f;
    Portal m_ExitPortal;
    protected AudioClip m_CubeDeath;
    bool m_IsDestroyed;
    private void Start()
    {
        m_CanBeTeleportable = false;
        m_IsDestroyed = false;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CubeDeath = Resources.Load<AudioClip>("Sounds/CubeDeath");
    }
    public virtual void SetTeleportable(bool Teleportable)
    {
        m_CanBeTeleportable = Teleportable;
        if (Teleportable) GetComponent<Collider>().enabled = true;
        else GetComponent<Collider>().enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal") && m_CanBeTeleportable)
        {
            Portal l_Portal = other.GetComponent<Portal>(); 
            if (!l_Portal.CanPortalBeUsed()) return;
            if (l_Portal != m_ExitPortal) Teleport(l_Portal);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            if (other.GetComponent<Portal>() == m_ExitPortal) m_ExitPortal = null;
        }
    }
    protected virtual void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosition =
            _Portal.m_OtherPortalTransform.InverseTransformPoint(transform.position);
        Vector3 l_LocalDirection =
            _Portal.m_OtherPortalTransform.InverseTransformDirection(transform.forward);
        Vector3 l_LocalVelocity =
            _Portal.m_OtherPortalTransform.InverseTransformDirection(m_Rigidbody.velocity);
        Vector3 l_WorldVelocity =
            _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalVelocity);

        m_Rigidbody.isKinematic = true;
        transform.forward =
            _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirection);

        transform.position =
            _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition) + transform.forward * (m_TeleportOffset*transform.localScale.x);

        //transform.localScale *= (_Portal.m_MirrorPortal.transform.localScale.x / _Portal.transform.localScale.x);
        transform.localScale = _Portal.m_MirrorPortal.transform.localScale.x*Vector3.one; //just a game design decision
        m_Rigidbody.isKinematic = false; 
        m_Rigidbody.velocity = l_WorldVelocity;
        m_ExitPortal = _Portal.m_MirrorPortal;


    }
     public void DestroyCube()
    {
        if (m_IsDestroyed) return;
        m_IsDestroyed = true;


        Transform[] Objects = ObjectUtilities.GetObjectUtilities().MakeItRagdoll(transform);
        SoundController.GetSoundController().PlayOneShot3D(gameObject, m_CubeDeath);
        ObjectUtilities.GetObjectUtilities().DestroyTransformArray(Objects, m_CubeDeath.length);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [Range(0.0f, 90.0f)] public float m_AngleToEnterPortal = 50.0f;
    public float m_TeleportOffset = 2f;
    FPSPlayerController m_PlayerController;
    private void Start()
    {
        m_PlayerController = GameController.GetGameController().GetPlayer();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            Portal l_Portal = other.GetComponent<Portal>();
            if (!l_Portal.CanPortalBeUsed()) return;
            if (Vector3.Dot(-m_PlayerController.m_Direction, l_Portal.transform.forward) > Mathf.Cos(m_AngleToEnterPortal) * Mathf.Deg2Rad)
            {
                Teleport(l_Portal);
            }
        }
    }

    void Teleport(Portal _Portal)
    {

        Vector3 l_LocalPosition =
            _Portal.m_OtherPortalTransform.InverseTransformPoint(transform.position);
        Vector3 l_LocalDirection =
            _Portal.m_OtherPortalTransform.InverseTransformDirection(transform.forward);
        Vector3 l_LocalDirectionMovement =
            _Portal.m_OtherPortalTransform.InverseTransformDirection(m_PlayerController.m_Direction);
        Vector3 m_WorldDirectionMovement =
            _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirectionMovement);


        m_PlayerController.m_CharacterController.enabled = false;
        transform.forward =
            _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirection);

        m_PlayerController.m_Yaw = transform.rotation.eulerAngles.y;

        transform.localScale = _Portal.m_MirrorPortal.transform.localScale.x * Vector3.one;

        transform.position =
            _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition) + m_WorldDirectionMovement * (m_TeleportOffset * transform.localScale.x); //if gives more or less offset depending on the scale, so never can be setted behind the wall

        m_PlayerController.m_CharacterController.enabled = true;

    }

}

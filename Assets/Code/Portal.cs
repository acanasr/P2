using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    bool m_CanBeUsed;
    public bool CanPortalBeUsed() { return m_CanBeUsed; }

    public Camera m_Camera; //Portal Camera that is rendered on the other portal.
    public Transform m_OtherPortalTransform; //Acts as a Virtual MirrorPortal
    public Portal m_MirrorPortal; //The other Portal
    FPSPlayerController m_Player;
    public GameObject m_Cross;

    public float m_OffsetNearPlane = 0.5f; //Offset for portal camera to go further than any wall so walls are not seen
    public GameObject m_RingPrePortal;
    public MeshRenderer m_PortalCylinder;
    Material m_BlackMaterial;
    public Material m_ShaderPortalMaterial;

    public List<Transform> m_ValidPoints;

    public float m_MaxValidDistance; //Maximum valid distance between raycast point and every valid point
    public float m_MinValidDistance; //Minimum valid distance between raycast point and every valid point
    public float m_MinDotValidAngle; //Minimum dot value for considering that the normal of every Valid point and the raycast is in the same plane.

    public Laser m_Laser;
    float m_TimeSinceLastTeleport;

    public bool m_LaserEnabled;

    void Start()
    {
        m_Player = GameController.GetGameController().GetPlayer();
        m_CanBeUsed = false;
        m_BlackMaterial = Resources.Load<Material>("Materials/BlackMaterial");
    }

    void LateUpdate()
    {
        //This function is to translate the player world position to the mirror portal's camera 
        Vector3 l_WorldPosition = m_Player.m_Camera.transform.position; //World Position Of the Player's Camera
        Vector3 l_LocalPosition = m_OtherPortalTransform.InverseTransformPoint(l_WorldPosition); // Local Position between the portal and the player's camera, saves an "offset vector" used to apply it to the other's mirror renderer.
        m_MirrorPortal.m_Camera.transform.position = m_MirrorPortal.transform.TransformPoint(l_LocalPosition); //Gets the other portal camera and sets the local position of the player (in the mirror portal) to a world position


        //This function is to translate the player world rotation to the mirror portal's camera 
        Vector3 l_WorldDirection = m_Player.m_Camera.transform.forward; //World Forward Vector of the Player's Camera
        Vector3 l_LocalDirection = m_OtherPortalTransform.InverseTransformDirection(l_WorldDirection); // Local Rotation between the portal and the player's camera, saves an "offset vector" used to apply the rotation to the other's mirror renderer.
        m_MirrorPortal.m_Camera.transform.forward = m_MirrorPortal.transform.TransformDirection(l_LocalDirection);//Gets the other portal camera and sets the forward of the player (in the mirror portal) to a world position

        float l_Distance = Vector3.Distance(m_MirrorPortal.m_Camera.transform.position, m_MirrorPortal.transform.position);//Gets the distance between the portal and the camera
        m_MirrorPortal.m_Camera.nearClipPlane = l_Distance + m_OffsetNearPlane; //As the portal is set after a wall or floor, we set the nearClipPlane a bit further so any wall, floor or the own portal is not rendered in a portal's camera.

        if(!m_MirrorPortal.gameObject.activeSelf && m_CanBeUsed)
        {
            m_CanBeUsed = false;
            m_PortalCylinder.material = m_BlackMaterial;
            m_Laser.gameObject.SetActive(false);

        }
        else if (m_MirrorPortal.gameObject.activeSelf && !m_CanBeUsed)
        {
            m_CanBeUsed = true;
            m_PortalCylinder.material = m_ShaderPortalMaterial;

        }
        m_Laser.gameObject.SetActive(m_LaserEnabled);
        m_MirrorPortal.m_LaserEnabled = false;
        //Checks if is receiving teleport info or not, if its not unsets the other portal Laser.
        //m_TimeSinceLastTeleport += Time.deltaTime;
        //if (m_MirrorPortal.m_Laser.gameObject.activeSelf && m_TimeSinceLastTeleport > 0.1f) m_MirrorPortal.m_LaserEnabled = false;
    }
    public bool IsValidPosition(Vector3 StartPosition, Vector3 Forward, float MaxDistance, LayerMask PortalLayerMask, out Vector3 Position, out Vector3 Normal, out bool IsNonDrawableWall)
    {

        Position = Vector3.zero;
        Normal = Vector3.forward;
        IsNonDrawableWall = false;

        RaycastHit l_RaycastHit;
        Ray l_Ray = new Ray(StartPosition, Forward);

        if (Physics.Raycast(l_Ray, out l_RaycastHit, MaxDistance, PortalLayerMask.value))
        {
            if (l_RaycastHit.collider.tag == "DrawableWall")
            {
                Position = l_RaycastHit.point;
                Normal = l_RaycastHit.normal;

                transform.position = Position;
                transform.rotation = Quaternion.LookRotation(Normal);

                m_RingPrePortal.transform.position = Position;
                m_RingPrePortal.transform.rotation = Quaternion.LookRotation(Normal);


                for (int i = 0; i < m_ValidPoints.Count; i++)
                {
                    Vector3 l_Direction = m_ValidPoints[i].position - StartPosition;
                    l_Direction.Normalize();
                    l_Ray = new Ray(StartPosition, l_Direction);
                    if (Physics.Raycast(l_Ray, out l_RaycastHit, MaxDistance, PortalLayerMask.value))
                    {
                        if (l_RaycastHit.collider.tag == "DrawableWall")
                        {
                            float l_Distance = Vector3.Distance(Position, l_RaycastHit.point);
                            //Debug.Log("Distance = " + l_Distance);
                            float l_DotAngle = Vector3.Dot(Normal, l_RaycastHit.normal);
                            //Debug.Log("dist " + l_Distance + " - " + l_DotAngle + " a " + l_RaycastHit.collider.name);
                            if (!(l_Distance >= m_MinValidDistance && l_Distance <= m_MaxValidDistance && l_DotAngle > m_MinDotValidAngle)) return false;
                        }
                        else return false;
                    }
                    else return false;
                }
            }
            else if (l_RaycastHit.collider.tag == "NotDrawableWall")
            {
                m_Cross.transform.position = l_RaycastHit.point;
                m_Cross.transform.rotation = Quaternion.LookRotation(l_RaycastHit.normal);
                IsNonDrawableWall = true;
                return false;
            }
            else return false;
        }
        else return false;

        return true;
    }

    public void TeleportLaser(Vector3 hitPoint, LineRenderer LaserToTeleport)
    {
        //if (m_MirrorPortal.m_Laser.gameObject.activeSelf) return; // if laser's active return (it wont update as good but prevents stack overflow)
        if (!m_MirrorPortal.gameObject.activeSelf) return; //if other portal is not active do not do the logic
        if (m_MirrorPortal.m_LaserEnabled) return;
        m_TimeSinceLastTeleport = 0.0f;

        m_MirrorPortal.m_LaserEnabled = true;

        Vector3 l_LocalPosition =
            m_OtherPortalTransform.InverseTransformPoint(hitPoint);
        Vector3 l_LocalDirection =
            m_OtherPortalTransform.InverseTransformDirection(LaserToTeleport.transform.forward);

        Vector3 l_WorldPosition =
            m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        Vector3 l_WorldDirection =
            m_MirrorPortal.transform.TransformDirection(l_LocalDirection);

        Plane l_Plane = new Plane(transform.forward, transform.position);
        Ray l_Ray=new Ray(hitPoint, LaserToTeleport.transform.forward);
        float l_Distance = 0.0f;
        l_Plane.Raycast(l_Ray, out l_Distance);

        m_MirrorPortal.m_Laser.transform.position = l_WorldPosition+ l_WorldDirection.normalized* l_Distance;

        m_MirrorPortal.m_Laser.transform.forward = l_WorldDirection;

        //LaserToTeleport.SetPosition(1, hitPoint + l_WorldDirection.normalized * l_Distance); Trying to go deep in first laser

        m_MirrorPortal.m_Laser.Shoot();

    }
}


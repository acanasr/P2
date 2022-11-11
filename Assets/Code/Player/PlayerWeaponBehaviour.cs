using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponBehaviour : MonoBehaviour
{
    FPSPlayerController m_PlayerController;
    [Header("Portals")]
    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public GameObject m_OrangeRingPrePortal;
    public GameObject m_BlueRingPrePortal;
    public PortalShooting m_PortalShooting; //If player presses for blue, until player does not end up pressing for blue, orange shooting process will not work and viceversa
    public GameObject m_Cross; //Cross if trying to put portal into non drawable wall

    [Header("Keys")]
    KeyCode m_BluePortalKey = KeyCode.Mouse0;
    KeyCode m_OrangePortalKey = KeyCode.Mouse1;
    KeyCode m_ThrowCubeKey = KeyCode.Mouse0;
    KeyCode m_DropCubeKey = KeyCode.Mouse1;
    KeyCode m_AttachingObjectKeyCode = KeyCode.E;

    [Header("Portal Shooting Related")]
    public float m_MaxShootDistance = 100.0f; //How far a Portal can go
    public LayerMask m_LayerMask;
    bool m_CanShootPortal;
    AudioClip m_ShootPortalAudio;


    //Particles
    ParticleSystem m_AttachingParticles; //Particles prefab of attaching from cube to player
    ParticleSystem m_UsingAttachingParticlesGO; //Particles GameObject, to instanciate in this go and destroy this go when wanted
    ParticleSystem m_PortalParticles; //Particles prefab of portal

    public Image m_CrossHairImage; //Crosshair image
    Sprite m_PortalCrossHair; //No portal setted
    Sprite m_PortalCrossHairB; //Blue portal setted texture
    Sprite m_PortalCrossHairO; //Orange portal setted texture
    Sprite m_PortalCrossHairBO; //Blue and Orange portal setted texture

    float m_MinLocalScaleValue = 0.5f;
    float m_MaxLocalScaleValue = 1.5f;
    float m_LocalScaleSpeed = 0.05f;

    Animation m_WeaponAnimation;
    AnimationClip m_WeaponIdle;
    AnimationClip m_WeaponShoot;


    [Header("Throwable Cubes Variables")]

    public Transform m_AttachingPosition;
    Quaternion m_AttachingObjectStartRotation;

    Rigidbody m_ObjectAttached;

    public LayerMask m_AttachObjectLM;

    public float m_AttachingObjectSpeed;
    public float m_MaxDistanceToAttachObject;
    public float m_ThrowForce;

    AudioClip m_ShootCubeAudio;
    AudioClip m_AttachCubeAudio;

    public ParticleSystem m_ShootCubeParticle;

    bool m_AttachingObject = false;

    private void OnEnable()
    {
        PlayerCheckpointSystem.OnPlayerDeath += PlayerDied;
    }
    private void OnDisable()
    {
        PlayerCheckpointSystem.OnPlayerDeath -= PlayerDied;
    }
    private void Start()
    {
        m_BluePortal.gameObject.SetActive(false);
        m_OrangePortal.gameObject.SetActive(false);
        m_OrangeRingPrePortal.SetActive(false);
        m_BlueRingPrePortal.SetActive(false);

        m_CanShootPortal = true;

        m_PortalCrossHair = Resources.Load<Sprite>("Images/portal crossair");
        m_PortalCrossHairB = Resources.Load<Sprite>("Images/portal crossair B");
        m_PortalCrossHairO = Resources.Load<Sprite>("Images/portal crossair O");
        m_PortalCrossHairBO = Resources.Load<Sprite>("Images/portal crossair BO");

        m_PortalParticles = Resources.Load<ParticleSystem>("Particles/Portal");
        m_AttachingParticles = Resources.Load<ParticleSystem>("Particles/AttachingObject");

        m_ShootPortalAudio = Resources.Load<AudioClip>("Sounds/Portal");
        m_ShootCubeAudio = Resources.Load<AudioClip>("Sounds/CubeShot");
        m_PortalShooting = PortalShooting.None;

        m_PlayerController = GetComponent<FPSPlayerController>();
        m_WeaponAnimation = GetComponent<Animation>();

        m_WeaponIdle = Resources.Load<AnimationClip>("AnimationClips/WeaponIdle");
        m_WeaponShoot = Resources.Load<AnimationClip>("AnimationClips/WeaponShoot");
        m_AttachCubeAudio = Resources.Load<AudioClip>("Sounds/CubeAttach");

    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(m_BluePortalKey) || Input.GetKeyDown(m_OrangePortalKey))
        {

            if (Time.timeScale == 0) m_CanShootPortal = false;
            else m_CanShootPortal = true;
        }


        if (m_AttachingObject) UpdateAttachedObject(); //If player started attaching a cube, the lerp between cube's position and player's attaching pos

        if (Input.GetKeyDown(m_AttachingObjectKeyCode) && CanAttachObject()) AttachObject(); //Checks whether player can attach an object (when doesnt have an object attached)

        if (m_ObjectAttached && !m_AttachingObject)
        {
            if (Input.GetKeyDown(m_ThrowCubeKey))
            {
                ThrowAttachedObject(m_ThrowForce);
                m_CanShootPortal = false;
            }
            if (Input.GetKeyDown(m_DropCubeKey))
            {
                ThrowAttachedObject(0.0f);
                m_CanShootPortal = false;
            }

        }
        else if (!m_AttachingObject)
        {
            if (Input.GetKeyDown(m_BluePortalKey))// This way always that presses button portal will be disabled, even if pressing right and left button same time
            {
                m_BluePortal.gameObject.SetActive(false);
                if (Time.timeScale != 0) m_CanShootPortal = true;
                if (m_PortalShooting == PortalShooting.None) m_PortalShooting = PortalShooting.Blue; //But if it detects that no portal is being pre-drawed (on shooting process) it sets which portal can be setted until player stops pressing the portal's shooting key.
            }
            if (Input.GetKeyDown(m_OrangePortalKey))
            {
                m_OrangePortal.gameObject.SetActive(false);
                if (Time.timeScale != 0) m_CanShootPortal = true;
                if (m_PortalShooting == PortalShooting.None) m_PortalShooting = PortalShooting.Orange;
            }

            if (Input.GetKey(m_BluePortalKey) && m_PortalShooting == PortalShooting.Blue) PreSetPortalRing(m_BluePortal, m_BlueRingPrePortal);
            else if (Input.GetKey(m_OrangePortalKey) && m_PortalShooting == PortalShooting.Orange) PreSetPortalRing(m_OrangePortal, m_OrangeRingPrePortal);

            if (Input.GetKeyUp(m_BluePortalKey) && m_PortalShooting == PortalShooting.Blue) ShootPortal(m_BluePortal, m_BlueRingPrePortal);
            else if (Input.GetKeyUp(m_OrangePortalKey) && m_PortalShooting == PortalShooting.Orange) ShootPortal(m_OrangePortal, m_OrangeRingPrePortal);
        }
    }

    #region Shooting Portals methods
    void PreSetPortalRing(Portal l_Portal, GameObject l_Ring) //Sets a pre-visualized ring so player knows where and in which scale portal is going to be setted up
    {
        if (!m_CanShootPortal) return;

        ResizingRing(l_Ring);

        Ray l_Ray = GameController.GetGameController().GetPlayer().m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 l_StartPoint = l_Ray.origin;
        Vector3 l_Direction = l_Ray.direction;

        Vector3 l_PortalPosition;
        Vector3 l_PortalNormal;
        bool l_IsNonDrawableWall;

        var l_isValid = l_Portal.GetComponent<Portal>().IsValidPosition(l_StartPoint, l_Direction, m_MaxShootDistance, m_LayerMask, out l_PortalPosition, out l_PortalNormal, out l_IsNonDrawableWall);

        if (l_isValid)
        {
            l_Ring.SetActive(true);
        }
        else
        {
            if (l_IsNonDrawableWall) m_Cross.SetActive(true); //If raycasts hit a non drawable wall it shows a red cross
            else m_Cross.SetActive(false);
            l_Ring.SetActive(false);
        }
    }

    void ResizingRing(GameObject l_Ring) //Increments or decrements Portal scale value through mouse wheel input
    {
        if (Input.mouseScrollDelta == Vector2.up && l_Ring.transform.localScale.x < m_MaxLocalScaleValue) //If wheels up increment by 0.05
        {
            l_Ring.transform.localScale += new Vector3(m_LocalScaleSpeed, m_LocalScaleSpeed, m_LocalScaleSpeed);
            if (l_Ring.transform.localScale.x > m_MaxLocalScaleValue) l_Ring.transform.localScale = new Vector3(m_MaxLocalScaleValue, m_MaxLocalScaleValue, m_MaxLocalScaleValue);
        }
        else if (Input.mouseScrollDelta == -Vector2.up && l_Ring.transform.localScale.x > m_MinLocalScaleValue)
        {
            l_Ring.transform.localScale -= new Vector3(m_LocalScaleSpeed, m_LocalScaleSpeed, m_LocalScaleSpeed);
            if (l_Ring.transform.localScale.x < m_MinLocalScaleValue) l_Ring.transform.localScale = new Vector3(m_MinLocalScaleValue, m_MinLocalScaleValue, m_MinLocalScaleValue);
        }
    }
    void ShootPortal(Portal l_Portal, GameObject l_Ring)
    {
        if(Time.deltaTime == 0) return;
        m_PortalShooting = PortalShooting.None;
        if (!m_CanShootPortal) return;

        if (l_Ring.activeSelf)
        {
            m_WeaponAnimation.CrossFade(m_WeaponShoot.name);
            m_WeaponAnimation.CrossFadeQueued(m_WeaponIdle.name, m_WeaponShoot.length);
            l_Portal.transform.localScale = l_Ring.transform.localScale;
            l_Portal.gameObject.SetActive(true);
            foreach (Transform t in l_Portal.m_ValidPoints)
            {
                Instantiate(m_PortalParticles, t.position, Quaternion.identity);
            }
            SoundController.GetSoundController().PlayOneShot(m_ShootPortalAudio);

        }
        else l_Portal.gameObject.SetActive(false);

        l_Ring.SetActive(false);
        m_Cross.SetActive(false);

        ChangeCrossHair();
    }
    void PlayerDied()
    {
        m_BluePortal.gameObject.SetActive(false);
        m_OrangePortal.gameObject.SetActive(false);
        m_PortalShooting = PortalShooting.None;
        ChangeCrossHair();
    }
    void ChangeCrossHair()
    {
        if (!m_BluePortal.gameObject.activeSelf && !m_OrangePortal.gameObject.activeSelf) { SetCrossHair(m_PortalCrossHair); return; }

        if (m_BluePortal.gameObject.activeSelf && m_OrangePortal.gameObject.activeSelf) { SetCrossHair(m_PortalCrossHairBO); return; }

        if (m_BluePortal.gameObject.activeSelf) { SetCrossHair(m_PortalCrossHairB); return; }

        if (m_OrangePortal.gameObject.activeSelf) { SetCrossHair(m_PortalCrossHairO); return; }
    }
    void SetCrossHair(Sprite l_Crosshair)
    {
        m_CrossHairImage.sprite = l_Crosshair;
    }

    #endregion

    #region Attaching objects methods
    bool CanAttachObject()
    {
        return m_ObjectAttached == null;
    }
    void AttachObject()
    {

        Ray l_Ray = m_PlayerController.m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RayCastHit;
        if (Physics.Raycast(l_Ray, out l_RayCastHit, m_MaxDistanceToAttachObject, m_AttachObjectLM.value))
        {
            foreach (string AttachableTag in GameController.GetGameControllerData().m_AttachableObjectsTags)
            {
                if (l_RayCastHit.collider.CompareTag(AttachableTag))
                {
                    m_AttachingObject = true;
                    m_ObjectAttached = l_RayCastHit.collider.GetComponent<Rigidbody>();
                    m_AttachingObjectStartRotation = l_RayCastHit.collider.transform.rotation;
                    l_RayCastHit.collider.GetComponent<Rigidbody>().isKinematic = true;
                    SoundController.GetSoundController().PlayOneShot(m_AttachCubeAudio);
                    if(m_ObjectAttached.GetComponent<ICube>() != null) m_ObjectAttached.GetComponent<ICube>().SetTeleportable(false);
                    m_UsingAttachingParticlesGO = Instantiate(m_AttachingParticles, l_RayCastHit.collider.transform);
                }
            }
            
        }


    }
    void ThrowAttachedObject(float ThrowForce)
    {
        if (m_ObjectAttached != null && Time.timeScale != 0)
        {
            m_ObjectAttached.transform.SetParent(null);
            m_ObjectAttached.isKinematic = false;
            m_ObjectAttached.AddForce(m_PlayerController.m_PitchController.forward * ThrowForce);
            if (m_ObjectAttached.GetComponent<ICube>() != null) m_ObjectAttached.GetComponent<ICube>().SetTeleportable(true);
            if (ThrowForce > 0)
            {
                m_ShootCubeParticle.Play();
                SoundController.GetSoundController().PlayOneShot(m_ShootCubeAudio);
            }
            m_WeaponAnimation.CrossFade(m_WeaponShoot.name);
            m_WeaponAnimation.CrossFadeQueued(m_WeaponIdle.name, m_WeaponShoot.length);
            m_ObjectAttached = null;
        }
    }
    void UpdateAttachedObject()
    {
        Vector3 l_EulerAngles = m_AttachingPosition.rotation.eulerAngles;
        Vector3 l_Direction = m_AttachingPosition.transform.position - m_ObjectAttached.transform.position;
        float l_Distance = l_Direction.magnitude;
        float l_Movement = m_AttachingObjectSpeed * Time.deltaTime;

        if (l_Movement >= l_Distance)
        {
            Destroy(m_UsingAttachingParticlesGO.gameObject);
            m_AttachingObject = false;
            m_ObjectAttached.transform.SetParent(m_AttachingPosition);
            m_ObjectAttached.transform.localPosition = Vector3.zero;
            m_ObjectAttached.transform.localRotation = Quaternion.identity;
            m_ObjectAttached.MovePosition(m_AttachingPosition.position);
            m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
        }
        else
        {
            l_Direction /= l_Distance;
            m_ObjectAttached.MovePosition(m_ObjectAttached.transform.position + l_Direction * l_Movement);
            m_ObjectAttached.MoveRotation(Quaternion.Lerp(m_AttachingObjectStartRotation, Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z), 1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));
        }
    }
    #endregion
}
public enum PortalShooting { Blue, Orange, None }//If player presses for blue, until player does not end up pressing for blue, orange shooting process will not work
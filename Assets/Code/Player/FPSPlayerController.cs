using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerController : MonoBehaviour
{
    //Conventions given by Jordi.
    //m_ = prefix for class variables
    //l_ = prefix for local variables

    [HideInInspector] public float m_Yaw; // Yaw angle: rotation of player in horizontal axis (X)
    [HideInInspector] public float m_Pitch; //Pitch angle: rotation of player in vertical axis (Y)

    [Header("Player Components")]
    public CharacterController m_CharacterController;
    public Transform m_PitchController;
    public Camera m_Camera;
    public Camera m_WeaponCamera;

    [Header("Speeds")]//Player's speed
    public float m_walkSpeed;
    public float m_runSpeed;
    public float m_jumpSpeed;
    [HideInInspector] public float m_Speed;

    [Header("Rotation Speed")] //Controls the sensibility of the mouse for the player
    [SerializeField][Range(180,540)] float m_YawRotationalSpeed;
    [SerializeField][Range(0,360)]   float m_PitchRotationalSpeed;

    [Header("Pitch Variables")] //Controls Pitch variables as how up/down can player look
    [SerializeField][Range(-100,-50)] float m_MinPitch;
    [SerializeField][Range(50,100)] float m_MaxPitch;

    [Header("KeyCodes")] //Keys
    public KeyCode m_WKeyCode = KeyCode.W;
    public KeyCode m_SKeyCode = KeyCode.S;
    public KeyCode m_AKeyCode = KeyCode.A;
    public KeyCode m_DKeyCode = KeyCode.D;
    public KeyCode m_ShiftKeyCode = KeyCode.LeftShift;
    public KeyCode m_SpaceKeyCode = KeyCode.Space;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;

    [Header("LifeHacks for Unity Editor")]
    bool m_AimLocked = true;
    bool m_AngleLocked = false;


    [Header("CameraVariables")]
    [SerializeField][Range(0, 0.5f)] float m_smoothChangeFOV;
    public float m_NormalMovementFOV = 60f;
    public float m_RunMovementFOV = 75.0f;

    [Header("Other Variables")]
    public bool m_UseYawInverted;
    public bool m_UsePitchInverted;
    [HideInInspector] public Vector3 m_Direction;

    [HideInInspector] public float m_VerticalSpeed = 0.0f;
    bool m_OnGround = true;
    float m_TimeOnAir;





    void Start()
    {
        m_Yaw = transform.rotation.y;
        m_Pitch = m_PitchController.localRotation.x;
        Cursor.lockState = CursorLockMode.Locked;

        GameController.GetGameController().SetPlayer(this);

        
        
    }

    void Update()
    {
        if (Time.timeScale == 0.0f) return;
        //Initialize Vectors
        Vector3 l_RightDirection = transform.right; 
        Vector3 l_ForwardDirection = transform.forward;
        m_Direction = Vector3.zero;
       
        float l_mouseX = Input.GetAxis("Mouse X");
        float l_mouseY = Input.GetAxis("Mouse Y");
#if UNITY_EDITOR
        if (m_AngleLocked)
        {
            l_mouseX = 0.0f;
            l_mouseY = 0.0f;
        }
#endif
        m_Yaw += l_mouseX * m_YawRotationalSpeed * Time.deltaTime*(m_UseYawInverted? -1.0f:1.0f);

        m_Pitch += l_mouseY * m_PitchRotationalSpeed * Time.deltaTime * (m_UsePitchInverted ? -1.0f : 1.0f);
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch); //clamps how up/down can player looks


        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);//rotation of the player horizonal axis
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f); //rotation of the weapon's vertical axis

        //Forward Movement
        if (Input.GetKey(m_WKeyCode)) m_Direction = l_ForwardDirection;
        if (Input.GetKey(m_SKeyCode)) m_Direction = -l_ForwardDirection;

        //Right/Left Movement
        if (Input.GetKey(m_DKeyCode)) m_Direction += l_RightDirection;
        if (Input.GetKey(m_AKeyCode)) m_Direction -= l_RightDirection;

        //Jump
        if (Input.GetKeyDown(m_SpaceKeyCode) && m_TimeOnAir < 0.1f) m_VerticalSpeed = m_jumpSpeed; //if has touched ground less than 0.1f ago (is on ground, coyoteJump or bugfix for skin width component of the character controller (fixes as due to his width not every frame controller detects collision))

        m_Direction.Normalize(); //normalizes vector as if it touches 2 input it doesnt add one vector to another going faster

        m_Speed = Input.GetKey(m_ShiftKeyCode) ? m_runSpeed : m_walkSpeed; //Sets Speed depending if running or walking

        //Sets FOV depending if running or walking
        float l_FOV = Input.GetKey(m_ShiftKeyCode) && m_Direction != Vector3.zero ? m_RunMovementFOV : m_NormalMovementFOV; 
        m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, l_FOV, m_smoothChangeFOV); //Lerp = Interpolation between two FOV settings
        m_WeaponCamera.fieldOfView = Mathf.Lerp(m_WeaponCamera.fieldOfView, l_FOV, m_smoothChangeFOV);

        Vector3 l_Movement = m_Direction * m_Speed * Time.deltaTime; //Sets Player Movement

        //Sets Player Vertical Speed (gravity)
        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags =  m_CharacterController.Move(l_Movement);

        //Collision Flag is just for checking if player is colliding with something behind, if he is, VerticalSpeed should be 0 as he is not falling.
        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f) m_VerticalSpeed = 0.0f;

        if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)
        {
            m_VerticalSpeed = 0f;
            m_OnGround = true;
        }
        else
        {
            m_OnGround = false;
        }

        if (!m_OnGround) m_TimeOnAir += Time.deltaTime;
        else m_TimeOnAir = 0;

        if (m_VerticalSpeed < 0.0f) m_VerticalSpeed *=1.05f;
        m_VerticalSpeed = Mathf.Clamp(m_VerticalSpeed, -10, 10);
        /* TO UNDERSTAND HOW COLLISIONFLAGS WORK
        if ((controller.collisionFlags & CollisionFlags.Sides) != 0)
        {
            print("Touching sides!");
        }

        if (controller.collisionFlags == CollisionFlags.Sides)
        {
            print("Only touching sides, nothing else!");
        }

        This is because of logic portal stuff. 0001 == touches sides. 0101 == also touches sides, but also above. 
        */


        #if UNITY_EDITOR
        UpdateInputDebug();
#endif

       
    }


    private void UpdateInputDebug()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
    }




}

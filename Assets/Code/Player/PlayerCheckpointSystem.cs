using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCheckpointSystem : MonoBehaviour
{
    bool m_IsPlayerDead;
    public GameObject m_DeathCanvas;
    FPSPlayerController m_PlayerController;

    //Position and rotation when passing checkpoint
    Vector3 m_respawnPosition;
    float m_YawOnCheckpoint;
    float m_PitchOnCheckpoint;

    //Actions that can be used when death/respawn
    public static Action OnPlayerRespawn; 
    public static Action OnPlayerDeath;

    //AudioClips
    AudioClip m_ClickButton;
    AudioClip m_DieSound;

    void Start()
    {
        m_PlayerController = GameController.GetGameController().GetPlayer();

        m_ClickButton = Resources.Load<AudioClip>("Sounds/Button");
        m_DieSound = Resources.Load<AudioClip>("Sounds/Die");

        SetCheckpoint();
        m_DeathCanvas.SetActive(false);
        m_IsPlayerDead = false;


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            PlayerDie();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("DeadZone"))
        {
            PlayerDie();
        }
    }

    public void PlayerDie() //When Player die
    {
        if (m_IsPlayerDead) return;
        m_IsPlayerDead = true;
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.None;
        SoundController.GetSoundController().PlayOneShot(m_DieSound);
        Cursor.visible = true;
        m_DeathCanvas.SetActive(true);
        OnPlayerDeath?.Invoke();

    }

    public void Respawn() //When player respawn
    {
        m_IsPlayerDead = false;
        SoundController.GetSoundController().PlayOneShot(m_ClickButton);

        m_PlayerController.m_CharacterController.enabled = false;
        m_DeathCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_PlayerController.gameObject.transform.position = m_respawnPosition;
        m_PlayerController.m_VerticalSpeed = 0.0f;

        m_PlayerController.m_Yaw = m_YawOnCheckpoint;
        m_PlayerController.m_Pitch = m_PitchOnCheckpoint;

        Time.timeScale = 1.0f;
        OnPlayerRespawn?.Invoke();
        m_PlayerController.m_CharacterController.enabled = true;

    }

    public void Exit()
    {
        SoundController.GetSoundController().PlayOneShot(m_ClickButton);
        Application.Quit();
    }

    public void SetCheckpoint(GameObject obj = null) //sets checkpoint when entering a trigger zone
    {
        Debug.Log("Checkpoint position checked at " + transform.position);
        m_respawnPosition = m_PlayerController.gameObject.transform.position;

        m_PitchOnCheckpoint = m_PlayerController.m_Pitch;
        m_YawOnCheckpoint = m_PlayerController.m_Yaw;
        Destroy(obj);
    }
    public void RestartGame() //Restarts game
    {
        m_DeathCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        Time.timeScale = 1.0f;
        GameController.GetGameController().ChangeScene(0); 
    }
}

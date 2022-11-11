using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    Animation m_DoorAnimation;

    AudioClip m_OpenDoorSound;

    void Start()
    {
        m_DoorAnimation = GetComponent<Animation>();
        m_OpenDoorSound = Resources.Load<AudioClip>("Sounds/DoorOpen");


    }

    public void DoorButtonPressed()
    {
        m_DoorAnimation.Play();
        SoundController.GetSoundController().PlayOneShot3D(gameObject, m_OpenDoorSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            DoorButtonPressed();
        }
    }
}

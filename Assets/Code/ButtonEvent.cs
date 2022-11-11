using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonEvent : MonoBehaviour
{
    public UnityEvent m_Action;
    public MeshRenderer m_Button;
    Material m_ButtonPressedMaterial;
    Material m_ButtonNotPressedMaterial;

    bool m_CanTriggerBeUsed;

    public bool m_CanWorkAgainInTimerSeconds = false;
    public float m_TimerToWorkAgain = 5f;

    AudioClip m_ButtonPressedAudioClip;

    private void Start()
    {
        m_ButtonNotPressedMaterial = m_Button.material;
        m_ButtonPressedMaterial = Resources.Load<Material>("Materials/Door/ButtonPressed");
        m_ButtonPressedAudioClip = Resources.Load<AudioClip>("Sounds/RedButton");
        m_CanTriggerBeUsed = true;
    }
    public void OnTriggerEnter(Collider other)
    {
        foreach (string CubeTag in GameController.GetGameControllerData().m_CubeTags)
        {
            if (other.CompareTag(CubeTag))
            {
                UseButton();
            }
        }
    }
    public void UseButton()
    {
        if (!m_CanTriggerBeUsed) return;

        m_Action.Invoke();
        m_Button.material = m_ButtonPressedMaterial;
        m_CanTriggerBeUsed = false;
        SoundController.GetSoundController().PlayOneShot3D(gameObject, m_ButtonPressedAudioClip);

        if (m_CanWorkAgainInTimerSeconds)
        {
            Invoke("MakeButtonUsable", m_TimerToWorkAgain);
        }
    }
    void MakeButtonUsable()
    {
        m_Button.material = m_ButtonNotPressedMaterial;
        m_CanTriggerBeUsed = true;
    }
}

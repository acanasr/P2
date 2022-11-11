using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointPickUp : PickUpClass
{
    PlayerCheckpointSystem m_CheckPoint;

    Animation m_Animation;

    private void Start()
    {
        m_clip = Resources.Load<AudioClip>("Sounds/Checkpoint");
        m_ShouldBeDestroyedWhenPicked = false;
        m_ShouldBeUnabledColliderWhenPicked = true;

        m_Animation = GetComponent<Animation>();    

        
    }
    public override bool CanDoEffect()
    {
        return true;
    }

    public override void DoEffect()
    {
        m_CheckPoint = GameController.GetGameController().GetPlayer().GetComponent<PlayerCheckpointSystem>();
        m_CheckPoint.SetCheckpoint();
        SoundController.GetSoundController().PlayOneShot(gameObject, m_clip, 0.5f);
        m_Animation.Play();
    }

    public override void DisableObject()
    {
        GetComponent<Collider>().enabled = false;
    }

    public override void EnableObject()
    {
    }
}

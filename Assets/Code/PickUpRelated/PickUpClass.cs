using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUpClass : MonoBehaviour
{
    [Header("Do not change on inspector")]
    public AudioClip m_clip;

    [Header("Change in Inspector as wanted")]
    public bool m_ShouldBeDestroyedWhenPicked = true;
    public bool m_ShouldBeUnabledColliderWhenPicked = false;
    public abstract bool CanDoEffect();
    public abstract void DoEffect();

    public abstract void DisableObject();
    public abstract void EnableObject();
}

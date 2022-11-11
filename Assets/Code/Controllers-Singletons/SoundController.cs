using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    static SoundController m_SoundController = null;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static SoundController GetSoundController()
    {
        if (m_SoundController == null)
        {
            m_SoundController = new GameObject("SoundController").AddComponent<SoundController>();
        }
        return m_SoundController;
    }
    public static void DestroySingleton()
    {
        if (m_SoundController != null)
        {
            GameObject.Destroy(m_SoundController.gameObject);
        }
        m_SoundController = null;
    }
    public void PlayOneShot(AudioClip clip, float volume = 1.0f)
    {
        AudioSource l_AudioSource = gameObject.AddComponent<AudioSource>();
        l_AudioSource.volume = volume;
        l_AudioSource.PlayOneShot(clip);
        StartCoroutine(DestroyAudioSource(l_AudioSource, clip.length));
    }
    public void PlayOneShot(GameObject obj,AudioClip clip, float volume = 1.0f)
    {
        AudioSource l_AudioSource = obj.AddComponent<AudioSource>();
        l_AudioSource.volume = volume;
        l_AudioSource.PlayOneShot(clip);
        StartCoroutine(DestroyAudioSource(l_AudioSource, clip.length));
    }
    public void PlayOneShot3D(GameObject obj, AudioClip clip, float volume = 1.0f)
    {
        AudioSource l_AudioSource = obj.AddComponent<AudioSource>();
        l_AudioSource.spatialBlend = 1.0f;
        l_AudioSource.volume = volume;
        l_AudioSource.PlayOneShot(clip);
        StartCoroutine(DestroyAudioSource(l_AudioSource, clip.length));
    }

    public AudioSource PlayLoop(GameObject obj, AudioClip clip, float volume = 1.0f)
    {
        AudioSource l_AudioSource = obj.AddComponent<AudioSource>();
        l_AudioSource.volume = volume;
        l_AudioSource.clip = clip;
        l_AudioSource.loop = true;


        l_AudioSource.Play();
        return l_AudioSource;
    }
    public AudioSource PlayLoop3D(GameObject obj, AudioClip clip, float SpatialBlend = 1.0f, float volume = 1.0f)
    {
        AudioSource l_AudioSource = obj.AddComponent<AudioSource>();
        l_AudioSource.volume = volume;
        l_AudioSource.clip = clip;
        l_AudioSource.loop = true;
        l_AudioSource.spatialBlend = SpatialBlend;

        l_AudioSource.Play();

        return l_AudioSource;
    }
    public void DestroyAudioSource(AudioSource source)
    {
        Destroy(source);
    }

    IEnumerator DestroyAudioSource(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(source);
    }
}

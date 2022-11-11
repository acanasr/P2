using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public Companion m_CompanionPrefab;
    public List<Transform> m_SpawnPosition;
    public float m_InstantiateSpeed;

    AudioClip m_CubeSpawnClip;
    ParticleSystem m_SpawnerParticle;

    private void Start()
    {
        m_CubeSpawnClip = Resources.Load<AudioClip>("Sounds/CannonShot");
        m_SpawnerParticle = Resources.Load<ParticleSystem>("Particles/Explosion");
    }

    public void CreateCube()
    {
        foreach (Transform spawner in m_SpawnPosition)
        {
            GameObject cube = Instantiate(m_CompanionPrefab.gameObject, spawner.position, spawner.rotation);
            cube.GetComponent<Rigidbody>().AddForce(spawner.forward * m_InstantiateSpeed);
            Instantiate(m_SpawnerParticle, spawner.position, Quaternion.identity);
            SoundController.GetSoundController().PlayOneShot3D(spawner.gameObject, m_CubeSpawnClip);
        }
    }
}

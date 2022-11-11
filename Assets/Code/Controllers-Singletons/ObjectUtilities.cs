using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUtilities : MonoBehaviour
{
    static ObjectUtilities m_ObjectUtilities = null;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static ObjectUtilities GetObjectUtilities()
    {
        if (m_ObjectUtilities == null)
        {
            m_ObjectUtilities = new GameObject("ObjectUtilities").AddComponent<ObjectUtilities>();
        }
        return m_ObjectUtilities;
    }
    public static void DestroySingleton()
    {
        if (m_ObjectUtilities != null)
        {
            GameObject.Destroy(m_ObjectUtilities.gameObject);
        }
        m_ObjectUtilities = null;
    }

    public Transform[] MakeItRagdoll(Transform parent)
    {
        
        Transform[] ret = new Transform[parent.childCount+1];
        int index = 0;
        ret[index] = parent;

        foreach (Transform child in parent)
        {
            index++;
            child.gameObject.AddComponent<BoxCollider>();
            child.gameObject.AddComponent<Rigidbody>();
            ret[index] = child;
        }
        parent.DetachChildren();
        return ret;
    }

    public void DestroyTransformArray(Transform[] array, float time)
    {
        foreach (Transform obj in array)
        {
            Destroy(obj.gameObject, time);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameControllerData", menuName = "Resources/ScriptableObjects/GameControllerData", order =0)]
public class GameControllerData : ScriptableObject
{
    //Saves all data that needs to be saved and loaded on sceneload

    public string[] m_AttachableObjectsTags;

    public string[] m_CubeTags;

}

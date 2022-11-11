using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    static GameController m_GameController = null;

    static GameObject fades;
    int m_SceneIndex;

    static GameControllerData m_GameControllerData;

    FPSPlayerController m_PlayerController;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        fades = Resources.Load<GameObject>("Prefabs/Fades");
        SetFadeOutCanvas();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SetFadeOutCanvas();
    }

    #region SingletonStuff
    public static GameController GetGameController()
    {
        if(m_GameController == null)
        {
            m_GameController = new GameObject("GameController").AddComponent<GameController>();
            m_GameControllerData = Resources.Load<GameControllerData>("ScriptableObjects/GameControllerData/GameControllerData");

        }
        return m_GameController;
    }
    public static GameControllerData GetGameControllerData()
    {
        GetGameController();
        return m_GameControllerData;
    }
    public void SetPlayer(FPSPlayerController Player)
    {
        m_PlayerController = Player;
    }
    public FPSPlayerController GetPlayer()
    {
        if (m_PlayerController == null) m_PlayerController = FindObjectOfType<FPSPlayerController>();
        return m_PlayerController;
    }
    public static void DestroySingleton()
    {
        if (m_GameController != null)
        {
            GameObject.Destroy(m_GameController.gameObject);
        }
        m_GameController = null;
    }
    #endregion
    
    public void ChangeScene(int index)
    {
        SetFadeInCanvas();
        m_SceneIndex = index;
       
    }

    #region FadeInOutSetters
    public void SetFadeInCanvas()
    {
        GameObject l_FadeInGO = Instantiate(fades);

        Image l_FadeImage = l_FadeInGO.GetComponentInChildren<Image>();
        l_FadeImage.color = new Color(0, 0, 0, 0f);

        StartCoroutine(SetFadeIn(l_FadeInGO, l_FadeImage));
    }
    IEnumerator SetFadeIn(GameObject FadeInGO,  Image l_FadeImage)
    {
        float l_AlphaColor = l_FadeImage.color.a;
        while (l_FadeImage.color.a <= 1f) { 
            l_AlphaColor += Time.deltaTime;
            l_FadeImage.color = new Color(0, 0, 0, l_AlphaColor); 
            yield return new WaitForEndOfFrame();
        }
        Destroy(FadeInGO);
        SceneManager.LoadSceneAsync(m_SceneIndex);

    }
    public void SetFadeOutCanvas()
    {
        GameObject l_FadeOutGO = Instantiate(fades);

        Image l_FadeImage = l_FadeOutGO.GetComponentInChildren<Image>();
        l_FadeImage.color = new Color(0, 0, 0, 1.0f);

        StartCoroutine(SetFadeOut(l_FadeOutGO, l_FadeImage));
    }

    IEnumerator SetFadeOut(GameObject FadeOutGO, Image l_FadeImage)
    {
        float l_AlphaColor = l_FadeImage.color.a;
        while (l_FadeImage.color.a >= 0f)
        {
            l_AlphaColor -= Time.deltaTime;
            l_FadeImage.color = new Color(0, 0, 0, l_AlphaColor);
            yield return new WaitForEndOfFrame();
        }
        Destroy(FadeOutGO);
    }
    #endregion


}

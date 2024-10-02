using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  Global singleton to manage overall game logic.
/// </summary>
public class GameManager : MonoBehaviour
{
    private GameManager instance;

    public GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    instance = singletonObject.AddComponent<GameManager>();
                    DontDestroyOnLoad(singletonObject);
                    Debug.Log("GameManager created.");
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("GameManager initialized.");
        // In bootstrap scene, go to the main scene.
        if(SceneManager.GetActiveScene().name == "BootstrapScene")
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    
}

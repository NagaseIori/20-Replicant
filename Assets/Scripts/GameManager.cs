using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  Global singleton to manage overall game logic.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static readonly double PixelsPerUnit = 100;

    private static GameManager instance;

    public static GameManager Instance
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

    /// Player Functions
    public static Player GetPlayer() {
        return FindObjectOfType<Player>();
    }
    public static Vector2 GetPlayerPosition() {
        var player = GetPlayer();
        if(player == null) return Camera.main.transform.position;
        return GetPlayer().transform.position;
    }
    public static bool IsPlayerAlive() {
        return GetPlayer() != null;
    }

    /// Utils Functions
    
    public static Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public static Vector2 GetMouseLocalPosition(Transform trans)
    {
        var worldPos = GetMouseWorldPosition();
        var worldPos3 = new Vector3(worldPos.x, worldPos.y, trans.position.z);
        return trans.InverseTransformPoint(worldPos3);
    }
    public static double PixelToUnit(double pixels)
    {
        return pixels / PixelsPerUnit;
    }
}

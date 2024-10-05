using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public float poiShift = 0.02f;          // Lerp value between player and mouse

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get the poi.
        var POI = Vector2.Lerp(GameManager.GetPlayerPosition(), GameManager.GetMouseWorldPosition(), poiShift);

        // Set camera's position.
        var npos = transform.position;
        npos.x = POI.x;
        npos.y = POI.y;
        transform.position = npos;
    }
}

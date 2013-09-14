using UnityEngine;
using System.Collections;

public class SceneScript : MonoBehaviour 
{
    public static float s_customTimeScale=0.0f;
	// Use this for initialization
	void Start () 
    {
        //Screen.showCursor = false;
        //Screen.lockCursor = true;
        Random.seed = (int)(Time.realtimeSinceStartup*10000);
        s_customTimeScale = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
    {
        //Screen.lockCursor = true;
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        //Screen.showCursor = false;
	}

    public static float deltaTime()
    {
        return Time.deltaTime * s_customTimeScale;
    }
}

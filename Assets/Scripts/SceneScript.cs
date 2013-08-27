using UnityEngine;
using System.Collections;

public class SceneScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        Screen.showCursor = false;
        Screen.lockCursor = true;
        Random.seed = (int)(Time.realtimeSinceStartup*10000);
	}
	
	// Update is called once per frame
	void Update () 
    {
        Screen.lockCursor = true;
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        //Screen.showCursor = false;
	}
}

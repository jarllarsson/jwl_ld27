using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour {
    public float m_time = 6.0f;
    public float m_maxSpeed = 10.0f;
    private Vector3 m_currentVel=Vector3.zero;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (SceneScript.s_customTimeScale > 0.0f)
        {
            transform.position = Vector3.zero;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position,
                                                        Vector3.zero, ref m_currentVel,
                                                        m_time, m_maxSpeed);
        }

	}
}

using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour 
{
    public float m_followTime;
    public float m_followMaxSpeed;
    private Vector3 m_currentVel;

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (GlobalTime.getState() == GlobalTime.State.ADVANCING)
            smoothFollow();
	}

    void Update()
    {
        if (GlobalTime.getState() == GlobalTime.State.REWINDING)
            smoothFollow();
    }

    void smoothFollow()
    {
        if (SceneScript.s_customTimeScale>0.0f && !GameOverScript.m_gameEnd)
        {
            Transform target = StaticPlayerHandle.m_player;
            Vector3 goal = new Vector3(target.position.x, target.position.y,
                                       transform.position.z);
            float multiplier = GlobalTime.getCurrentSpeedup();
            transform.position = Vector3.SmoothDamp(transform.position,
                                                    goal, ref m_currentVel,
                                                    m_followTime / multiplier, m_followMaxSpeed * multiplier);
        }
    }
}

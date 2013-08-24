using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour 
{
    public Transform m_target;
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
        Vector3 goal = new Vector3(m_target.position.x, m_target.position.y,
                                   transform.position.z);
        float multiplier = GlobalTime.getCurrentSpeedup();
	    transform.position = Vector3.SmoothDamp(transform.position,
                                                goal,ref m_currentVel,
                                                m_followTime / multiplier, m_followMaxSpeed * multiplier);
    }
}

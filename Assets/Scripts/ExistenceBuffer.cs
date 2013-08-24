using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExistenceBuffer : MonoBehaviour 
{
    private class TimeState
    {
        public Vector3 m_position;
        public Quaternion m_rotation;
        public Vector3 m_scale;
    }

    private List<TimeState> m_buffer;


    // markers
    private float m_startTime;
    private float m_endTime;
    private float m_currentDeltaTime;
    // step size
    private static float m_stepSizeSec;
    private static float m_maxBufferSizeSec = 11.0f; // 10 seconds+security padding
    // mode
    private bool m_readingFromBuffer;


	// Use this for initialization
	void Start () 
    {
        m_startTime = GlobalTime.getTime();
        //m_buffer = new List<TimeState>
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public bool isReadingFromBuffer()
    {
        return m_readingFromBuffer;
    }



}

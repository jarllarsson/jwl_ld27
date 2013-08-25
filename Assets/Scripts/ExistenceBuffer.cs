using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExistenceBuffer : MonoBehaviour 
{
    private class TimeState
    {
        public TimeState(Vector3 p_position,
            Quaternion p_rotation, Vector3 p_scale)
        {
            m_position = new Vector2(p_position.x,p_position.y);
            m_rotation = new Vector4(p_rotation.x,p_rotation.y,p_rotation.z,p_rotation.w);
            m_scale = p_scale;
        }

        public TimeState(Vector3 p_position,
                         Vector2 p_frameData, Vector3 p_scale)
        {
            m_position = new Vector2(p_position.x, p_position.y);
            m_rotation = new Vector4(p_frameData.x, p_frameData.y, 0.0f,0.0f);
            m_scale = p_scale;
        }

        public Vector2      m_position;
        public Vector4      m_rotation;
        public Vector3      m_scale;
    }

    public enum TimeScaleState
    {
        READING_BUFFER,     // Reading existing data
        WRITING_BUFFER,     // Writing new data
        NOT_SPAWNED         // At point before data was written
    }

    List<TimeState> m_buffer;
    //TimeState m_oldState;

    // markers
    float m_startTime;
    float m_endTime;
    float m_currentDeltaTime;
    float m_oldTime;
    // step size
    private static float m_stepSizeSec=0.02f;
    private static float m_maxBufferSizeSec = 10.0f; // 10 seconds
    private int m_maxBufferLength = 0;
    // mode
    TimeScaleState m_timeScaleState=TimeScaleState.WRITING_BUFFER;
    private bool m_cloned = false;

    public bool m_hideWhenNotSpawned = false;
    private Renderer[] m_renderers;
    private bool m_renderersDisabled = false;
    private int m_rendererReenableTick = 0;
    private FrameData m_frameData;

    public void cloneData(ExistenceBuffer p_other)
    {
        m_cloned = true;
        m_startTime = p_other.m_startTime;
        m_endTime = GlobalTime.getTime();
        m_buffer = new List<TimeState>(p_other.m_buffer.ToArray());
        float rest=0.0f;
        int newLast=System.Math.Max(0,getBufferPosFromTime(m_endTime, out rest));
        m_buffer.RemoveRange(newLast, m_buffer.Count - newLast);
        m_currentDeltaTime = p_other.m_currentDeltaTime;
    }

	// Use this for initialization
	void Start () 
    {
        m_maxBufferLength = (int)(m_maxBufferSizeSec / m_stepSizeSec)+1; // buffer size for all timestates, with padding
        if (!m_cloned)
        {
           // Debug.Log(m_maxBufferLength);
            m_startTime = GlobalTime.getTime();
            m_endTime = m_startTime - 1.0f;
            m_buffer = new List<TimeState>(m_maxBufferLength);
        }
        m_renderers = transform.GetComponentsInChildren<Renderer>();
        m_frameData = transform.GetComponent<FrameData>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        float currentTime = GlobalTime.getTime();
        m_currentDeltaTime = currentTime - m_oldTime;

        // Check if inside buffer, then read from it
        setTimescaleStateBasedOnTime(currentTime);

        // Try to read from buffer
        if (isReadingFromBuffer())
        {
            float t=0.0f;
            int deltaSign = getDeltaSign();
            int currentPos = getBufferPosFromTime(currentTime, out t);
            // int nextPos = getBufferPosFromTime(GlobalTime.getTime(),out t);
            if (currentPos >= 0 && currentPos<m_buffer.Count)
            {
                TimeState currentState = m_buffer[currentPos];
                // TimeState nextState = m_buffer[currentPos];
                transform.position = new Vector3(currentState.m_position.x,currentState.m_position.y,transform.position.z);
                if (m_frameData == null)
                {
                    transform.rotation = new Quaternion(currentState.m_rotation.x, currentState.m_rotation.y, currentState.m_rotation.z, currentState.m_rotation.w);
                }
                else
                {
                    m_frameData.m_frameData = new Vector2(currentState.m_rotation.x, currentState.m_rotation.y);
                }
                transform.localScale = currentState.m_scale;
            }
            else // outside buffer, resume realtime
            {
                setTimescaleStateBasedOnTime(currentTime);
            }

        }

        // second check in case it was not read
        if (isWritingToBuffer())
        {
            float t = 0.0f;
            int currentPos = getBufferPosFromTime(currentTime, out t);
            //
            TimeState newState = null;
            if (m_frameData == null)
            {
                newState = new TimeState(transform.position, transform.rotation, transform.localScale);
            }
            else
            {
                newState = new TimeState(transform.position, m_frameData.m_frameData, transform.localScale);
            }
            
            if (currentPos > m_buffer.Count-1)
            {
                m_buffer.Add(newState);
                m_endTime = currentTime;
            }
            else if (currentPos>=0)
            {
                m_buffer[currentPos] = newState;
            }
            // do not allow more than the specified amount of seconds
            if (m_buffer.Count > m_maxBufferLength)
            {
                m_startTime += m_stepSizeSec;
                m_buffer.RemoveAt(0);
            }
        }

        if (m_hideWhenNotSpawned) hideWhenNotExisting();

        m_oldTime = currentTime;
	}

    void hideWhenNotExisting()
    {
        if (isBeforeBuffer())
        {
            // Hide when not spawned
            if (!m_renderersDisabled)
            {
                foreach (Renderer r in m_renderers)
                {
                    r.enabled = false;
                }
                m_renderersDisabled = true;
                m_rendererReenableTick = 2;
            }
        }
        else if (m_renderersDisabled)
        {
            // Show when spawned and was hidden
            if (m_rendererReenableTick <= 0)
            {
                foreach (Renderer r in m_renderers)
                {
                    r.enabled = true;
                }
                m_renderersDisabled = false;
            }
            m_rendererReenableTick--;
        }
    }

    void setTimescaleStateBasedOnTime(float p_currentTime)
    {
        if (p_currentTime >= m_startTime && p_currentTime < m_endTime)
        {
            m_timeScaleState = TimeScaleState.READING_BUFFER;
            if (rigidbody)
            {
                rigidbody.isKinematic = true;
                //rigidbody.detectCollisions = false;
            }
        }
        else if (p_currentTime >= m_endTime) // outside buffer, resume realtime
        {
            m_timeScaleState = TimeScaleState.WRITING_BUFFER;
            if (rigidbody)
            {
                rigidbody.isKinematic = false;
                rigidbody.WakeUp();
                //rigidbody.detectCollisions = false;
            }
        }
        else
        {
            m_timeScaleState = TimeScaleState.NOT_SPAWNED;
        }
    }

    public float getEndTime()
    {
        return m_endTime;
    }

    public float getStartTime()
    {
        return m_startTime;
    }

    int getDeltaSign()
    {
        float deltaSign = m_currentDeltaTime/Mathf.Abs(m_currentDeltaTime);
        int returnVal = 1;
        if (deltaSign < 0.0f)
            returnVal = -1;
        return returnVal;
    }

    private int getBufferPosFromTime(float p_timeSec, out float p_outRest)
    {
        int returnPos = 0;
        p_outRest = 0.0f;

        float pos = (p_timeSec - m_startTime) / m_stepSizeSec;
        returnPos = (int)pos;
        p_outRest = pos - returnPos;
            
        return returnPos;
    }

    public bool isReadingFromBuffer()
    {
        return m_timeScaleState==TimeScaleState.READING_BUFFER;
    }

    public bool isWritingToBuffer()
    {
        return m_timeScaleState == TimeScaleState.WRITING_BUFFER;
    }

    public bool isBeforeBuffer()
    {
        return m_timeScaleState == TimeScaleState.NOT_SPAWNED;
    }

}

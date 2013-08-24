using UnityEngine;
using System.Collections;

public class GlobalTime : MonoBehaviour 
{
    public enum State
    {
        ADVANCING,
        REWINDING
    }

    private static float m_time=0.0f;
    private static bool m_locked = false;
    private static State m_currentState = State.ADVANCING;

    public float m_dbgTime = 0.0f;

    private static float m_currentRewindAmountSec = 0.0f;

    public bool m_dbgRewind = false;
    public static float m_maxRewindTimeSec = 10.0f;
    public static float m_rewindCooldownTimeSec = 0.0f;



	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_dbgRewind)
        {
            performRewind();
        }
        else
        {
            setRealtimeState();
        }


        if (!m_locked) advanceTime();
        m_dbgTime = m_time;
	}

    private static bool performRewind()
    {
        bool success=false;
        if (getCurrentCooldown() <= 0.0f &&
            getCurrentRewindAmountSec() < m_maxRewindTimeSec)
        {
            m_currentState = State.REWINDING;
            lockTime();
            retardTime();
            success=true;
        }
        else
        {
            setRealtimeState();
        }
        return success;
    }

    private static void setRealtimeState()
    {
        if (m_currentState == State.REWINDING)
        {
            m_rewindCooldownTimeSec = m_currentRewindAmountSec;
            m_currentState = State.ADVANCING;
            m_currentRewindAmountSec = 0.0f;
            unlockTime();
        }
        cooldownRewind();
    }

    private static void cooldownRewind()
    {
        if (m_rewindCooldownTimeSec > 0.0f)
        {
            m_rewindCooldownTimeSec -= Time.deltaTime;
        }
        else
        {
            m_rewindCooldownTimeSec = 0.0f;
        }
    }

    public static float getCurrentRewindAmountSec()
    {
        return m_currentRewindAmountSec;
    }

    public static float getCurrentRewindAmountPercent()
    {
        return m_currentRewindAmountSec / m_maxRewindTimeSec;
    }

    public static float getCurrentCooldown()
    {
        return m_rewindCooldownTimeSec;
    }

    public static float getCurrentCooldownPercent()
    {
        return m_rewindCooldownTimeSec / m_maxRewindTimeSec;
    }

    public static float advanceTime()
    {
        m_time += Time.deltaTime;
        return m_time;
    }

    public static float retardTime() // ahahahahahah. tired
    {
        float oldTime = m_time;
        m_time -= Time.deltaTime;
        if (m_time < 0.0f)
        {
            m_time = 0.0f;
        }
        m_currentRewindAmountSec += oldTime - m_time;
        return m_time;
    }

    public static State getState()
    {
        return m_currentState;
    }

    public static float getTime()
    {
        return m_time;
    }

    public static void lockTime()
    {
        m_locked = true;
    }

    public static void unlockTime()
    {
        m_locked = false;
    }

}

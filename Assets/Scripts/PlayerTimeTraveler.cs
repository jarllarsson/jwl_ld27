using UnityEngine;
using System.Collections;

public class PlayerTimeTraveler : MonoBehaviour 
{
    // Disablescripts
    public MonoBehaviour[] m_scriptsToDisableOnClone;

    public Material m_ghostMaterial;
    private bool m_rewindButtonPressed = false;
    private bool m_rewindButtonReleased = false;
    public ExistenceBuffer m_buffer;
    public Transform m_playerPrefab;
    private GlobalTime.State m_oldState;
    private float m_cooldown = 0.3f;
    private static int   m_ghostCreatedCounter = 0;
    private static float m_maxGhostDepthOffset = 80;
    private static float m_ghostDepthOffsetStep = 4.0f;
    private bool m_justCreated = true;
    public Transform m_timetravelAvailableEffectPrefab;
    public Transform m_timetravelAvailableEffect;

	// Use this for initialization
	void Start () 
    {      
        if (m_buffer == null)
        {
            ExistenceBuffer buffer = transform.GetComponent<ExistenceBuffer>();
            m_buffer = buffer;
        }
	}

    void OnDestroy()
    {
        m_ghostCreatedCounter = 0;
        m_maxGhostDepthOffset = 80;
        m_ghostDepthOffsetStep = 4.0f;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (m_cooldown <= 0.0f && !GameOverScript.m_gameEnd)
        {
            if (m_justCreated && GlobalTime.getCurrentCooldown() <= 0.0f)
            {
                m_justCreated = false;
                showTimeTravelAbility();
            }
       

            // new input
            float rewindButton = Input.GetAxis("Fire2");

            if (rewindButton > 0.5f && GlobalTime.getCurrentCooldown()<=0.0f)
            {
                m_rewindButtonReleased = false;
                m_rewindButtonPressed = true;
                GlobalTime.m_doRewind = true;
            }
            else
            {
                if (m_rewindButtonPressed)
                    m_rewindButtonReleased = true;
                m_rewindButtonPressed = false;

            }

            // If button released when we were reading from buffer and rewinding,
            // Instantiate clone and render this one useless
            bool reset = false;
            //         if (m_buffer.isReadingFromBuffer() && GlobalTime.getState()==GlobalTime.State.REWINDING && 
            //             !m_rewindButtonPressed)
            //         {
            //             reset = true;
            //         }

            if (m_rewindButtonReleased || (m_oldState == GlobalTime.State.REWINDING && GlobalTime.getState() == GlobalTime.State.ADVANCING))
            {
                reset = true;
                m_rewindButtonReleased = false;
            }

            if (reset)
            {
                GlobalTime.m_doRewind = false;
                GlobalTime.setRealtimeState();
                m_oldState = GlobalTime.State.ADVANCING;
                m_ghostCreatedCounter++;

                // Timetravel effect
                if (m_timetravelAvailableEffect != null)
                {
                    DestroyImmediate(m_timetravelAvailableEffect.gameObject);
                }

                // Set up clone
                Transform newPlayer = Instantiate(m_playerPrefab, transform.position, transform.rotation) as Transform;
                newPlayer.name = "Player" + m_ghostCreatedCounter;
                ExistenceBuffer buf = newPlayer.GetComponent<ExistenceBuffer>();
                buf.cloneData(m_buffer);


                // Set this player as a ghost
                Renderer playerRenderer = transform.GetComponentInChildren<Renderer>();
                if (playerRenderer) playerRenderer.material = m_ghostMaterial;
                TimeBasedDisabler disabler = gameObject.AddComponent<TimeBasedDisabler>() as TimeBasedDisabler;
                disabler.m_popEffect = true;
                disabler.m_time = m_buffer.getEndTime();
                transform.position -= Vector3.forward * (1.0f+((m_ghostDepthOffsetStep *(float) m_ghostCreatedCounter)%m_maxGhostDepthOffset));
                m_buffer.m_hideWhenNotSpawned = true;

                
                // destroy scripts on clone
                // Debug.Log("HEJ");
                foreach (MonoBehaviour script in m_scriptsToDisableOnClone)
                {
                    Destroy(script);
                }
            }
        }
        else
        {
            m_cooldown-=Time.deltaTime;
        }

        m_oldState = GlobalTime.getState();
	}


    void showTimeTravelAbility()
    {
        m_timetravelAvailableEffect = Instantiate(m_timetravelAvailableEffectPrefab, transform.position, transform.rotation) as Transform;
        m_timetravelAvailableEffect.parent = transform;
    }
}

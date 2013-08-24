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
    private float m_cooldown = 1.0f;

	// Use this for initialization
	void Start () 
    {      
        if (m_buffer == null)
        {
            ExistenceBuffer buffer = transform.GetComponent<ExistenceBuffer>();
            m_buffer = buffer;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_cooldown <= 0.0f)
        {

            // new input
            float rewindButton = Input.GetAxis("Fire1");

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
                Transform newPlayer = Instantiate(m_playerPrefab, transform.position+Vector3.up*5.0f, transform.rotation) as Transform;
                ExistenceBuffer clonedBuf = newPlayer.gameObject.AddComponent<ExistenceBuffer>() as ExistenceBuffer;
                // Set this player as a ghost
                Renderer playerRenderer = transform.GetComponentInChildren<Renderer>();
                if (playerRenderer) playerRenderer.material = m_ghostMaterial;
                //clonedBuf.cloneData(m_buffer);
                // destroy scripts on clone
                Debug.Log("HEJ");
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
}

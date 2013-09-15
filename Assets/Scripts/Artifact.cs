using UnityEngine;
using System.Collections;

public class Artifact : MonoBehaviour 
{
    public Transform m_loggedHealthObject;
    public ExistenceBuffer m_healthObjBuffer;
    public Transform m_model;
    public Transform m_modelContainer;
    private Vector3 m_modelContainerDefaultPos;
    private Color m_modelDefaultColor;
    public float m_health=100.0f;
    public float m_hitCooldown = 1.0f;
    private float m_hitTick = 0.0f;
    public bool m_dbgHit = false;
    public ParticleSystem m_hurtParticles;
    public ParticleSystem m_moveParticles;
    private float m_defaultYPos;
    private float m_oldTime;
	// Use this for initialization
    public AudioSource m_hurtSound;
    public Transform m_goal;
    public GameOverScript m_gameOverObj;
    public GameOverScript m_winObj;
    bool m_gameDecided = false;
	void Start () 
    {
        m_modelContainerDefaultPos = m_modelContainer.localPosition;
        m_modelDefaultColor = m_model.renderer.material.color;
        m_defaultYPos = transform.localPosition.y;
        m_oldTime = GlobalTime.getTime();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!m_gameDecided && transform.position.y >= m_goal.position.y - 3.0f)
        {
            m_winObj.Run();
            m_gameDecided = true;
        }

        if (!m_gameDecided && m_health<=0.0f)
        {
            m_gameOverObj.Run();
            m_gameDecided = true;
        }

        if (!m_gameDecided)
        {
            if (m_dbgHit)
            {
                damage(0.1f);
                m_dbgHit = false;
            }
            m_modelContainer.localRotation = Quaternion.Euler(new Vector3(m_modelContainer.localRotation.x, 
                Mathf.Rad2Deg*GlobalTime.getTime()*0.5f, 
                m_modelContainer.localRotation.z));

            float delta = 0.5f*(GlobalTime.getTime() - m_oldTime);
            if (delta < 0.0f)
            {
                delta *= 2.5f-(0.1f*(Startscript.m_difficulty-1));
                if (!m_moveParticles.isPlaying) m_moveParticles.Play();
            }
            else
            {
                if (m_moveParticles.isPlaying) m_moveParticles.Stop();
            }
            transform.localPosition += new Vector3(0.0f, -delta, 0.0f);
            if (transform.localPosition.y < m_defaultYPos) 
                transform.localPosition = new Vector3(transform.localPosition.x, m_defaultYPos, transform.localPosition.z);
            m_oldTime = GlobalTime.getTime();

            m_modelContainer.localPosition = m_modelContainerDefaultPos;
            m_model.renderer.material.color = m_modelDefaultColor*m_health*0.01f;
            if (m_hitTick > 0.0f)
            {
                m_hitTick -= SceneScript.deltaTime();
                m_modelContainer.localPosition += new Vector3(Mathf.Sin(m_hitTick*50.0f)*0.7f,0.0f,0.0f);
                m_model.renderer.material.color = new Color(0.5f+Random.Range(0.0f, 1.0f), 0.5f+Random.Range(0.0f, 1.0f), 0.5f+Random.Range(0.0f, 1.0f));
            }
            // m_health = m_loggedHealthObject.localScale.x*100.0f;
            if (m_health < 99.0f)
            {
                if (!m_hurtParticles.isPlaying) m_hurtParticles.Play();
                float hitPrcnt = (100.0f - m_health) * 0.01f;
                float shakeX = Mathf.Sin(GlobalTime.getTime() * (hitPrcnt*50.0f)) * hitPrcnt * Random.Range(-1.0f,1.0f);
                float shakeY = Mathf.Cos(GlobalTime.getTime() * (hitPrcnt * 50.0f)) * hitPrcnt * Random.Range(-1.0f,1.0f);
                m_modelContainer.localPosition += new Vector3(shakeX, shakeY, 0.0f);
                m_hurtParticles.startSize = hitPrcnt * 4.0f;
            }
            else
            {
                if (m_hurtParticles.isPlaying) m_hurtParticles.Stop();
            }
        }
        else
        {
            if (transform.localScale.x > 0.0f)
            {
                transform.localScale -= Vector3.one * SceneScript.deltaTime() * 0.3f;
            }
            else
            {
                transform.localScale = Vector3.zero;
            }

        }
        
	}

    public bool damage(float p_value)
    {
        bool success = false;
        if (m_hitTick <= 0.0f &&
            m_healthObjBuffer.isWritingToBuffer() &&
            GlobalTime.getState()==GlobalTime.State.ADVANCING)
        {
            if (m_hurtSound && !m_hurtSound.isPlaying && GameOverScript.m_gameEnd == false) m_hurtSound.Play();
            m_hitTick = m_hitCooldown;
            m_loggedHealthObject.localScale -= new Vector3(p_value, p_value, p_value);
            success = true;
        }
        return success;
    }

    public bool damage2(float p_value, Transform p_caller)
    {
        bool success = false;
        if (m_hitTick <= 0.0f)
        {
            if (m_hurtSound && !m_hurtSound.isPlaying && GameOverScript.m_gameEnd == false) m_hurtSound.Play();
            m_hitTick = m_hitCooldown;
            m_health -= p_value*100.0f;
            success = true;
            //Debug.Log("hurt by: " + p_caller.gameObject.name);
        }
        return success;
    }

    public void reverseDamage(float p_value)
    {
        //bool success = false;
//         if (m_hitTick <= 0.0f &&
//             m_healthObjBuffer.isWritingToBuffer())
        //{
            //if (m_hurtSound && !m_hurtSound.isPlaying) m_hurtSound.Play();
            //m_hitTick = m_hitCooldown;
            m_health += p_value * 100.0f;
            //success = true;
        //}
        //return success;
    }
}

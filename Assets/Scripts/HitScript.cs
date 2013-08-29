using UnityEngine;
using System.Collections;

public class HitScript : MonoBehaviour 
{
    public float m_hitAnimTimeLim = 1.0f;
    private float m_hitAnimTime = 0.0f;
    public float m_hitBlinkSpeed = 10.0f;
    public float m_hitCooldownTimeLim = 1.0f;
    private float m_hitCooldownTime = 0.0f;
    public float m_hitCooldownBlinkSpeed = 20.0f;
    public float  m_punishAmount = 0.3f;
    public float m_hitPushForce = 10.0f;
    public Transform m_animObj;
    private Vector3 m_hitDir = Vector3.zero;
    private Color m_originalTint;
    public Color m_hurtTint;
    public Color m_cooldownTint;
    private bool m_wasHurtPreviousFrame=false;
    private int m_onlyRaycastLayer = 9;
    private int m_layerMask;
    public Vector2 m_shakeMul;
    public ParticleSystem m_hurtBarPunishmentParticle;
	// Use this for initialization
	void Start () 
    {
        m_hurtBarPunishmentParticle = GameObject.Find("CoolDownPunishmentFx").particleSystem;
        m_layerMask = (1 << m_onlyRaycastLayer);
        m_originalTint = m_animObj.renderer.material.GetColor("_TintColor");
        if (m_hitCooldownTimeLim < m_hitAnimTimeLim)
            m_hitCooldownTimeLim = m_hitAnimTimeLim;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Ray ray = new Ray(transform.position,Vector3.forward);
        RaycastHit hitData;
        float rdist = 200.0f;
        bool hit = Physics.SphereCast(ray, 0.5f, out hitData, rdist, m_layerMask);
        if (hit)
        {
            Debug.DrawLine(transform.position, hitData.point, Color.magenta);
            hitPlayer(hitData.rigidbody.gameObject);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position+Vector3.forward*rdist, Color.grey);
        }

        if (GlobalTime.getState() == GlobalTime.State.ADVANCING)
        {
            m_hitAnimTime -= Time.deltaTime;
            m_hitCooldownTime -= Time.deltaTime;
        }
        if (isCooldown())
        {
            if (m_hitAnimTime > 0.0f)
                animateHurtTint();
            else
                animateCooldownTint();
            m_wasHurtPreviousFrame=true;
        }
        else
        {
            if (m_wasHurtPreviousFrame) resetTint();
            m_wasHurtPreviousFrame=false;
        }
	}

    void resetTint()
    {
        m_animObj.renderer.material.SetColor("_TintColor", m_originalTint);
    }

    void animateHurtTint()
    {
        float sinVal = Mathf.Sin(m_hitAnimTime / m_hitAnimTimeLim * m_hitBlinkSpeed);
        float cosVal = Mathf.Cos(m_hitAnimTime / m_hitAnimTimeLim * m_hitBlinkSpeed);
        transform.position+=Vector3.right*sinVal*m_shakeMul.x;
        transform.position+=Vector3.up * cosVal*m_shakeMul.y;
        if (sinVal > 0.0f)
        {
            m_animObj.renderer.material.SetColor("_TintColor", m_originalTint);
        }
        else
            m_animObj.renderer.material.SetColor("_TintColor", m_hurtTint);
    }

    void animateCooldownTint()
    {
        if (Mathf.Sin(m_hitCooldownTime/m_hitCooldownTimeLim * m_hitCooldownBlinkSpeed) > 0.0f)
            m_animObj.renderer.material.SetColor("_TintColor", m_originalTint);
        else
            m_animObj.renderer.material.SetColor("_TintColor", m_cooldownTint);
    }



    void hitPlayer(GameObject p_hitter)
    {
        if (m_hitCooldownTime <= 0.0f &&
            GlobalTime.getState() == GlobalTime.State.ADVANCING &&
            p_hitter.tag == "Enemy")
        {
            ExistenceBuffer m_enemBuffer = p_hitter.GetComponent<ExistenceBuffer>();
            if (!m_enemBuffer || (m_enemBuffer && !m_enemBuffer.isBeforeBuffer() && m_enemBuffer.isWritingToBuffer()))
            { 
                Debug.Log("HIT!");
                m_hurtBarPunishmentParticle.Play();
                GlobalTime.addCooldownPunishment(m_punishAmount);
                m_hitDir = transform.position - p_hitter.transform.position;
                m_hitDir.y += 0.3f;
                m_hitDir.z = 0.0f; m_hitDir.Normalize();
                rigidbody.AddForce(m_hitDir * m_hitPushForce);
                m_hitAnimTime = m_hitAnimTimeLim;
                m_hitCooldownTime = m_hitCooldownTimeLim;
            }
        }
    }

    public bool isCooldown()
    {
        return m_hitCooldownTime > 0.0f;
    }

    public bool isHit()
    {
        return m_hitAnimTime > 0.0f;
    }
}

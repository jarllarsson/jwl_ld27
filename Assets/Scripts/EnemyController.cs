using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour 
{
    public enum Type
    {
        WALKING,
        FLYING
    }

    public ExistenceBuffer m_buffer;
    private TimeBasedDisabler m_disabler;
    public float m_dieAnimTime = 1.0f;
    public Transform m_animObj;
    private float m_dyingTime = 9999999999999.0f;
    private Color m_defaultColor;
    private bool m_dieBlink = false;
    public Type m_enemyType=Type.WALKING;
    private float m_depth = 0.0f;
    private Transform m_artifact;
    // Movement
    public float m_jumpPower;
    public float m_moveSpeed;
    public float m_airMoveSpeed;
    public float m_maxMovespeed;
    private float m_movedir;
    private float m_movedirY=0.0f;
    private bool m_onGround;
    private bool m_jumpVrtButtonReleased = true;
    private float m_lookDir = 1.0f;
    public float m_dmgToArtifact=0.1f;
    private List<float> m_artifactDmgList = new List<float>(); // list of all points where  damage was dealt
    private Artifact m_artifactScript;
    public FrameData m_frameData;
    public float m_animSpd = 10.0f;
    private float m_frameCalc = 0.0f;
    private float m_fakePathFind = 1.0f;
    private float m_coolDownFakePathfind = 0.0f;
    private float m_flipDirPathfindTick = 0.0f;
    private int m_ignoreLayer = 8;
    private int m_layerMask;
	// Use this for initialization
	void Start () 
    {
        m_layerMask = ~(1 << m_ignoreLayer);
        m_fakePathFind = Mathf.Clamp((float)Random.Range(-2, 2),-1.0f,1.0f);
        if (m_fakePathFind == 0.0f) m_fakePathFind = 1.0f;
        if (m_artifact == null)
        {
            GameObject artifact = GameObject.FindGameObjectWithTag("Artifact");
            if (artifact) m_artifact = artifact.transform;
        }

        m_defaultColor = m_animObj.renderer.material.GetColor("_TintColor");
        if (m_enemyType == Type.FLYING)
            m_depth = transform.position.z+35.0f;
        else
            m_depth = transform.position.z;
        transform.position = new Vector3(transform.position.x, transform.position.y, m_depth);
	}
	
	// Update is called once per frame
	void Update () 
    {
        dieAnim();
        handleReverseDmg();
	}

    void FixedUpdate()
    {
        if (GlobalTime.getState() == GlobalTime.State.ADVANCING && m_artifact &&
            !m_buffer.isBeforeBuffer() && m_buffer.isWritingToBuffer())
        {
            Vector3 targetDir = (m_artifact.position - transform.position).normalized;

            if (m_enemyType == Type.WALKING)
            {
                //Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + Vector3.down * 3.0f, Color.red, 1.0f);
                if (m_coolDownFakePathfind>0.0f ||
                    Physics.Raycast(new Ray(transform.position + Vector3.up - Vector3.forward * 30.0f, Vector3.down), 3.0f,m_layerMask))
                {
                    targetDir = new Vector3(m_fakePathFind, targetDir.y, targetDir.z);
                    //Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + Vector3.down * 3.0f, Color.green, 1.0f);
                    m_coolDownFakePathfind = 1.0f;
                }
                m_coolDownFakePathfind -= Time.deltaTime;
                m_flipDirPathfindTick -= Time.deltaTime;
                if (m_flipDirPathfindTick < -10.0f)
                {
                    m_flipDirPathfindTick = Random.Range(1.0f,20.0f);
                    m_fakePathFind *= -1.0f;
                }
            }


            float spd = m_moveSpeed;
            if (m_onGround)
            {
                m_movedir *= 0.8f;
            }
            else
            {
                spd = m_airMoveSpeed;
            }
            float horizAxis = targetDir.x;
            float vertAxis = targetDir.y;
            m_movedir += horizAxis * m_moveSpeed;
            m_movedir = Mathf.Clamp(m_movedir, -m_maxMovespeed, m_maxMovespeed);
            if (m_enemyType == Type.FLYING)
            {
                m_movedirY += vertAxis * m_moveSpeed;
                m_movedirY = Mathf.Clamp(m_movedirY, -m_maxMovespeed, m_maxMovespeed);
            }
            if (Mathf.Abs(m_movedir) > Mathf.Epsilon) m_lookDir = m_movedir / Mathf.Abs(m_movedir);
            bool doJump = false;
            if (m_enemyType==Type.WALKING) doJump = vertAxis > 0.1f;

            handleTurning();
            animate();


            Vector3 oldVelocity = rigidbody.velocity;
            Vector3 velocity = Vector3.zero;

            // Jump
            if (doJump && m_jumpVrtButtonReleased && m_onGround)
            {
                //m_isJumping = true;
                m_onGround = false;
                rigidbody.AddForce(0.0f, m_jumpPower, 0.0f);
               // if (!m_jumpSound.isPlaying) m_jumpSound.Play();
            }

            // Movement
            velocity = new Vector3(m_movedir * Time.deltaTime, m_movedirY*Time.deltaTime, 0.0f);



            if (!doJump)
            {
                m_jumpVrtButtonReleased = true;
            }


            // Apply movement
            Vector3 relativeVelocity = velocity - oldVelocity;
            if (m_enemyType == Type.WALKING) relativeVelocity.y = 0.0f;
            rigidbody.AddForce(relativeVelocity, ForceMode.VelocityChange);
        }
    }

    public bool damage(float p_dmg)
    {
        bool success = false;
        bool goAhead = true;
        if (GlobalTime.getTime() > m_dyingTime || !m_animObj.renderer.enabled || m_buffer.isBeforeBuffer())
            goAhead = false;

        if (goAhead)
        {
            if (m_disabler == null)
            {
                TimeBasedDisabler disabler = gameObject.AddComponent<TimeBasedDisabler>() as TimeBasedDisabler;
                disabler.m_popEffectName = "EnemDeath";
                disabler.m_popEffect = true;
                // disabler.m_disableColliders = false;
                // disabler.m_disableRigidbodies = false;
                m_disabler = disabler;
            }
            else
            {
                m_disabler.m_forceRefresh = true;
            }
            m_disabler.m_time = GlobalTime.getTime() + m_dieAnimTime;
            m_dyingTime = GlobalTime.getTime();
            success = true;
        }
        return success;
    }

    void dieAnim()
    {
        if (m_disabler)
        {
            if (GlobalTime.getTime() > m_dyingTime &&
                GlobalTime.getTime() < m_disabler.m_time)
            {
                m_dieBlink = true;
                m_animObj.renderer.material.SetColor("_TintColor", new Color(Random.Range(1.0f, -1.0f), Random.Range(1.0f, -1.0f), Random.Range(1.0f, -1.0f)));
                if (m_buffer.isWritingToBuffer())
                {
                    transform.localScale += 3.0f*Time.deltaTime*new Vector3(Random.Range(-1.0f, 2.0f), Random.Range(-2.0f, 2.0f), 0.0f);
                }
            }
            else
            {
                if (m_dieBlink)
                {
                    m_animObj.renderer.material.SetColor("_TintColor", m_defaultColor);
                    m_dieBlink = false;
                }
            }
        }
    }



    void handleTurning()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -m_lookDir, transform.localScale.y, transform.localScale.z);
    }

    void OnCollisionEnter(Collision p_hit)
    {
        collisionResponse(p_hit);
    }

    void OnCollisionStay(Collision p_hit)
    {
        collisionResponse(p_hit);
    }

    void collisionResponse(Collision p_hit)
    {
        if (!m_buffer.isBeforeBuffer() && m_buffer.isWritingToBuffer())
        {
            Vector3 avgNormal = averageGroundNormal(p_hit);
            wallBounce(avgNormal);
            tryEnableJump(avgNormal);
        }
    }

    void OnTriggerStay(Collider other)
    {
        hurtArtifact(other);
    }

    void OnTriggerEnter(Collider other)
    {
        hurtArtifact(other);
    }

    void hurtArtifact(Collider other)
    {
        bool goAhead = true;
        if (GlobalTime.getTime() > m_dyingTime || !m_animObj.renderer.enabled || m_buffer.isBeforeBuffer())
            goAhead = false;

        if (goAhead)
        {
            if (GlobalTime.getState() == GlobalTime.State.ADVANCING)
            {
                if (other.gameObject.tag == "Artifact")
                {
                    if (!m_artifactScript) m_artifactScript = other.gameObject.GetComponent<Artifact>();
                    if (m_artifactScript)
                    {
                        if (m_artifactScript.damage2(m_dmgToArtifact, transform))
                        {
                            m_artifactDmgList.Add(GlobalTime.getTime());
                        }
                    }
                }
            }
        }

    }

    void handleReverseDmg()
    {
        if (m_buffer.isReadingFromBuffer() && m_artifactScript &&
            GlobalTime.getState() == GlobalTime.State.REWINDING)
        {
            float time = GlobalTime.getTime();
            int index=m_artifactDmgList.Count - 1;
            if (index >= 0)
            {
                while (time <= m_artifactDmgList[index])
                {
                    m_artifactScript.reverseDamage(m_dmgToArtifact);
                    m_artifactDmgList.RemoveAt(index);
                    index = m_artifactDmgList.Count - 1;
                    if (index < 0) break;
                }
            }
        }
    }

    void animate()
    {
        if (m_frameData)
        {
            if (Mathf.Abs(m_movedir) > 0.0f) // moving
            {
                m_frameCalc += Time.deltaTime * m_animSpd;
                m_frameData.m_frameData = new Vector2((float)((int)m_frameCalc % 4), 0.0f);
            }
        }
    }


    private void wallBounce(Vector3 p_hitNormalVector)
    {
        float absVecX = Mathf.Abs(p_hitNormalVector.x);
        if (absVecX > 0.7f)
        {
            m_movedir += p_hitNormalVector.x * m_moveSpeed * 10.0f;
            m_fakePathFind = p_hitNormalVector.x;
        }
    }

    private void tryEnableJump(Vector3 p_hitNormalVector)
    {
        if (p_hitNormalVector.y > 0.7f &&
            rigidbody.velocity.y <= 0.0f)
        {
            //if (!m_onGround && !m_landSound.isPlaying) m_landSound.Play();
            m_onGround = true;
        }
    }

    Vector3 averageGroundNormal(Collision p_collision)
    {
        Vector3 normal = Vector3.zero;
        int count = 0;
        foreach (ContactPoint contact in p_collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white, 1.0f);
            count++;
            normal += contact.normal;
        }
        if (count > 0) normal /= (float)count;
        return normal;
    }

}

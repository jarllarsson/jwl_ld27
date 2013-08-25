using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public float m_jumpPower;
    public float m_moveSpeed;
    public float m_airMoveSpeed;
    public float m_maxMovespeed;
    private float m_movedir;
    //private bool m_isJumping;
    private bool m_onGround;
    private bool m_jumpButtonReleased=true;
    public Transform m_laser;
    public float m_laserFeedbackY = 100.0f;
    public float m_laserFeedbackX = 100.0f;
    public float m_laserTimeout=1.0f;
    private float m_laserTimeoutTick = 0.0f;
    private float m_lookDir=1.0f;
    private Vector3 m_laserThrowback;
    public AudioSource m_jumpSound;
    public AudioSource m_landSound;

    public FrameData m_frameData;
    public float m_animRunSpd = 10.0f;
    public float m_animIdleSpd = 5.0f;
    private float m_frameCalc = 0.0f;
    public float m_shootAnimLen = 0.3f;
    private float m_shootAnimTick = 0.0f;
    private Vector2 m_shootPointDir;
    
	// Use this for initialization
    void Awake()
    {
        ExistenceBuffer buffer = transform.GetComponent<ExistenceBuffer>();
        if (buffer == null)
        {
            gameObject.AddComponent<ExistenceBuffer>();
        }
    }


	void Start () 
    {
        StaticPlayerHandle.m_player = transform;
	}

    void Update()
    {
        if (m_laserTimeoutTick > 0.0f)
        {
            m_laserTimeoutTick -= Time.deltaTime;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (GlobalTime.getState()==GlobalTime.State.ADVANCING)
        {
            float spd = m_moveSpeed;
            if (m_onGround)
            {
                m_movedir *= 0.8f;
            }
            else
            {
                spd = m_airMoveSpeed;
            }
            float horizAxis = Input.GetAxis("Horizontal");
            float vertAxis = Input.GetAxis("Vertical");
            m_movedir += horizAxis * m_moveSpeed;
            m_movedir = Mathf.Clamp(m_movedir, -m_maxMovespeed, m_maxMovespeed);
            if (Mathf.Abs(m_movedir) > Mathf.Epsilon) m_lookDir = m_movedir / Mathf.Abs(m_movedir);
            bool doJump = Input.GetAxis("Jump") > 0.1f;

            handleTurning();
            animate();


            Vector3 oldVelocity = rigidbody.velocity;
            Vector3 velocity = Vector3.zero;

            // Jump
            if (doJump && m_jumpButtonReleased && m_onGround)
            {
                //m_isJumping = true;
                m_onGround = false;
                rigidbody.AddForce(0.0f, m_jumpPower, 0.0f);
                if (!m_jumpSound.isPlaying) m_jumpSound.Play();
            }

            // Movement
            velocity = new Vector3(m_movedir * Time.deltaTime, 0.0f, 0.0f);



            if (!doJump)
            {
                m_jumpButtonReleased = true;
            }

            // Laser
            if (Input.GetAxis("Fire1") > 0.3f && m_laserTimeoutTick <= 0.0f)
            {
                m_laserTimeoutTick = m_laserTimeout;
                float hpoint = m_lookDir;
                if (Mathf.Abs(horizAxis) > 0.0f || Mathf.Abs(vertAxis) > 0.0f) hpoint = horizAxis;
                m_shootPointDir = new Vector2(hpoint, vertAxis);
                m_laserThrowback = new Vector3(-m_shootPointDir.x * m_laserFeedbackX, 0.0f, 0.0f);
                rigidbody.AddForce(0.0f, -m_shootPointDir.y * m_laserFeedbackY, 0.0f);
                Instantiate(m_laser, transform.position, Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(m_shootPointDir.y, m_shootPointDir.x), Vector3.forward));
                m_shootAnimTick = m_shootAnimLen;
            }
            velocity += m_laserThrowback * Time.deltaTime;
            m_laserThrowback *= 0.99f;
            if (m_shootAnimTick > 0.0f)
                m_shootAnimTick -= Time.deltaTime;
            else
                m_shootAnimTick = 0.0f;

            // Apply movement
            Vector3 relativeVelocity = velocity - oldVelocity;
            relativeVelocity.y = 0.0f;
            rigidbody.AddForce(relativeVelocity, ForceMode.VelocityChange);
        }
	}

    void animate()
    {
        if (m_frameData)
        {
            if (m_shootAnimTick <= 0.0f)
            {
                if (Mathf.Abs(m_movedir) > m_moveSpeed) // moving
                {
                    m_frameCalc += Time.deltaTime * m_animRunSpd;
                    m_frameData.m_frameData = new Vector2((float)((int)m_frameCalc%4), 2.0f);
                }
                else // idle
                {
                    m_frameCalc += Time.deltaTime * m_animIdleSpd;
                    m_frameData.m_frameData = new Vector2((float)((int)m_frameCalc % 4), 3.0f);
                }
            }
            else
            {
                float aim = 0.0f; // up
                if (Mathf.Abs(m_shootPointDir.x) > 0.0f) aim = 2.0f; // sideways
                if (m_shootPointDir.y < 0.0f) aim = 1.0f; // down
                m_frameData.m_frameData = new Vector2(aim, 1.0f);
            }

            
        }
    }

    void handleTurning()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * m_lookDir, transform.localScale.y, transform.localScale.z);
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
        Vector3 avgNormal = averageGroundNormal(p_hit);
        wallBounce(avgNormal);
        tryEnableJump(avgNormal);
    }

    private void wallBounce(Vector3 p_hitNormalVector)
    {
        float absVecX = Mathf.Abs(p_hitNormalVector.x);
        if (absVecX > 0.7f)
        {
            m_movedir += p_hitNormalVector.x*m_moveSpeed*10.0f;
        }
    }

    private void tryEnableJump(Vector3 p_hitNormalVector)
    {
        if (p_hitNormalVector.y > 0.7f && 
            rigidbody.velocity.y<=0.0f)
        {
            if (!m_onGround && !m_landSound.isPlaying) m_landSound.Play();
            m_onGround = true;
        }
    }

    Vector3 averageGroundNormal(Collision p_collision)
    {
        Vector3 normal = Vector3.zero;
        int count = 0;
        foreach (ContactPoint contact in p_collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white,1.0f);
            count++;
            normal += contact.normal;
        }
        if (count>0) normal /= (float)count;
        return normal;
    }
}

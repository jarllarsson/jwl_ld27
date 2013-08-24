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
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
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

        m_movedir += Input.GetAxis("Horizontal")*m_moveSpeed;
        m_movedir = Mathf.Clamp(m_movedir,-m_maxMovespeed,m_maxMovespeed);
        bool doJump = Input.GetAxis("Jump")>0.1f;

        Vector3 oldVelocity = rigidbody.velocity;
        Vector3 velocity = Vector3.zero;

        // Jump
        if (doJump && m_jumpButtonReleased && m_onGround)
        {
            //m_isJumping = true;
            m_onGround = false;
            rigidbody.AddForce(0.0f, m_jumpPower, 0.0f);
        }

        // Movement
        velocity = new Vector3(m_movedir*Time.deltaTime,0.0f,0.0f);

        // Apply movement
        Vector3 relativeVelocity = velocity-oldVelocity;
        relativeVelocity.y = 0.0f;
        rigidbody.AddForce(relativeVelocity, ForceMode.VelocityChange);

        if (!doJump)
        {
            m_jumpButtonReleased = true;
        }
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

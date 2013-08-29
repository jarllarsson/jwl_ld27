using UnityEngine;
using System.Collections;

public class CustomGravity : MonoBehaviour 
{
    public float m_gravity;
    private ExistenceBuffer m_buffer;
	// Use this for initialization
	void Start () 
    {
        if (!m_buffer)
        {
            m_buffer = transform.GetComponent<ExistenceBuffer>();
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (GlobalTime.getState()==GlobalTime.State.ADVANCING &&
            !m_buffer || (m_buffer && !m_buffer.isBeforeBuffer() && m_buffer.isWritingToBuffer()))
        {        
            rigidbody.AddForce(Vector3.down*m_gravity);
        }
	}
}

using UnityEngine;
using System.Collections;

public class TimeBasedDisabler : MonoBehaviour 
{
    public float m_time;
    private bool m_disableEngaged = false;
    private Rigidbody[] m_rbodies;
    private Collider[] m_colliders;
    private Renderer[] m_renderers;
	// Use this for initialization
	void Start () 
    {
        m_rbodies = GetComponentsInChildren<Rigidbody>();
        m_colliders = GetComponentsInChildren<Collider>();
        m_renderers = GetComponentsInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GlobalTime.getTime() > m_time)
        {
            // DISABLE
            m_disableEngaged = true;
            foreach (Rigidbody rbody in m_rbodies)
            {
                disableRigidBody(rbody);
            }
            foreach (Collider coll in m_colliders)
            {
                coll.enabled = false;
            }
            foreach (Renderer renderer in m_renderers)
            {
                renderer.enabled = false;
            }
        }
        else if (m_disableEngaged)
        {
            m_disableEngaged = false;
            // ENABLE
            foreach (Rigidbody rbody in m_rbodies)
            {
                enableRigidBody(rbody);
            }
            foreach (Collider coll in m_colliders)
            {
                coll.enabled = true;
            }
            foreach (Renderer renderer in m_renderers)
            {
                renderer.enabled = true;
            }
        }
	}

    void disableRigidBody(Rigidbody p_rbody)
    {
        p_rbody.isKinematic = true;
        p_rbody.detectCollisions = false;
    }

    void enableRigidBody(Rigidbody p_rbody)
    {
        p_rbody.isKinematic = false;
        p_rbody.detectCollisions = true;
        p_rbody.WakeUp();
    }
}

using UnityEngine;
using System.Collections;

public class TimeBasedDisabler : MonoBehaviour 
{
    public float m_time;
    private bool m_disableEngaged = false;
    private Rigidbody[] m_rbodies;
    private Collider[] m_colliders;
    private Renderer[] m_renderers;

    private static Transform m_particleEffect;
    private PopInTimelineEffect m_timelineEffect;
    public bool m_popEffect = false;

	// Use this for initialization
	void Start () 
    {
        m_rbodies = GetComponentsInChildren<Rigidbody>();
        m_colliders = GetComponentsInChildren<Collider>();
        m_renderers = GetComponentsInChildren<Renderer>();
        if (m_popEffect)
        {
            if (m_particleEffect == null)
            {
                m_particleEffect = GameObject.Find("PopOutOfTimeline").transform;
            }
            Transform particleeffect = Instantiate(m_particleEffect, transform.position, transform.rotation) as Transform;
            m_timelineEffect = particleeffect.gameObject.AddComponent<PopInTimelineEffect>();
            particleeffect.name = "popeffect-" + gameObject.name;
            //particleeffect.parent = transform;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GlobalTime.getTime() > m_time)
        {
            // DISABLE
            if (!m_disableEngaged)
            {
                if (m_timelineEffect) m_timelineEffect.play(transform.position);
            }
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
            if (m_timelineEffect) m_timelineEffect.play(transform.position);
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

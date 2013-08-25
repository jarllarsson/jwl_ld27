using UnityEngine;
using System.Collections;

public class PopInTimelineEffect : MonoBehaviour 
{
    ParticleSystem[] m_particleSystems;


	// Use this for initialization
	void Start () 
    {
	    m_particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void play(Vector3 p_position)
    {
        transform.position = p_position;
        foreach (ParticleSystem psystem in m_particleSystems)
        {
            psystem.Play();
        }
    }
}

using UnityEngine;
using System.Collections;

public class PopInTimelineEffect : MonoBehaviour 
{
    ParticleSystem[] m_particleSystems;
    AudioSource m_audio;

	// Use this for initialization
	void Start () 
    {
	    m_particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
        m_audio = transform.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void play(Vector3 p_position)
    {
        transform.position = p_position;
        if (m_audio) m_audio.Play();
        foreach (ParticleSystem psystem in m_particleSystems)
        {
            psystem.Play();
        }
    }
}

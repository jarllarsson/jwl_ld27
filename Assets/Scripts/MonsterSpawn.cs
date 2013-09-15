using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterSpawn : MonoBehaviour {

    public float m_interval=3.0f;
    private float m_intervalCount = 0.0f;
    private float m_maxGlobalTime=-1.0f;
    public float m_speedIncrease = 0.0f;
    public float m_maxSpeedIncrease = 0.0f;
    private float m_intervalAdd = 0.0f;
    public Transform m_enemyPrefab;
    private ParticleSystem[] m_effects;
    private bool m_started = false;
	// Use this for initialization
	void Start () 
    {
        m_intervalCount = Random.Range(0.0f,m_interval);
        m_effects = transform.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in m_effects)
        {
            ps.Clear();
            ps.Stop();
        }
        
	}
	
	// Update is called once per frame
	void Update () 
    {

        if (SceneScript.s_customTimeScale > 0.0f && GlobalTime.getState() == GlobalTime.State.ADVANCING
            /*&& GlobalTime.getTime() > m_maxGlobalTime*/)
        {
            if (!m_started)
            {
                foreach (ParticleSystem ps in m_effects)
                {
                    ps.Play();
                }
            }
            if (!m_started) m_started = true;
            m_intervalCount += (1.0f + m_intervalAdd + (0.1f * (Startscript.m_difficulty - 1))) * SceneScript.deltaTime();     
            m_maxGlobalTime=GlobalTime.getTime();
            if (m_intervalCount > m_interval)
            {
                if (GlobalTime.getTime() > m_maxGlobalTime)
                {
                    m_intervalAdd += m_speedIncrease;
                    if (m_intervalAdd > m_maxSpeedIncrease) m_intervalAdd = m_maxSpeedIncrease;
                }
                m_intervalCount = 0.0f;
                Instantiate(m_enemyPrefab, transform.position, transform.rotation);
            }
        }
	}
}

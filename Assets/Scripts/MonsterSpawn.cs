using UnityEngine;
using System.Collections;

public class MonsterSpawn : MonoBehaviour {

    public float m_interval=3.0f;
    private float m_intervalCount = 0.0f;
    private float m_maxGlobalTime=-1.0f;
    public float m_speedIncrease = 0.0f;
    public float m_maxSpeedIncrease = 0.0f;
    private float m_intervalAdd = 0.0f;
    public Transform m_enemyPrefab;
	// Use this for initialization
	void Start () 
    {
        m_intervalCount = Random.Range(0.0f,m_interval);
	}
	
	// Update is called once per frame
	void Update () 
    {

        if (GlobalTime.getState()==GlobalTime.State.ADVANCING
            /*&& GlobalTime.getTime() > m_maxGlobalTime*/)
        {
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

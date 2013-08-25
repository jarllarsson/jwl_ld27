using UnityEngine;
using System.Collections;

public class TimeStateVisualizer : MonoBehaviour {

    public Transform m_cooldownVisBar;
    private float m_cooldownBarOrigSize = 1.0f;
    public Transform m_playIcon;
    public Transform m_rewindIcon;
    public GUIText m_timer;

	// Use this for initialization
	void Start () 
    {
        m_cooldownBarOrigSize = m_cooldownVisBar.localScale.x;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GlobalTime.getCurrentRewindAmountSec()<=0.0f)
            m_timer.text = GlobalTime.getTime().ToString("F2");
        else
            m_timer.text = GlobalTime.getTime().ToString("F2") + " (<color=#f13975>r" + Mathf.Round(GlobalTime.getCurrentRewindAmountSec()) + "</color>)";

        //
        GlobalTime.State currentState = GlobalTime.getState();
        if (currentState == GlobalTime.State.ADVANCING)
        {
            m_playIcon.renderer.enabled = true;
            m_rewindIcon.renderer.enabled = false; 
            
            if (GlobalTime.getCurrentCooldownPercent() > 0.0f)
            {
                m_cooldownVisBar.localScale = new Vector3(0.05f+GlobalTime.getCurrentCooldownPercent() * (m_cooldownBarOrigSize),
                    m_cooldownVisBar.localScale.y, m_cooldownVisBar.localScale.z);
            }
            else
            {
                m_cooldownVisBar.localScale = new Vector3(0.0f,
                     m_cooldownVisBar.localScale.y, m_cooldownVisBar.localScale.z);
            }
        }
        else
        {
            m_playIcon.renderer.enabled = false;
            m_rewindIcon.renderer.enabled = true;
            m_cooldownVisBar.localScale = new Vector3(0.05f + GlobalTime.getCurrentRewindAmountPercent() * (m_cooldownBarOrigSize),
    m_cooldownVisBar.localScale.y, m_cooldownVisBar.localScale.z);
        }



	}
}

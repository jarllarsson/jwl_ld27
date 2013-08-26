using UnityEngine;
using System.Collections;

public class Startscript : MonoBehaviour 
{
    public static bool m_start = false;
    private float m_step=0.0f;
    public Transform m_fadePlane;
    public GUIText m_infoText;
    public GUIText m_upperText;
    public GUIText m_lowerText;
    public Renderer m_playerIcon;
	// Use this for initialization
	void Start () 
    {
        GlobalTime.reset();
        Time.timeScale = 0.0f;
        m_fadePlane.renderer.enabled = true;
        m_upperText.enabled = false;
        m_lowerText.enabled = false;
        m_infoText.enabled = true;
        m_infoText.text = "Protect the cube.\nRewind to elevate it to its goal.\n[Press SPACE to start]";
	}
	
	// Update is called once per frame
	void Update () 
    {
        m_playerIcon.enabled = false;
        if (!m_start && Input.GetKey(KeyCode.Space))
        {
            m_start = true;
        }
        if (m_start)
        {
            m_step+=10.0f*Mathf.Max(0.01f,Time.deltaTime);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, m_step);
            m_fadePlane.renderer.material.color = Color.Lerp(m_fadePlane.renderer.material.color, new Color(0.0f, 0.0f, 0.0f, 0.0f), m_step);
            if (m_step >= 1.0f)
            {
                m_playerIcon.enabled = true;
                Destroy(gameObject);
                m_upperText.enabled = true;
                m_lowerText.enabled=true;
                m_infoText.enabled = false;
                m_start = false;
            }
        }
	}
}

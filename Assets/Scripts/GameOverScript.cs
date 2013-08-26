using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour 
{
    public static bool m_start = false;
    private float m_step = 0.0f;
    public Transform m_fadePlane;
    public GUIText m_infoText;
    public GUIText m_upperText;
    public GUIText m_lowerText;
    public string m_text;
    public bool m_showTime=false;
    public Renderer m_playerIcon;
    public Renderer m_rewindIcon;
    public Renderer m_rewindBar;
    public Renderer m_rewindBar2;
    // Use this for initialization
    void Start()
    {
        m_start = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (m_start)
        {
            m_playerIcon.enabled = false;
            m_rewindIcon.enabled = false;
            m_rewindBar.enabled = false;
            m_rewindBar2.enabled = false;
            m_step += Mathf.Max(0.0001f, Time.deltaTime*0.01f);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.0f, m_step);
            m_fadePlane.renderer.material.color = Color.Lerp(m_fadePlane.renderer.material.color, new Color(0.0f, 0.0f, 0.0f, 0.7f), m_step);
//             if (m_step >= 1.0f)
//             {
//                 Destroy(gameObject);
//                 m_upperText.enabled = true;
//                 m_lowerText.enabled = true;
//                 m_infoText.enabled = false;
//             }
            if (Input.GetKey(KeyCode.Space))
            {
                GlobalTime.reset();
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }

    public void Run()
    {
        if (m_showTime)
        {
            m_text += "\nAchieved in <color=#E2DE7D>" + GlobalTime.getTime().ToString() + "</color> seconds!";
        }
        m_text += "\n[press SPACE to restart]";
        m_start = true;
        m_fadePlane.renderer.enabled = true;
        m_upperText.enabled = false;
        m_lowerText.enabled = false;
        m_infoText.enabled = true;
        m_infoText.text = m_text;
    }
}

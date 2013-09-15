using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour 
{
    public bool m_start = false;
    public static bool m_gameEnd = false;
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
    public PlayerController   m_playerController;
    public PlayerTimeTraveler m_playerTimetraveler;
    private bool hasPressed = true;

    public Transform m_camera;
    private Vector3 m_camStartDiff;
    public ButtonScript m_resetBtn;

    // Use this for initialization
    void Start()
    {
        m_camStartDiff = transform.position - m_camera.position;
        m_start = false;
        m_resetBtn.m_active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_start)
        {
            transform.position = new Vector3(m_camera.position.x + m_camStartDiff.x, m_camera.position.y + m_camStartDiff.y, transform.position.z);
            m_infoText.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
            bool press = m_resetBtn.readPress();
            m_gameEnd = true;
            if (m_playerController != null) m_playerController.enabled = false;
            if (m_playerTimetraveler != null) m_playerTimetraveler.enabled = false;
            m_playerIcon.enabled = false;
            m_rewindIcon.enabled = false;
            m_rewindBar.enabled = false;
            m_rewindBar2.enabled = false;
            m_step += Mathf.Max(0.0001f, Time.deltaTime);
            SceneScript.s_customTimeScale = Mathf.Lerp(SceneScript.s_customTimeScale, 0.0f, m_step);
            m_fadePlane.renderer.material.color = Color.Lerp(m_fadePlane.renderer.material.color, new Color(0.0f, 0.0f, 0.0f, 0.7f), m_step);
//             if (m_step >= 1.0f)
//             {
//                 Destroy(gameObject);
//                 m_upperText.enabled = true;
//                 m_lowerText.enabled = true;
//                 m_infoText.enabled = false;
//             }
            if (press && !hasPressed)
            {
                m_gameEnd = false;
                m_start = false;
                GlobalTime.reset();
                Application.LoadLevel(Application.loadedLevel);
            }
            hasPressed = press;
        }
    }

    public void Run()
    {
        if (m_showTime)
        {
            m_text += "\nAchieved in <color=#E2DE7D>" + GlobalTime.getTime().ToString() + "</color> seconds!\nDifficulty: "+Startscript.m_difficulty+".";
        }
        m_text += "\n[press SPACE to restart]";
        m_start = true;
        m_fadePlane.renderer.enabled = true;
        m_upperText.enabled = false;
        m_lowerText.enabled = false;
        m_infoText.enabled = true;
        m_resetBtn.m_active = true;
        m_infoText.text = m_text;
    }
}

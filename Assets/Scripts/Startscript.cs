using UnityEngine;
using System.Collections;

public class Startscript : MonoBehaviour 
{
    public static bool m_start = false;
    public static int m_difficulty = 1;
    private float m_step=0.0f;
    public Transform m_fadePlane;
    public GUIText m_infoText;
    //public GUIText m_diffText;
    public GUIText m_upperText;
    public GUIText m_lowerText;
    public Renderer m_playerIcon;
    private bool hasPressed = true;

    public ButtonScript m_startEasy;
    public ButtonScript m_startNormal;
    public ButtonScript m_startHard;
    public ButtonScript m_startXtreme;

	// Use this for initialization
	void Start () 
    {
        m_difficulty = 1;
        GlobalTime.reset();
        SceneScript.s_customTimeScale = 0.0f;
        m_fadePlane.renderer.enabled = true;
        m_upperText.enabled = false;
        m_lowerText.enabled = false;
        m_infoText.enabled = true;
        //m_diffText.enabled = true;
        m_infoText.text = "Protect the cube.\nRewind to elevate it to its goal.";
	}
	
	// Update is called once per frame
	void Update () 
    {
        bool press = Input.GetKey(KeyCode.Space);
//         #if UNITY_ANDROID || UNITY_IPHONE
//             press = true;
//             hasPressed = false;
//         #endif

        m_playerIcon.enabled = false;
        if (!m_start && press && !hasPressed)
        {
            m_start = true;
        }

        // difficulty
        int inDiffc = m_difficulty;
        if (m_startEasy.readPress())    {inDiffc = 1;m_start=true;}
        if (m_startNormal.readPress())  {inDiffc = 2;m_start=true;}
        if (m_startHard.readPress())    {inDiffc = 3;m_start=true;}
        if (m_startXtreme.readPress())  {inDiffc = 4;m_start=true;}
        m_difficulty = inDiffc;

//         string ds = "";
//         for (int i = 1; i <= 4; i++)
//         {
//             string prt = " ";
//             if (inDiffc == i) prt += "<color=#ff5fc1>[";
//             prt += i.ToString();
//             if (inDiffc == i) prt += "]</color>";
//             prt += " ";
//             ds += prt;
//         }
//         m_diffText.text = ds;

        if (m_start)
        {
            m_step += 10.0f * Mathf.Max(0.01f, Time.deltaTime);
            SceneScript.s_customTimeScale = Mathf.Lerp(SceneScript.s_customTimeScale, 1.0f, m_step);
            m_fadePlane.renderer.material.color = Color.Lerp(m_fadePlane.renderer.material.color, new Color(0.0f, 0.0f, 0.0f, 0.0f), m_step);
            if (m_step >= 1.0f)
            {
                SceneScript.s_customTimeScale = 1.0f;
                m_playerIcon.enabled = true;
                Destroy(gameObject);
                m_upperText.enabled = true;
                m_lowerText.enabled = true;
                m_infoText.enabled = false;
                //m_diffText.enabled = false;
                m_start = false;
            }
        }
        hasPressed = press;
	}
}

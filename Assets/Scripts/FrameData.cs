using UnityEngine;
using System.Collections;

public class FrameData : MonoBehaviour {
    public Vector2 m_frameData;
    public Vector2 m_animstep; 
    public float m_animSpd = 1.0f;
    public Renderer m_animObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        setFrame();
	}

    void setFrame()
    {
        if (m_animObject)
        {
            m_animObject.material.SetTextureOffset("_MainTex",
                new Vector2(m_frameData.x * m_animstep.x,
                            m_frameData.y * m_animstep.y));
        }
    }
}

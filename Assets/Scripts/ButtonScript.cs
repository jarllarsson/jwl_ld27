using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour 
{
    public ButtonScript m_neighbourLeft;
    public ButtonScript m_neighbourRight;
    public ButtonScript m_neighbourUp;
    public ButtonScript m_neighbourDown;

    public bool m_selected;
    private bool m_pressed=false;
    private bool m_selectedFromNeighbour=false;
    private static float s_selectFromNeighbourCooldownLimMax = 0.3f;
    private static float s_selectFromNeighbourCooldownLim = 0.3f;
    private static float s_selectFromNeighbourCooldown = 0.3f;

    public bool m_active=true;
    private bool m_oldActiveStat;

    private static int m_buttonLayer = 10;
    private static ButtonScript s_perFrameButtonSelected;
    private static bool s_perFrameSelectionDone=false;
    private static Camera m_camera=null;
    private static int m_layerMask;

    private Renderer[] m_graphics;

	// Use this for initialization
	void Start () 
    {
        if (m_selected && m_active)
        {
            s_perFrameButtonSelected = this;
            s_perFrameSelectionDone = true;
        }
        m_oldActiveStat = m_active;
        m_layerMask = 1 << m_buttonLayer;
        if (m_camera==null) m_camera = Camera.main;
        m_graphics = transform.GetComponentsInChildren<Renderer>();
        if (!m_active)
        {
            foreach (Renderer r in m_graphics)
            {
                r.enabled = false;
            }
            collider.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_active)
        {
            if (m_oldActiveStat==false)
            {
                m_oldActiveStat = true;
                foreach (Renderer r in m_graphics)
                {
                    r.enabled = true;
                }
                if (m_selected)
                {
                    s_perFrameButtonSelected = this;
                    s_perFrameSelectionDone = true;
                }
                collider.enabled = true;
            }

            if (collider)
            {
                bool hover = staticSelect(collider);
                if (!hover && s_perFrameButtonSelected != this)
                    m_selected = false;
                else
                    m_selected = true;
            }

            if (m_selected)
            {
                bool res = neighbourAffect();
                if (res) {m_selected = false;}
                transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            }
            else
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (inputDir.sqrMagnitude > 0.001f && !m_selected && m_active && s_perFrameButtonSelected == null)
                {
                    m_selected = true;
                    s_perFrameButtonSelected = this;
                    s_perFrameSelectionDone = true;
                }
            }

            //         if (m_pressed)
            //         {
            //             renderer.material.SetColor("_Color",Color.green);
            //         }
            //         else
            //         {
            //             renderer.material.SetColor("_Color",Color.white);
            //         }


            // if keypress
            if (!m_pressed)
            {
                m_pressed = checkPress();
            }
        }
        else
        {
            if (m_oldActiveStat == true)
            {
                m_oldActiveStat = false;
                foreach (Renderer r in m_graphics)
                {
                    r.enabled = false;
                }
                if (m_selected)
                {
                    s_perFrameButtonSelected = null;
                    s_perFrameSelectionDone = false;
                }
                collider.enabled = true;
            }
        }

	}

    void LateUpdate()
    {
        if (m_active) s_perFrameSelectionDone = false;
    }

    bool checkPress()
    {
        bool isPressed = false;
        bool mouse = Input.GetMouseButton(0);
        if (m_selected &&
            (Input.GetAxis("Fire1") > 0.0f || mouse))
        {
            bool proceed = true;
            if (mouse) proceed = rayHit(Input.mousePosition, collider);
            if (proceed) isPressed = true;
        }
//         else
//         {
// #if UNITY_ANDROID || UNITY_IPHONE
//             Vector3 touchPos;
//             if (touch(out touchPos))
//             {
//                 isPressed = true;
//             }
// #endif
//         }
        return isPressed;
    }

    bool touch(out Vector3 p_outPos)
    {
        bool touching = false;
        Vector3 pos=Vector3.zero;
#if UNITY_ANDROID || UNITY_IPHONE
        // first check movement
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
            {                
                pos = new Vector3(touch.position.x,touch.position.y,0.0f);
                if (rayHit(pos, collider))
                {
                    touching=true;
                }
            }
        }
#endif
        p_outPos = pos;
        return touching;
    }

    private static bool staticSelect(Collider p_collider)
    {
        bool result=false;
        if (!s_perFrameSelectionDone)
        {
            result = rayHit(Input.mousePosition, p_collider);
            if (result)
            {
                s_perFrameSelectionDone = true;
                s_perFrameButtonSelected = p_collider.gameObject.GetComponent<ButtonScript>();
            }
        }
        return result;
    }

    private static bool rayHit(Vector3 p_screenPos, Collider p_collider)
    {
        bool result=false;
        Ray ray = m_camera.ScreenPointToRay(p_screenPos);
        RaycastHit hit;
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100.0f, Color.cyan);
        bool hitRes = Physics.Raycast(ray, out hit, 30.0f, m_layerMask);
        if (hitRes)
        {                
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.magenta);
            if (hit.collider == p_collider)
            {
                result = true;

            }
        }
        return result;
    }

    // selection from neighbour,
    // good for gamepad menus
    void selectFromNeighbour()
    {
        m_selected = true;
        m_selectedFromNeighbour = true;
        s_perFrameSelectionDone = true;
        s_perFrameButtonSelected = this;
    }

    public bool readPress()
    {
        bool result=m_pressed;
        m_pressed=false;
        if (result)
        {
            m_selected = false;
            s_perFrameSelectionDone = false;
            s_perFrameButtonSelected = null;
        }
        return result;
    }

    bool neighbourAffect()
    {        
        bool res = false;             
        float lim = 0.1f;          
        Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (!m_selectedFromNeighbour)
        {
            if (inputDir.x > lim && m_neighbourRight)
            {
                m_neighbourRight.selectFromNeighbour();
                res = true;
            }
            else if (inputDir.x < -lim && m_neighbourLeft)
            {
                m_neighbourLeft.selectFromNeighbour();
                res = true;
            }
            else if (inputDir.y > lim && m_neighbourUp)
            {
                m_neighbourUp.selectFromNeighbour();
                res = true;
            }
            else if (inputDir.y < -lim && m_neighbourDown)
            {
                m_neighbourDown.selectFromNeighbour();
                res = true;
            }
            if (res)
            {
                s_selectFromNeighbourCooldown = s_selectFromNeighbourCooldownLim;
                s_selectFromNeighbourCooldownLim *= 0.8f;
                if (s_selectFromNeighbourCooldownLim < 0.1f) s_selectFromNeighbourCooldownLim = 0.1f;
            }
        }
        if (inputDir.sqrMagnitude < 0.001f)
        {
            m_selectedFromNeighbour = false;
            s_selectFromNeighbourCooldownLim = s_selectFromNeighbourCooldownLimMax;
        }
       

        if (m_selectedFromNeighbour)
        {
            if (s_selectFromNeighbourCooldown < 0.0f)
            {
                m_selectedFromNeighbour = false;
            }
            s_selectFromNeighbourCooldown -= Mathf.Max(Time.deltaTime,Time.fixedDeltaTime);
        }

        return res;
    }
}

using UnityEngine;
using System.Collections;

public class CustomGravity : MonoBehaviour 
{
    public float m_gravity;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        rigidbody.AddForce(Vector3.down*m_gravity);
	}
}

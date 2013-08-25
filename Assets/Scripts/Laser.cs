using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour 
{
    public string[] m_killList;
    public float m_speed=1.0f;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        Vector3 velocity = (transform.right * m_speed) - rigidbody.velocity;
        rigidbody.AddForce(velocity, ForceMode.VelocityChange);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other != collider && other.gameObject.tag!="Player")
        {
            Destroy(gameObject);
        }

    }
}

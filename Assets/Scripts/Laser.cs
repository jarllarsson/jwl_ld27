using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour 
{
    public string[] m_killList;
    public float m_speed=1.0f;
    public ExistenceBuffer m_buffer;
    private TimeBasedDisabler m_disabler;
    public Transform m_explodeEffect;
    private float m_explodeRecreateTime = 0.3f;
    private float m_explodeRecreateTick = 0.0f;
    private PopInTimelineEffect m_laserExplode;
    public AudioSource m_sound;
    private float m_explodedTime = -1.0f;
	// Use this for initialization
	void Start () 
    {
        if (m_explodeEffect == null)
        {
            m_explodeEffect = GameObject.Find("LaserPof").transform;
        }
        Transform particleeffect = Instantiate(m_explodeEffect, transform.position, transform.rotation) as Transform;
        m_laserExplode = particleeffect.gameObject.AddComponent<PopInTimelineEffect>();
        particleeffect.name = "laserpof-" + gameObject.name;
	}

    void OnDestroy()
    {
        if (m_laserExplode)
        {
            Destroy(m_laserExplode.gameObject);
        }
    }

    void Update()
    {
        if (m_explodeRecreateTick > 0.0f)
        {
            m_explodeRecreateTick -= Time.deltaTime;
        }
        if (!m_sound.isPlaying && GlobalTime.getTime() < m_buffer.getStartTime() + 0.1f &&
            GlobalTime.getTime() > m_buffer.getStartTime() - 0.01f &&
            GameOverScript.m_gameEnd==false)
        {
            m_sound.Play();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        Vector3 velocity = (transform.right * m_speed) - rigidbody.velocity;
        rigidbody.AddForce(velocity, ForceMode.VelocityChange);

        if (GlobalTime.getTime() < m_explodedTime + 0.1f && 
            GlobalTime.getTime() > m_explodedTime - 0.01f)
            explode();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other != collider && 
            other.gameObject.tag!="Laser" &&
            other.gameObject.tag!="Player" &&
            other.gameObject.tag!="Artifact" &&
            other.gameObject.tag!="EnemDeath")
        {


            if (other.gameObject.tag == "Enemy")
            {
                EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
                if (enemy)
                {
                    if (enemy.damage(1.0f))
                    {
                        updateDisabler();
                        explode();
                    }
                }
            }
            else
            {
                updateDisabler();
                explode();
            }

        }

    }

    void explode()
    {
        if (m_laserExplode && m_explodeRecreateTick <= 0.0f)
        {
            m_laserExplode.transform.rotation = transform.rotation;
            m_laserExplode.play(transform.position);
            m_explodeRecreateTick = m_explodeRecreateTime;
        }
    }

    void updateDisabler()
    {
        if (m_disabler == null)
        {
            TimeBasedDisabler disabler = gameObject.AddComponent<TimeBasedDisabler>() as TimeBasedDisabler;
            disabler.m_time = m_buffer.getEndTime();
            m_explodedTime = GlobalTime.getTime();
            m_disabler = disabler;
        }
    }
}

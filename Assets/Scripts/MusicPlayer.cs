using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource m_normalMusic;
    public AudioSource m_reverseMusic;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GlobalTime.getState() == GlobalTime.State.ADVANCING)
        {
            m_reverseMusic.Stop();
            if (!m_normalMusic.isPlaying) m_normalMusic.Play();
        }
        else
        {
            m_normalMusic.Stop();
            if (!m_reverseMusic.isPlaying) m_reverseMusic.Play();
        }
	}
}

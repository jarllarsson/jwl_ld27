using UnityEngine;
using System.Collections;

public class DifficultyActivator : MonoBehaviour {
    public int m_lowestDifficultyForActivation;

	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Startscript.m_start && 
            Startscript.m_difficulty < m_lowestDifficultyForActivation)
            DestroyImmediate(gameObject);
	}
}

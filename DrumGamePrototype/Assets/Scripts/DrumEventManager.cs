using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class DrumEventManager : MonoBehaviour {

    public DrumEventManager Instance;

    public delegate void SwitchSections(string loopName);
    public static event SwitchSections switchLoop1;
    public static event SwitchSections switchLoop2;
    public static event SwitchSections switchLoop3;

    public delegate void RestartLoop();
    public static event RestartLoop restartCurrentLoop;


    //enforce singleton pattern
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject); 
        } else {
            Instance = this;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable() {
        
    }

    private void OnDisable() {
        
    }
}

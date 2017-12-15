using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;

public class InputEvaluator : MonoBehaviour {

    public bool withinWindow;

    public float nextBeatDSPTime;

    public float currentDSPTime;

    public PlayerController playerController;
    public SongManager songManager;

    bool nextBeatCued;

   


	void Start () {
        withinWindow = false;
        nextBeatCued = false;
        nextBeatDSPTime = 0f;

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        currentDSPTime = (float)AudioSettings.dspTime;

        if (nextBeatDSPTime == 0f) {
            //use this to initialize the beat
            nextBeatDSPTime = (float)Clock.Instance.AtNextBeat();
    
        }
        if (currentDSPTime > nextBeatDSPTime - (float)Clock.Instance.SixteenthLength() 
            && currentDSPTime < nextBeatDSPTime + (float)Clock.Instance.SixteenthLength()) {
            //check if we're in the window

            withinWindow = true;

        }

        if (withinWindow && currentDSPTime >= nextBeatDSPTime && !nextBeatCued) {
            //events to be called immedately after crossing beat
            //this should happen as close to the beat as possible - maybe use
            //clock sync function instead?

            //update the next beat in songManager
            
            songManager.currentBeat++;
            songManager.CueNextObstacles(nextBeatDSPTime + Clock.Instance.QuarterLength());
            nextBeatCued = true;
            
        }
        if (withinWindow && currentDSPTime >= nextBeatDSPTime + (float)Clock.Instance.SixteenthLength()) {
            //At window End
            withinWindow = false;

            //Evaluate our inputs
            EvaluateWindow(playerController.triggersPressedThisWindow, songManager.triggersRequired);

            //reset the beat
            nextBeatDSPTime = (float)Clock.Instance.AtNextBeat();
            nextBeatCued = false;

            //Clear out input cache
            playerController.triggersPressedThisWindow.Clear();
            songManager.triggersRequired.Clear();

            //clear out current note list
            songManager.currentNotes.Clear();


        }

        //failsafe for resetting next beat

        if (!withinWindow) {
            nextBeatDSPTime = (float)Clock.Instance.AtNextBeat();
        }

	}

    /*
    public void EarlyEvalutateWindow(PlayerController.DrumTriggerPress<bool> triggersPressed, PlayerController.DrumTriggerPress<bool> triggersRequired) {
        //TODO call this function earlier in the required window in order
        //to have more responsive feedback (i.e. not waiting until the end of the
        //window to provide juice)


        foreach (Note note in songManager.currentNotes) {
            //There is a better way to do this... right now having a seperate
            //list for the "notes" that is decoupled from the triggersRequired class
            //cueing the notes into a list based on their associated times, and connecting them
            //to the associated input required via the note names... ugh

            if (triggersRequired != null) {
                if (note.name == "F#2" && triggersRequired[PlayerController.drumTriggers.HiHat]) {
                    if (triggersPressed[PlayerController.drumTriggers.HiHat]) {
                        Debug.Log("hi hat success!");

                        note.go.GetComponent<Obstacle>().TriggerSuccess();


                    } else {
                        //failure state
                        note.go.GetComponent<Obstacle>().TriggerFailure();
                    }
                }

                if (note.name == "C2" && triggersRequired[PlayerController.drumTriggers.Kick]) {
                    if (triggersPressed[PlayerController.drumTriggers.Kick]) {
                        Debug.Log("kick success!");
                        note.go.GetComponent<Obstacle>().TriggerSuccess();
                    } else {
                        note.go.GetComponent<Obstacle>().TriggerFailure();
                    }
                }

                if (triggersRequired[PlayerController.drumTriggers.Snare]) {
                    if (triggersPressed[PlayerController.drumTriggers.Snare]) {
                        Debug.Log("snare success!");
                    }
                }

                if (triggersRequired[PlayerController.drumTriggers.HiTom]) {
                    if (triggersPressed[PlayerController.drumTriggers.HiTom]) {
                        Debug.Log("HiTom success!");
                    }
                }
            }

        }
    }
    */

    public void EvaluateWindow(PlayerController.DrumTriggerPress<bool> triggersPressed, PlayerController.DrumTriggerPress<bool> triggersRequired) {

        foreach (MusicNote note in songManager.currentNotes) {
            //There is a better way to do this... right now having a seperate
            //list for the "notes" that is decoupled from the triggersRequired class
            //cueing the notes into a list based on their associated times, and connecting them
            //to the associated input required via the note names... ugh

            if (triggersRequired != null && note.go != null) {
                if (note.name == "F#2" && triggersRequired[PlayerController.drumTriggers.HiHat]) {
                    if (triggersPressed[PlayerController.drumTriggers.HiHat]) {
                        Debug.Log("hi hat success!");
                        note.go.GetComponent<Obstacle>().TriggerSuccess();

                    }
                    else {
                        Debug.Log("hi hat failure");
                        //failure state
                        note.go.GetComponent<Obstacle>().TriggerFailure();
                    }
                }

                if (note.name == "C2" && triggersRequired[PlayerController.drumTriggers.Kick]) {
                    if (triggersPressed[PlayerController.drumTriggers.Kick]) {
                        Debug.Log("kick success!");
                        note.go.GetComponent<Obstacle>().TriggerSuccess();


                    } else {
                        note.go.GetComponent<Obstacle>().TriggerFailure();
                    }
                }

                if (note.name == "D2" && triggersRequired[PlayerController.drumTriggers.Snare]) {
                    if (triggersPressed[PlayerController.drumTriggers.Snare]) {
                        Debug.Log("snare success!");
                        note.go.GetComponent<Obstacle>().TriggerSuccess();
                    } else {
                        note.go.GetComponent<Obstacle>().TriggerFailure();
                    }
                }

                if (triggersRequired[PlayerController.drumTriggers.HiTom]) {
                    if (triggersPressed[PlayerController.drumTriggers.HiTom]) {
                        Debug.Log("HiTom success!");
                    }
                }
            }

        }
        

    }




}

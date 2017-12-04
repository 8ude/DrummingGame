using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;
using DG.Tweening;

public class LevelMove : MonoBehaviour {
    //TODO subsume into LevelTrack or LevelCreator

    public bool gameOver;

    public SongManager songManager;

    public float moveSpeed = 1;
    public double nextMoveTime = 0d;
    public double prevMoveTime = 0d;

    public bool continuousMovement;
    float levelScale = 1;

    float startZPosition;
    float endZPosition;

    [SerializeField] float contMoveSpeed;


    void Start() {
        gameOver = false;
        levelScale = songManager.levelScale;

        nextMoveTime = AudioSettings.dspTime + Clock.Instance.StartDelay + 0.25f;
        prevMoveTime = nextMoveTime;

        endZPosition = -(songManager.BeatOfLastNote - songManager.levelOffset) * levelScale;
        Debug.Log(endZPosition);

    }



    void FixedUpdate() {
        //if (nextMoveTime == 0d) {
        //Debug.Log("initial move");
        //nextMoveTime = Clock.Instance.AtNextMeasure();
        //}
        if (continuousMovement) {
            if (Clock.Instance.Time < 0d) {
                return;
            } else {
                ContinuousMove();
            }
        }
        else if (AudioSettings.dspTime >= nextMoveTime) {
            
            prevMoveTime = nextMoveTime;

            if (nextMoveTime != 0d) {
                MoveLevel();
                Debug.Log("moving");
            }
            nextMoveTime = Clock.Instance.AtNextQuarter();

        }
    }
	
	// Update is called once per frame

    public void MoveLevel() {
        transform.DOMoveZ(transform.position.z - 1f, Clock.Instance.EighthLength()).SetEase(Ease.OutSine);
    }

    public void ContinuousMove() {
        float songPercentage = 1f - ((songManager.LastNoteTime - (float)Clock.Instance.Time) / songManager.LastNoteTime);
        //Debug.Log("song % " + songPercentage);

        transform.position = new Vector3(0f, 0f, songPercentage * endZPosition);
        
    }

}

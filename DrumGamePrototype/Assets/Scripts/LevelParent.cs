using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Beat;
using Dreamteck.Splines;

public class LevelParent : MonoBehaviour {

    [SerializeField] int numTracks;
    [SerializeField] float cylinderRadius;

    [SerializeField] SplineComputer sComputer;

    public bool gameOver;

    public GameObject trackPlanePrefab;

    public LevelTrack activeTrack;
    public int activeTrackIndex;

    GameObject[] tracks;

    public float moveSpeed = 1;
    public double nextMoveTime = 0d;
    public double prevMoveTime = 0d;

    public bool continuousMovement;
    float levelScale = 1;

    float startZPosition;
    float endZPosition;
    float rotationAngle;

    float currentRotation;

    [SerializeField] float contMoveSpeed;




    void Awake() {
        
        
    }


    void Start() {

        numTracks = SongManager.instance.CurrentSong.tracks.Length;
        tracks = new GameObject[numTracks];
        rotationAngle = 360f  / numTracks;
        currentRotation = 0f;

        for (int i = 0; i < numTracks; i++) {
            float xPos = cylinderRadius * Mathf.Sin(2 * Mathf.PI * i / numTracks);
            float yPos = cylinderRadius * Mathf.Cos(2 * Mathf.PI * i / numTracks);
            Vector3 origPos = new Vector3(xPos, yPos, 0);


            GameObject newTrack = Instantiate(trackPlanePrefab, Vector3.zero, Quaternion.identity);

            //Fill track with notes, assign those notes to game objects
            newTrack.GetComponent<LevelTrack>().trackIndex = i;
            newTrack.GetComponent<LevelTrack>().PropogateTrack(SongManager.instance.CurrentSong.tracks[i], sComputer);


            newTrack.transform.Rotate(- Vector3.forward * 360f * i / numTracks);
            newTrack.transform.position = origPos;


            tracks[i] = newTrack;
            newTrack.transform.SetParent(transform);



        }

        activeTrack = tracks[0].GetComponent<LevelTrack>();

        gameOver = false;
        levelScale = SongManager.instance.levelScale;

        nextMoveTime = AudioSettings.dspTime + Clock.Instance.StartDelay + 0.25f;
        prevMoveTime = nextMoveTime;

        transform.position = new Vector3(0f, -1f * cylinderRadius, 0f);

        endZPosition = -(SongManager.instance.BeatOfLastNote - SongManager.instance.levelOffset) * levelScale;
       
    }



    void FixedUpdate() {
        //if (nextMoveTime == 0d) {
        //Debug.Log("initial move");
        //nextMoveTime = Clock.Instance.AtNextMeasure();
        //}
        if (continuousMovement) {
            if (Clock.Instance.Time < 0d) {
                return;
            }
            else {
                //ContinuousMove();
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
        //Debug.Log(SongManager.instance.LastNoteTime);

        float songPercentage = 1f - ((SongManager.instance.LastNoteTime - (float)Clock.Instance.Time) / SongManager.instance.LastNoteTime);
        //Debug.Log("last note time " + SongManager.instance.LastNoteTime);
        //Debug.Log("song % " + songPercentage);

        transform.position = new Vector3(0f, -1f * cylinderRadius, songPercentage * endZPosition);

    }

    public void RotateRight() {
        //StartCoroutine(RotateLevelRight(Clock.Instance.SixteenthLength()));
        //transform.DOLocalRotate(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation + rotationAngle), Clock.Instance.SixteenthLength());
        //currentRotation += rotationAngle;
        activeTrackIndex++;
        if (activeTrackIndex >= numTracks) {
            activeTrackIndex = activeTrackIndex % numTracks;

        } 
        activeTrack = tracks[activeTrackIndex].GetComponent<LevelTrack>();

        Debug.Log(activeTrackIndex);

    }

    public void RotateLeft() {
        //StartCoroutine(RotateLevelLeft(Clock.Instance.SixteenthLength()));
        //transform.DOLocalRotate(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation - rotationAngle), Clock.Instance.SixteenthLength());
        //currentRotation -= rotationAngle;

        activeTrackIndex--;
        if (activeTrackIndex < 0) {
            activeTrackIndex = numTracks - 1;
        } 
        activeTrack = tracks[activeTrackIndex].GetComponent<LevelTrack>();
    }


}

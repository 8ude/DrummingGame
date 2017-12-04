using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Beat;

//Modifying this for Drum Rhythm Game

public class SongManager : MonoBehaviour {

    public static SongManager instance;

    Note lastNote;

    private AudioSource audio;
    private Track[] tracks;

    //Todo - remove
    //private Note[,] notes;

    LevelParent levelParent;


    private float[] hihatTimes, kickTimes, snareTimes, sTomTimes, lTomTimes;

    public Material hiHatMaterial, kickMaterial, snareMaterial, hTomMaterial, lTomMaterial;


    private float timeSpent;
    private float startTime;
    private float speed;
    private float delay;
    //private string[] noteMap;
    //private Hashtable posMap;
    private int posCount;

    public float timeScale = 2f;
    public float levelOffset = 2f;

    private bool isGameOver = false;
    private bool isGameStoped = false;

    private float endWaitTime = 2f;

    private GameObject player;

    Song currentSong;

    public Song CurrentSong {
        get {
            return currentSong;
        }
    }
    public float levelScale;
    public GameObject levelPrefab;
    public GameObject snareBlockPrefab;
    public GameObject stomperPrefab;
    public GameObject trackParent;

    public PlayerController.DrumTriggerPress<bool> triggersRequired = new PlayerController.DrumTriggerPress<bool>();


    [SerializeField]
    private bool songStart = false;

    private bool songStarted = false;

    [SerializeField]

    private int lastPos;

    public int currentBeat;

    public List<Note> currentNotes = new List<Note>();


    //time in ms of last note
    private float timeOfLastNote;
    public float LastNoteTime {
        get {
            return timeOfLastNote;
        }
    }

    private float _beatOfLastNote;
    public float BeatOfLastNote {
        get {
            return _beatOfLastNote;
        }
    }

    #region initialization methods

    void Awake() {
         
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject); 
        }
        levelParent = GameObject.FindWithTag("Level").GetComponent<LevelParent>();
        PrepareData("testdrums");
    }

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");

        currentBeat = 0;

    }

    public void SetupGame(SongInfo songInfo) {
        AudioClip clip = Resources.Load<AudioClip>("Audios/" + songInfo.bgMusicName);
        audio = GetComponent<AudioSource>();
        audio.clip = clip;
        //speed = songInfo.timeFromShooterToCenter;
        //delay = songInfo.manuallyDelay;

    }

    void PrepareData(string name) {
        currentSong = SongsSource.getSong(name);
        if (currentSong.tracks.Length > 0) {
            

            //Debug.Log("notes length row " + notes.GetLength(1));
            for (int i = 0; i < currentSong.tracks.Length; i++) {
                //go through each track first
                for (int j = 0; j < currentSong.tracks[i].notes.Length; j++) {
                    //Debug.Log("i = " + i + " j = " + j);

                    //this will convert the note time from seconds to beats
                    currentSong.tracks[i].notes[j].beatTime = currentSong.tracks[i].notes[j].time * currentSong.header.bpm / 60f;
                    // assuming common time for now
                    int measure = Mathf.FloorToInt(currentSong.tracks[i].notes[j].beatTime / 4f);
                    int beat = Mathf.FloorToInt(currentSong.tracks[i].notes[j].beatTime - (measure * 4));
                    int tick = Mathf.FloorToInt((currentSong.tracks[i].notes[j].beatTime - (measure * 4) - beat) * 96);
                    currentSong.tracks[i].notes[j].mbtValue = new MBT(measure, beat, tick);
                }

                //PropogateLevel(i);
                GetAbsoluteTimes(i);
            }
        }


        Debug.Log("last mbt = " + lastNote.mbtValue.ToString());



    }


    /*
     * MOVED TO LEVEL TRACK SCRIPT
     * 
    void PropogateLevel(Track track) {


        for (int i = 0; i < notes.GetLength(trackIndex); i++) {
            float zPos = notes[trackIndex, i].beatTime * levelScale + levelOffset;
            Vector3 newBlockPosition = new Vector3(0f, 0f, zPos);
            GameObject newBlock;



            //notes.time starts at 4, which is 12 beats before the first input

            switch (notes[trackIndex,i].name) {
                case "C2":
                    //kick

                    //top block
                    newBlock = Instantiate(levelPrefab, newBlockPosition, Quaternion.identity);
                    Vector3 newPos = new Vector3(newBlock.transform.position.x, 4f, zPos);
                    Vector3 newScale = new Vector3(4f, notes[trackIndex,i].velocity * 2f, 0.75f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(trackParent.transform);
                    newBlock.GetComponent<Renderer>().material = kickMaterial;

                    //stomp pad (connected to evaluator)
                    Vector3 stompPos = new Vector3(newBlock.transform.position.x, -0.7f, zPos);
                    GameObject newStomp = Instantiate(stomperPrefab, stompPos, Quaternion.identity);
                    newStomp.transform.SetParent(trackParent.transform);
                    notes[trackIndex,i].go = newStomp;

                    break;
                case "D2":
                    //snare
                    newBlock = Instantiate(snareBlockPrefab, newBlockPosition, Quaternion.identity);
                    newPos = new Vector3(newBlock.transform.position.x, 0f, zPos);
                    newScale = new Vector3(4f, notes[trackIndex,i].velocity * 2f, 0.75f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(trackParent.transform);
                    newBlock.GetComponent<Renderer>().material = snareMaterial;
                    notes[trackIndex, i].go = newBlock;
                    break;
                case "F2":
                    //low tom, doesn't work on current controller?

                    newBlock = Instantiate(levelPrefab, newBlockPosition, Quaternion.identity);
                    newPos = new Vector3(-2.5f, 0f, zPos);
                    newScale = new Vector3(2f, 5f, 1f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(trackParent.transform);
                    newBlock.GetComponent<Renderer>().material = lTomMaterial;
                    notes[trackIndex, i].go = newBlock;


                    break;
                case "F#2":
                    newBlock = Instantiate(levelPrefab, newBlockPosition, Quaternion.identity);
                    newPos = new Vector3(0f, 0f, zPos);
                    newScale = new Vector3(3f, 0.5f, 0.5f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(trackParent.transform);
                    newBlock.GetComponent<Renderer>().material = hiHatMaterial;
                    notes[trackIndex, i].go = newBlock;
                    //hi hat
                    break;
                case "A2":
                    newBlock = Instantiate(levelPrefab, newBlockPosition, Quaternion.identity);
                    newPos = new Vector3(0f, 0f, zPos);
                    newScale = new Vector3(3f, 5f, 0.75f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(trackParent.transform);
                    newBlock.GetComponent<Renderer>().material = hTomMaterial;
                    notes[trackIndex, i].go = newBlock;
                    //hi tom
                    break;
            }
        }

    }
    */

    public void GetAbsoluteTimes(int trackIndex) {

        //just to save on line length
        Track currentTrack = currentSong.tracks[trackIndex];

        //propogetes the "game time" property of Notes (in s), relative to Clock.Instance.Time
        for (int k = 0; k < currentTrack.notes.Length; k++) {
            currentTrack.notes[k].gameTime = ((currentTrack.notes[k].beatTime) * timeScale * 60f / (float)Clock.Instance.BPM);
        }
        if (currentTrack.notes[currentTrack.notes.Length - 1].gameTime > timeOfLastNote) {
            lastNote = currentTrack.notes[currentTrack.notes.Length - 1];
            timeOfLastNote = currentTrack.notes[currentTrack.notes.Length - 1].gameTime;
            Debug.Log("time of last note = " + timeOfLastNote);
            _beatOfLastNote = currentTrack.notes[currentTrack.notes.Length - 1].beatTime;
        }
        //Debug.Log(timeOfLastNote);
    }

    #endregion

    public void CueNextObstacles(float obstacleCrossTime) {
        //TODO make this dynamic according to what track we're on
        //Debug.Log("cue");
        foreach (Note note in levelParent.activeTrack.notes) {
            if (Mathf.RoundToInt(note.beatTime) == currentBeat) {
                currentNotes.Add(note);
                switch (note.name) {
                    case "C2":
                        //kick
                        triggersRequired[PlayerController.drumTriggers.Kick] = true;

                        break;
                    case "D2":
                        //snare
                        triggersRequired[PlayerController.drumTriggers.Snare] = true;
                        break;
                    case "F2":
                        triggersRequired[PlayerController.drumTriggers.LowTom] = true;
                        //low tom
                        break;
                    case "F#2":
                        triggersRequired[PlayerController.drumTriggers.HiHat] = true;

                        note.go.GetComponent<Obstacle>().ConnectFunction(obstacleCrossTime, player);
                        //hi hat
                        break;
                    case "A2":
                        triggersRequired[PlayerController.drumTriggers.HiTom] = true;
                        //hi tom
                        break;
                }
            }
        }
    }


    #region unused methods
    void StartLoop() {
        //StartCoroutine(ShootBeatClock());
    }

    void SongEnd() {
        //        GameObject scoreManager = GameObject.Find ("ScoreManager");
        //        scoreManager.GetComponent<ScoreManager> ().ShowResult ();
        isGameOver = true;
    }

    void GameOverEnd() {
        //        GameObject scoreManager = GameObject.Find ("ScoreManager");
        //        scoreManager.GetComponent<ScoreManager> ().ShowGameOver ();
    }



    public void AdvanceBeat() {

    }

    /*
    IEnumerator ShootBeatClock()
    {

        for(int i = 0; i < notes.Length; i++) {

            if (isGameOver) {
                if (!isGameStoped) {
                    Invoke ("GameOverEnd", 1f);
                    isGameStoped = true;
                }
            } else {

                float adjustTime = Time.time + speed + delay - notes [i].time - timeSpent;
                float timeToWait = 0;

                if (i == (notes.Length - 1)) {
                    timeToWait = Time.time - timeSpent + speed - notes [i].time;
                    Invoke ("SongEnd", endWaitTime);
                } else {
                    timeToWait = notes [i + 1].time - notes [i].time - adjustTime;
                }


                    
//                GameObject.Find (name).GetComponent<SpriteRenderer> ().material.DOFade(0.3f, 0.1f);
                //float initRotateZ = (float)(pos + 1) * 360 / posCount - (1 - arcInitAngle)*180;
//                GameObject arcObj = Instantiate (arc, new Vector3 (0, 0, arcInitZ), Quaternion.identity);
//                arcObj.GetComponent<ArcController> ().InitArc (arcInitAngle, initRotateZ, arcInitZ, speed, arcRotateSpeed, spin, spinReverse);
//                GameObject beatObj = Instantiate (beat[pos], new Vector3 (dist * Mathf.Cos (Mathf.Deg2Rad * (360 / shooterNum * pos)), dist * Mathf.Sin (Mathf.Deg2Rad * (360 / shooterNum * pos)), 0), Quaternion.identity);
//                Vector2 dir = new Vector2 (dist * Mathf.Cos (Mathf.Deg2Rad * (360 / shooterNum * pos)), dist * Mathf.Sin (Mathf.Deg2Rad * (360 / shooterNum * pos)));
//                beatObj.GetComponent<Rigidbody2D> ().velocity = -dir.normalized * (dist / speed);

//                if (!showTrailEffect) {
//                    beatObj.GetComponent<TrailRenderer> ().enabled = false;
//                }

                float progressPercent = (float)(i + 1) / (float)notes.Length;
                //progressBar.GetComponent<SpriteRenderer>().material.SetFloat ("_Progress", progressPercent);
                float progressTime = notes [i].duration;
//                progress.GetComponent<ProgressController> ().UpdateProgress (progressPercent, progressTime);

                //set next time
                yield return new WaitForSeconds(timeToWait);
            }
        }
    }
    */
    #endregion

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Beat;


public class SongManager : MonoBehaviour {

    public static SongManager instance;

    MusicNote lastNote;

    public MusicNote LastNote {
        get {
            return lastNote;
        }
    }

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
    private int posCount;

    public float timeScale = 1f;
    public float levelOffset = 2f;

    private bool isGameOver = false;
    private bool isGameStoped = false;

    private float endWaitTime = 2f;

    public float tubeRadius = 5f;

    float numLoops;
    public float NumLoops {
        get {
            return numLoops;
        }
    }

    private GameObject player;
    public GameObject PlayerObject {
        get {
            return player;
        }
    }

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

    public List<MusicNote> currentNotes = new List<MusicNote>();


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
        numLoops = 0f;
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
            

            for (int i = 0; i < currentSong.tracks.Length; i++) {
                //go through each track first
                for (int j = 0; j < currentSong.tracks[i].notes.Length; j++) {

                    //this will convert the note time from seconds to beats
                    currentSong.tracks[i].notes[j].beatTime = currentSong.tracks[i].notes[j].time * currentSong.header.bpm / 60f;
                    //assuming 4/4 time for now
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
    /// <summary>
    /// Cues up the next obstacle(s) in the timeline based on what track the player is on
    /// </summary>
    /// <param name="obstacleCrossTime">Obstacle cross time.</param>
    public void CueNextObstacles(float obstacleCrossTime) {

        foreach (MusicNote note in levelParent.activeTrack.notes) {
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

    public void RepeatLoop() {
        Debug.Log("repeatLoop");
        currentBeat = 0;
        numLoops += 1f;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class LevelTrack : MonoBehaviour {

    public bool currentActiveTrack = false;

    public int trackIndex;

    public GameObject levelPrefab;
    public GameObject snareBlockPrefab;
    public GameObject stomperPrefab;

    public MusicNote[] notes;
    private float[] hihatTimes, kickTimes, snareTimes, sTomTimes, lTomTimes;

    public Material hiHatMaterial, kickMaterial, snareMaterial, hTomMaterial, lTomMaterial;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PropogateTrack() {
        //Take Method from Song Manager, put it here 
        //parent everything to the plane's local transform; 
        //translate/rotate to be part of LevelParent cylinder


    }
    public void PropogateTrack(Track track) {

        notes = track.notes;


        for (int i = 0; i < notes.Length; i++) {
            //Need to set the position to a 0-1 scale for use with spline positioner
            float zPos = notes[i].beatTime * SongManager.instance.levelScale + SongManager.instance.levelOffset;

            float angle = trackIndex * 360f / SongManager.instance.CurrentSong.tracks.Length;
            Vector3 newBlockPosition = new Vector3(0f, 0f, zPos);
            GameObject newBlock;
            


            //notes.time starts at 4, which is 12 beats before the first input

            switch (notes[i].name) {
                case "C2":
                    //kick

                    //top block
                    newBlock = Instantiate(levelPrefab, newBlockPosition, Quaternion.identity);
                    Vector3 newPos = new Vector3(newBlock.transform.position.x, 4f, zPos);
                    Vector3 newScale = new Vector3(4f, notes[i].velocity * 2f, 0.75f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = kickMaterial;

                    //stomp pad (connected to evaluator)
                    Vector3 stompPos = new Vector3(newBlock.transform.position.x, -0.7f, zPos);
                    GameObject newStomp = Instantiate(stomperPrefab, stompPos, Quaternion.identity);
                    newStomp.transform.SetParent(transform);
                    notes[i].go = newStomp;

                    break;
                case "D2":
                    //snare
                    newBlock = Instantiate(snareBlockPrefab, newBlockPosition, Quaternion.identity);
                    newPos = new Vector3(newBlock.transform.position.x, 0f, zPos);
                    newScale = new Vector3(4f, notes[i].velocity * 2f, 0.75f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = snareMaterial;
                    notes[i].go = newBlock;
                    break;
                case "F2":
                    //low tom, doesn't work on current controller?

                    newBlock = Instantiate(levelPrefab, newBlockPosition, Quaternion.identity);
                    newPos = new Vector3(-2.5f, 0f, zPos);
                    newScale = new Vector3(2f, 5f, 1f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = lTomMaterial;
                    notes[i].go = newBlock;


                    break;
                case "F#2":
                    newBlock = Instantiate(levelPrefab, newBlockPosition, Quaternion.identity);
                    newPos = new Vector3(0f, 0f, zPos);
                    newScale = new Vector3(3f, 0.5f, 0.5f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = hiHatMaterial;
                    notes[i].go = newBlock;
                    //hi hat
                    break;
                case "A2":
                    newBlock = Instantiate(levelPrefab, newBlockPosition, Quaternion.identity);
                    newPos = new Vector3(0f, 0f, zPos);
                    newScale = new Vector3(3f, 5f, 0.75f);
                    newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = hTomMaterial;
                    notes[i].go = newBlock;
                    //hi tom
                    break;
            }
        }
    }


}

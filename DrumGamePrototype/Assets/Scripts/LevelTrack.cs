using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Beat;

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

  

    //Take Method from Song Manager, put it here 
    //parent everything to the plane's local transform; 
    //translate/rotate to be part of LevelParent cylinder
    public void PropogateTrack(Track track, SplineComputer splineComp) {

        notes = track.notes;


        for (int i = 0; i < notes.Length; i++) {
            //Need to set the position to a 0-1 scale for use with spline positioner
            //NOTE - not necessary with spline component
            //float zPos = notes[i].beatTime * SongManager.instance.levelScale + SongManager.instance.levelOffset;
            //Vector3 newBlockPosition = new Vector3(0f, 0f, zPos);

            double songPercent = (double)(notes[i].gameTime) / (SongManager.instance.LastNoteTime);

            float angle = trackIndex * 2f * Mathf.PI / SongManager.instance.CurrentSong.tracks.Length;
            //Debug.Log(angle);

            GameObject newBlock;


            switch (notes[i].name) {
                case "C2":
                    //kick

                    //top block
                    newBlock = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);



                    //Vector3 newPos = new Vector3(newBlock.transform.position.x, 4f, zPos);
                    Vector3 newScale = new Vector3(4f, notes[i].velocity * 2f, 0.75f);
                    //newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;

                    SplinePositioner sPositioner = newBlock.GetComponent<SplinePositioner>();
                    sPositioner.computer = splineComp;
         
                    sPositioner.motion.offset = new Vector2(Mathf.Sin(angle) * SongManager.instance.tubeRadius, Mathf.Cos(angle) * SongManager.instance.tubeRadius);
                    sPositioner.motion.rotationOffset = new Vector3(0f, 0f, -Mathf.Rad2Deg * angle );
                    sPositioner.SetPercent(songPercent);


                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = kickMaterial;

                    //stomp pad (connected to evaluator)
                    ///Vector3 stompPos = new Vector3(newBlock.transform.position.x, -0.7f, zPos);
                    GameObject newStomp = Instantiate(stomperPrefab, Vector3.zero, Quaternion.identity);
                    SplinePositioner stompPositioner = newStomp.GetComponent<SplinePositioner>();
                    stompPositioner.computer = splineComp;
                    stompPositioner.motion.offset = new Vector2(Mathf.Sin(angle) * SongManager.instance.tubeRadius, Mathf.Cos(angle) * SongManager.instance.tubeRadius);
                    stompPositioner.motion.rotationOffset = new Vector3(0f, 0f, -Mathf.Rad2Deg * angle);
                    stompPositioner.SetPercent(songPercent);

                    //newStomp.transform.SetParent(transform);
                    notes[i].go = newStomp;

                    break;
                case "D2":
                    //snare
                    newBlock = Instantiate(snareBlockPrefab, Vector3.zero, Quaternion.identity);
                    //newPos = new Vector3(newBlock.transform.position.x, 0f, zPos);
                    newScale = new Vector3(4f, notes[i].velocity * 2f, 0.75f);
                    //newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;

                    //Set Block's position along the spline (based on it's % in the song loop)
                    //Adjust rotation accordingly
                    sPositioner = newBlock.GetComponent<SplinePositioner>();
                    sPositioner.computer = splineComp;
                    sPositioner.motion.offset = new Vector2(Mathf.Sin(angle) * SongManager.instance.tubeRadius, Mathf.Cos(angle) * SongManager.instance.tubeRadius);
                    sPositioner.motion.rotationOffset = new Vector3(0f, 0f, -Mathf.Rad2Deg * angle);
                    sPositioner.SetPercent(songPercent);

                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = snareMaterial;
                    notes[i].go = newBlock;
                    break;
                case "F2":
                    //low tom, doesn't work on current controller

                    newBlock = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
                    //newPos = new Vector3(-2.5f, 0f, zPos);
                    newScale = new Vector3(2f, 5f, 1f);
                    //newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;

                    sPositioner = newBlock.GetComponent<SplinePositioner>();
                    sPositioner.computer = splineComp;
                    sPositioner.motion.offset = new Vector2(Mathf.Sin(angle) * SongManager.instance.tubeRadius, Mathf.Cos(angle) * SongManager.instance.tubeRadius);
                    sPositioner.motion.rotationOffset = new Vector3(0f, 0f, -Mathf.Rad2Deg * angle);
                    sPositioner.SetPercent(songPercent);

                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = lTomMaterial;
                    notes[i].go = newBlock;


                    break;
                case "F#2":
                    newBlock = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
                    //newPos = new Vector3(0f, 0f, zPos);
                    newScale = new Vector3(3f, 0.5f, 0.5f);

                    sPositioner = newBlock.GetComponent<SplinePositioner>();
                    sPositioner.computer = splineComp;

                    sPositioner.motion.offset = new Vector2(Mathf.Sin(angle) * SongManager.instance.tubeRadius, Mathf.Cos(angle) * SongManager.instance.tubeRadius);
                    sPositioner.motion.rotationOffset = new Vector3(0f, 0f, -Mathf.Rad2Deg * angle);
                    sPositioner.SetPercent(songPercent);

                    //newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;
                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = hiHatMaterial;
                    notes[i].go = newBlock;
                    //hi hat
                    break;
                case "A2":
                    newBlock = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
                    //newPos = new Vector3(0f, 0f, zPos);
                    newScale = new Vector3(3f, 5f, 0.75f);
                    //newBlock.transform.position = newPos;
                    newBlock.transform.localScale = newScale;

                    sPositioner = newBlock.GetComponent<SplinePositioner>();
                    sPositioner.computer = splineComp;
                    sPositioner.motion.offset = new Vector2(Mathf.Sin(angle) * SongManager.instance.tubeRadius, Mathf.Cos(angle) * SongManager.instance.tubeRadius);
                    sPositioner.motion.rotationOffset = new Vector3(0f, 0f, -Mathf.Rad2Deg * angle);
                    sPositioner.SetPercent(songPercent);

                    newBlock.transform.SetParent(transform);
                    newBlock.GetComponent<Renderer>().material = hTomMaterial;
                    notes[i].go = newBlock;
                    //hi tom
                    break;
            }
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public GameObject winParticles;
    public GameObject loseParticles;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ConnectFunction(float timeToCross, GameObject player) {
        StartCoroutine(ConnectAtTime(timeToCross, player));
    }

    public IEnumerator ConnectAtTime(float timeToCross, GameObject player) {
       // Debug.Log("moving");

        //Debug.Log("time to cross " + timeToCross);
        //Debug.Log("dsp time " + AudioSettings.dspTime);
        while (AudioSettings.dspTime < timeToCross) {

            //Debug.Log("moving");

            Vector3 velocity = player.transform.position - transform.position;

            velocity *= 1f/(timeToCross - (float)AudioSettings.dspTime);

            //Debug.Log("velocity: " + velocity); 

            transform.Translate( velocity * Time.deltaTime);

            yield return null;
        }
    }

    public void TriggerSuccess() {
        gameObject.GetComponent<Renderer>().enabled = false;
        GameObject successParticles = Instantiate(winParticles, transform.position, Quaternion.identity);
        Destroy(successParticles, 1f);
        Destroy(gameObject, 1f);



    }

    public void TriggerFailure() {
        gameObject.GetComponent<Renderer>().enabled = false;
        GameObject failParticles = Instantiate(loseParticles, transform.position, Quaternion.identity);
        Destroy(failParticles, 1f);
        Destroy(gameObject, 1f);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluffyUnderware.Curvy.Controllers;
using DG.Tweening;

public class CurveFollowerTest : MonoBehaviour {

    float rotationTime = 0.2f;
    float rotationZ;

	// Use this for initialization
	void Start () {
        rotationZ = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("drum_far_right")) {
            //GetComponent<SplineController>();

            transform.parent.DOLocalRotate(new Vector3(transform.rotation.x, transform.rotation.y, rotationZ + 30f), 0.4f);
            rotationZ += 30f;
        } else if (Input.GetButtonDown("drum_far_left")) {
            
        }	
	}
}

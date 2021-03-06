﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

	public GameObject OVRCameraRig;
	//public GameObject leftHandAnchor;
	//public GameObject rightHandAnchor;
	public float scaleFactor = 2; 
	public float rotateSpeed = 100;

	private float prevDist = 0f; //Check previous distance of controllers

	private float handTriggerStateLeft = 0;
	private float oldHandTriggerStateLeft = 0;

	private float handTriggerStateRight = 0;
	private float oldHandTriggerStateRight = 0;

	private bool leftIsPressed = false;
	private bool rightIsPressed = false;

	private Vector3 referencePosLeft;
	private Vector3 referencePosRight;

	//private Vector3 referenceCamPos;
	//private Vector3 referenceCamScale;

	// Update is called once per frame
	void Update () {

		//Take in hand positions.
		//Vector3 lpos = leftHandAnchor.transform.position;
		//Vector3 rpos = rightHandAnchor.transform.position;
		Vector3 lpos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch); 
		Vector3 rpos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

		//Take in trigger positions, comparing to previous positions.
		oldHandTriggerStateLeft = handTriggerStateLeft;
		oldHandTriggerStateRight = handTriggerStateRight;

		handTriggerStateLeft = OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
		handTriggerStateRight = OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);


		//Set booleans that are true if the triggers are pressed.
		if (handTriggerStateLeft > 0.9f) {
			leftIsPressed = true;
		} else {
			leftIsPressed = false;
		}

		if (handTriggerStateRight > 0.9f) {
			rightIsPressed = true;
		} else {
			rightIsPressed = false;
		}
			


		//NAVIGATION - SCALING: Scale at constant rate as long as controllers are moving towards/away
		if (leftIsPressed && rightIsPressed) {
			float dist = (lpos - rpos).magnitude;
			float delta = dist - prevDist;
			if (Mathf.Abs (delta) > 0.01f) {
				OVRCameraRig.transform.localScale -= (Vector3.one * Time.deltaTime * Mathf.Sign (delta) * scaleFactor);
			}
			prevDist = dist;
		} else if (leftIsPressed || rightIsPressed) {
			

			//When the triggers are pressed, set the controller's current location and the camera's position as reference positions
			if ((leftIsPressed && oldHandTriggerStateLeft < 0.9f) || leftIsPressed && oldHandTriggerStateRight > 0.9f) {
				referencePosLeft = lpos;
				//referenceCamPos = OVRCameraRig.transform.localPosition;
				print ("Reference point set.");

			}

			if ((rightIsPressed && oldHandTriggerStateRight < 0.9f) || (rightIsPressed && oldHandTriggerStateLeft > 0.9f)) {
				referencePosRight = rpos;
				//referenceCamPos = OVRCameraRig.transform.localPosition;

			}


			//NAVIGATION - MOVING: Let the user pull themselves around the environment by pressing one grip.
			if (leftIsPressed) {
				Vector3 rawDelta = lpos - referencePosLeft; 
				Vector3 delta = rawDelta;
				//OVRCameraRig.transform.localPosition = referenceCamPos - delta * OVRCameraRig.transform.localScale.magnitude;
				OVRCameraRig.transform.Translate((-1)*delta * OVRCameraRig.transform.localScale.magnitude);
				referencePosLeft = lpos;

			} else if (rightIsPressed) {
				Vector3 delta = rpos - referencePosRight;
				//OVRCameraRig.transform.localPosition = referenceCamPos - delta * OVRCameraRig.transform.localScale.magnitude;
				OVRCameraRig.transform.Translate((-1)*delta * OVRCameraRig.transform.localScale.magnitude);
				referencePosRight = rpos;

			}
				
			//ROTATION - Use either anlog stick while either grip is pressed to rotate.
			Vector2 primeThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
			Vector2 secondaryThumbstick = OVRInput.Get (OVRInput.Axis2D.SecondaryThumbstick);
			float rotationCommand = primeThumbstick.x + secondaryThumbstick.x;

			if (Mathf.Abs (rotationCommand) > 0) {
				//print (primeThumbstick);
				//print (rotationCommand);
				Vector3 rotationVector = new Vector3 (0f, rotationCommand * rotateSpeed * Time.deltaTime, 0f);
				OVRCameraRig.transform.eulerAngles = OVRCameraRig.transform.eulerAngles + rotationVector;

				//reset references
				//referenceCamPos = OVRCameraRig.transform.localPosition;
				//referencePosLeft = lpos;
				//referencePosRight = rpos;
			}

		}


		/*
		//Attempting to set camera scale, instsead of adjusting at constant rate
		//Multiplies noise - causes jitters when at furthest from reference position
		//If both triggers are pressed...
		if (leftIsPressed && rightIsPressed) {

			//Set reference camera scale if one of the buttons was just pressed
			if ((leftIsPressed && oldHandTriggerStateLeft < 0.9f) || (rightIsPressed && oldHandTriggerStateRight < 0.9f)) {
				referenceCamScale = OVRCameraRig.transform.localScale;
			}

			//Compare reference distance against current distance
			float referenceDistance = (referencePosLeft - referencePosRight).magnitude;
			float currentDistance = (lpos - rpos).magnitude;
			float delta = currentDistance - referenceDistance;

			Vector3 deltaVector = (Vector3.one * Time.deltaTime * delta * scaleFactor);

			//Set the new camera scale to be bigger or smaller than the reference scale by an amount proportional to the difference in controller distance.
			OVRCameraRig.transform.localScale = referenceCamScale + deltaVector;

		}

		*/
	}
}

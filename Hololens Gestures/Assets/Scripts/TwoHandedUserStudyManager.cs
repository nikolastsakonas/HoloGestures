// Copyright (C) 2017 The Regents of the University of California (Regents).
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//
//     * Redistributions in binary form must reproduce the above
//       copyright notice, this list of conditions and the following
//       disclaimer in the documentation and/or other materials provided
//       with the distribution.
//
//     * Neither the name of The Regents or University of California nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//
// Please contact the author of this library if you have any questions.
// Author: Nikolas Chaconas (nikolas.chaconas@gmail.com)

﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public class TransformHelper {
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 localScale;

	public TransformHelper(Transform transform) {
		position = transform.position;
		rotation = transform.rotation;
		localScale = transform.localScale;
	}
}

public class SaveDataClass
{
    public SaveDataClass(TwoHandedUserStudyManager _instance)
    {
        /*int section1 = _instance.USER_STUDY_TECHNIQUE_INDEX + 1;
        int technique1 = _instance.USER_STUDY_TECHNIQUE + 1;
        int hologramNum = _instance.hologramIndex + 1;

        userID = _instance.currentUserID;
        section = section1.ToString();
        technique = technique1.ToString();
        hologram = hologramNum.ToString();
        time = _instance.getTime();

        if (TwoHandedUserStudyManager.handsManager.foundLeft)
        {
            leftHandPositionX = TwoHandedUserStudyManager.handsManager.leftHandPosition.x.ToString();
            leftHandPositionY = TwoHandedUserStudyManager.handsManager.leftHandPosition.y.ToString();
            leftHandPositionZ = TwoHandedUserStudyManager.handsManager.leftHandPosition.z.ToString();
        }

        if (TwoHandedUserStudyManager.handsManager.foundRight)
        {
            rightHandPositionX = TwoHandedUserStudyManager.handsManager.rightHandPosition.x.ToString();
            rightHandPositionY = TwoHandedUserStudyManager.handsManager.rightHandPosition.y.ToString();
            rightHandPositionZ = TwoHandedUserStudyManager.handsManager.rightHandPosition.z.ToString();
        }

        int index = _instance.hologramIndex;
        GameObject manipulatableObject = _instance.manipulatableGameObjects[index];

        Quaternion manipulatableObjectRotation = manipulatableObject.transform.rotation;
        Vector3 manipulatableObjectLocalScale = manipulatableObject.transform.localScale;

		if (_instance.hologramShown) {
			rotationX = manipulatableObjectRotation.x.ToString ();
			rotationY = manipulatableObjectRotation.y.ToString ();
			rotationZ = manipulatableObjectRotation.z.ToString ();

			localScale = manipulatableObjectLocalScale.x.ToString ();
		}*/
    }

    public void setTransforms(GameObject manipulatableObject, GameObject targetObject)
    {
        /*Quaternion manipulatableObjectRotation = manipulatableObject.transform.rotation;
        Vector3 manipulatableObjectLocalScale = manipulatableObject.transform.localScale;

        Quaternion targetObjectRotation = targetObject.transform.rotation;
        Vector3 targetObjectLocalScale = targetObject.transform.localScale;

        rotationX = manipulatableObjectRotation.x.ToString();
        rotationY = manipulatableObjectRotation.y.ToString();
        rotationZ = manipulatableObjectRotation.z.ToString();

        localScale = manipulatableObjectLocalScale.x.ToString();

        targetRotationX = targetObjectRotation.x.ToString();
        targetRotationY = targetObjectRotation.y.ToString();
        targetRotationZ = targetObjectRotation.z.ToString();

        targetLocalScale = targetObjectLocalScale.x.ToString();

        angleBetweenRotations = Quaternion.Angle(manipulatableObjectRotation, targetObjectRotation).ToString();*/
    }


    //need these
    public bool startButtonPress = false;
    public bool continueButtonPress = false;
    public bool beginningGesture = false;
    public bool endingGesture = false;
    public bool leftLost = false;
    public bool rightLost = false;
    public bool leftDown = false;
    public bool rightDown = false;
    public bool leftUp = false;
    public bool rightUp = false;
    public bool inRotationGesture = false;
    public bool inResizeGesture = false;
	public bool update = false;
	public bool foundLeft = false;
	public bool foundRight = false;

    public string targetRotationX = "N/A";
    public string targetRotationY = "N/A";
    public string targetRotationZ = "N/A";

    public string angleBetweenRotations = "N/A";

    public string targetLocalScale = "N/A";

    public bool training = false;
    public string trainingSection = "N/A";


    //dont need fill these unless diff than default
    public string userID = "N/A";
    public string section = "N/A";
    public string technique = "N/A";
    public string hologram = "N/A";
    public string time = "N/A";

    public string leftHandPositionX = "N/A";
    public string leftHandPositionY = "N/A";
    public string leftHandPositionZ = "N/A";

    public string rightHandPositionX = "N/A";
    public string rightHandPositionY = "N/A";
    public string rightHandPositionZ = "N/A";

    public string rotationX = "N/A";
    public string rotationY = "N/A";
    public string rotationZ = "N/A";
    public string localScale = "N/A";

}

public class TwoHandedUserStudyManager {
	public static TwoHandedGesturesManager handsManager = null;
	internal bool IN_USER_STUDY = false;

	internal const int NUM_TECHNIQUES = 5;
	internal GameObject[] manipulatableGameObjects;
	internal GameObject[] targetGameObjects;
	internal GameObject[] boundingCubes;

	public List<string> recordingStrings;


	internal bool hologramShown = false;

	internal int hologramCount;
	internal Dictionary<GameObject, TransformHelper> initialTransforms;
	internal bool justManipulated = false;

	internal bool doPractice = false;
	public static string loggingTitles;
    public static string loggingTitlesExtra;
	public bool RECORD_INFORMATION = true;
	public bool justRested = false;

    public bool shouldPlay = false;
	public bool showPracticeDoneInstruction = false;
	public bool disableTimedButton = false;

	public int instructionSequence = 0;
	public int USER_STUDY_TECHNIQUE_INDEX = 0;
	public int[] techniquesPermuted;
	public int sequenceJustShown;
	public bool disableImages = false;
	public bool resting = false;

    public bool reset = false;

	public const int REST_SEQUENCE = 27;
	public const int COMPLETE_PRACTICE_SEQUENCE = 26;

	public int practiceFrame = 0;
	public const int NUM_PRACTICE_FRAMES = 4;
	/*
		Study logging variables
	*/
    internal string currentUserID;
	internal int USER_STUDY_TECHNIQUE = -1;
	internal int hologramIndex = -1;
	public Stopwatch totalTimeInRound;
	public Stopwatch totalTimeManipulating;
	public int leftHandLosses;
	public int rightHandLosses;
	public int gestureAttempts;
	public int leftTaps;
	public int rightTaps;

	public int timeToWait = 5;

    bool roundStarted = false;

    long videoLength;
    long videoTimestamp;

	public TwoHandedUserStudyManager(TwoHandedGesturesManager _handsManager) {
		UnityEngine.Debug.Log("Starting my UserStudy manager\n");
		handsManager = _handsManager;

		Transform manipulatable = GameObject.Find ("/HologramCollection").transform;
		Transform target = GameObject.Find("/GameObjectCollection").transform;
		hologramCount = manipulatable.childCount;
		int index = 0;

		targetGameObjects = new GameObject[hologramCount];
		manipulatableGameObjects = new GameObject[hologramCount];
		boundingCubes = new GameObject[hologramCount];
		TwoHandedCursorManager.boundingCubes.Keys.CopyTo (boundingCubes, 0);

		foreach (Transform child in manipulatable) {
			GameObject g = child.gameObject;
			manipulatableGameObjects[index++] = g;
		}

		index = 0;

		foreach (Transform child in target) {
			GameObject g = child.gameObject;
			targetGameObjects[index++] = g;
		}

		initialTransforms = new Dictionary<GameObject, TransformHelper>();

		for(index = 0; index < hologramCount; index++) {
			initialTransforms.Add(manipulatableGameObjects[index], new TransformHelper(manipulatableGameObjects[index].transform));
		}

		loggingTitles = "User ID, Section, Technique, Hologram, Total Time (s), Time Spent Manipulating (s), Left Hand Losses, Right Hand Losses, Left Hand Taps, Right Hand Taps, Rotation X, Rotation Y, Rotation Z, ";
		loggingTitles += "Target Rotation X, Target Rotation Y, Target Rotation Z, Angle Between Rotations, Local Scale, Target Local Scale, Gesture Attempts, Training";

		loggingTitlesExtra = "User ID, Section, Technique, Hologram, Time, Event Type, Gesture Type, ";
		loggingTitlesExtra += "Left Hand Position X, ";
        loggingTitlesExtra += "Left Hand Position Y, Left Hand Position Z, Right Hand Position X, Right Hand Position Y, Right Hand Position Z, Rotation X, Rotation Y, Rotation Z, ";
        loggingTitlesExtra += "Target Rotation X, Target Rotation Y, Target Rotation Z, Angle Between Rotations, Local Scale, Target Local Scale, Training, Training Section, Instructions Active, Instruction Sequence Number Shown";


		techniquesPermuted = new int[NUM_TECHNIQUES];

		for(int i = 0; i < NUM_TECHNIQUES; i++) {
			techniquesPermuted[i] = i;
		}

		//reshuffle(techniquesPermuted);

        totalTimeInRound = new Stopwatch ();
		totalTimeManipulating = new Stopwatch ();

		recordingStrings = new List<string>();
	}


	void reshuffle(int[] techniques)
  {
      // Knuth shuffle algorithm :: courtesy of Wikipedia
      for (int t = 0; t < techniques.Length; t++ )
      {
          int tmp = techniques[t];
          int r = UnityEngine.Random.Range(t, techniques.Length);
          techniques[t] = techniques[r];
          techniques[r] = tmp;
      }
  }

	public void numberPressed(int i) {
			if(i == 10) {
				string text = handsManager.userIDText.text;
				if (text.Length > 0) {
					handsManager.userIDText.text = text.Substring(0, text.Length - 1);
				}
			} else {
				handsManager.userIDText.text += i;
			}
	}

	public void hideHologramList(GameObject[] objects) {
		for(int i = 0; i < hologramCount; i++) {
			objects[i].SetActive(false);
		}
	}

	public void hideAllHolograms() {
		handsManager.userStudyOrientationText.gameObject.SetActive(false);
		handsManager.userStudyRoundText.gameObject.SetActive (false);
		hideHologramList(manipulatableGameObjects);
		hideHologramList(targetGameObjects);
		hideHologramList (boundingCubes);
		hologramShown = false;
	}


    public void SaveDataExtended(SaveDataClass data)
    {
		/*string type = "N/A";
		if (data.startButtonPress) {
			UnityEngine.Debug.Log("Event Type: startButtonPress");
			type = "startButtonPress";
		}
		if (data.continueButtonPress) {
			UnityEngine.Debug.Log("Event Type: continueButtonPress");
			type = "continueButtonPress";
		}
		if (data.beginningGesture) {
			UnityEngine.Debug.Log("Event Type: beginningGesture");
			type = "beginningGesture";
		}
		if (data.endingGesture) {
			UnityEngine.Debug.Log("Event Type: endingGesture");
			type = "endingGesture";
		}
		if (data.leftLost) {
			UnityEngine.Debug.Log("Event Type: leftLost");
			type = "leftLost";
		}
		if (data.rightLost) {
			UnityEngine.Debug.Log("Event Type: rightLost");
			type = "rightLost";
		}
		if (data.foundLeft) {
			UnityEngine.Debug.Log("Event Type: leftFound");
			type = "foundLeft";
		}
		if (data.foundRight) {
			UnityEngine.Debug.Log("Event Type: rightFound");
			type = "foundRight";
		}
		if (data.leftDown) {
			UnityEngine.Debug.Log("Event Type: leftDown");
			type = "leftDown";
		}
		if (data.rightDown) {
			UnityEngine.Debug.Log("Event Type: rightDown");
			type = "rightDown";
		}
		if (data.leftUp) {
			UnityEngine.Debug.Log("Event Type: leftUp");
			type = "leftUp";
		}
		if (data.rightUp) {
			UnityEngine.Debug.Log("Event Type: rightUp");
			type = "rightUp";
		}
		if (data.update) {
			type = "update";
		}

		string rotationType = "N/A";

		if (data.inRotationGesture) {
			switch (handsManager.rotationType) {
				case TwoHandedRotationManager.pitchRotation:
					rotationType = "pitchRotation";
					break;
				case TwoHandedRotationManager.rollRotation:
				rotationType = "rollRotation";
					break;
				case TwoHandedRotationManager.yawRotation:
				rotationType = "yawRotation";
					break;
				case TwoHandedRotationManager.spindleRotation:
				rotationType = "spindleRotation";
					break;
				case TwoHandedRotationManager.arcBallRotation:
				rotationType = "arcBallRotation";
					break;
				case TwoHandedRotationManager.wireFrameRotation:
				rotationType = "wireFrameRotation";
					break;
				default:
					rotationType = "N/A";
					break;
			}
		}
		else if(data.inResizeGesture)
			rotationType = "resizeGesture";

        string toRecord = "";

        toRecord = data.userID + ", " + data.section + ", " + data.technique + ", " + data.hologram + ", " + data.time + ", " + type + ", " + rotationType + ", ";
		toRecord += data.leftHandPositionX + ", ";
        toRecord += data.leftHandPositionY + ", " + data.leftHandPositionZ + ", " + data.rightHandPositionX + ", " + data.rightHandPositionY + ", " + data.rightHandPositionZ + ", " + data.rotationX + ", " + data.rotationY + ", " + data.rotationZ + ", ";
        toRecord += data.targetRotationX + ", " + data.targetRotationY + ", " + data.targetRotationZ + ", " + data.angleBetweenRotations + ", " + data.localScale + ", " + data.targetLocalScale + ", " + data.training + ", " + data.trainingSection + ", " + handsManager.instructionPanel.activeSelf + ", " + sequenceJustShown;

		recordingStrings.Add (toRecord);


		if (recordingStrings.Count == 100) {
			string[] strings = recordingStrings.ToArray ();
            UserStudyLogger.Instance.Record (strings);
			recordingStrings.Clear ();
		}*/
    }

	public void SaveData(GameObject manipulatableObject, GameObject targetObject, bool training) {
		/*int technique = USER_STUDY_TECHNIQUE + 1;
		int hologramNum = hologramIndex + 1;
		int section = USER_STUDY_TECHNIQUE_INDEX + 1;
		string toRecord = "";

		Quaternion manipulatableObjectRotation = manipulatableObject.transform.rotation;
		Vector3 manipulatableObjectLocalScale = manipulatableObject.transform.localScale;

		Quaternion targetObjectRotation = targetObject.transform.rotation;
		Vector3 targetObjectLocalScale = targetObject.transform.localScale;

		long timeInRound = totalTimeInRound.ElapsedMilliseconds;
		long timeManipulating = totalTimeManipulating.ElapsedMilliseconds;

		float timeInRoundRounded = timeInRound / 1000.0f;
		float timeManipulatingRounded = timeManipulating / 1000.0f;

		UnityEngine.Debug.Log("Time in Round: " + timeInRoundRounded  + "s and Time Manipulating: " + timeManipulatingRounded + "s");
		float angle = Quaternion.Angle (manipulatableObjectRotation, targetObjectRotation);

		toRecord = (currentUserID + ", " + section + ", " + technique + ", " + hologramNum + ", " + timeInRoundRounded + ", " + timeManipulatingRounded + ", ");
		toRecord += (leftHandLosses + ", " + rightHandLosses + ", " + leftTaps + ", " + rightTaps + ", " + manipulatableObjectRotation.x + ", " + manipulatableObjectRotation.y + ", ");
		if(training) {
			toRecord += (manipulatableObjectRotation.z + ", N/A, N/A, N/A, N/A, ");
			toRecord += (manipulatableObjectLocalScale.x + ", " + "N/A, " + gestureAttempts + ", " + training);
		} else {
			toRecord += (manipulatableObjectRotation.z + ", " + targetObjectRotation.x + ", " + targetObjectRotation.y + ", " + targetObjectRotation.z + ", " + angle + ", ");
			toRecord += (manipulatableObjectLocalScale.x + ", " + targetObjectLocalScale.x + ", " + gestureAttempts + ", " + training);
		}
		UnityEngine.Debug.Log("Saving User " + currentUserID + " Data for Training: " + training + ", Technique: " + technique + "/" + NUM_TECHNIQUES + " and Hologram: " + hologramNum + "/" + hologramCount);
		UserStudyLogger.Instance.Record(toRecord, 0);

		resetStats();*/
	}

    public void performTechnique(string s)
    {

		if (s.Length < 9)
			return;

        int technique = s [0] - '0';



		techniquesPermuted = new int[5];
		for (int i = 1; i < 6; i++) {
			int tech = s [i] - '0';
			techniquesPermuted [i - 1] = tech - 1;
		}

		string id = s.Substring (6, 3);

		currentUserID = id;

		// create a new log file
		UserStudyLogger.Instance.OpenLogFile(currentUserID);

        UnityEngine.Debug.Log("Initializing UserStudy");
        justRested = false;
        practiceFrame = 0;
        handsManager.setTechnique(technique);
        IN_USER_STUDY = true;
        reset = true;
        disableImages = true;
        UnityEngine.Debug.Log("Created Log file");
        hideUserIDMenu();
        justManipulated = false;
        hideAllHolograms();
        hologramIndex = 0;
        USER_STUDY_TECHNIQUE_INDEX = 0;
        for(int i = 0; i < NUM_TECHNIQUES; i++)
        {
            if(techniquesPermuted[i] == (technique - 1))
            {
                USER_STUDY_TECHNIQUE_INDEX = i;
                USER_STUDY_TECHNIQUE = techniquesPermuted[i];
                break;
            }
        }
        UnityEngine.Debug.Log("User study technique is " + USER_STUDY_TECHNIQUE);
        hideHologramAtIndex(0);
        doPractice = true;

        resetStats();

        handsManager.instructionTitleText.text = "Instructions";
        handsManager.counter = 0;
        timeToWait = 5;

        instructionSequence = USER_STUDY_TECHNIQUE * (NUM_PRACTICE_FRAMES + 1) + 1;
        showInstructions();
        hideAllHolograms();
        practiceFrame = 1;
        disableTimedButton = false;
        int trainingRoundNumber = practiceFrame + 1;
        int section = USER_STUDY_TECHNIQUE_INDEX + 1;
        handsManager.instructionTitleText.text = handsManager.techniqueNames[USER_STUDY_TECHNIQUE] + " Technique: Training";
        handsManager.menuText.text = "Section " + section + " of " + NUM_TECHNIQUES + ", Training Round " + trainingRoundNumber;
    }


    public void bufferVideo(int seq)
    {
        VideoPlayer player = handsManager.videoPlayer.GetComponent<VideoPlayer>();
        player.source = VideoSource.VideoClip;
        player.clip = handsManager.videosToPlay[seq];
        player.Prepare();
    }

	public void hideInstructions() {
		handsManager.instructionPanel.SetActive(false);
        handsManager.videoPlayer.GetComponent<Renderer>().enabled = false;
        handsManager.videoPlayer.GetComponent<VideoPlayer>().Stop();
		AudioSource audio = handsManager.instructionPanel.GetComponent<AudioSource> ();
		audio.Stop ();
        sequenceJustShown = -1;
	}

	public void hideHologramAtIndex(int index) {
		GameObject manipulatableObject = manipulatableGameObjects[index];
		GameObject targetObject = targetGameObjects[index];

		targetObject.SetActive(false);



		manipulatableObject.SetActive(false);
		TwoHandedCursorManager.boundingCubesInverse [manipulatableObject].SetActive (false);
	}

	public void resetStats() {
		totalTimeInRound.Reset();
		totalTimeManipulating.Reset();
		leftHandLosses = 0;
		rightHandLosses = 0;
		gestureAttempts = 0;
		leftTaps = 0;
		rightTaps = 0;
	}

	public void resetAndShowHologramAtIndex(int index, bool showTarget) {

		int hologramNum = index + 1;
		UnityEngine.Debug.Log("Resetting Hologram " + hologramNum + " and showTarget is " + showTarget);
		Vector3 textPosition, roundTextPosition;
		GameObject manipulatableObject = manipulatableGameObjects[index];
		GameObject targetObject = targetGameObjects[index];

        hideAllHolograms();

		//reset hologram to initial state
		TransformHelper initialManipulatableTransform = initialTransforms[manipulatableObject];
		manipulatableObject.transform.position = initialManipulatableTransform.position;
		manipulatableObject.transform.localScale = initialManipulatableTransform.localScale;
		manipulatableObject.transform.rotation = initialManipulatableTransform.rotation;

		TwoHandedCursorManager.SetBoundingCubeSize(manipulatableObject, TwoHandedCursorManager.boundingCubesInverse [manipulatableObject]);

		TwoHandedCursorManager.extents[manipulatableObject] = manipulatableObject.GetComponent<Renderer>().bounds.extents.magnitude * TwoHandedCursorManager.RADIUS_MULTIPLIER;


		TwoHandedCursorManager.boundingCubesInverse [manipulatableObject].SetActive (true);

		if(showTarget && roundStarted) {
            roundStarted = false;
			handsManager.userStudyOrientationText.gameObject.SetActive(true);
			handsManager.userStudyRoundText.gameObject.SetActive (true);
            handsManager.startRoundButton.gameObject.SetActive(false);
            manipulatableObject.SetActive(true);
            targetObject.SetActive(true);
			textPosition = targetObject.GetComponent<Renderer>().bounds.center;
			textPosition.y += targetObject.GetComponent<Renderer>().bounds.size.y/2 * 1.1f;
			handsManager.userStudyOrientationText.text = "Target Image Orientation and Size";

			roundTextPosition = (targetObject.GetComponent<Renderer>().bounds.center + manipulatableObject.GetComponent<Renderer>().bounds.center) / 2.0f;

			roundTextPosition.y += 0.6f;

			handsManager.userStudyRoundText.text = "Round " + hologramNum + "/" + hologramCount;

			float roundTextHeight = handsManager.userStudyRoundText.preferredHeight * handsManager.menu.transform.parent.localScale.x / 2;
			roundTextPosition.y += roundTextHeight;

			handsManager.userStudyRoundText.transform.position = roundTextPosition;
			handsManager.userStudyRoundText.gameObject.SetActive(true);

            float textHeight = handsManager.userStudyOrientationText.preferredHeight * handsManager.menu.transform.parent.localScale.x / 2;
            textPosition.y += textHeight;

            handsManager.userStudyOrientationText.transform.position = textPosition;
            handsManager.userStudyOrientationText.gameObject.SetActive(true);

			hologramShown = true;
            //reset stats
            resetStats();
            totalTimeInRound.Start();
        } else if(!showTarget) {
            manipulatableObject.SetActive(true);
			hologramShown = true;
            handsManager.userStudyRoundText.gameObject.SetActive(false);
			textPosition = manipulatableObject.GetComponent<Renderer>().bounds.center;
			textPosition.y -= manipulatableObject.GetComponent<Renderer>().bounds.size.y*2;

            handsManager.userStudyOrientationText.transform.position = textPosition;
            handsManager.userStudyOrientationText.gameObject.SetActive(true);

            //reset stats
            resetStats();
            totalTimeInRound.Start();
        } else if (!roundStarted)
        {
			disableImages = true;
            handsManager.startRoundButton.gameObject.SetActive(true);
			handsManager.userStudyOrientationText.gameObject.SetActive(false);
			handsManager.userStudyRoundText.gameObject.SetActive (false);
			handsManager.userStudyButtonText.text = "Press Start Round When Ready";

			disableTimedButton = true;
			handsManager.beginUserStudyButton.interactable = false;
            handsManager.startRoundButton.GetComponentInChildren<Text>().text = "Rest your arms. When you are ready, press here to start round " + hologramNum;
			hologramShown = false;
        }


	}

	public void finishUserStudy() {
		IN_USER_STUDY = false;
		justManipulated = false;
		instructionSequence = 0;
		for (int i = 0; i < NUM_TECHNIQUES; i++) {
			handsManager.techniqueButtons[i].enabled = true;
			handsManager.techniqueButtons[i].interactable = true;
		}
		handsManager.userStudyButtonText.text = "Begin User Study";
		//handsManager.setTechnique(1);
		handsManager.userIDText.text = "";
		//showUserIDMenu ();

		resetStats();
		hideInstructions ();
		currentUserID = "";
		USER_STUDY_TECHNIQUE = -1;
		hologramIndex = -1;
		resetAndShowHologramAtIndex(0, false);
        handsManager.userStudyRoundText.gameObject.SetActive(false);
        handsManager.userStudyOrientationText.gameObject.SetActive(false);
		disableImages = false;
		handsManager.setTechnique (TwoHandedGesturesManager.TECHNIQUE_SELECTED);
	}

	public void hideUserIDMenu() {
		handsManager.userIDMenu.SetActive (false);
		//handsManager.userIDMenu.GetComponent<Renderer> ().enabled = false;
	}

	public void showUserIDMenu() {
		handsManager.userIDMenu.SetActive (true);
		//handsManager.userIDMenu.GetComponent<Renderer> ().enabled = true;
	}

    public void showInstructions() {
		//no holograms while showing instructoins
		sequenceJustShown = instructionSequence;
		handsManager.beginUserStudyButton.interactable = false;
		handsManager.instructionPanel.SetActive(true);
        // handsManager.instructionPanel.transform.LookAt(2 * handsManager.instructionPanel.transform.position - Camera.main.transform.position);


		AudioSource audio = handsManager.instructionPanel.GetComponent<AudioSource> ();
        if (instructionSequence >= 1 && instructionSequence <= 25) {
			handsManager.videoPlayer.GetComponent<Renderer>().enabled = true;

            VideoPlayer player = handsManager.videoPlayer.GetComponent<VideoPlayer>();
            player.source = VideoSource.VideoClip;
            player.clip = handsManager.videosToPlay[instructionSequence - 1];
            player.Prepare();
            shouldPlay = true;
            handsManager.userStudyButtonText.text = "Practice Gestures Before Continuing";


			audio.clip = handsManager.audioToPlay [instructionSequence];
			audio.Play ();
		} else {
            if (instructionSequence == COMPLETE_PRACTICE_SEQUENCE)
            {
				handsManager.videoPlayer.GetComponent<Renderer>().enabled = true;
                VideoPlayer player = handsManager.videoPlayer.GetComponent<VideoPlayer>();
                player.source = VideoSource.VideoClip;
                player.clip = handsManager.videosToPlay[USER_STUDY_TECHNIQUE * (NUM_PRACTICE_FRAMES + 1)];
                player.Prepare();
                shouldPlay = true;
                handsManager.userStudyButtonText.text = "Practice Gestures Before Continuing";

				audio.clip = handsManager.audioToPlay [instructionSequence];
				audio.Play ();
            }
            else
            {
				if (instructionSequence == 0) {
					audio.clip = handsManager.audioToPlay [instructionSequence];
					audio.Play ();
				} else {
					audio.Stop ();
				}

                shouldPlay = false;
                handsManager.videoPlayer.GetComponent<Renderer>().enabled = false;
                handsManager.userStudyButtonText.text = "Read Instructions Below Before Proceeding";
                disableTimedButton = false;
            }
		}
		//for (int i = 0; i < NUM_TECHNIQUES; i++) {
		//		handsManager.techniqueButtons[i].interactable = false;
		//		handsManager.techniqueButtons[i].enabled = true;
		//}



        handsManager.instructionPanelText.text = handsManager.instructionTexts [instructionSequence];
		instructionSequence = -1;
	}

    public bool initializeUserStudy() {
		recordingStrings = new List<string> ();
		hologramShown = false;
		UnityEngine.Debug.Log("Initializing UserStudy");
		justRested = false;
		practiceFrame = 0;
		IN_USER_STUDY = true;
		if(handsManager.userIDText.text == "") return false;
		instructionSequence = 0;
        disableTimedButton = false;
		currentUserID = handsManager.userIDText.text;
		UnityEngine.Debug.Log("User ID is " + currentUserID);
		// creates csv log file
		UserStudyLogger.Instance.CreateLogFile (currentUserID);
		UnityEngine.Debug.Log("Created Log file");
		hideUserIDMenu ();
        disableImages = true;
		justManipulated = false;
		hideAllHolograms();
		hologramIndex = 0;

		USER_STUDY_TECHNIQUE_INDEX = 0;
		techniquesPermuted [0] = TwoHandedGesturesManager.TECHNIQUE_SELECTED - 1;
        //reshuffle(techniquesPermuted);

        USER_STUDY_TECHNIQUE_INDEX = 0;
		USER_STUDY_TECHNIQUE = TwoHandedGesturesManager.TECHNIQUE_SELECTED - 1;//techniquesPermuted[USER_STUDY_TECHNIQUE_INDEX];
		UnityEngine.Debug.Log("User study technique is " + USER_STUDY_TECHNIQUE);
		hideHologramAtIndex(0);
		doPractice = true;
        int seq = USER_STUDY_TECHNIQUE * (NUM_PRACTICE_FRAMES + 1);
        bufferVideo(seq);

        resetStats();
		handsManager.instructionTitleText.text = "Instructions";
        handsManager.counter = 0;
        timeToWait = 5;
		showInstructions();
		hideAllHolograms();
		UserStudyButtonPressed ();
		return true;
	}

    public string getTime()
    {
        return DateTime.Now.ToString("h:mm:ss tt");
    }

    public void beginRound()
    {
        SaveDataClass data = new SaveDataClass(this);
        data.startButtonPress = true;


        SaveDataExtended(data);

        roundStarted = true;
        resetAndShowHologramAtIndex(hologramIndex, true);
		disableImages = false;
    }

    public bool inTraining()
    {
        return (((doPractice && (USER_STUDY_TECHNIQUE_INDEX != NUM_TECHNIQUES)) &&
                !(USER_STUDY_TECHNIQUE_INDEX != 0 && !justRested && !reset)) || showPracticeDoneInstruction);
    }
	public void UserStudyButtonPressed() {
		handsManager.beginUserStudyButton.interactable = false;
		EventSystem.current.SetSelectedGameObject(null);
		resting = false;
		if(USER_STUDY_TECHNIQUE == -1) {
			initializeUserStudy();
			return;
		}



		if(!handsManager.instructionPanel.activeSelf) {
			int index = hologramIndex;
			GameObject manipulatableObject = manipulatableGameObjects [index];
			GameObject targetObject = targetGameObjects [index];

			// if we just did a manipulation, save the state
			if(justManipulated)	{
				if (USER_STUDY_TECHNIQUE_INDEX < NUM_TECHNIQUES) {
					SaveData (manipulatableObject, targetObject, false);


                    SaveDataClass data = new SaveDataClass(this);
                    data.setTransforms(manipulatableObject, targetObject);
                    data.continueButtonPress = true;
                    SaveDataExtended(data);

                    hideHologramAtIndex (index);
				}
				if(++hologramIndex == hologramCount) {
					justRested = false;
					doPractice = true;
					hologramIndex = 0;
					USER_STUDY_TECHNIQUE_INDEX++;
                    reset = false;
					if(USER_STUDY_TECHNIQUE_INDEX != NUM_TECHNIQUES)
						USER_STUDY_TECHNIQUE = techniquesPermuted[USER_STUDY_TECHNIQUE_INDEX];
				} else {
					resetAndShowHologramAtIndex(hologramIndex, true);
				}
			}

			else if(showPracticeDoneInstruction) {
				showPracticeDoneInstruction = false;
				disableTimedButton = false;
				// we just had a practice stage, show instructions about timed stage entering
				instructionSequence = COMPLETE_PRACTICE_SEQUENCE;
				handsManager.instructionTitleText.text = handsManager.techniqueNames[USER_STUDY_TECHNIQUE] + " Technique: Final Practice";
                handsManager.counter = 0;
                timeToWait = 5;
				handsManager.userStudyOrientationText.text = "Final Practice on Object";
				showInstructions();
                resetAndShowHologramAtIndex(hologramIndex, false);
				disableImages = false;
                //Maybe save data here
                //saves the practice data
				SaveData (manipulatableObject, targetObject, true);

                int numFrames = NUM_PRACTICE_FRAMES + 1;
                SaveDataClass data = new SaveDataClass(this);
                data.continueButtonPress = true;
                data.training = true;
                data.trainingSection = numFrames.ToString();
                SaveDataExtended(data);

                return;
			}



			if(USER_STUDY_TECHNIQUE_INDEX < NUM_TECHNIQUES) {
				/*for (int i = 0; i < NUM_TECHNIQUES; i++) {
					if(i != USER_STUDY_TECHNIQUE) {
						handsManager.techniqueButtons[i].interactable = false;
						handsManager.techniqueButtons[i].enabled = true;
					} else {
						handsManager.techniqueButtons[i].interactable = true;
						handsManager.techniqueButtons[i].enabled = false;
						handsManager.setTechnique(i + 1);
					}
				}*/

				if(doPractice && USER_STUDY_TECHNIQUE_INDEX != NUM_TECHNIQUES) {
					//completed a section
					if(USER_STUDY_TECHNIQUE_INDEX != 0 && !justRested && !reset) {
						justRested = true;
						int left = NUM_TECHNIQUES - USER_STUDY_TECHNIQUE_INDEX;
						instructionSequence = REST_SEQUENCE;
						hideAllHolograms();
						string word = " section";
						if(left != 1)
							word += "s";
						disableTimedButton = false;
						int lastTechnique = techniquesPermuted [USER_STUDY_TECHNIQUE_INDEX - 1];
						handsManager.instructionTitleText.text = handsManager.techniqueNames[lastTechnique] + " Technique: Section Complete";
                        disableImages = true;
						handsManager.instructionTexts[REST_SEQUENCE] = "You have completed section " + USER_STUDY_TECHNIQUE_INDEX + ". You have " + left + word
							+ " left to complete.\nPlease say the following code to the study administrator:\n\n\n" + handsManager.codes[lastTechnique] + "\n\n\nPlease carefully remove the hololens and fill out the questionnaire for this section.";
                        handsManager.counter = 0;
                        timeToWait = 30;
						showInstructions();
						justManipulated = false;
						resting = true;
                        int seq = USER_STUDY_TECHNIQUE * (NUM_PRACTICE_FRAMES + 1);
                        bufferVideo(seq);
						return;
					}



					int trainingRoundNumber = practiceFrame + 1;
					int section = USER_STUDY_TECHNIQUE_INDEX + 1;
					handsManager.menuText.text = "Section " + section + " of " + NUM_TECHNIQUES + ", Training Round " + trainingRoundNumber;
					justManipulated = false;

					instructionSequence = USER_STUDY_TECHNIQUE * (NUM_PRACTICE_FRAMES + 1) + practiceFrame + 1;

                   	resetAndShowHologramAtIndex(hologramIndex, false);

                    if (practiceFrame == NUM_PRACTICE_FRAMES) {
						doPractice = false;
						disableTimedButton = true;
						showPracticeDoneInstruction = true;

						SaveData (manipulatableObject, targetObject, true);

                        SaveDataClass data = new SaveDataClass(this);
                        data.continueButtonPress = true;
                        data.training = true;
                        data.trainingSection = NUM_PRACTICE_FRAMES.ToString();
                        SaveDataExtended(data);

                        practiceFrame = 0;
                        handsManager.userStudyOrientationText.text = "Practice Roll Rotation on Object";
						handsManager.instructionTitleText.text = handsManager.techniqueNames[USER_STUDY_TECHNIQUE] + " Technique: Roll Rotate Training";
					} else {
						if (practiceFrame == 0) {
							handsManager.instructionTitleText.text = handsManager.techniqueNames[USER_STUDY_TECHNIQUE] + " Technique: Training";
							disableTimedButton = false;
							disableImages = false;
						} else {
							disableImages = false;
							if (practiceFrame == 1) {
								handsManager.userStudyOrientationText.text = "Practice Resizing on Object";
								handsManager.instructionTitleText.text = handsManager.techniqueNames[USER_STUDY_TECHNIQUE] + " Technique: Resize Training";
							} else if (practiceFrame == 2)
                            {
								handsManager.userStudyOrientationText.text = "Practice Pitch Rotation on Object";
                                handsManager.instructionTitleText.text = handsManager.techniqueNames[USER_STUDY_TECHNIQUE] + " Technique: Pitch Rotate Training";
                            } else if (practiceFrame == 3)
                            {
								handsManager.userStudyOrientationText.text = "Practice Yaw Rotation on Object";
                                handsManager.instructionTitleText.text = handsManager.techniqueNames[USER_STUDY_TECHNIQUE] + " Technique: Yaw Rotate Training";
                            }
							SaveData (manipulatableObject, targetObject, true);

                            SaveDataClass data = new SaveDataClass(this);
                            data.continueButtonPress = true;
                            data.training = true;
                            data.trainingSection = practiceFrame.ToString();
                            SaveDataExtended(data);

                            disableTimedButton = true;
						}
						practiceFrame++;
					}

                    //if its our first technique, we need to setup the practice hologram
                    // the if statement above wont go on the very first time
                    handsManager.counter = 0;
                    timeToWait = 5;
					showInstructions();
				}	else {
					handsManager.userStudyButtonText.text = "Perform Gesture Before Continuing";

					// within data recording stage
					RECORD_INFORMATION = true;

					//only need to do this if we just had a practice stage
					if(!justManipulated) {
						resetAndShowHologramAtIndex(hologramIndex, true);
					}

					justManipulated = true;
				}
			} else if(USER_STUDY_TECHNIQUE_INDEX == NUM_TECHNIQUES && justManipulated) {
					instructionSequence = REST_SEQUENCE;
					hideAllHolograms();
					disableTimedButton = false;
					int lastTechnique = techniquesPermuted [NUM_TECHNIQUES - 1];
					handsManager.instructionTitleText.text = handsManager.techniqueNames[lastTechnique] + " Technique: Section Complete";
					disableImages = true;
					handsManager.instructionTexts[REST_SEQUENCE] = "You have completed the final section.\nPlease say the following code to the study administrator:\n\n\n" + handsManager.codes[lastTechnique] + "\n\n\nPlease carefully remove the hololens and fill out the questionnaire for this section.";
                    handsManager.counter = 0;
                    timeToWait = 30;
					showInstructions();
					justManipulated = false;

					IN_USER_STUDY = false;

					for (int i = 0; i < recordingStrings.Count; i++) {
						UserStudyLogger.Instance.Record (recordingStrings [i], 1);
					}
					USER_STUDY_TECHNIQUE_INDEX++;
                    resetAndShowHologramAtIndex(0, false);
            } else {
				finishUserStudy();
			}
		} else {

			if (USER_STUDY_TECHNIQUE_INDEX >= NUM_TECHNIQUES) {
				finishUserStudy ();
				return;
			}
			// if we just showed instruction 0
			if(sequenceJustShown == 0) {
				doPractice = true;
				justRested = false;
			}

			hideInstructions();
			UserStudyButtonPressed();
		}
	}

}

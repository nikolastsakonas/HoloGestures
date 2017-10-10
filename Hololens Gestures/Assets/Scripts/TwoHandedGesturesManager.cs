using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine.VR.WSA.Input;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.Video;

/*
	The three techniques for two handed rotation and scaling that can be employed

	Technique 1: Two starting positions
			One hand higher than the other on the y axis
				Scaling
			Hands equal on y axis
				Rotation

			For rotation, the direction in which hands begin to move after pinching will start the rotation gesture and show the appropriate arrows for that rotation

			Hands move apart on the z axis
				Yaw rotation
			Hands move apart on the y axis
				Roll rotation
			Hands move together on the y axis
				Pitch rotation

		Technique 2: Four starting positions
			One hand higher than other on the y axis, and hands are on equal z axis
				Scaling arrows shown
			One hand higher than the other on y axis, and hands are not on equal z axis
				Pitch rotation arrows shown
			Hands are equal on y axis and z axis
				Roll rotation arrows shown
			Hands are equal on y axis and hands are not equal on z axis
				Yaw rotation arrows shown


		Technique 3 Spindle technique (Mapes and Moshell) with the possibility of Remote Cursor Points?
			Hands treated as if spindle connects them
				Scaling/rotation is not locked to any particular axis
				All movements can cause rotation and scaling of the object
				“Wheel” technique is not possible (Pitch), as hand rotation cannot be detected by hololens.
*/

public class TwoHandedGesturesManager : Singleton<TwoHandedGesturesManager>, IInputHandler, IInputClickHandler {
	const int MESH_LAYER = 31;
	public const int TECHNIQUE_1 = 1;
	public const int TECHNIQUE_2 = 2;
	public const int TECHNIQUE_3 = 3;
	public const int TECHNIQUE_4 = 4;
	public const int TECHNIQUE_5 = 5;
	public const int TECHNIQUE_6 = 6;

	public static int TECHNIQUE_SELECTED = TECHNIQUE_1;

	public GameObject leftLine;
	public GameObject rightLine;
	public GameObject leftTriangle;
	public GameObject rightTriangle;
	public GameObject leftDot;
	public GameObject rightDot;
	public GameObject rotationAxis;
	public GameObject rotationAxis2;
	public GameObject pitchImage;
	public GameObject yawImage;
	public GameObject rollImage;
	public GameObject pitchImage2;
	public GameObject yawImage2;
	public GameObject rollImage2;
	public GameObject rotationImage;
	public GameObject technique2Image;
	public GameObject resizeImage;
	public GameObject boundingSphere;
	public GameObject menu;
	public GameObject userIDMenu;
	public GameObject instructionPanel;
	public GameObject normalCursor;
	public GameObject resizeCursor;
	public GameObject pitchCursor;
	public GameObject yawCursor;
	public GameObject rollCursor;
	public GameObject technique5Panel;
    public string sentText = "";

	public string[] codes;
	public Text userIDText;
	public Text instructionPanelText;
	[TextArea(3, 100)]
	public string[] instructionTexts;
    public VideoClip[] videosToPlay;
	public AudioClip[] audioToPlay;
    public string[] techniqueNames;

	public Button beginUserStudyButton;
	public Button [] techniqueButtons;
	public Button [] numberButtons;
	public Button [] technique5Buttons;

	public Text userStudyOrientationText;
	public Text userStudyRoundText;
	public Text instructionTitleText;

	public Material arrowMaterial;
	public Material debuggingMaterial;
	public Text menuText;
	public Text userStudyButtonText;

	int frames = 0;
	internal float initialExtent;
	internal Vector3 objCenter;
	internal Vector3 axisForRotation;
	public static bool resizing;
	public static bool rotating;
	internal int rotationType;
	internal bool rotationGesture = false;
	internal bool resizingGesture = false;
	internal Vector3 initialLeft;
	internal Vector3 initialRight;
	internal Vector3 initialSize;
	internal float initialBoundingWidth;
	internal float initialBoundingHeight;
	internal float initialBoundingLength;
	internal Quaternion initialRotation;
	internal GameObject objFocused;
	public static GameObject focusedForBox;
	internal Vector3 leftHandPosition;
	internal float leftHandDistance;

    public Button startRoundButton;
	public GameObject Managers;
	public GameObject GameObjectCollection;
	public GameObject HologramCollection;
	public GameObject Menu;
	public GameObject ArrowsAndAxis;
	public GameObject BoundingSphere;
	public GameObject Technique5Objects;
    public GameObject videoPlayer;

	internal float arcBallRadius;
	internal Vector3 rightHandPosition;
	internal float rightHandDistance;
	internal Vector3 leftHandSpindlePosition;
	internal Vector3 rightHandSpindlePosition;
	internal Vector3 leftHandInitialSpindlePosition;
	internal Vector3 rightHandInitialSpindlePosition;
	internal Vector3 handBeingTrackedPosition;
	internal Vector3 initialHandBeingTrackedPosition;
	internal Vector3 arcBallHandPosition;
	internal Vector3 initialArcballHandPosition;
	internal int leftID;
	internal int rightID;
	internal int handBeingTracked;
	internal bool foundLeft;
	internal bool foundRight;
	internal bool foundTracked;
	internal bool hasBeenLoaded = false;
	internal GameObject boundingCube;
	internal int rotationFrameNumber;
	internal int scalingFrameNumber;
	internal const float ROTATION_THRESHOLD = 0.06f;
	internal const int Horizontal = 0;
	internal const int LeftUp = 1;
	internal const int RightUp = 2;
	internal int technique5Selection = 0;

	internal static TwoHandedGesturesManager _instance;
	public static bool showWireBox = false;

	Color leftCorrectColor;
	Color rightCorrectColor;
	public int counter = 0;

	internal bool leftTapped = false;
	internal bool rightTapped = false;
	internal static TwoHandedCursorManager myCursorManager;
	internal static TwoHandedResizeManager myResizeManager;
	internal static TwoHandedArrowsManager myArrowsManager;
	internal static TwoHandedRotationManager myRotateManager;
	internal static TwoHandedUserStudyManager myUserStudyManager;

	static bool isLoaded = false;

    public void setTechnique(int type) {
		if (type == TECHNIQUE_5)
			BoundBoxes_BoundBox.initialized = false;

		TwoHandedGesturesManager.TECHNIQUE_SELECTED = type;
		int index = myUserStudyManager.hologramIndex + 1;
		int section = myUserStudyManager.USER_STUDY_TECHNIQUE_INDEX + 1;

		if(myUserStudyManager.IN_USER_STUDY)
			menuText.text = "Section " + section + " of " + TwoHandedUserStudyManager.NUM_TECHNIQUES + ", Round " + index + " of " + myUserStudyManager.hologramCount;
		else
			menuText.text = techniqueNames[type - 1];
		CancelManipulation();
		myArrowsManager.HideArrows();
	} 

	void changeGestureType(int i) {
			technique5Selection = i;
			print("selection is now " + i);
	}

	void Awake () {
		print ("here");
		print("isloaded is " + isLoaded);
		if (_instance != null && _instance != this) {
			print ("Destorying myself");
			//resetNullObjects ();

			Destroy(Managers);
			Destroy(GameObjectCollection);
			Destroy(HologramCollection);
			Destroy(Menu);
			Destroy(ArrowsAndAxis);
			Destroy(BoundingSphere);
			Destroy(Technique5Objects);
		} else {
			print ("not destroying myself");
			_instance = this;
			DontDestroyOnLoad(GameObject.Find("Managers"));
			DontDestroyOnLoad (GameObject.Find ("GameObjectCollection"));
			DontDestroyOnLoad (GameObject.Find ("HologramCollection"));
			DontDestroyOnLoad (GameObject.Find ("Menu"));
			DontDestroyOnLoad (GameObject.Find ("ArrowsAndAxis"));
			DontDestroyOnLoad (GameObject.Find ("BoundingSphere"));
			DontDestroyOnLoad (GameObject.Find ("Technique5Objects"));
			myStart ();
		}

	}
	// Use this for initialization
	void myStart () {
		print ("starting my Two Handed Gesture manager");
		rightID = -1;
		leftID = -1;
		handBeingTracked = -1;
		leftCorrectColor = rightCorrectColor = new Color(0, 255, 0);
		myResizeManager = new TwoHandedResizeManager(this);
		myRotateManager = new TwoHandedRotationManager(this);
		myCursorManager = new TwoHandedCursorManager (this);
		myArrowsManager = new TwoHandedArrowsManager (this);
		myUserStudyManager = new TwoHandedUserStudyManager (this);

		myArrowsManager.InitializeArrows ();
		myArrowsManager.InitializeRotationAxis ();
		myArrowsManager.HideArrows();

        myUserStudyManager.hideInstructions();

		// listeners for techniquebuttons
		techniqueButtons[0].onClick.AddListener(() => { setTechnique(1); });
		techniqueButtons[1].onClick.AddListener(() => { setTechnique(2); });
		techniqueButtons[2].onClick.AddListener(() => { setTechnique(3); });
		techniqueButtons[3].onClick.AddListener(() => { setTechnique(4); });
		techniqueButtons[4].onClick.AddListener(() => { setTechnique(5); });
		beginUserStudyButton.onClick.AddListener(() => { myUserStudyManager.UserStudyButtonPressed(); });

		// listeners for number buttons
		numberButtons[0].onClick.AddListener(() => { myUserStudyManager.numberPressed(0); });
		numberButtons[1].onClick.AddListener(() => { myUserStudyManager.numberPressed(1); });
		numberButtons[2].onClick.AddListener(() => { myUserStudyManager.numberPressed(2); });
		numberButtons[3].onClick.AddListener(() => { myUserStudyManager.numberPressed(3); });
		numberButtons[4].onClick.AddListener(() => { myUserStudyManager.numberPressed(4); });
		numberButtons[5].onClick.AddListener(() => { myUserStudyManager.numberPressed(5); });
		numberButtons[6].onClick.AddListener(() => { myUserStudyManager.numberPressed(6); });
		numberButtons[7].onClick.AddListener(() => { myUserStudyManager.numberPressed(7); });
		numberButtons[8].onClick.AddListener(() => { myUserStudyManager.numberPressed(8); });
		numberButtons[9].onClick.AddListener(() => { myUserStudyManager.numberPressed(9); });
		numberButtons[10].onClick.AddListener(() => { myUserStudyManager.numberPressed(10); });


		technique5Buttons[0].onClick.AddListener(() => { changeGestureType(0); });
		technique5Buttons[1].onClick.AddListener(() => { changeGestureType(1); });
		technique5Buttons[2].onClick.AddListener(() => { changeGestureType(2); });
		technique5Buttons[3].onClick.AddListener(() => { changeGestureType(3); });

        startRoundButton.onClick.AddListener(() => { myUserStudyManager.beginRound(); });
        startRoundButton.gameObject.SetActive(false);
        setTechnique (1);

        VideoPlayer player = videoPlayer.GetComponent<VideoPlayer>();
        player.playbackSpeed = 0.65f;
        CancelManipulation();
		//tap recognized anywhere
		InputManager.Instance.PushFallbackInputHandler (gameObject);

		codes = new	string[5];

		codes [0] = "NBV7R48F";
		codes [1] = "LQC3ZWCT";
		codes [2] = "VCGKYVFR";
		codes [3] = "AFY4M4CG";
		codes [4] = "XYTEC2XA";
		/*
        initialHologramCollectionPosition = HologramCollection.transform.position;
        initialGameObjectCollectionPosition = GameObjectCollection.transform.position;
        initialMenuCollectionPosition = MenuCollection.transform.position;
		initialHologramCollectionRotation = HologramCollection.transform.rotation;
		initialGameObjectCollectionRotation = GameObjectCollection.transform.rotation;
		initialMenuCollectionRotation = MenuCollection.transform.rotation;*/

		setInteractable(beginUserStudyButton);
	}

	void CancelManipulation() {
		if((resizing || rotating) && myUserStudyManager.IN_USER_STUDY)
        {
            
            SaveDataClass data = new SaveDataClass(myUserStudyManager);

            if (resizing)
            {
                data.inResizeGesture = true;
            }
            if (rotating)
            {
                data.inRotationGesture = true;
            }
            data.endingGesture = true;

            if (myUserStudyManager.inTraining())
            {
                data.training = true;
                data.trainingSection = myUserStudyManager.practiceFrame.ToString();
            } else
            {
                int index = myUserStudyManager.hologramIndex;
                GameObject manipulatableObject = myUserStudyManager.manipulatableGameObjects[index];
                GameObject targetObject = myUserStudyManager.targetGameObjects[index];
                data.setTransforms(manipulatableObject, targetObject);
            }

            myUserStudyManager.SaveDataExtended(data);
        }
		rotating = false;
		resizing = false;
		rotationGesture = false;
		resizingGesture = false;
		leftTapped = false;
		rightTapped = false;
		rotationType = -1;
		rotationFrameNumber = 0;
		scalingFrameNumber = 0;

		if (TECHNIQUE_SELECTED == TECHNIQUE_5) {
			BoundBoxes_BoundBox.initialized = false;
		}
		//UserStudyLogger.Instance.Record(toRecord, UserStudyLogger.Instance.saveFileExtended);
		myUserStudyManager.totalTimeManipulating.Stop();

	}
	public virtual void OnInputUp(InputEventData eventData) {
        if (myUserStudyManager.IN_USER_STUDY)
        {
            SaveDataClass data = new SaveDataClass(myUserStudyManager);

            if (resizing)
            {
                data.inResizeGesture = true;
            }
            if (rotating)
            {
                data.inRotationGesture = true;
            }
            if (eventData.SourceId == leftID)
                data.leftUp = true;
            if (eventData.SourceId == rightID)
                data.rightUp = true;

            if (myUserStudyManager.inTraining())
            {
                data.training = true;
                data.trainingSection = myUserStudyManager.practiceFrame.ToString();
            }
            else
            {
                int index = myUserStudyManager.hologramIndex;
                GameObject manipulatableObject = myUserStudyManager.manipulatableGameObjects[index];
                GameObject targetObject = myUserStudyManager.targetGameObjects[index];
                data.setTransforms(manipulatableObject, targetObject);
            }

            myUserStudyManager.SaveDataExtended(data);
        }

        CancelManipulation();
	}

	public void beginGesture() {
		

		initialLeft = leftHandPosition;
		initialRight = rightHandPosition;
		leftHandInitialSpindlePosition = leftHandSpindlePosition;
		rightHandInitialSpindlePosition = rightHandSpindlePosition;
		initialArcballHandPosition = arcBallHandPosition;
		initialHandBeingTrackedPosition = handBeingTrackedPosition;
		initialSize = objFocused.transform.localScale;
		initialRotation = objFocused.transform.localRotation;
		initialExtent = TwoHandedCursorManager.extents[objFocused];
		if(TECHNIQUE_SELECTED == TECHNIQUE_1) {
			if (Mathf.Abs(leftHandPosition.y - rightHandPosition.y) < ROTATION_THRESHOLD) {
				rotationGesture = true;
				rotating = true;
				rotationFrameNumber = 0;
				scalingFrameNumber = 0;
				resizing = false;
				resizingGesture	= false;
			} else {
				resizingGesture = true;
				rotating = false;
				resizing = true;
				rotationGesture = false;
			}
		} else if (TECHNIQUE_SELECTED == TECHNIQUE_2) {
			if (rotationType != -1) {
				rotationGesture = true;
				rotating = true;
				rotationFrameNumber = 0;
				scalingFrameNumber = 0;
				resizing = false;
				resizingGesture	= false;
			} else {
				resizingGesture = true;
				rotating = false;
				resizing = true;
				rotationGesture = false;
			}
		} else if (TECHNIQUE_SELECTED == TECHNIQUE_3) {
			rotating = true;
			resizing = true;
			rotationGesture = true;
			rotationFrameNumber = 0;
			scalingFrameNumber = 0;
			resizingGesture	= true;
		} else if (TECHNIQUE_SELECTED == TECHNIQUE_4)  {
			if(foundLeft && foundRight) {
				rotationGesture = false;
				rotating = false;
				rotationFrameNumber = 0;
				scalingFrameNumber = 0;
				resizing = true;
				resizingGesture	= true;
			} else {
				rotationGesture = true;
				rotating = true;
				rotationFrameNumber = 0;
				scalingFrameNumber = 0;
				resizing = false;
				resizingGesture	= false;
			}
		} else if (TECHNIQUE_SELECTED == TECHNIQUE_5) {
			if(myCursorManager.onSphere) {
				rotationGesture = true;
				rotating = true;
				rotationFrameNumber = 0;
				scalingFrameNumber = 0;
				resizing = false;
				resizingGesture	= false;
				rotationType = TwoHandedRotationManager.wireFrameRotation;
				myArrowsManager.setRotationAxis();
			} else {
				rotationGesture = false;
				rotating = false;
				rotationFrameNumber = 0;
				scalingFrameNumber = 0;
				resizing = true;
				resizingGesture	= true;
			}
		} else if (TECHNIQUE_SELECTED == TECHNIQUE_6) {
			if(technique5Selection < 3) {
				rotationGesture = true;
				rotating = true;
				rotationFrameNumber = 0;
				scalingFrameNumber = 0;
				resizing = false;
				resizingGesture	= false;
			} else {
				rotationGesture = false;
				rotating = false;
				rotationFrameNumber = 0;
				scalingFrameNumber = 0;
				resizing = true;
				resizingGesture	= true;
			}
		}

        if (myUserStudyManager.IN_USER_STUDY)
        {
            setInteractable(beginUserStudyButton);
            userStudyButtonText.text = "Continue";
            myUserStudyManager.gestureAttempts++;
            print("beginning manipulation timer");
            myUserStudyManager.totalTimeManipulating.Start();

            SaveDataClass data = new SaveDataClass(myUserStudyManager);
            data.beginningGesture = true;

            if (resizing)
            {
                data.inResizeGesture = true;
            }
            if (rotating)
            {
                data.inRotationGesture = true;
            }
            if (myUserStudyManager.inTraining())
            {
                data.training = true;
                data.trainingSection = myUserStudyManager.practiceFrame.ToString();
            }
            else
            {
                int index = myUserStudyManager.hologramIndex;
                GameObject manipulatableObject = myUserStudyManager.manipulatableGameObjects[index];
                GameObject targetObject = myUserStudyManager.targetGameObjects[index];
                data.setTransforms(manipulatableObject, targetObject);
            }

            myUserStudyManager.SaveDataExtended(data);
        }

        leftTapped = rightTapped = false;
	}

	public virtual void OnInputDown(InputEventData eventData) {

        if(myUserStudyManager.IN_USER_STUDY)
        {
            SaveDataClass data = new SaveDataClass(myUserStudyManager);

            if (resizing)
            {
                data.inResizeGesture = true;
            }
            if (rotating)
            {
                data.inRotationGesture = true;
            }

            if (eventData.SourceId == leftID)
                data.leftDown = true;
            if (eventData.SourceId == rightID)
                data.rightDown = true;

            if (myUserStudyManager.inTraining())
            {
                data.training = true;
                data.trainingSection = myUserStudyManager.practiceFrame.ToString();
            }
            else
            {
                int index = myUserStudyManager.hologramIndex;
                GameObject manipulatableObject = myUserStudyManager.manipulatableGameObjects[index];
                GameObject targetObject = myUserStudyManager.targetGameObjects[index];
                data.setTransforms(manipulatableObject, targetObject);
            }

            myUserStudyManager.SaveDataExtended(data);
        }
		if (eventData.SourceId == leftID) {
			myUserStudyManager.leftTaps++;
			leftTapped = true;
		} else {
			myUserStudyManager.rightTaps++;
			rightTapped = true;
		}

		if (!myCursorManager.onHologram)
			return;
		if((TECHNIQUE_SELECTED == TECHNIQUE_5) && (myCursorManager.onCube || myCursorManager.onSphere) && !resizing && !rotating) {
			handBeingTracked = (int)eventData.SourceId;
			if (handBeingTracked == leftID) {
				handBeingTrackedPosition = leftHandPosition;
			} else {
				handBeingTrackedPosition = rightHandPosition;
			}
			beginGesture();
		}
		else if (leftTapped && rightTapped && foundLeft && foundRight && !resizing && !rotating) {
			beginGesture();

		} else if(!resizing && !rotating && (TECHNIQUE_SELECTED == TECHNIQUE_4 || TECHNIQUE_SELECTED == TECHNIQUE_6)) {
			handBeingTracked = (int)eventData.SourceId;
			if (handBeingTracked == leftID) {
				handBeingTrackedPosition = leftHandPosition;
			} else {
				handBeingTrackedPosition = rightHandPosition;
			}
				if((leftTapped && (handBeingTracked == leftID) && !foundRight) || (rightTapped && (handBeingTracked == rightID) && !foundLeft)) {
					beginGesture();
				}
		}
	}

	public virtual void OnInputClicked(InputClickedEventData eventData) {
		if (eventData.SourceId == leftID) {
			myUserStudyManager.leftTaps--;

		} else {
			myUserStudyManager.rightTaps--;
		}
		CancelManipulation ();
	}
	public void ColorLeftObjects(Color col) {
		ColorObject(leftDot, col);
		ColorObject (leftLine, col);
		ColorObject (leftTriangle, col);
	}

	public void ColorRightObjects(Color col) {
		ColorObject(rightDot, col);
		ColorObject (rightLine, col);
		ColorObject (rightTriangle, col);
	}
	public void FindHandAndColorArrows(InteractionSourceState[] states) {
		Vector3 camPos;
		Vector3 gazePos;
		camPos = Camera.main.transform.position;
		gazePos = Camera.main.transform.forward;

		int handsOpen = 0;
		int handsFound = 0;

		bool rightFound;
		bool leftFound;
		bool trackedFound;
		bool leftOpen = false;
		bool rightOpen = false;

		rightFound = leftFound = trackedFound = false;

		if (states.Length > 2)
			print ("UUUHH OHHH");

		for (int i = 0; i < states.Length; i++) {
			Vector3 pos;
			InteractionSourceState hand = states [i];
			if(hand.source.kind == InteractionSourceKind.Hand) {
				handsFound++;
				hand.properties.location.TryGetPosition (out pos);
				if (DotProduct (camPos, gazePos, pos) < 0) {
					rightID = (int)hand.source.id;
					if(handBeingTracked == -1) {
						trackedFound = true;
						handBeingTracked = rightID;
					}
					rightFound = true;
					rightHandPosition = pos;
					if(handBeingTracked == rightID) {
						trackedFound = true;
						handBeingTrackedPosition = pos;
					}
					rightHandDistance = Vector3.Distance(pos, camPos);

					if(!hand.pressed) {
						rightCorrectColor = new Color(0, 255, 0);
						handsOpen++;
						rightTapped = false;
						rightOpen = true;
					} else {
						rightCorrectColor = new Color(255, 255, 0);
					}
					ColorRightObjects(rightCorrectColor);
					if(TECHNIQUE_SELECTED == TECHNIQUE_4) {
						ColorLeftObjects(leftCorrectColor);
					}
				} else {
						leftID = (int)hand.source.id;
						if(handBeingTracked == -1) {
							handBeingTracked = leftID;
						}
						leftFound = true;
						leftHandPosition = pos;
						leftHandDistance = Vector3.Distance(pos, camPos);
						if(handBeingTracked == leftID) {
							handBeingTrackedPosition = pos;
							trackedFound = true;
						}
						if(!hand.pressed) {
							leftCorrectColor = new Color(0, 255, 0);
							handsOpen++;
							leftTapped = false;
							leftOpen = true;
						} else {
							leftCorrectColor = new Color(255, 255, 0);
						}
						ColorLeftObjects(leftCorrectColor);
						if(TECHNIQUE_SELECTED == TECHNIQUE_4) {
							ColorRightObjects(leftCorrectColor);
						}
				}
			}
		}
		//if we cant find left now, but we used to have it
		if(myUserStudyManager.RECORD_INFORMATION) {
			if(!leftFound && foundLeft)
				myUserStudyManager.leftHandLosses++;
			if(!rightFound && foundRight)
				myUserStudyManager.rightHandLosses++;
		}

		//if we found a hand that was previously lost
		if (myUserStudyManager.IN_USER_STUDY && ((leftFound && !foundLeft) || (rightFound && !foundRight)))
		{
			SaveDataClass data = new SaveDataClass(myUserStudyManager);

			if (resizing)
			{
				data.inResizeGesture = true;
			}
			if (rotating)
			{
				data.inRotationGesture = true;
			}
			if (leftFound && !foundLeft)
				data.foundLeft = true;
			if (rightFound && !foundRight)
				data.foundRight = true;

			if (myUserStudyManager.inTraining())
			{
				data.training = true;
				data.trainingSection = myUserStudyManager.practiceFrame.ToString();
			}
			else
			{
				int index = myUserStudyManager.hologramIndex;
				GameObject manipulatableObject = myUserStudyManager.manipulatableGameObjects[index];
				GameObject targetObject = myUserStudyManager.targetGameObjects[index];
				data.setTransforms(manipulatableObject, targetObject);
			}

			myUserStudyManager.SaveDataExtended(data);
		}

		//if we lost a hand that was previously found
		if (myUserStudyManager.IN_USER_STUDY && ((!leftFound && foundLeft) || (!rightFound && foundRight)))
        {
            SaveDataClass data = new SaveDataClass(myUserStudyManager);

            if (resizing)
            {
                data.inResizeGesture = true;
            }
            if (rotating)
            {
                data.inRotationGesture = true;
            }
            if (!leftFound && foundLeft)
                data.leftLost = true;
            if (!rightFound && foundRight)
                data.rightLost = true;

            if (myUserStudyManager.inTraining())
            {
                data.training = true;
                data.trainingSection = myUserStudyManager.practiceFrame.ToString();
            }
            else
            {
                int index = myUserStudyManager.hologramIndex;
                GameObject manipulatableObject = myUserStudyManager.manipulatableGameObjects[index];
                GameObject targetObject = myUserStudyManager.targetGameObjects[index];
                data.setTransforms(manipulatableObject, targetObject);
            }

            myUserStudyManager.SaveDataExtended(data);
        }

        foundRight = rightFound;
		foundLeft = leftFound;
		foundTracked = trackedFound;

		if ((TECHNIQUE_SELECTED == TECHNIQUE_4) || TECHNIQUE_SELECTED == TECHNIQUE_5) {
			if ((handBeingTracked == leftID) && leftOpen)
				CancelManipulation ();
			if ((handBeingTracked == rightID) && rightOpen)
				CancelManipulation ();
		}
		else if(handsOpen != 0) {
			if(resizing || rotating)
				CancelManipulation();
		}

		if (myCursorManager.onHologram && (objFocused != null)) {
			arcBallRadius = TwoHandedCursorManager.extents [objFocused];
			Vector3 center = (rightHandPosition + leftHandPosition) / 2;
			Vector3 translation = objCenter - center;

			rightHandSpindlePosition = rightHandPosition + translation;
			leftHandSpindlePosition = leftHandPosition + translation;

			float leftDistance = Vector3.Distance (leftHandSpindlePosition, objCenter);
			float rightDistance = Vector3.Distance (rightHandSpindlePosition, objCenter);

			rightHandSpindlePosition = Vector3.MoveTowards (rightHandSpindlePosition, objCenter, rightDistance - arcBallRadius * 1.2f);
			leftHandSpindlePosition = Vector3.MoveTowards (leftHandSpindlePosition, objCenter, leftDistance - arcBallRadius * 1.2f);

			if (TECHNIQUE_SELECTED == TECHNIQUE_6) {
				float scale = Vector3.Distance (handBeingTrackedPosition, objCenter) / 2.0f;
				Vector3 scaledLocation = Vector3.MoveTowards (handBeingTrackedPosition, Camera.main.transform.position, -scale);

				arcBallHandPosition = Vector3.MoveTowards (scaledLocation, objCenter, Vector3.Distance (scaledLocation, objCenter) - arcBallRadius);


				switch (technique5Selection) {
				case 0:
					arcBallHandPosition.x = objCenter.x;
					break;
				case 1:
					arcBallHandPosition.y = objCenter.y;
					break;
				case 2:
					arcBallHandPosition.z = objCenter.z;
					break;
				}

				arcBallHandPosition = Vector3.MoveTowards (arcBallHandPosition, objCenter, Vector3.Distance (arcBallHandPosition, objCenter) - arcBallRadius);

			} else {
				float scale;
				// lower the number of division to increase scale
				scale = Vector3.Distance (handBeingTrackedPosition, objCenter) / 1.2f;
	            
				
				Vector3 scaledLocation = Vector3.MoveTowards (handBeingTrackedPosition, Camera.main.transform.position, -scale);

				arcBallHandPosition = Vector3.MoveTowards (scaledLocation, objCenter, Vector3.Distance (scaledLocation, objCenter) - arcBallRadius);
			}
		}
		//if hands are lost and we're doing spindle technique, bail on manipulation
		if((handsFound < 2) && (TECHNIQUE_SELECTED == TECHNIQUE_3) && (resizing || rotating)) {
					CancelManipulation();
					if(myUserStudyManager.RECORD_INFORMATION) {
						if(foundLeft)
							myUserStudyManager.leftHandLosses++;
						if(foundRight)
							myUserStudyManager.rightHandLosses++;
					}

           
            foundLeft = false;
					foundRight = false;
					myArrowsManager.HideArrows();
		}
	}

    public void resetGameObjects()
    {
        SceneManager.LoadScene(0);
        sentText = "";
    }

	void OnGUI() {
		Event e = Event.current;
        if (e.type == EventType.KeyUp && e.isKey && e.keyCode != KeyCode.None)
        {
            Debug.Log("Detected key code: " + e.keyCode.ToString());
            sentText += e.keyCode.ToString();
            print("sent text is now " + sentText);
            if (e.keyCode.ToString().Equals("X"))
            {
                sentText = "";
            }
            if(sentText.Equals("RESET"))
            {
                resetGameObjects();
            }
            if(sentText.Equals("HARDRESET"))
            {
				sentText = "";
                SceneManager.LoadScene(0);
            }
            if(sentText.Contains("USER"))
            {
                if(sentText.EndsWith("USER") && sentText.Length != "USER".Length)
                {
                    sentText = sentText.Replace("USER", "");
                    sentText = sentText.Replace("Alpha", "");

                    print("result is " + sentText);
					userIDText.text = sentText;
					myUserStudyManager.UserStudyButtonPressed ();
                    sentText = "";
                }
            }
            if (sentText.Contains("TECHNIQUE"))
            {
                if (sentText.EndsWith("TECHNIQUE") && sentText.Length != "TECHNIQUE".Length)
                {
                    sentText = sentText.Replace("TECHNIQUE", "");
                    sentText = sentText.Replace("Alpha", "");

                    myUserStudyManager.performTechnique(sentText);
                    sentText = "";
                }
            }
        }
	}

	void DrawWireSphere() {
			if(TECHNIQUE_SELECTED == TECHNIQUE_4 || TECHNIQUE_SELECTED == TECHNIQUE_3 || TECHNIQUE_SELECTED == TECHNIQUE_6) {
				float scale = arcBallRadius / 0.5f;
				boundingSphere.GetComponent<Renderer>().transform.position = objCenter;
				boundingSphere.GetComponent<Renderer>().transform.localScale = new Vector3(scale, scale, scale);
				boundingSphere.SetActive(true);
				// myCursorManager.SHOW_BOUNDING_BOX = false;
			} else {
				boundingSphere.SetActive(false);
				// myCursorManager.SHOW_BOUNDING_BOX = true;
			}
	}

	void setInteractable(Button button) {
		button.interactable = true;
		button.OnDeselect(null);
	}


	void resetNullObjects() {
		_instance.leftLine = leftLine;
		_instance.rightLine = rightLine;
		_instance.leftTriangle = leftTriangle;
		_instance.rightTriangle = rightTriangle;
		_instance.leftDot = leftDot;
		_instance.rightDot = rightDot;
		_instance.rotationAxis = rotationAxis;
		_instance.rotationAxis2 = rotationAxis2;
		_instance.pitchImage = pitchImage;
		_instance.yawImage = yawImage;
		_instance.rollImage = rollImage;
		_instance.pitchImage2 = pitchImage2;
		_instance.yawImage2 = yawImage2;
		_instance.rollImage2 = rollImage2;
		_instance.rotationImage = rotationImage;
		_instance.technique2Image = technique2Image;
		_instance.resizeImage = resizeImage;
		_instance.boundingSphere = boundingSphere;
		_instance.menu = menu;
		_instance.userIDMenu = userIDMenu;
		_instance.instructionPanel = instructionPanel;
		_instance.normalCursor = normalCursor;
		_instance.resizeCursor = resizeCursor;
		_instance.pitchCursor = pitchCursor;
		_instance.yawCursor = yawCursor;
		_instance.rollCursor = rollCursor;
		_instance.technique5Panel = technique5Panel;
		_instance.userIDText = userIDText;
		_instance.instructionPanelText = instructionPanelText;
		_instance.instructionTexts = instructionTexts;
		_instance.techniqueNames = techniqueNames;
		_instance.numberButtons = numberButtons;
		_instance.technique5Buttons = technique5Buttons;
		_instance.userStudyOrientationText = userStudyOrientationText;
		_instance.userStudyRoundText = userStudyRoundText;
		_instance.instructionTitleText = instructionTitleText;
		_instance.arrowMaterial = arrowMaterial;
		_instance.debuggingMaterial = debuggingMaterial;
		_instance.menuText = menuText;
		_instance.userStudyButtonText = userStudyButtonText;
		_instance.GameObjectCollection = GameObjectCollection;
		_instance.HologramCollection = HologramCollection;
		//_instance.MenuCollection = MenuCollection;
		_instance.counter = counter;
	}
		// Update is called once per frame
	void Update () {
		userIDText.text = "101";
		//print ("camera is at " + Camera.main.transform.position);
		if (myUserStudyManager.disableImages) {
			showWireBox = false;

			technique2Image.SetActive(false);

			myArrowsManager.rotationAxisRenderer2.enabled = false;
		} else {
			if(TECHNIQUE_SELECTED == TECHNIQUE_5 && (objFocused != null))
				showWireBox = true;
			if(TECHNIQUE_SELECTED == TECHNIQUE_2 && (objFocused != null)) {
				myArrowsManager.enableTechnique2Image();
			} else {
                technique2Image.SetActive(false);
                myArrowsManager.rotationAxisRenderer2.enabled = false;
            }
		}
		VideoPlayer player = videoPlayer.GetComponent<VideoPlayer> ();
		if (player.isPrepared && !player.isPlaying && myUserStudyManager.shouldPlay)
			player.Play ();

		if(myUserStudyManager.IN_USER_STUDY && (myUserStudyManager.USER_STUDY_TECHNIQUE_INDEX == TwoHandedUserStudyManager.NUM_TECHNIQUES)) {
			// print("beginUserStudyButton: Enabled: true Interactable: true");
			beginUserStudyButton.enabled = true;
			setInteractable(beginUserStudyButton);
			myUserStudyManager.USER_STUDY_TECHNIQUE_INDEX++;
		}
		if(!myUserStudyManager.IN_USER_STUDY) {
			//if(userIDText.text == "") {
				// print("beginUserStudyButton: Enabled: unknown Interactable: false");
				//beginUserStudyButton.interactable = false;
			//} else {
				// print("beginUserStudyButton: Enabled: true Interactable: true");
				setInteractable(beginUserStudyButton);
			//}
		} else {
			if(instructionPanel.activeSelf && !myUserStudyManager.disableTimedButton) {
				int seconds = myUserStudyManager.timeToWait;
				int fps = 40;
				if (counter++ >= (seconds * fps)) {
					setInteractable (beginUserStudyButton);
					userStudyButtonText.text = "Continue";
					counter = 0;
					myUserStudyManager.disableTimedButton = true;
				} else {
					if (beginUserStudyButton.interactable == false) {
						int time = seconds - counter / fps;
						userStudyButtonText.text = "Read Instructions Below Before Proceeding (" + time + ")";
					}
				}
			} else {
				counter = 0;
			}
		}

		if(TECHNIQUE_SELECTED == TECHNIQUE_6) {
				technique5Panel.SetActive(true);
				Vector3 panelPosition;
				Vector3 objSize = boundingCube.transform.localScale;

				panelPosition = TwoHandedArrowsManager.GetBoundingCornerPosition (0, objSize.y + .09f, -objSize.z/2, boundingCube);

				MoveObject(technique5Panel, panelPosition);

				technique5Buttons[technique5Selection].Select();
		} else {
			technique5Panel.SetActive(false);
		}
		menu.transform.LookAt(2 * menu.transform.position - Camera.main.transform.position);
		myCursorManager.Update ();
		objFocused = myCursorManager.hologram ? myCursorManager.hologram : objFocused;
		focusedForBox = myCursorManager.hologram;
		boundingCube = myCursorManager.cube ? myCursorManager.cube : boundingCube;
		objCenter = objFocused ? objFocused.GetComponent<Renderer> ().bounds.center : new Vector3(0,0,0);

		//search for hands and if found, color arrow accordingly
		// if(onHologram) {
		//
		// }

		//update writing to file
		if (myUserStudyManager.IN_USER_STUDY && !myUserStudyManager.resting && (++frames % 3 == 0))
		{
			frames = 0;
			SaveDataClass data = new SaveDataClass(myUserStudyManager);
			data.update = true;
			if (resizing)
			{
				data.inResizeGesture = true;
			}
			if (rotating)
			{
				data.inRotationGesture = true;
			}

			if (myUserStudyManager.inTraining())
			{
				data.training = true;
				data.trainingSection = myUserStudyManager.practiceFrame.ToString();
			}
			else
			{
				int index = myUserStudyManager.hologramIndex;
				GameObject manipulatableObject = myUserStudyManager.manipulatableGameObjects[index];
				GameObject targetObject = myUserStudyManager.targetGameObjects[index];
				data.setTransforms(manipulatableObject, targetObject);
			}

			myUserStudyManager.SaveDataExtended(data);
		}

		if(InteractionManager.numSourceStates != 0) {
			InteractionSourceState[] states = InteractionManager.GetCurrentReading ();
			FindHandAndColorArrows(states);
			DrawWireSphere();
		} else {
			if(resizing || rotating) {
				CancelManipulation();
			}
			if(myUserStudyManager.RECORD_INFORMATION) {
				if(foundLeft)
					myUserStudyManager.leftHandLosses++;
				if(foundRight)
					myUserStudyManager.rightHandLosses++;
			}

			if (myUserStudyManager.IN_USER_STUDY && (foundLeft || foundRight) && InteractionManager.numSourceStates == 0)
            {
                SaveDataClass data = new SaveDataClass(myUserStudyManager);

                if (resizing)
                {
                    data.inResizeGesture = true;
                }
                if (rotating)
                {
                    data.inRotationGesture = true;
                }
                if (foundLeft)
                    data.leftLost = true;
                if (foundRight)
                    data.rightLost = true;

                if (myUserStudyManager.inTraining())
                {
                    data.training = true;
                    data.trainingSection = myUserStudyManager.practiceFrame.ToString();
                }
                else
                {
                    int index = myUserStudyManager.hologramIndex;
                    GameObject manipulatableObject = myUserStudyManager.manipulatableGameObjects[index];
                    GameObject targetObject = myUserStudyManager.targetGameObjects[index];
                    data.setTransforms(manipulatableObject, targetObject);
                }

                myUserStudyManager.SaveDataExtended(data);
            }
			if (InteractionManager.numSourceStates == 0) {
				foundLeft = false;
				foundRight = false;
				foundTracked = false;
				leftID = rightID = handBeingTracked = -1;
				myArrowsManager.HideArrows ();
			}
		}

		if (!myCursorManager.onHologram) {
			rotating = false;
			resizing = false;
			myArrowsManager.HideArrows();
		}

		if (myCursorManager.onHologram && objFocused.layer != MESH_LAYER && (foundLeft || foundRight)) {
			Vector3 objPosition = objFocused.transform.position;
			Vector3 objSize = boundingCube.transform.localScale;

			float width = objSize.x / 2;
			float height = objSize.y / 2;
			float length = objSize.z / 2;

			boundingCube.transform.LookAt(Camera.main.transform);
			if (!rotating) {
				//grab initial width length height of bounding box so that arrows don't slide in and out when rotating
				initialBoundingWidth = width;
				initialBoundingLength = length;
				initialBoundingHeight = height;
			}
			//set arrow position based on orientation of hands/gesture
			if(TECHNIQUE_SELECTED != TECHNIQUE_5)
				myArrowsManager.SetArrowPositionsAndShow(initialBoundingWidth, initialBoundingLength, initialBoundingHeight);
		}

		if(((handBeingTracked == leftID) && !foundLeft) || ((handBeingTracked == rightID) && !foundRight) || !foundTracked) {
			handBeingTracked = -1;
		}
		//if we lost a hand
		if (!foundLeft && !resizing && !rotating) {
			ColorLeftObjects(new Color(255, 0, 0));
			leftID = -1;
		}
		if (!foundRight && !resizing && !rotating) {
			ColorRightObjects(new Color(255, 0, 0));
			rightID = -1;
		}

		if (resizing) {
			myResizeManager.ScaleObject();
		}
		if(rotating) {
			myRotateManager.RotateObject();
		}
	}

	public void HideObject(GameObject g) {
		g.GetComponent<Renderer>().enabled = false;
	}

	public void ShowObject(GameObject g) {
		g.GetComponent<Renderer>().enabled = true;
	}

	public void MoveObject(GameObject g, Vector3 pos) {
		g.transform.position = pos;
	}

	public void ColorObject(GameObject g, Color c) {
		g.GetComponent<Renderer> ().material.color = c;
	}

	// for now ignoring the y axis. May change that later
	public float DotProduct(Vector3 a, Vector3 b, Vector3 c) {
		float dot = (b.x - a.x)*(c.z - a.z) - (b.z - a.z)*(c.x - a.x);
		//if (b.z < 0)
			//dot *= -1;
		return dot;
	}

}

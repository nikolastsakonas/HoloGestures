using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandedRotationManager {
	private TwoHandedGesturesManager handsManager = null;
	private int NUM_FRAMES_RESET = 30;
	private Vector3 rotationInitialLeft;
	private Vector3 rotationInitialRight;
	internal const int pitchRotation = 0;
	internal const int rollRotation = 1;
	internal const int yawRotation = 2;
	internal const int spindleRotation = 3;
	internal const int arcBallRotation = 4;
	internal const int wireFrameRotation = 5;
	internal const int selectionRotation = 6;
	public static float direction = 1;
	float initialRightFaceDistance;
	float initialLeftFaceDistance;
	float initialDistance;
	float rotationThreshold = .03f;
	float lastYawRotationAmount;
	float lastPitchRotationAmount;
	float lastRollRotationAmount;
	public static Vector3 initialGazeDirection;
	public Vector3 lastAxis;

	public TwoHandedRotationManager(TwoHandedGesturesManager _handsManager) {
		handsManager = _handsManager;
		handsManager.rotationType = -1;
	}

	public void RotateOnAxis(float sign, Vector3 rotationAxis, ref float lastRotationAmount) {
		float distance;
		float rotationAmount;
		Vector3 objCenter = handsManager.objCenter;

		distance = Mathf.Abs(sign);
		if(initialDistance < 0) initialDistance = distance;

		rotationAmount = distance * 1800;

		//rotate object
		rotationAmount *= sign < 0 ? -1 : 1;
		handsManager.objFocused.transform.RotateAround(objCenter, rotationAxis, rotationAmount - lastRotationAmount);
		TwoHandedCursorManager.SetBoundingCubeSize (handsManager.objFocused, handsManager.boundingCube);

		direction = sign;
		lastRotationAmount = rotationAmount;
	}

	public void RotateObject() {
		Vector3 cameraPosition = Camera.main.transform.position;
		float currentRightFaceDistance = Vector3.Distance (cameraPosition, handsManager.rightHandPosition);
		float currentLeftFaceDistance = Vector3.Distance (cameraPosition, handsManager.leftHandPosition);
		float sign;
		Quaternion rotation;
		float angle;
		Vector3 cross;
		Vector3 newPosition, oldPosition;
		int resetFrames = NUM_FRAMES_RESET;
		Vector3 rotationAxis = handsManager.axisForRotation;
		Vector3 pitchAxis, rollAxis, yawAxis;
		float rollSign, yawSign, pitchSign;

		if(handsManager.rotationFrameNumber == 0) {
			SetInitialPositions(handsManager.initialRight, handsManager.initialLeft);
			initialGazeDirection = handsManager.objCenter - Camera.main.transform.position; //or can use Camera.main.transform.forward;
		}

		switch(handsManager.rotationType) {
			case pitchRotation:
				if(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_1) {
					sign = ((handsManager.rightHandPosition.y - rotationInitialRight.y) + (handsManager.leftHandPosition.y - rotationInitialLeft.y))/2;
				} else {
						sign = ((initialRightFaceDistance - currentRightFaceDistance) + (currentLeftFaceDistance - initialLeftFaceDistance))/2;
						if(rotationInitialRight.y > rotationInitialLeft.y) {
							sign *= -1;
						}
				}
				RotateOnAxis(sign, rotationAxis, ref lastPitchRotationAmount);
				break;
			case rollRotation:
				sign = ((handsManager.rightHandPosition.y - rotationInitialRight.y) + (rotationInitialLeft.y - handsManager.leftHandPosition.y))/2;
				RotateOnAxis(sign, rotationAxis, ref lastRollRotationAmount);
				break;
			case yawRotation:
				sign = ((initialRightFaceDistance - currentRightFaceDistance) + (currentLeftFaceDistance - initialLeftFaceDistance))/2;
				RotateOnAxis(sign, rotationAxis, ref lastYawRotationAmount);
				break;
			case spindleRotation:
				Vector3 newRightSide = handsManager.rightHandSpindlePosition - handsManager.objCenter;
				Vector3 oldRightSide = handsManager.rightHandInitialSpindlePosition - handsManager.objCenter;
				Vector3 newLeftSide = handsManager.leftHandSpindlePosition - handsManager.objCenter;
				Vector3 oldLeftSide = handsManager.leftHandInitialSpindlePosition - handsManager.objCenter;

				angle = -(Vector3.Angle(oldRightSide, newRightSide) + Vector3.Angle(oldLeftSide, newLeftSide)) / 2;

				cross = Vector3.Cross(newRightSide, oldRightSide);

				rotation = Quaternion.AngleAxis(angle, cross);

				// attempt at pitch starts here
				sign = ((handsManager.rightHandPosition.y - rotationInitialRight.y) + (handsManager.leftHandPosition.y - rotationInitialLeft.y))/3;
				rotationAxis = handsManager.rightHandSpindlePosition - handsManager.leftHandSpindlePosition;
				RotateOnAxis(sign, rotationAxis, ref lastPitchRotationAmount);
				// and it ends here

				if(handsManager.rotationFrameNumber != 0) {
					handsManager.objFocused.transform.RotateAround(handsManager.objCenter, lastAxis, -lastYawRotationAmount);
					handsManager.boundingSphere.transform.RotateAround(handsManager.objCenter, lastAxis, -lastYawRotationAmount);
				}
				lastAxis = cross;
				handsManager.objFocused.transform.RotateAround(handsManager.objCenter, cross, angle);// = rotation;
				handsManager.boundingSphere.transform.RotateAround(handsManager.objCenter, cross, angle);
				TwoHandedCursorManager.SetBoundingCubeSize (handsManager.objFocused, handsManager.boundingCube);

				lastYawRotationAmount = angle;
				++handsManager.rotationFrameNumber;
				break;
			case arcBallRotation:
				newPosition = handsManager.arcBallHandPosition - handsManager.objCenter;
				oldPosition = handsManager.initialArcballHandPosition - handsManager.objCenter;


				angle = -Vector3.Angle(oldPosition, newPosition);

				cross = Vector3.Cross(newPosition, oldPosition);

				if(handsManager.rotationFrameNumber != 0) {
					handsManager.objFocused.transform.RotateAround(handsManager.objCenter, lastAxis, -lastPitchRotationAmount);
					handsManager.boundingSphere.transform.RotateAround(handsManager.objCenter, lastAxis, -lastPitchRotationAmount);
				}
				lastAxis = cross;
				handsManager.objFocused.transform.RotateAround(handsManager.objCenter, cross, angle);
				handsManager.boundingSphere.transform.RotateAround(handsManager.objCenter, cross, angle);
				TwoHandedCursorManager.SetBoundingCubeSize (handsManager.objFocused, handsManager.boundingCube);

				lastPitchRotationAmount = angle;
				++handsManager.rotationFrameNumber;
			return;
		case wireFrameRotation:
				float initialDistance;
				float newDistance;
				Vector3 C, A, B;

				C = handsManager.handBeingTrackedPosition;
				A = handsManager.initialHandBeingTrackedPosition;
				B = handsManager.objCenter;


				initialDistance = Vector3.Distance (handsManager.initialHandBeingTrackedPosition, handsManager.objCenter);
				newDistance = Vector3.Distance (handsManager.handBeingTrackedPosition, handsManager.objCenter);

				float sign2;

				if(TwoHandedCursorManager.sphereIndexUnderCursor < 4) {
					sign = handsManager.DotProduct (Camera.main.transform.position, Camera.main.transform.forward, C) - handsManager.DotProduct (Camera.main.transform.position, Camera.main.transform.forward, A);
					//yaw
				} else if (TwoHandedCursorManager.sphereIndexUnderCursor < 6) {
					// pitch
					sign = initialDistance - newDistance;
					sign2 = handsManager.handBeingTrackedPosition.y - handsManager.initialHandBeingTrackedPosition.y;
					sign = (sign + sign2) ;
				} else {
					sign = handsManager.DotProduct (Camera.main.transform.position, Camera.main.transform.forward, A) - handsManager.DotProduct (Camera.main.transform.position, Camera.main.transform.forward, C);
					sign2 = handsManager.initialHandBeingTrackedPosition.y - handsManager.handBeingTrackedPosition.y;
					sign = (sign + sign2);
					//roll
				}
				RotateOnAxis(sign, handsManager.axisForRotation, ref lastPitchRotationAmount);
				++handsManager.rotationFrameNumber;
			return;
			case selectionRotation:
				// switch(handsManager.technique5Selection) {
				// 	case 0:
				// 		sign = handsManager.initialArcballHandPosition.y - rotationInitialRight.y;
				// 		RotateOnAxis(sign, rotationAxis, ref lastPitchRotationAmount);
				// 		break;
				// 	case 1:
				// 		sign = ((initialRightFaceDistance - currentRightFaceDistance) + (currentLeftFaceDistance - initialLeftFaceDistance))/2;
				// 		RotateOnAxis(sign, rotationAxis, ref lastYawRotationAmount);
				// 		break;
				// 	case 2:
				// 		sign = ((handsManager.rightHandPosition.y - rotationInitialRight.y) + (rotationInitialLeft.y - handsManager.leftHandPosition.y))/2;
				// 		RotateOnAxis(sign, rotationAxis, ref lastRollRotationAmount);
				// 		break;
				//
				// }

				newPosition = handsManager.arcBallHandPosition - handsManager.objCenter;
				oldPosition = handsManager.initialArcballHandPosition - handsManager.objCenter;


				angle = -Vector3.Angle(oldPosition, newPosition);

				cross = Vector3.Cross(newPosition, oldPosition);

				if(handsManager.rotationFrameNumber != 0) {
					handsManager.objFocused.transform.RotateAround(handsManager.objCenter, lastAxis, -lastPitchRotationAmount);
					handsManager.boundingSphere.transform.RotateAround(handsManager.objCenter, lastAxis, -lastPitchRotationAmount);
				}

				lastAxis = cross;

				handsManager.objFocused.transform.RotateAround(handsManager.objCenter, cross, angle);
				handsManager.boundingSphere.transform.RotateAround(handsManager.objCenter, cross, angle);
				TwoHandedCursorManager.SetBoundingCubeSize (handsManager.objFocused, handsManager.boundingCube);

				lastPitchRotationAmount = angle;
				++handsManager.rotationFrameNumber;
			return;
			default:
				GuessRotationType();
				return;
		}

		if(++handsManager.rotationFrameNumber % resetFrames == 0) {
			SetInitialPositions(handsManager.rightHandPosition, handsManager.leftHandPosition);
		}
	}

	public void SetInitialPositions(Vector3 initialRight, Vector3 initialLeft) {
		//if we have locked into a rotation
		if(handsManager.rotationType != -1) {
			rotationInitialRight = handsManager.rightHandPosition;
			rotationInitialLeft = handsManager.leftHandPosition;
		}
		handsManager.initialRotation = handsManager.objFocused.transform.rotation;
		handsManager.rightHandInitialSpindlePosition = handsManager.rightHandSpindlePosition;
		handsManager.leftHandInitialSpindlePosition = handsManager.leftHandSpindlePosition;
		Vector3 cameraPosition = Camera.main.transform.position;
		initialRightFaceDistance = Vector3.Distance(cameraPosition, initialRight);
		initialLeftFaceDistance = Vector3.Distance(cameraPosition, initialLeft);

		lastYawRotationAmount = lastPitchRotationAmount = lastRollRotationAmount = 0;
		initialDistance = -1;
	}
	public void GuessRotationType() {
		SetInitialPositions(handsManager.initialRight, handsManager.initialLeft);
		initialGazeDirection =  handsManager.objCenter - Camera.main.transform.position; //or can use Camera.main.transform.forward;
		Vector3 cameraPosition = Camera.main.transform.position;

		float initialLeftY = handsManager.initialLeft.y;
		float initialRightY = handsManager.initialRight.y;
		float currentLeftY = handsManager.leftHandPosition.y;
		float currentRightY = handsManager.rightHandPosition.y;

		float currentLeftFaceDistance = Vector3.Distance (cameraPosition, handsManager.leftHandPosition);
		float currentRightFaceDistance = Vector3.Distance (cameraPosition, handsManager.rightHandPosition);

		// hands are moving towards or away from face to rotate around y axis
		if((Mathf.Abs(initialRightFaceDistance - currentRightFaceDistance) > rotationThreshold)
			&& (Mathf.Abs(initialLeftFaceDistance - currentLeftFaceDistance) > rotationThreshold)) {

			handsManager.rotationType = yawRotation;
		}

		// One hand moving up or one hand moving down
		else if(((currentLeftY - initialLeftY > rotationThreshold) && (currentRightY - initialRightY < -rotationThreshold))
			||(( currentLeftY - initialLeftY < -rotationThreshold) && (currentRightY - initialRightY > rotationThreshold))) {

			handsManager.rotationType = rollRotation;
		}

		// hands are moving either up together or down together
		else if(((currentLeftY -  initialLeftY > rotationThreshold) && (currentRightY - initialLeftY > rotationThreshold))
			|| ((currentLeftY -  initialLeftY < -rotationThreshold) && (currentRightY - initialLeftY < -rotationThreshold)) ) {

			handsManager.rotationType = pitchRotation;
		}

		//if we've guessed the rotation type
		if(handsManager.rotationType != -1) {
			handsManager.initialRight = handsManager.rightHandPosition;
			handsManager.initialLeft = handsManager.leftHandPosition;
		}
	}
}

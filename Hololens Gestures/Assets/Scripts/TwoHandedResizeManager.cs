using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.VR.WSA.Input;

public class TwoHandedResizeManager {
	public static TwoHandedGesturesManager handsManager = null;

	public TwoHandedResizeManager(TwoHandedGesturesManager _handsManager) {
		Debug.Log("Starting my Resize manager\n");
		handsManager = _handsManager;
	}
		
	public void ScaleObject() {
		float initialDistance;
		float newDistance;
		float linearScale;
		float scale;
		float difference;

		if(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5) {
			initialDistance = Vector3.Distance (handsManager.initialHandBeingTrackedPosition, handsManager.objCenter);
			newDistance = Vector3.Distance (handsManager.handBeingTrackedPosition, handsManager.objCenter);
			Vector3 C = handsManager.handBeingTrackedPosition;
			Vector3 A = handsManager.initialHandBeingTrackedPosition;

			float sign = handsManager.DotProduct (handsManager.objCenter, Camera.main.transform.position - handsManager.objCenter, TwoHandedCursorManager.objectUnderCursor.transform.position);
			float sign2 = handsManager.DotProduct (Camera.main.transform.position, Camera.main.transform.forward, C) - handsManager.DotProduct (Camera.main.transform.position, Camera.main.transform.forward, A);;
			if (sign > 0) {
				newDistance -= sign2;
			} else {
				newDistance += sign2;
			}
				
				
		} else {
			initialDistance = Vector3.Distance (handsManager.initialLeft, handsManager.initialRight);
			newDistance = Vector3.Distance (handsManager.leftHandPosition, handsManager.rightHandPosition);
		}

		linearScale = newDistance / initialDistance;
		difference = newDistance - initialDistance;

		if(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5) {
			scale = (float)Mathf.Pow(linearScale, 15f);
		} else if(TwoHandedGesturesManager.TECHNIQUE_SELECTED != TwoHandedGesturesManager.TECHNIQUE_3)
			scale = (float)Mathf.Pow(linearScale, 4f);
		else {
			// print(difference);
			// if(Mathf.Abs(difference) > .01)
				scale = (float)Mathf.Pow(linearScale, 2f);
			// else
			// 	return;
		}



		Vector3 newSize = handsManager.initialSize;
		newSize *= scale;

		Vector3 newBoundsSize = handsManager.objFocused.GetComponent<Renderer>().bounds.size * scale;

		float x = newBoundsSize.x, y = newBoundsSize.y, z = newBoundsSize.z;
		float smallestSideLimit = .017f;

		float largestSide = Mathf.Max(x, Mathf.Max(y, z));
		if(largestSide <= smallestSideLimit) {
			if((x > y) && (x > z))
				newSize.x = newSize.y = newSize.z = smallestSideLimit * newSize.x / x;
			else if( (y > x) && (y > z))
				newSize.x = newSize.y = newSize.z = smallestSideLimit * newSize.y / y;
			else
				newSize.x = newSize.y = newSize.z = smallestSideLimit * newSize.z / z;
		}

		TwoHandedCursorManager.extents[handsManager.objFocused] = newSize.x / handsManager.initialSize.x * handsManager.initialExtent;

		Vector3 oldCenter = handsManager.objFocused.GetComponent<Renderer> ().bounds.center;
		handsManager.objFocused.transform.localScale = newSize;
		Vector3 newCenter = handsManager.objFocused.GetComponent<Renderer> ().bounds.center;
		handsManager.objFocused.transform.position += (oldCenter - newCenter);
		TwoHandedCursorManager.SetBoundingCubeSize (handsManager.objFocused, handsManager.boundingCube);

	}
}

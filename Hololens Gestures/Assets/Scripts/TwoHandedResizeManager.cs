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

		if(TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5) {
			scale = (float)Mathf.Pow(linearScale, 15f);
		} else if(TwoHandedGesturesManager.TECHNIQUE_SELECTED != TwoHandedGesturesManager.TECHNIQUE_3)
			scale = (float)Mathf.Pow(linearScale, 4f);
		else {
				scale = (float)Mathf.Pow(linearScale, 2f);
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

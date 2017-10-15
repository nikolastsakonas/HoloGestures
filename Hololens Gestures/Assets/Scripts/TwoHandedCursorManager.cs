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

using UnityEngine;
using System.Collections.Generic;

public class TwoHandedCursorManager
{
	public float MaxGazeDistance = 4.0f;
	public bool onHologram;
	public GameObject hologram = null;
	public GameObject lastHologram = null;
	public GameObject cube = null;
	public static Dictionary <GameObject, GameObject> boundingCubes;
	public static Dictionary <GameObject, GameObject> boundingCubesInverse;
	public static Dictionary <GameObject, float> extents;
	public TwoHandedGesturesManager handsManager = null;
	private GameObject oldCube = null;
	public Vector3 position;
	public static float BOUNDING_BOX_SCALE_FACTOR = 1.2f;
	public static float RADIUS_MULTIPLIER = 1.0f;
	public bool SHOW_BOUNDING_BOX = false;
	Color32 highlightedCubeColor = new Color32(146, 231, 255, 255);
	Color32 regularCubeColor = new Color32 (46, 131, 255, 255);
	public bool onCube = false;
	public bool onSphere = false;
	public static int sphereIndexUnderCursor = -1;
	public Vector3 objectUnderCursorTranslation;
	public static GameObject objectUnderCursor;
	public Vector3 originalOjectLocalScale;
	public static Dictionary <GameObject, Quaternion> initialRotations;
	public static Dictionary <GameObject, Vector3> initialLocalScales;
	public static Dictionary <GameObject, Bounds> initialBounds;

	public TwoHandedCursorManager(TwoHandedGesturesManager _handsManager) {
		handsManager = _handsManager;

		Debug.Log ("Starting my Cursor Manager");
		boundingCubes = new Dictionary<GameObject, GameObject> ();
		boundingCubesInverse = new Dictionary<GameObject, GameObject> ();
		extents = new Dictionary<GameObject, float>();
		initialRotations = new Dictionary<GameObject, Quaternion> ();
		initialBounds = new Dictionary<GameObject, Bounds> ();
		initialLocalScales = new Dictionary<GameObject, Vector3> ();
		AddBoundingCubes ();
	}

	public static void SetBoundingCubeSize(GameObject g, GameObject boundingCube) {
		Bounds b = g.GetComponent<Renderer>().bounds;
		Vector3 boundingSize;

		boundingCube.transform.position = b.center;
		boundingSize = b.size;
		boundingSize *= BOUNDING_BOX_SCALE_FACTOR;
		boundingCube.transform.localScale = boundingSize;
		boundingCube.GetComponent<Renderer> ().enabled = false;
		boundingCube.GetComponent<BoxCollider> ().enabled = false;
	}

	void AddBoundingCubes() {
		Transform gameObjects = GameObject.Find ("/HologramCollection").transform;

		foreach (Transform child in gameObjects) {
			GameObject g = child.gameObject;
			GameObject boundingCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.DontDestroyOnLoad (boundingCube);
			Material material;
			material = new Material (handsManager.debuggingMaterial);
			extents.Add(g, g.GetComponent<Renderer>().bounds.extents.magnitude * RADIUS_MULTIPLIER);

			boundingCube.GetComponent<Renderer> ().material = material;
			boundingCube.GetComponent<Renderer> ().enabled = false;

			if (!g.activeSelf) {
				boundingCube.SetActive (false);
			}

			SetBoundingCubeSize (g, boundingCube);

			boundingCubes.Add (boundingCube, g);
			boundingCubesInverse.Add (g, boundingCube);


			Bounds b = g.GetComponent<Renderer> ().bounds;
			Vector3 newSize = b.size;
			newSize.y += .005f;
			b.size = newSize;
			initialRotations.Add (g, g.transform.rotation);
			initialLocalScales.Add (g, g.transform.localScale);
			initialBounds.Add (g, b);
		}
	}

	void moveCursors(Vector3 point) {
		handsManager.normalCursor.transform.position = point;
		handsManager.resizeCursor.transform.position = point;
		handsManager.yawCursor.transform.position = point;
		handsManager.pitchCursor.transform.position = point;
		handsManager.rollCursor.transform.position = point;
	}

	void rotateCursors(Quaternion rotation) {
		//handsManager.normalCursor.transform.rotation = rotation;
		handsManager.resizeCursor.transform.rotation = rotation;
		//handsManager.yawCursor.transform.rotation = rotation;
		//handsManager.pitchCursor.transform.rotation = rotation;
		//handsManager.rollCursor.transform.rotation = rotation;
	}

	// Update is called once per frame
	internal void Update()
	{
		// Do a raycast into the world based on the user's
		// head position and orientation.
		var headPosition = Camera.main.transform.position;
		var gazeDirection = Camera.main.transform.forward;
		Vector3 point;
		bool hit = false;
		bool getDistance = true;
		RaycastHit hitInfo;
		oldCube = cube;
		Vector3 normal = Camera.main.transform.position.normalized;

		if (!TwoHandedGesturesManager.resizing && !TwoHandedGesturesManager.rotating) {

			if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, MaxGazeDistance))
			{
				hit = true;
			}


			onHologram = true;

			if (hit) {
				normal = hitInfo.normal;
				point = hitInfo.point;
				cube = hitInfo.transform.gameObject;
				rotateCursors(Quaternion.FromToRotation(Vector3.forward, normal));
				//if we hit an object
				if (boundingCubesInverse.ContainsKey (cube)) {
					hologram = cube;
					boundingCubesInverse.TryGetValue (cube, out cube);
					getDistance = false;
				} else if (boundingCubes.TryGetValue (cube, out hologram)) {
					getDistance = false;
				} else {
					cube = null;
					hologram = null;
				}
			} else {
				point = headPosition + (gazeDirection * MaxGazeDistance);
				cube = null;
				hologram = null;
			}



			if (TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5) {

				moveCursors(point);
				GameObject boxCube, sphere;
				bool hitCube = false;
				bool hitSphere = false;

				float distanceToPoint = .5f;
					for (int i = 0; i < 8; i++) {
						boxCube = BoundBoxes_BoundBox.cubes [i];
						sphere = BoundBoxes_BoundBox.spheres [i];

						// we hit a cube
						if (hit && ((hitInfo.transform.gameObject == boxCube) || Vector3.Distance(point,  boxCube.transform.position) <= BoundBoxes_BoundBox.currentCubeSize * distanceToPoint)) {
							objectUnderCursorTranslation = point - boxCube.transform.position;
							originalOjectLocalScale = boxCube.transform.localScale;
							objectUnderCursor = boxCube;
							handsManager.ColorObject (boxCube, highlightedCubeColor);
							onCube = true;
							hitCube = true;
							handsManager.resizeCursor.SetActive(true);
							handsManager.normalCursor.SetActive(false);

							handsManager.yawCursor.SetActive(false);
							handsManager.pitchCursor.SetActive(false);
							handsManager.rollCursor.SetActive(false);
						} else {
							handsManager.ColorObject (boxCube, regularCubeColor);
						}

						// we hit a sphere
						if (hit && ((hitInfo.transform.gameObject == sphere) || Vector3.Distance(point,  sphere.transform.position) <= BoundBoxes_BoundBox.currentCubeSize * distanceToPoint)) {
							objectUnderCursorTranslation = point - sphere.transform.position;
							originalOjectLocalScale = sphere.transform.localScale;
							objectUnderCursor = sphere;
							handsManager.ColorObject (sphere, highlightedCubeColor);
							onSphere = true;
							sphereIndexUnderCursor = i;
							hitSphere = true;
							if(sphereIndexUnderCursor < 4) {
									handsManager.yawCursor.SetActive(true);
									handsManager.pitchCursor.SetActive(false);
									handsManager.rollCursor.SetActive(false);
							} else if(sphereIndexUnderCursor < 6) {
								handsManager.pitchCursor.SetActive(true);
								handsManager.rollCursor.SetActive(false);
								handsManager.yawCursor.SetActive(false);
							} else {
								handsManager.rollCursor.SetActive(true);
								handsManager.yawCursor.SetActive(false);
								handsManager.pitchCursor.SetActive(false);
							}

							handsManager.normalCursor.SetActive(false);
							handsManager.resizeCursor.SetActive(false);
						} else {
							handsManager.ColorObject (sphere, regularCubeColor);
						}
					}
					if (!hitCube)
						onCube = false;
					if (!hitSphere)
						onSphere = false;

					if(!hitCube && !hitSphere) {
						handsManager.yawCursor.SetActive(false);
						handsManager.pitchCursor.SetActive(false);
						handsManager.rollCursor.SetActive(false);

						handsManager.resizeCursor.SetActive(false);
						handsManager.normalCursor.SetActive(true);
					}
			} else {
				GameObject menu = GameObject.Find ("Menu");
				float dist = Vector3.Distance (point, handsManager.menu.transform.position);

				if ((menu && hit && hitInfo.transform.IsChildOf(menu.transform)) ||  (dist < 0.75f)) {
					moveCursors (point);
					handsManager.normalCursor.SetActive (true);
				} else {
					handsManager.normalCursor.SetActive (false);
					//moveCursors (headPosition + (gazeDirection * MaxGazeDistance));
				}
				handsManager.resizeCursor.SetActive (false);
				handsManager.yawCursor.SetActive (false);
				handsManager.pitchCursor.SetActive (false);
				handsManager.rollCursor.SetActive (false);
			}

			if (getDistance) {
				//find closest object
				float smallest_distance = 100000;
				float distance;
				GameObject closest_cube = cube;

				foreach (KeyValuePair<GameObject, GameObject> entry in boundingCubes) {
					distance = Vector3.Distance (entry.Key.transform.position, point);
					if ((distance < smallest_distance) && entry.Value.activeSelf) {
						smallest_distance = distance;
						closest_cube = entry.Key;
					}
				}
				//if(smallest_distance < min_distance) {
				if (closest_cube != null) {
					cube = closest_cube;
					boundingCubes.TryGetValue (cube, out hologram);
				} else {
					onHologram = false;
				}
			}

			//if we change targets
			if (hologram != lastHologram) {
				BoundBoxes_BoundBox.initialized = false;
			}
			lastHologram = hologram;

			position = point;

			if (SHOW_BOUNDING_BOX) {
				if(oldCube != null && oldCube != cube)
					oldCube.GetComponent<Renderer> ().enabled = false;
				if (onHologram) {
					cube.GetComponent<Renderer> ().enabled = true;
				}
			} else {
				if(onHologram)
					cube.GetComponent<Renderer>().enabled = false;
			}
		} else {
			if((TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5)) {
				moveCursors(objectUnderCursor.transform.position + objectUnderCursorTranslation * objectUnderCursor.transform.localScale.x / originalOjectLocalScale.x);
			}
		}

	}
}

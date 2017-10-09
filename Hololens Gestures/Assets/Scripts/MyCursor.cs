using UnityEngine;
using System.Collections.Generic;

public class MyCursor : MonoBehaviour
{
	private MeshRenderer meshRenderer;
	public float MaxGazeDistance = 4.0f;
	public static MyCursor Instance;
	public bool onHologram;
	public GameObject hologram = null;
	public GameObject cube = null;
	public Dictionary <GameObject, GameObject> boundingCubes;
	public Dictionary <GameObject, GameObject> boundingCubesInverse;
	private static TwoHandedGesturesManager handsManager = null;
	private GameObject oldCube = null;
	public Material debuggingMaterial;

	bool SHOW_BOUNDING_BOX = true;

	public MyCursor(TwoHandedGesturesManager _handsManager) {
		handsManager = _handsManager;
	}

	void AddBoundingCubes() {
		Transform gameObjects = GameObject.Find ("/ManipulatableObjects").transform;

		foreach (Transform child in gameObjects) {
			GameObject g = child.gameObject;
			GameObject boundingCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Material material;
			material = new Material (debuggingMaterial);

			boundingCube.GetComponent<Renderer> ().material = material;
			boundingCube.GetComponent<Renderer> ().enabled = false;

			boundingCube.transform.position = g.transform.position;
			boundingCube.transform.localScale = g.transform.localScale * 1.1f;

			boundingCubes.Add (boundingCube, g);
			boundingCubesInverse.Add (g, boundingCube);
		}
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		print ("Starting my Cursor Manager");
		boundingCubes = new Dictionary<GameObject, GameObject> ();
		boundingCubesInverse = new Dictionary<GameObject, GameObject> ();
		AddBoundingCubes ();

		// Grab the mesh renderer that's on the same object as this script.
		meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
		meshRenderer.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		// Do a raycast into the world based on the user's
		// head position and orientation.
		var headPosition = Camera.main.transform.position;
		var gazeDirection = Camera.main.transform.forward;
		Vector3 point;
		bool hit = false;
		bool getDistance = true;
		float min_distance = 4.0f;
		RaycastHit hitInfo;
		oldCube = cube;
		Vector3 normal = Camera.main.transform.position.normalized;

		if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
		{
			hit = true;
			normal = hitInfo.normal;
			point = hitInfo.point;
			cube = hitInfo.transform.gameObject;
		}
		else
		{
			point = headPosition + (gazeDirection * MaxGazeDistance);
			this.transform.position = headPosition + (gazeDirection * MaxGazeDistance);
			this.transform.rotation = Quaternion.FromToRotation(Vector3.up, gazeDirection);
			cube = null;
		}

		onHologram = true;


		if (hit) {
			//if we hit an object
			if (boundingCubesInverse.ContainsKey (cube)) {
				hologram = cube;
				boundingCubesInverse.TryGetValue (cube, out cube);
				getDistance = false;
			} else if (boundingCubes.TryGetValue (cube, out hologram)) {
				getDistance = false;
			}
		}

		if (getDistance) {
			//find closest object
			float smallest_distance = 100000;
			float distance;
			GameObject closest_cube = cube;

			foreach (KeyValuePair<GameObject, GameObject> entry in boundingCubes) {
				distance = Vector3.Distance (entry.Key.transform.position, point);
				if (distance < smallest_distance) {
					smallest_distance = distance;
					closest_cube = entry.Key;
				}
			}
			if(smallest_distance < min_distance) {
				cube = closest_cube;
				boundingCubes.TryGetValue (cube, out hologram);
			} else {
				onHologram = false;
			}
		}

		// Move the cursor to the point where the raycast hit.
		this.transform.position = point;

		// Rotate the cursor to hug the surface of the hologram.
		this.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);


		if(oldCube != null && oldCube != cube)
			oldCube.GetComponent<Renderer> ().enabled = false;
		if (onHologram && SHOW_BOUNDING_BOX) {
			cube.GetComponent<Renderer> ().enabled = true;
		}
	}
}

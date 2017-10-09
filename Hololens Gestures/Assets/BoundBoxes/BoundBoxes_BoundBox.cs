using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BoundBoxes_BoundBox : MonoBehaviour {

	public bool colliderBased = false;
	public bool permanent = false; //permanent//onMouseDown

	public Color lineColor = new Color(0f,1f, 0.4f,0.74f);
	public static bool drawLines = true;

	private Bounds bound;

	private Vector3[] corners;

	private Vector3[,] lines;

	private Quaternion quat;

	public static float currentCubeSize = 0.0f;
	public static float currentSphereRadius = 0.0f;

	private Camera mcamera;

	private BoundBoxes_drawLines cameralines;

	private Dictionary <GameObject, Renderer[]> renderers;
	private Dictionary <GameObject, MeshFilter[]> meshes;

	private Material[][] Materials;

	private Vector3 topFrontLeft;
	private Vector3 topFrontRight;
	private Vector3 topBackLeft;
	private Vector3 topBackRight;
	private Vector3 bottomFrontLeft;
	private Vector3 bottomFrontRight;
	private Vector3 bottomBackLeft;
	private Vector3 bottomBackRight;
	public static bool initialized = false;
	public static GameObject[] cubes;
	public static GameObject[] spheres;
	public Material mat;
	public GameObject colliderCube;

	float BOX_SCALE = 1.5f;
	float COLLIDER_SCALE = 1.5f;
	float SMALL_OBJECT_SCALE = .12f;
	float SMALL_OBJECT_COLLIDER_SCALE = 2.1f;

	void Awake () {
		renderers = new Dictionary<GameObject, Renderer[]> ();
		meshes = new Dictionary<GameObject, MeshFilter[]> ();

		Transform gameObjects = GameObject.Find ("/HologramCollection").transform;
		foreach (Transform child in gameObjects) {
			GameObject g = child.gameObject;
			renderers.Add(g, g.GetComponentsInChildren<Renderer>());
			meshes.Add(g, g.GetComponentsInChildren<MeshFilter>());



		}

		cubes = new GameObject[8];
		spheres = new GameObject[8];
		colliderCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		for (int i = 0; i < 8; i++) {
			cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Material material = new Material (mat);
			cubes[i].GetComponent<Renderer> ().material = material;
			cubes[i].SetActive (false);
			cubes [i].GetComponent<BoxCollider> ().size *= SMALL_OBJECT_COLLIDER_SCALE;

			spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			material = new Material (mat);
			spheres[i].GetComponent<Renderer> ().material = material;
			spheres[i].SetActive (false);
			spheres [i].GetComponent<SphereCollider> ().radius *= SMALL_OBJECT_COLLIDER_SCALE;
		}

		/*
		Materials = new Material[renderers.Length][];
		for(int i = 0; i < renderers.Length; i++) {
			Materials[i]= renderers[i].materials;
		}
		*/
	}

	void Start () {
		mcamera = Camera.main;
		cameralines = mcamera.GetComponent<BoundBoxes_drawLines>();
		cameralines.boxManager = this;
		init();
	}

	public void init() {
		if (TwoHandedGesturesManager.focusedForBox && (TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5) && renderers.ContainsKey(TwoHandedGesturesManager.focusedForBox)) {
			initialized = true;
			calculateBounds ();
			setPoints ();
			setLines ();
			cameralines.setOutlines (lines, lineColor);
		}
	}

	void Update() {
		if (TwoHandedGesturesManager.TECHNIQUE_SELECTED == TwoHandedGesturesManager.TECHNIQUE_5 && TwoHandedGesturesManager.showWireBox) {
			colliderCube.SetActive (true);
			if ((TwoHandedGesturesManager.resizing || TwoHandedGesturesManager.rotating)) {
				init ();
			}

			if (TwoHandedGesturesManager.focusedForBox) {
				if (!initialized)
					init ();
				drawLines = true;
				cameralines.setOutlines (lines, lineColor);
			} else {
				drawLines = false;
				for (int i = 0; i < 8; i++) {
					cubes [i].SetActive (false);
					spheres [i].SetActive (false);
				}
				colliderCube.GetComponent<Renderer> ().enabled = false;
			}
		} else {
			drawLines = false;
			for (int i = 0; i < 8; i++) {
				cubes [i].SetActive (false);
				spheres [i].SetActive (false);
			}
			colliderCube.SetActive (false);
		}
	}

	void calculateBounds() {
		if (!TwoHandedGesturesManager.focusedForBox)
			return;
			quat = TwoHandedGesturesManager.focusedForBox.transform.rotation;//object axis AABB
			if(renderers[TwoHandedGesturesManager.focusedForBox][0].isPartOfStaticBatch) quat = Quaternion.Euler(0f,0f,0f);//world axis

			if(colliderBased){
				BoxCollider coll = GetComponent<BoxCollider>();
				if(coll){
					GameObject co = new GameObject("dummy");
					co.transform.position = TwoHandedGesturesManager.focusedForBox.transform.position;
					co.transform.localScale = TwoHandedGesturesManager.focusedForBox.transform.lossyScale;
					BoxCollider cobc = co.AddComponent<BoxCollider>();
					quat = TwoHandedGesturesManager.focusedForBox.transform.rotation;
					cobc.center = coll.center;
					cobc.size = coll.size;
					bound = cobc.bounds;
					Destroy(co);
				}else{
					Debug.Log("No collider attached");
				}
				return;
			}
			bound = new Bounds();
			if(renderers[TwoHandedGesturesManager.focusedForBox][0].isPartOfStaticBatch){
				bound = renderers[TwoHandedGesturesManager.focusedForBox][0].bounds;
				for(int i = 1; i < renderers[TwoHandedGesturesManager.focusedForBox].Length; i++) {
					bound.Encapsulate(renderers[TwoHandedGesturesManager.focusedForBox][i].bounds);
				}
				return;
			}
			TwoHandedGesturesManager.focusedForBox.transform.rotation = Quaternion.Euler(0f,0f,0f);
			for(int i = 0; i < meshes[TwoHandedGesturesManager.focusedForBox].Length; i++) {
				Mesh ms = meshes[TwoHandedGesturesManager.focusedForBox][i].mesh;
				Vector3 tr = meshes[TwoHandedGesturesManager.focusedForBox][i].gameObject.transform.position;
				Vector3 ls = meshes[TwoHandedGesturesManager.focusedForBox][i].gameObject.transform.lossyScale * BOX_SCALE;
				Quaternion lr = meshes[TwoHandedGesturesManager.focusedForBox][i].gameObject.transform.rotation;
				int vc = ms.vertexCount;
				for(int j = 0; j < vc; j++) {
					if(i==0&&j==0){
						bound = new Bounds(tr + lr*Vector3.Scale(ls,ms.vertices[j]), Vector3.zero);
					}else{
						bound.Encapsulate(tr + lr*Vector3.Scale(ls,ms.vertices[j]));
					}
				}
			}
			TwoHandedGesturesManager.focusedForBox.transform.rotation = quat;
	}

	void setPoints() {
		if (!TwoHandedGesturesManager.focusedForBox)
			return;
		Vector3 bc = TwoHandedGesturesManager.focusedForBox.transform.position + quat *(bound.center - TwoHandedGesturesManager.focusedForBox.transform.position);

		topFrontRight = bc +  quat *Vector3.Scale(bound.extents, new Vector3(1, 1, 1));
		topFrontLeft = bc +  quat *Vector3.Scale(bound.extents, new Vector3(-1, 1, 1));
		topBackLeft = bc +  quat *Vector3.Scale(bound.extents, new Vector3(-1, 1, -1));
		topBackRight = bc +  quat *Vector3.Scale(bound.extents, new Vector3(1, 1, -1));
		bottomFrontRight = bc +  quat *Vector3.Scale(bound.extents, new Vector3(1, -1, 1));
		bottomFrontLeft = bc +  quat *Vector3.Scale(bound.extents, new Vector3(-1, -1, 1));
		bottomBackLeft = bc +  quat *Vector3.Scale(bound.extents, new Vector3(-1, -1, -1));
		bottomBackRight = bc +  quat *Vector3.Scale(bound.extents, new Vector3(1, -1, -1));
		corners = new Vector3[]{topFrontRight,topFrontLeft,topBackLeft,topBackRight,bottomFrontRight,bottomFrontLeft,bottomBackLeft,bottomBackRight};

		Vector3 size = TwoHandedGesturesManager.focusedForBox.GetComponent<Renderer> ().bounds.size;
		Bounds b = TwoHandedGesturesManager.focusedForBox.GetComponent<Renderer> ().bounds;
		Transform t = TwoHandedGesturesManager.focusedForBox.transform;

		float scale = t.localScale.x / TwoHandedCursorManager.initialLocalScales [TwoHandedGesturesManager.focusedForBox].x * COLLIDER_SCALE;
		Vector3 mid1, mid2;

		mid1 = (corners[5] + corners[7]) / 2.0f; mid2 = (corners[1] + corners[3]) / 2.0f;
		colliderCube.transform.position = (mid1 + mid2) / 2.0f;
		colliderCube.transform.localScale = TwoHandedCursorManager.initialBounds[TwoHandedGesturesManager.focusedForBox].size * scale;
		colliderCube.transform.rotation = t.rotation * Quaternion.Inverse(TwoHandedCursorManager.initialRotations[TwoHandedGesturesManager.focusedForBox]);
		colliderCube.SetActive (true);
		colliderCube.GetComponent<Renderer> ().enabled = false;

		Vector3 localScaleSize = colliderCube.transform.localScale;

		float x = localScaleSize.x, y = localScaleSize.y, z = localScaleSize.z;
		float maxSide = Mathf.Max (x, Mathf.Max (y, z)) * SMALL_OBJECT_SCALE;
		currentCubeSize = maxSide;
		currentSphereRadius = maxSide;


		int i;

		for (i = 0; i < 8; i++) {
			setCube (i, maxSide, i, b, t);
		}

		//yaw 0 - 3
		for (i = 0; i < 4; i++) {
			setSphere (i, maxSide, i, i + 4);
		}

		setSphere (4, maxSide, 0, 3);
		setSphere (5, maxSide, 1, 2);
		setSphere (6, maxSide, 4, 7);
		setSphere (7, maxSide, 5, 6);





	}

	void setCube (int index, float maxSide, int i, Bounds b, Transform t) {
		cubes [i].transform.localScale =  new Vector3(maxSide, maxSide, maxSide);
		Vector3 smallBoxSize = cubes [i].transform.localScale;
		Vector3 pos = corners [i];
		pos = Vector3.MoveTowards (pos, b.center, smallBoxSize.x / 2.0f);
		cubes [i].transform.position = pos;
		if (TwoHandedGesturesManager.rotating || TwoHandedGesturesManager.resizing) {
			if (cubes [i] == TwoHandedCursorManager.objectUnderCursor) {
				cubes [i].SetActive (true);
			} else {
				cubes [i].SetActive (false);
			}
		} else {
			cubes [i].SetActive (true);
		}
		cubes [i].transform.rotation = t.rotation;
	}

	void setSphere (int index, float maxSide, int begin, int end) {
		// pitch
		spheres [index].transform.localScale =  new Vector3(maxSide, maxSide, maxSide);
		Vector3 smallSphereSize = spheres [index].GetComponent<Renderer> ().bounds.size;

		Vector3 pos = (corners [begin] + corners [end]) / 2.0f;

		spheres [index].transform.position = pos;
		if (TwoHandedGesturesManager.rotating || TwoHandedGesturesManager.resizing) {
			if (spheres [index] == TwoHandedCursorManager.objectUnderCursor) {
				spheres [index].SetActive (true);
			} else {
				spheres [index].SetActive (false);
			}
		} else {
			spheres [index].SetActive (true);
		}

	}

	void setLines() {

		int i1;
		int linesCount = 12;

		lines = new Vector3[linesCount,2];
		for (int i=0; i<4; i++) {
			i1 = (i+1)%4;//top rectangle
			lines[i,0] = corners[i];
			lines[i,1] = corners[i1];
			//break;
			i1 = i + 4;//vertical lines
			lines[i+4,0] = corners[i];
			lines[i+4,1] = corners[i1];
			//bottom rectangle
			lines[i+8,0] = corners[i1];
			i1 = 4 + (i+1)%4;
			lines[i+8,1] = corners[i1];
		}
	}

	void OnMouseDown() {
		if(permanent) return;
		enabled = !enabled;
	}

}

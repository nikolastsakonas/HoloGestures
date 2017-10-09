using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BoundBoxes_drawLines : MonoBehaviour {
	public Material lineMaterial;
	public Color lColor;
	public List<Vector3[,]> outlines;
	public List<Color> colors;
	public BoundBoxes_BoundBox boxManager;
	// Use this for initialization

	void Awake () {
		outlines = new List<Vector3[,]>();
		colors = new List<Color>();
		CreateLineMaterial();
	}
	
	void Start () {
//		outlines = new List<Vector3[,]>();
//		colors = new List<Color>();
//		CreateLineMaterial();
	}

	void CreateLineMaterial()
{
    if( !lineMaterial ) {

        lineMaterial = new Material(Shader.Find("Custom/GizmoShader"));

        //lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        //lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
    }
}

	void OnPostRender() {
		if(outlines==null) return;
		if (boxManager && BoundBoxes_BoundBox.drawLines) {
			CreateLineMaterial ();
			lineMaterial.SetPass (0);
			GL.Begin (GL.LINES);
			for (int j = 0; j < outlines.Count; j++) {
				GL.Color (colors [j]);
				for (int i = 0; i < outlines [j].GetLength (0); i++) {
					GL.Vertex (outlines [j] [i, 0]);
					GL.Vertex (outlines [j] [i, 1]);
				}
			}
			GL.End ();
		}
	}
		
	public void setOutlines(Vector3[,] newOutlines, Color newcolor) {
		if(newOutlines.GetLength(0)>0)	{
			outlines.Add(newOutlines);
			//Debug.Log ("no "+newOutlines.GetLength(0).ToString());
			colors.Add(newcolor);
		}
	}	
	
	// Update is called once per frame
	void Update () {
		outlines = new List<Vector3[,]>();
		colors = new List<Color>();
	}
}

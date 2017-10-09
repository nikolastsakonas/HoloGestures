using UnityEngine;
using HoloToolkit.Unity;

using System.Text;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.SceneManagement;
public class reset : Singleton<reset> {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void resetGameObjects()
	{
		SceneManager.LoadScene(0);
	}
}

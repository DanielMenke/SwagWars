using UnityEngine;
using System.Collections;

namespace Completed
{

public class StartGameScript : MonoBehaviour {

		//public GameManager manager = null;
		public GameObject gameManager;


	// Use this for initialization
	void Start () {
			Instantiate (gameManager);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartGame(){
			//GameManager.Destroy ();
			//gameManager.GetComponent<GameManager> ().gameOver = true;
			gameManager.GetComponent<GameManager> ().restart ();
			gameManager.GetComponent<GameManager> ().level = 0;
		Application.LoadLevel("Main");
	}

	public void ShowHighScores(){
		Application.LoadLevel ("HighScores");
	}

	public void ShowMenu(){
			//PlayerPrefs.SetString ("Restart", "YES");
		Application.LoadLevel ("Menu");
	}

	public void ShowInstructions(){
		Application.LoadLevel ("Instructions");
	}
}

}

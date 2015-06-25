using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;
using UnityEngine.UI;

public class HighScoreSetter : MonoBehaviour {

	public GameObject first;
	public GameObject second;
	public GameObject third;

	// Use this for initialization
	void Start () {
		First (first);
		Second (second);
		Third (third);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void setHighscoreItem(GameObject textObject, string fileName){
		string scoreName;
		int score;
		
		if (File.Exists (fileName)) {
			var document = File.OpenText (fileName);
			var line = document.ReadLine ();
			scoreName = nameFromLine(line);
			score = scoreFromLine(line);
			document.Close();
		} else {
			scoreName = "...";
			score = 0;
			Debug.Log ("Could not Open the highscore-file for reading.");
		}
		string lineText = stringForHighscoreText(scoreName, score);
		textObject.GetComponent<Text> ().text = lineText;

	}

	void First(GameObject first){
		string firstFile = "first.txt";
		setHighscoreItem (first, firstFile);
	}

	void Second(GameObject second){
		string secondFile = "second.txt";
		setHighscoreItem (second, secondFile);
	}

	void Third(GameObject third){
		string thirdFile = "third.txt";
		setHighscoreItem (third, thirdFile);
	}


	/*
	 * if (File.Exists (firstFile)) {
				var document = File.OpenText (firstFile);
				var line = document.ReadLine ();
				firstName = nameFromLine(line);
				firstScore = scoreFromLine(line);
				document.Close();
			} else {
				firstName = "...";
				firstScore = 0;
				Debug.Log ("Could not Open the highscore-file for reading.");
				return;
			}

			if (File.Exists (secondFile)) {
				var document = File.OpenText (secondFile);
				var line = document.ReadLine ();
				secondName = nameFromLine(line);
				secondScore = scoreFromLine(line);
				document.Close();
			} else {
				secondName = "...";
				secondScore = 0;
				Debug.Log ("Could not Open the highscore-file for reading.");
				return;
			}

			if (File.Exists (thirdFile)) {
				var document = File.OpenText (thirdFile);
				var line = document.ReadLine ();
				thirdName = nameFromLine(line);
				thirdScore = scoreFromLine(line);
				document.Close();
			} else {
				thirdName = "...";
				thirdScore = 0;
				Debug.Log ("Could not Open the highscore-file for reading.");
				return;
			}*/

	private string stringForHighscoreText(string name, int score){
		string scoreText = stringFromIntWithLeadingZeros(score);
		return scoreText + " " + name;
	}
	
	private string stringFromIntWithLeadingZeros(int score){
		if (score < 10){
			return "00" + score.ToString();
		}
		if (score < 100){
			return "0" + score.ToString();
		}
		return score.ToString ();
	}

	private string nameFromLine(string line){
		string[] split = line.Split ('-');
		return split.ElementAt (0);
	}

	private int scoreFromLine(string line){
		string[] split = line.Split ('-');
		//Debug.Log ("splitlength: " + split.Count() + "line: " + line + "element " + split.ElementAt(split.Count() -1 ) + ". essdft" + split.ElementAt(0));
		return int.Parse (split.ElementAt(split.Count() -1 ).ToString());
	}
}

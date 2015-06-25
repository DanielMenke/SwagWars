using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
		public float turnDelay = 0.1f;							//Delay between each Player turn.
		public int playerFoodPoints = 100;						//Starting value for Player food points.
		public int playerEnemyDamage = 1;
		public int money = 0;
		private int itemPrice;
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		public static GameManager secondInstance = null;
		[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
		public bool[] itemsToDisplay;
		
		private Text levelText;									//Text to display current level number.
		private Text itemCaption;
		private Text priceText;
		private Text itemInfoText;
		private Text moneyText;
		private Text damageText;
		private Text foodText;
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
		private GameObject shopImage;
		private Player player;


		//Shop Items & Buttons

		public Button payButton;
		public GameObject payObject;
		public Button stonesButton;
		public Button brokenMateButton;
		public Button glassMateButton;
		public Button halfMateButton;
		public Button fullMateButton;

		private GameObject infoPanel;
		private GameObject stones;
		private GameObject brokenMate;


		private GameObject triangleOfDoom;
		public Button triangleOfDoomButton;

		private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
		public int level = 1;									//Current level number, expressed in game as "Day 1".
		private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
		private bool enemiesMoving;								//Boolean to check if enemies are moving.
		private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.
		private bool shopping;

		//Sceneswitchign
		//public GameObject sceneSwitcher;

		//Highscore
		public int currentScore = 0;
		
		private GameObject first;
		private GameObject second;
		private GameObject third;
		private GameObject nameInput;
		
		private int firstScore;
		private int secondScore;
		private int thirdScore;
		
		private string firstName;
		private string secondName;
		private string thirdName;


		public bool gameOver;

		public bool waitsForRestart = false;
		
		//Awake is always called before any Start functions
		public void Awake()
		{
			//Check if instance already exists
			if (instance == null) {
					//if not, set instance to this
					instance = this;
			}
			
			//If instance already exists and it's not this:
			else if (instance != this)
				
				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy(gameObject);	
			
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);


			//Assign enemies to a new List of Enemy objects.
			enemies = new List<Enemy>();
			itemsToDisplay = new bool[9];
			
			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();
			
			//Call the InitGame function to initialize the first level 
			InitGame();
		}

		public void restart(){
			waitsForRestart = false;
			Application.LoadLevel (Application.loadedLevel);
		}
		
		//This is called each time a scene is loaded.
		void OnLevelWasLoaded(int index)
		{
			//Add one to our level number.
			if (!gameOver){
				level++;
			}else{
				level = 1;
				playerEnemyDamage = 1;
				money = 0;
				playerFoodPoints = 100;
				for (int i = 0; i<itemsToDisplay.Length; i++){
					itemsToDisplay[i]=false;
				}
				SoundManager.instance.musicSource.Play();
			}
			//Debug.Log (level);
			//Call InitGame to initialize our level.
			InitGame();
		}
		
		//Initializes the game for each level.
		void InitGame()
		{
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;
			gameOver = false;
			StartCoroutine(ShopSequence ());
			//Clear any Enemy objects in our List to prepare for next level.
			enemies.Clear();
			
			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);

			doingSetup = false;
		}
		
		
		//Hides black image used between levels
		public void HideLevelImage()
		{
			if (levelImage == null) {
			levelImage = GameObject.Find ("LevelImage");
			}
			//Disable the levelImage gameObject.
			levelImage.SetActive (false);
			//Debug.Log ("levelImage hidden");
			doingSetup = false;
			//Set doingSetup to false allowing player to move again.
		}

		public void setPrice (int price){
			priceText = GameObject.Find ("PriceText").GetComponent<Text> ();
			priceText.text = price.ToString ()+" $";
			itemPrice = price;

		}
		public void setItemInfoText (string text){
			itemInfoText = GameObject.Find ("ItemInfoText").GetComponent<Text> ();
			itemInfoText.text = text;
		}
		public void setItemCaption (string text){
			itemCaption = GameObject.Find ("ItemCaption").GetComponent<Text> ();
			itemCaption.text = text;
		}
	
		public void  openInfoPanel (GameObject iPanel,GameObject button, int itemIndex){

			payButton.onClick.RemoveAllListeners ();
			iPanel.SetActive (true);
			//Debug.Log ("index: " + itemIndex);
			Button theButton = button.GetComponentInChildren<Button> ();
			Text buttonText = theButton.GetComponentInChildren<Text> ();	
		
		
			switch (itemIndex) {
				case 0:
					setPrice (10);
					setItemCaption("Stonepile");
					setItemInfoText ("bag upgrade:\n\nStones for your foes\n\n +1 damage");
					break;
				case 1:
					setPrice (30);
					setItemCaption("Broken Mate");
					setItemInfoText ("bag upgrade:\n\nRecycling is great!\n\n +2 damage");
					break;
				case 2:
					setPrice (100);
					setItemCaption("Triangle of Doom");
					setItemInfoText ("bag upgrade:\n\nMetal!\nTriangle!\nSwag!\n\n +6 damage");
					break;
				case 3:
					setPrice(3);
					setItemCaption("A second hand\nBottle of Mate");
					setItemInfoText("Restores 5 Mate");
					break;
				case 4:
					setPrice (5);
					setItemCaption("A fresh\nBottle of Mate");
					setItemInfoText("Restores 10 Mate");
					break;
				case 5:
					setPrice (10);
					setItemCaption("Mate\nAmerican Size");
					setItemInfoText("Restores 20 Mate");
					break;
			}
			if (money < itemPrice) {
				buttonText.color = Color.red;
				payButton.interactable = false;
				
			} else {
				buttonText.color = Color.black;
				payButton.interactable = true;
			}
			//Debug.Log ("price " + itemPrice);
			//Debug.Log ("$ " +player.money);
			payButton.onClick.AddListener (() => 
			                               {
				player.addItem (itemIndex);
				itemsToDisplay[itemIndex] = true;
				money -= itemPrice;
				player.money = money;
				//Debug.Log(money + "$");
				infoPanel.SetActive (false);
				if (itemsToDisplay [0]) {
					stonesButton.interactable = false;
				}
				if (itemsToDisplay [1]) {
					brokenMateButton.interactable = false;
				}
				if (itemsToDisplay [2]) {
					triangleOfDoomButton.interactable = false;
				}
				if (itemsToDisplay [3]) {
					glassMateButton.interactable = false;
				}
				if (itemsToDisplay [4]) {
					halfMateButton.interactable = false;
				}
				if (itemsToDisplay [5]) {
					fullMateButton.interactable = false;
				}

				switch (itemIndex) {
					case 0:
						playerEnemyDamage +=1;
						player.enemyDamage = playerEnemyDamage;
						break;
					case 1:
						playerEnemyDamage +=2;
						player.enemyDamage = playerEnemyDamage;
						break;
					case 2:
						playerEnemyDamage += 6;
						player.enemyDamage = playerEnemyDamage;
						break;
					case 3:
						playerFoodPoints +=5;
						//Debug.Log ("Food " + playerFoodPoints);
						player.food = playerFoodPoints;
						//Debug.Log ("Food PlayerObj. "+ player.food);
						break;
					case 4:
						playerFoodPoints +=10;
						player.food = playerFoodPoints;
						//Debug.Log ("Food PlayerObj. "+ player.food);
						break;
					case 5:
						playerFoodPoints +=20;
						player.food = playerFoodPoints;
						//Debug.Log ("Food PlayerObj. "+ player.food);
						break;
				}
				moneyText.text = "$: " + money;
				damageText.text = "dmg: " + playerEnemyDamage;
				foodText.text = "Mate: " + playerFoodPoints;
			});

		}
		public IEnumerator ShopSequence(){

			shopping = true;
			shopImage = GameObject.Find ("Shop");
			infoPanel = GameObject.Find ("Infopanel");
			payObject = GameObject.Find ("PayButton");
			payButton = payObject.GetComponent<Button>();
			player = GameObject.Find ("Player").GetComponent<Player>();

			moneyText = GameObject.Find ("MoneyText").GetComponent<Text> ();
			foodText = GameObject.Find ("ShopFoodText").GetComponent<Text> ();
			damageText = GameObject.Find ("DamageText").GetComponent<Text> ();

			moneyText.text = "$: " + money;
			damageText.text = "dmg: " + playerEnemyDamage;
			foodText.text = "Mate: " + playerFoodPoints;

			for (int i = 3; i <=5; i++) { 
				itemsToDisplay[i] = false; // Reset the reusable food items
			}
			//Stones
			stones = GameObject.Find ("Stones");
			stonesButton = stones.GetComponent<Button> ();
			stonesButton.onClick.AddListener(()=> 
			{ 
				openInfoPanel(infoPanel,payObject, 0);
			});
	
			//BrokenMateItem
			brokenMate = GameObject.Find ("BrokenMate");
			brokenMateButton = brokenMate.GetComponent<Button> ();
			brokenMateButton.onClick.AddListener(()=> 
			{ 
				openInfoPanel(infoPanel,payObject, 1);
			});
		
			//TriangleOfDoom
			triangleOfDoom = GameObject.Find ("TriangleOfDoom");
			triangleOfDoomButton = triangleOfDoom.GetComponent<Button> ();
			triangleOfDoomButton.onClick.AddListener(()=> 
			{ 
				openInfoPanel(infoPanel,payObject, 2);					
			});
			//Glass of Mate
			glassMateButton = GameObject.Find ("GlassMate").GetComponent<Button>();
			glassMateButton.onClick.AddListener (() =>
			{
				openInfoPanel (infoPanel, payObject, 3);
				//Debug.Log ("food: "+ playerFoodPoints);	
			});
			
			halfMateButton = GameObject.Find ("HalfMate").GetComponent<Button>();
			halfMateButton.onClick.AddListener (() =>
			{
						openInfoPanel (infoPanel, payObject, 4);
			});
			fullMateButton = GameObject.Find ("FullMate").GetComponent<Button>();
			fullMateButton.onClick.AddListener (() =>
			{
						openInfoPanel (infoPanel, payObject, 5);
			});
			if (itemsToDisplay [0]) {
				stonesButton.interactable = false;
			}
			if (itemsToDisplay [1]) {
				brokenMateButton.interactable = false;
			}
			if (itemsToDisplay [2]) {
				triangleOfDoomButton.interactable = false;
			}

			shopImage.SetActive (false);

			levelImage = GameObject.Find("LevelImage");
			
			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			levelText = GameObject.Find("LevelText").GetComponent<Text>();

			//Set the text of levelText to the string "Day" and append the current level number.
			levelText.text = "Stage "+level;
			
			//Set levelImage to active blocking player's view of the game board during setup.
			levelImage.SetActive(true);
			FindHighScoreObjects ();
			DisableHighScoreObjects ();
		
			yield return new WaitForSeconds(2);
			levelText.text = "";
			infoPanel.SetActive (false);
			shopImage.SetActive(true);

			shopping = false;

		}



		//Update is called every frame.
		void Update()
		{
			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if(playersTurn || enemiesMoving || doingSetup || shopping)
				
				//If any of these are true, return and do not start MoveEnemies.
				return;
			
			//Start moving enemies.
			StartCoroutine (MoveEnemies ());
		}
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}
		
		
		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{

			//StartCoroutine (GameOverIterator());



			loadHighscoreIfExists ();
			//Set levelText to display number of levels passed and game over message
			levelText.fontSize = 24;
			levelText.text = "After " + level + " days, you starved. Score : " + currentScore;
			gameOver = true;
			//Enable black background image gameObject.
			levelImage.SetActive(true);
			shopImage.SetActive (false);
			FindHighScoreObjects ();
			
			first.SetActive (true);
			second.SetActive (true);
			third.SetActive (true);
			
			if (currentScore > thirdScore) {
				nameInput.GetComponent<InputField> ().onEndEdit.AddListener (delegate {
					string text = nameInput.GetComponent<InputField> ().text;
					
					if (currentScore > firstScore) {
						Debug.Log ("erster platz");
						thirdScore = secondScore;
						thirdName = secondName;
						
						
						secondScore = firstScore;
						secondName = firstName;
						
						
						firstScore = currentScore;
						firstName = text;
					} else if (currentScore < firstScore && currentScore > secondScore) {
						Debug.Log ("zweiter platz");
						thirdScore = secondScore;
						thirdName = secondName;
						secondScore = currentScore;
						secondName = text;
					} else if (currentScore < secondScore && currentScore > thirdScore) {
						Debug.Log ("dritter platz");
						thirdScore = currentScore;
						thirdName = text;
					}
					
					first.GetComponent<Text> ().text = stringForHighscoreText (firstName, firstScore);
					second.GetComponent<Text> ().text = stringForHighscoreText (secondName, secondScore);
					third.GetComponent<Text> ().text = stringForHighscoreText (thirdName, thirdScore);
					saveHighscoreToFile ();
					nameInput.SetActive (false);
					gameOver = true;
					//Invoke("restart", 4f);
					if (!waitsForRestart) {
						waitsForRestart = true;
						//yield return new WaitForSeconds (4f);
						//restart ();
						Invoke("restart", 4f);
					}
				});
				nameInput.SetActive (true);
			} else {
				gameOver = true;
				if (!waitsForRestart) {
					waitsForRestart = true;
					Invoke("restart", 4f);
					//yield return new WaitForSeconds (4f);
					//restart ();
				}
				//Invoke("restart", 4f);
			}
			first.GetComponent<Text>().text = stringForHighscoreText(firstName, firstScore);
			second.GetComponent<Text>().text = stringForHighscoreText(secondName, secondScore);
			third.GetComponent<Text>().text = stringForHighscoreText(thirdName, thirdScore);
			gameOver = true;
		}



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
		
		//Coroutine to move enemies in sequence.
		IEnumerator MoveEnemies()
		{
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;
			
			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);
			
			//If there are no enemies spawned (IE in first level):
			if (enemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
			}
			
			//Loop through List of Enemy objects.
			for (int i = 0; i < enemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				enemies[i].MoveEnemy ();
				
				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(enemies[i].moveTime);
			}
			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;
			
			//Enemies are done moving, set enemiesMoving to false.
			enemiesMoving = false;
		}
	
		private void loadHighscoreIfExists(){
			string firstFile = "first.txt";
			string secondFile = "second.txt";
			string thirdFile = "third.txt";
		
			if (File.Exists (firstFile)) {
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
			}
		}
		
		private void saveHighscoreToFile(){
			string firstFile = "first.txt";
			string secondFile = "second.txt";
			string thirdFile = "third.txt";
			var sr = File.CreateText(firstFile);
			sr.WriteLine (getLineForHighScoreFile(firstName, firstScore));
			sr.Close();
			sr = File.CreateText(secondFile);
			sr.WriteLine (getLineForHighScoreFile(secondName, secondScore));
			sr.Close();
			sr = File.CreateText(thirdFile);
			sr.WriteLine (getLineForHighScoreFile(thirdName, thirdScore));
			sr.Close();
		}
		
		private string getLineForHighScoreFile(string playername, int score){
			return playername + "---" + score;
		}
		
		private int scoreFromLine(string line){
			string[] split = line.Split ('-');
			//Debug.Log ("splitlength: " + split.Count() + "line: " + line + "element " + split.ElementAt(split.Count() -1 ) + ". essdft" + split.ElementAt(0));
			return int.Parse (split.ElementAt(split.Count() -1 ).ToString());
		}
		
		private string nameFromLine(string line){
			string[] split = line.Split ('-');
			return split.ElementAt (0);
		}

		public void addToCurrentScore(int addition){
			this.currentScore += addition;
		}

		void FindHighScoreObjects(){
			
			first = levelImage.transform.FindChild ("First").gameObject;
			second = levelImage.transform.FindChild ("Second").gameObject;
			third = levelImage.transform.FindChild ("Third").gameObject;
			nameInput = levelImage.transform.FindChild ("NameInput").gameObject;
		}
		
		void ActivateHighScoreObjects(){
			first.SetActive (true);
			second.SetActive (true);
			third.SetActive (true);
			nameInput.SetActive (true);
		}
		
		void DisableHighScoreObjects(){
			first.SetActive (false);
			second.SetActive (false);
			third.SetActive (false);
			nameInput.SetActive (false);
		}

		void BackToMenu(){
			//Disable this GameManager.
			//enabled = false;
			//StartGameScript starter = sceneSwitcher.GetComponent<StartGameScript> ();
			//Destroy (gameObject);
			//restart ();
			//starter.ShowMenu ();
		}

		IEnumerator GameOverIterator(){
			loadHighscoreIfExists ();
			//Set levelText to display number of levels passed and game over message
			levelText.fontSize = 24;
			levelText.text = "After " + level + " days, you starved. Score : " + currentScore;
			gameOver = true;
			//Enable black background image gameObject.
			levelImage.SetActive(true);
			shopImage.SetActive (false);
			FindHighScoreObjects ();
			
			first.SetActive (true);
			second.SetActive (true);
			third.SetActive (true);
			
			if (currentScore > thirdScore) {
				nameInput.GetComponent<InputField> ().onEndEdit.AddListener (delegate {
					string text = nameInput.GetComponent<InputField> ().text;
					
					if (currentScore > firstScore) {
						Debug.Log ("erster platz");
						thirdScore = secondScore;
						thirdName = secondName;
						
						
						secondScore = firstScore;
						secondName = firstName;
						
						
						firstScore = currentScore;
						firstName = text;
					} else if (currentScore < firstScore && currentScore > secondScore) {
						Debug.Log ("zweiter platz");
						thirdScore = secondScore;
						thirdName = secondName;
						secondScore = currentScore;
						secondName = text;
					} else if (currentScore < secondScore && currentScore > thirdScore) {
						Debug.Log ("dritter platz");
						thirdScore = currentScore;
						thirdName = text;
					}
					
					first.GetComponent<Text> ().text = stringForHighscoreText (firstName, firstScore);
					second.GetComponent<Text> ().text = stringForHighscoreText (secondName, secondScore);
					third.GetComponent<Text> ().text = stringForHighscoreText (thirdName, thirdScore);
					saveHighscoreToFile ();
					nameInput.SetActive (false);
					gameOver = true;
					//Invoke("restart", 4f);
					if (!waitsForRestart) {
						waitsForRestart = true;
						//yield return new WaitForSeconds (4f);
						//restart ();
						Invoke("restart", 4f);
					}
				});
				nameInput.SetActive (true);
			} else {
				gameOver = true;
				if (!waitsForRestart) {
					waitsForRestart = true;
					yield return new WaitForSeconds (4f);
					restart ();
				}
				//Invoke("restart", 4f);
			}
			first.GetComponent<Text>().text = stringForHighscoreText(firstName, firstScore);
			second.GetComponent<Text>().text = stringForHighscoreText(secondName, secondScore);
			third.GetComponent<Text>().text = stringForHighscoreText(thirdName, thirdScore);
			gameOver = true;
			/*
			if (!waitsForRestart) {
				waitsForRestart = true;
				yield return new WaitForSeconds (4f);
				restart ();
			}*/
			
			//Disable this GameManager.
			//enabled = false;
		}
	}

}

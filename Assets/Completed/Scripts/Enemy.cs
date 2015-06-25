using UnityEngine;
using System.Collections;

using System;

namespace Completed
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Enemy : MovingObject
	{
		public int playerDamage; 							//The amount of food points to subtract from the player when attacking.
		public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;					//Second of two audio clips to play when attacking the player.
		public AudioClip dieSound;
		public int maxHp = 2;
		public int allowedAtLevel = 0;
		public int pointsForScore = 1;
		public AnimationClip deathAnimation;
		
		private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.

		private Transform target;							//Transform to attempt to move toward each turn.
		private bool skipMove;								//Boolean to determine whether or not enemy should skip a turn or move this turn.
		private int hp;
		private Texture2D hpTexture;
		private GUIStyle style = new GUIStyle();
		private DateTime timeAtHit;

		//private TimeSpan hpHuiThresholdTime = 10;
		
		//Start overrides the virtual Start function of the base class.
		protected override void Start ()
		{
			//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
			//This allows the GameManager to issue movement commands.
			GameManager.instance.AddEnemyToList (this);
			
			//Get and store a reference to the attached Animator component.
			animator = GetComponent<Animator> ();
			//Find the Player GameObject using it's tag and store a reference to its transform component.
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			hp = maxHp;
			hpTexture = new Texture2D(1, 1);
			hpTexture.SetPixel(1, 1, Color.red);
			hpTexture.Apply ();
			//Call the start function of our base class MovingObject.
			base.Start ();
		}
		
		
		//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		//See comments in MovingObject for more on how base AttemptMove function works.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//Check if skipMove is true, if so set it to false and skip this turn.
			if(skipMove)
			{
				skipMove = false;
				return;
				
			}
			
			//Call the AttemptMove function from MovingObject.
			base.AttemptMove <T> (xDir, yDir);
			
			//Now that Enemy has moved, set skipMove to true to skip next move.
			skipMove = true;
		}
		
		
		//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
		public void MoveEnemy ()
		{
			if (hp > 0) {
				//Declare variables for X and Y axis move directions, these range from -1 to 1.
				//These values allow us to choose between the cardinal directions: up, down, left and right.
				int xDir = 0;
				int yDir = 0;
				
				//If the difference in positions is approximately zero (Epsilon) do the following:
				if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
					
					//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
					yDir = target.position.y > transform.position.y ? 1 : -1;
				
				//If the difference in positions is not approximately zero (Epsilon) do the following:
				else
					//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
					xDir = target.position.x > transform.position.x ? 1 : -1;
				
				//Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
				AttemptMove <Player> (xDir, yDir);
			}
				

			
		}

		
		//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
		//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
		protected override void OnCantMove <T> (T component)
		{
			//Declare hitPlayer and set it to equal the encountered component.
			Player hitPlayer = component as Player;
			
			//Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
			hitPlayer.LoseFood (playerDamage);
			
			//Set the attack trigger of animator to trigger Enemy attack animation.
			animator.SetTrigger ("enemyAttack");
			
			//Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
			SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}

		public void DamageEnemy (int loss){
			hp -= loss;
			//Debug.Log ("Enemy has taken" + loss + "damage. result: " + hp);
			timeAtHit = System.DateTime.Now;
			//If hit points are less than or equal to zero:
			if (hp <= 0) {
				StartCoroutine(Die ());
				//Disable the gameObject.


				
			}
		}
		public IEnumerator Die(){
			animator.SetTrigger("enemyDead");
			SoundManager.instance.PlaySingle (dieSound);
			float length = 0.98f;
			yield return new WaitForSeconds(length);
			boxCollider.enabled = false;
			yield return new WaitForSeconds(0.45f);
			GameManager.instance.addToCurrentScore(pointsForScore);
			gameObject.SetActive(false);
			
		}
		void OnGUI()
		{


			//TimeSpan duration = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime));
			DateTime current = DateTime.Now;
			//TimeSpan duration = current.Subtract (timeAtHit);

			Vector2 targetPos;
			targetPos = Camera.main.WorldToScreenPoint (transform.position);
			style.normal.background = hpTexture;
			float hpInPercent = (float)hp / (float)maxHp;
			//Debug.Log ("hp in percent: " + hpInPercent);
			Renderer renderer = gameObject.GetComponent<Renderer>();
			//float height = renderer.bounds.size.y;
			float height = 50.0f * (float) hpInPercent;
			//Debug.Log ("height: " + height);
			if (hpInPercent > 0.0 && hpInPercent < 0.99) {
				GUI.Box(new Rect(targetPos.x + 52, Screen.height- targetPos.y + (52.0f - height), 2, height), new GUIContent(""), style);
			}
			Debug.Log ("hpInpercent : " + hpInPercent);
			//GUI.Box(new Rect(targetPos.x, Screen.height- targetPos.y, 60, 20), hp + "/" + maxHp);
		}
	}
}

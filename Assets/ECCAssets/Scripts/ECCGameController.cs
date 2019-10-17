using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EndlessCarChase.Types;

namespace EndlessCarChase
{
	/// <summary>
	/// This script controls the game, starting it, following game progress, and finishing it with game over or victory.
	/// </summary>
	public class ECCGameController : MonoBehaviour 
	{
        [Tooltip("The camera object and the camera holder that contains it and follows the player")]
        public Transform cameraHolder;

        // Unused variable that holds a minimap camera
        internal Transform miniMap;

        // Unused code that makes the camera chase the player and rotate behind it
        internal float cameraRotate = 0;

        [Tooltip("The player object assigned from the project folder or from the shop")]
        public ECCCar playerObject;
        internal float playerDirection;

        [Tooltip("The ground object that follows the player position and gives a feeling of movement.")]
        public Transform groundObject;

        [Tooltip("The layer which the cars move up and down along terrain. A 'groundObject' must not be assigned or hidden  in order to make this work")]
        public LayerMask groundLayer;
        
        [Tooltip("The speed at which the texture of the ground object moves, make the player seems as if it is moving on the ground")]
        public float groundTextureSpeed = -0.4f;

        [Tooltip("How long to wait before starting the game. Ready? GO! time")]
        public float startDelay = 1;
        internal bool gameStarted = false;

        [Tooltip("The effect displayed before starting the game")]
        public Transform readyGoEffect;
        
        [Tooltip("The score of the player")]
        public int score = 0;

        [Tooltip("How many points we get per second")]
        public int scorePerSecond = 1;

        [Tooltip("The score text object which displays the current score of the player")]
        public Transform scoreText;

        [Tooltip("Text that is appended to the score value. We use this to add a money sign to the score")]
        public string scoreTextSuffix = "$";

        [Tooltip("The player prefs record of the total score we have ( not high score, but total score we collected in all games which is used as money )")]
        public string moneyPlayerPrefs = "Money";

        internal int highScore = 0;
		internal int scoreMultiplier = 1;

        [Tooltip("The canvas menu that displays the shop where we can unlock and select cars")]
        public ECCShop shopMenu;

        [Tooltip("The steering wheel canvas that allows you to control the player by dragging the wheel left/right")]
        public Slider steeringWheel;

        // Various canvases for the UI
        public Transform gameCanvas;
        public Transform healthCanvas;
        public Transform pauseCanvas;
		public Transform gameOverCanvas;

		// Is the game over?
		internal bool  isGameOver = false;
		
		// The level of the main menu that can be loaded after the game ends
		public string mainMenuLevelName = "StartMenu";
		
		// Various sounds and their source
		public AudioClip soundGameOver;
		public string soundSourceTag = "GameController";
		internal GameObject soundSource;
		
		// The button that will restart the game after game over
		public string confirmButton = "Submit";
		
		// The button that pauses the game. Clicking on the pause button in the UI also pauses the game
		public string pauseButton = "Cancel";
		internal bool  isPaused = false;

		// A general use index
		internal int index = 0;

        //public Transform slowMotionEffect; // Should dying happen in slow motion?

        // Unused variables that limit the game area and make it wrap around from edge to edge
        internal Vector2 gameArea = new Vector2(10, 10);
        internal bool wrapAroundGameArea = false;

        void Awake()
		{
            Time.timeScale = 1;

            // Activate the pause canvas early on, so it can detect info about sound volume state
            //if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(true);

            //Get the number of the current item
            //GameObject.FindObjectOfType<ECCShop>().gameObject.SetActive(true);
            if (shopMenu)
            {
                //Get the number of the current item
                shopMenu.currentItem = PlayerPrefs.GetInt(shopMenu.currentPlayerprefs, shopMenu.currentItem);

                // Update the player object based on the shop car we have selected
                playerObject = shopMenu.items[shopMenu.currentItem].itemIcon.GetComponent<ECCCar>();
            }

        }

		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start()
		{
            //Application.targetFrameRate = 30;

            // Update the score at 0
            ChangeScore(0);

            //Hide the cavases
            if ( shopMenu )    shopMenu.gameObject.SetActive(false);
            if ( gameOverCanvas )    gameOverCanvas.gameObject.SetActive(false);
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(false);
            if ( gameCanvas)    gameCanvas.gameObject.SetActive(false);

            //Get the highscore for the player
            highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HighScore", 0);

            //Assign the sound source for easier access
            if ( GameObject.FindGameObjectWithTag(soundSourceTag) )    soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);

            if (steeringWheel) steeringWheel.gameObject.SetActive(false);
        }

        public void StartGame()
        {
            // Spawn the player car if it exists
            if (playerObject)
            {
                // Create the player object in the scene
                playerObject = Instantiate(playerObject);

                // Set the player tag of the player so that we can refer to it 
                playerObject.tag = "Player";
            }

            // Start spawning objects in the scene
            if (GetComponent<ECCSpawnAroundObject>()) GetComponent<ECCSpawnAroundObject>().isSpawning = true;

            // Show the game UI
            if (gameCanvas) gameCanvas.gameObject.SetActive(true);

            // Create the ready?GO! effect
            if (readyGoEffect) Instantiate(readyGoEffect);

            // The game has started
            gameStarted = true;

            // Add to the player's score every second
            if (scorePerSecond > 0)    InvokeRepeating("ScorePerSecond", startDelay, 1);

            if (steeringWheel && Application.isMobilePlatform) steeringWheel.gameObject.SetActive(true);

        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
		{
            // If the game hasn't started yet, nothing happens
            if ( gameStarted == false) return;

			// Delay the start of the game
			if ( startDelay > 0 )
			{
				startDelay -= Time.deltaTime;
            }
			else
			{
				//If the game is over, listen for the Restart and MainMenu buttons
				if ( isGameOver == true )
				{
					//The jump button restarts the game
					if ( Input.GetButtonDown(confirmButton) )
					{
						Restart();
					}
					
					//The pause button goes to the main menu
					if ( Input.GetButtonDown(pauseButton) )
					{
						MainMenu();
					}
				}
				else
				{
                    // If there is a player object, move it forward and turn it in the correct direction
                    if (playerObject)
                    {
                        // If we are using mobile controls, turn left/right based on the tap side position on the screen
                        if (Application.isMobilePlatform)
                        {
                            // If we have a steering wheel slider assigned, use it
                            if (steeringWheel)
                            {
                                // If we press the mouse button, check our position relative to the screen center
                                if (Input.GetMouseButton(0))
                                {
                                    playerDirection = steeringWheel.value;
                                }
                                else // Otherwise, if we didn't press anything, don't rotate and straighten up
                                {
                                    steeringWheel.value = playerDirection = 0;
                                }

                                steeringWheel.transform.Find("Wheel").eulerAngles = Vector3.forward * playerDirection * -100;
                            }
                            else if (Input.GetMouseButton(0)) // If we press the mouse button, check our position relative to the screen center
                            {
                                // If we are to the right of the screen, rotate to the right
                                if (Input.mousePosition.x > Screen.width * 0.5f)
                                {
                                    playerDirection = 1;
                                }
                                else // Othwerwise, rotate to the left
                                {
                                    playerDirection = -1;
                                }
                            }
                            else // Otherwise, if we didn't press anything, don't rotate and straighten up
                            {
                                playerDirection = 0;
                            }
                        }
                        else // Otherwise, use gamepad/keyboard controls
                        {
                            playerDirection = Input.GetAxis("Horizontal");
                        }

                        // Calculate the rotation direction
                        playerObject.Rotate(playerDirection);

                        // Unused code that makes the player wrap around the edge of the game area
                        if (wrapAroundGameArea == true )
                        {
                            if (playerObject.transform.position.x > gameArea.x * 0.5f) playerObject.transform.position -= Vector3.right * gameArea.x;
                            if (playerObject.transform.position.x < gameArea.x * -0.5f) playerObject.transform.position += Vector3.right * gameArea.x;
                            if (playerObject.transform.position.z > gameArea.y * 0.5f) playerObject.transform.position -= Vector3.forward * gameArea.y;
                            if (playerObject.transform.position.z < gameArea.y * -0.5f) playerObject.transform.position += Vector3.forward * gameArea.y;
                        }
                    }
                    
                    //Toggle pause/unpause in the game
                    if ( Input.GetButtonDown(pauseButton) )
					{
						if ( isPaused == true )    Unpause();
						else    Pause(true);
					}
				}
			}
		}

        void LateUpdate()
        {
            if (playerObject)
            {
                if (cameraHolder)
                {
                    // Make the camera holder follow the position of the player
                    cameraHolder.position = playerObject.transform.position;

                    // Make the camera holder rotate in the direction the player is moving
                    if (cameraRotate > 0) cameraHolder.eulerAngles = Vector3.up * Mathf.LerpAngle(cameraHolder.eulerAngles.y, playerObject.transform.eulerAngles.y, Time.deltaTime * cameraRotate);
                }

                // If there is a minimap, make it follow the player
                if ( miniMap ) miniMap.position = playerObject.transform.position;

                // If there is a ground object make its UV map move based on the player position ( which gives a feeling of movement on the fround )
                if (groundObject)
                {
                    // Keep the ground object follwing the player position
                    groundObject.position = playerObject.transform.position;

                    // Update the texture UV of the ground based on the player position
                    groundObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(playerObject.transform.position.x, playerObject.transform.position.z) * groundTextureSpeed);
                }
            }
        }

        /// <summary>
        /// Changes the health of the player by the set value. This is used to make pickup items change the player health
        /// </summary>
        /// <param name="changeValue"></param>
        public void ChangeHealth(float changeValue)
        {
            if (playerObject) playerObject.ChangeHealth(changeValue);
        }

        /// <summary>
        /// Change the score and update it
        /// </summary>
        /// <param name="changeValue">Change value</param>
        public void  ChangeScore( int changeValue )
		{
            // Change the score value
			score += changeValue;

            //Update the score text
            if (scoreText)
            {
                scoreText.GetComponent<Text>().text = score.ToString();

                // Play the score object animation
                if (scoreText.GetComponent<Animation>()) scoreText.GetComponent<Animation>().Play();
            }
        }
        
        /// <summary>
        /// Set the score multiplier ( Get double score for hitting and destroying targets )
        /// </summary>
        void SetScoreMultiplier( int setValue )
		{
			// Set the score multiplier
			scoreMultiplier = setValue;
		}

        /// <summary>
        /// Adds score, used to gives score per second
        /// </summary>
        public void ScorePerSecond()
        {
            ChangeScore(scorePerSecond);
        }

        /// <summary>
        /// Pause the game, and shows the pause menu
        /// </summary>
        /// <param name="showMenu">If set to <c>true</c> show menu.</param>
        public void Pause(bool showMenu)
        {
            isPaused = true;

            //Set timescale to 0, preventing anything from moving
            Time.timeScale = 0;

            //Show the pause screen and hide the game screen
            if (showMenu == true)
            {
                if (pauseCanvas) pauseCanvas.gameObject.SetActive(true);
                if (gameCanvas) gameCanvas.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Resume the game
        /// </summary>
        public void Unpause()
        {
            isPaused = false;

            //Set timescale back to the current game speed
            Time.timeScale = 1;

            //Hide the pause screen and show the game screen
            if (pauseCanvas) pauseCanvas.gameObject.SetActive(false);
            if (gameCanvas) gameCanvas.gameObject.SetActive(true);
        }

        /// <summary>
        /// Runs the game over event and shows the game over screen
        /// </summary>
        IEnumerator GameOver(float delay)
		{
			isGameOver = true;

			yield return new WaitForSeconds(delay);
			
			//Remove the pause and game screens
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(false);
            if ( gameCanvas )    gameCanvas.gameObject.SetActive(false);

            //Get the number of money we have
            int totalMoney = PlayerPrefs.GetInt(moneyPlayerPrefs, 0);

            //Add to the number of money we collected in this game
            totalMoney += score;

            //Record the number of money we have
            PlayerPrefs.SetInt(moneyPlayerPrefs, totalMoney);

            //Show the game over screen
            if ( gameOverCanvas )    
			{
				//Show the game over screen
				gameOverCanvas.gameObject.SetActive(true);
				
				//Write the score text
				gameOverCanvas.Find("Base/TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();
				
				//Check if we got a high score
				if ( score > highScore )    
				{
					highScore = score;
					
					//Register the new high score
					PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
				}
				
				//Write the high sscore text
				gameOverCanvas.Find("Base/TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();

				//If there is a source and a sound, play it from the source
				if ( soundSource && soundGameOver )    
				{
					soundSource.GetComponent<AudioSource>().pitch = 1;
					
					soundSource.GetComponent<AudioSource>().PlayOneShot(soundGameOver);
				}
			}
		}
		
		/// <summary>
		/// Restart the current level
		/// </summary>
		void  Restart()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		
		/// <summary>
		/// Restart the current level
		/// </summary>
		void  MainMenu()
		{
			SceneManager.LoadScene(mainMenuLevelName);
		}

        void OnDrawGizmos()
        {
            //Gizmos.color = Color.blue;

            // Draw two lines to show the edges of the street
            //Gizmos.DrawWireCube(Vector3.zero, new Vector3(gameArea.x, 1, gameArea.y));
        }
    }
}
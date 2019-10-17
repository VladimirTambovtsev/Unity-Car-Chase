using UnityEngine;
using System.Collections;

namespace EndlessCarChase
{
	/// <summary>
	/// This script animates the UI while the game is paused
	/// </summary>
	public class ECCAnimateUI : MonoBehaviour
	{
		// The current real time, unrelated to Time.timeScale
		internal float currentTime;
		
		// The previous registered time, this is used to calculate the delta time
		internal float previousTime;
		
		// The delta time ( change in time ) calculated in order to allow animation over time
		internal float deltaTime;
		
		[Tooltip("The intro animation for this UI element")]
		public AnimationClip introAnimation;

		// The animation component that holds the animation clips
		internal Animation animationObject;
		
		// The current animation time. This is reset when starting a new animation
		internal float animationTime = 0;
		
		// Are we animating now?
		internal bool isAnimating = false;
		
		[Tooltip("Should the animation be played immediately when the UI element is enabled?")]
		public bool playOnEnabled = true;
		
		// Use this for initialization
		void Awake()
		{
			// Register the current time
			previousTime = currentTime = Time.realtimeSinceStartup;
			
			// Register the animation component for quicker access
			animationObject = GetComponent<Animation>();
		}
		
		// Update is called once per frame
		void Update()
		{
			// We are animating
			if ( introAnimation && isAnimating == true )
			{

				// Get the current real time, regardless of time scale
				currentTime = Time.realtimeSinceStartup;
				
				// Calculate the difference in time from our last Update()
				deltaTime = currentTime - previousTime;
				
				// Set the current time to be the same as the previous time, for the next Update() check
				previousTime = currentTime;
				
				// Calculate the current time in the current animation
				animationObject[introAnimation.name].time = animationTime;
				
				// Sample the animation from the time we set ( display the correct frame based on the animation time )
				animationObject.Sample();
				
				// Add to the animation time
				animationTime += deltaTime;
				
				// If the animation reaches the clip length, finish the animation
				if ( animationTime >= animationObject.clip.length )
				{
					// Set the animation time to the length of the clip ( make sure we get to the end of the animation )
					animationObject[introAnimation.name].time = animationObject.clip.length;
					
					// Sample the animation from the time we set ( display the correct frame based on the animation time )
					animationObject.Sample();
					
					// We are not animating anymore
					isAnimating = false;
				}
			}
		}

		/// <summary>
		/// Runs when the object has been enabled. ( If it was disabled before )
		/// </summary>
		void OnEnable()
		{
			// If the object has been enabled. play the animation
			if ( playOnEnabled == true )
			{
				PlayAnimation();
			}
		}

		/// <summary>
		/// Plays an animation, regardless of timeScale
		/// </summary>
		public void PlayAnimation()
		{
			if ( introAnimation ) 
			{
				// Reset the animation time
				animationTime = 0;
			
				// Register the current time
				previousTime = currentTime = Time.realtimeSinceStartup;
			
				// Start animating
				isAnimating = true;

				animationObject.Play();
			}
		}
	}
}


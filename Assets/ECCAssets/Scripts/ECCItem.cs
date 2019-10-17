using UnityEngine;

namespace EndlessCarChase
{
    /// <summary>
    /// This script defines an item which can be picked up by the player. An item can be money that the 
    /// player collects, or a repair kit that increases health
    /// </summary>
    public class ECCItem : MonoBehaviour
    {
        static ECCGameController gameController;
        
        [Tooltip("The function that runs when this object is touched by the target")]
        public string touchFunction = "ChangeScore";

        [Tooltip("The parameter that will be passed with the function")]
        public float functionParameter = 100;

        [Tooltip("The tag of the target object that the function will play from")]
        public string functionTarget = "GameController";

        [Tooltip("The effect that is created at the location of this item when it is picked up")]
        public Transform pickupEffect;

        [Tooltip("A random rotation given to the object only on the Y axis")]
        public float randomRotation = 360;

        void Start()
        {
            // Set a random rotation angle for the object
            transform.eulerAngles += Vector3.up * Random.Range(-randomRotation, randomRotation);

            // Get the gamecontroller from the scene
            if (gameController == null) gameController = GameObject.FindObjectOfType<ECCGameController>();
        }

        /// <summary>
        /// Is executed when this obstacle touches another object with a trigger collider
        /// </summary>
        /// <param name="other"><see cref="Collider"/></param>
        void OnTriggerEnter(Collider other)
        {
            // Check if the object that was touched has the correct tag
            if (gameController.playerObject && other.gameObject == gameController.playerObject.gameObject)
            {
                // Check that we have a target tag and function name before running
                if (touchFunction != string.Empty)
                {
                    // Run the function
                    GameObject.FindGameObjectWithTag(functionTarget).SendMessage(touchFunction, functionParameter);
                }

                // If there is a pick up effect, create it
                if (pickupEffect) Instantiate(pickupEffect, transform.position, transform.rotation);
                
                // Remove the object from the game
                Destroy(gameObject);
            }
        }
    }
}
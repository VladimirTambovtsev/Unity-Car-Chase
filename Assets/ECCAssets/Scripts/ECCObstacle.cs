using UnityEngine;

namespace EndlessCarChase
{
    /// <summary>
    /// This script defines an item which can be picked up by with the player. An item can be money that the 
    /// player collects, or a repair kit that increases health
    /// </summary>
    public class ECCObstacle : MonoBehaviour
    {
        [Tooltip("The damage caused by this obstacle")]
        public float damage = 1;

        [Tooltip("The effect that is created at the location of this object when it is hit")]
        public Transform hitEffect;

        [Tooltip("Should this obstacle be removed when hit by a car?")]
        public bool removeOnHit = false;

        [Tooltip("A random rotation given to the object only on the Y axis")]
        public float randomRotation = 360;

        void Start()
        {
            // Set a random rotation angle for the object
            transform.eulerAngles += Vector3.up * Random.Range( -randomRotation, randomRotation);
            
            // Resets the color of an obstacle periodically
            //InvokeRepeating("ResetColor", 0, 0.5f);
        }

        /// <summary>
        /// Is executed when this obstacle touches another object with a trigger collider
        /// </summary>
        /// <param name="other"><see cref="Collider"/></param>
        void OnTriggerStay(Collider other)
        {
            // If the hurt delay is over, and this obstacle was hit by a car, damage the car
            if (other.GetComponent<ECCCar>() )
            {
                //if ( other.GetComponent<ECCCar>().hurtDelayCount <= 0 )
                //{
                    // Reset the hurt delay
                    //other.GetComponent<ECCCar>().hurtDelayCount = other.GetComponent<ECCCar>().hurtDelay;

                    // Damage the car
                    other.GetComponent<ECCCar>().ChangeHealth(-damage);

                    // If there is a hit effect, create it
                    if (other.GetComponent<ECCCar>().health - damage > 0 && other.GetComponent<ECCCar>().hitEffect) Instantiate(other.GetComponent<ECCCar>().hitEffect, transform.position, transform.rotation);
                //}
                
                // If there is a hit effect, create it
                if (hitEffect) Instantiate(hitEffect, transform.position, transform.rotation);

                // Remove the object from the game
                if ( removeOnHit == true )    Destroy(gameObject);
            }
        }

        public void ResetColor()
        {
            GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
        }
    }
}
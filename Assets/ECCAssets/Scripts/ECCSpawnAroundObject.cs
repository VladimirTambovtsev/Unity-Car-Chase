using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EndlessCarChase.Types;

namespace EndlessCarChase
{
	/// <summary>
	/// This script spawns objects around a central object such as the player. Spawned objects can be for example Items, or enemy cars, or obstacles.
	/// </summary>
	public class ECCSpawnAroundObject : MonoBehaviour 
	{
        static ECCGameController gameController;

        [Tooltip("The tag of the object around which other objects are spawned within a limited range")]
        public string spawnAroundTag = "Player";
        internal Transform spawnAroundObject;

        [Tooltip("A toggle that turns spawning on and off. If True, we are spawning objects now")]
        public bool isSpawning = false;

        [System.Serializable]
        public class SpawnGroup
        {
            [Tooltip("A list of all Objects that will be spawned")]
            public Transform[] spawnObjects;

            [Tooltip("The rate at which objects are spawned, in seconds.")]
            public float spawnRate = 5;
            internal float spawnRateCount = 0;
            internal int spawnIndex = 0;

            [Tooltip("The distance at which this object is spawned relative to the spawnAroundObject")]
            public Vector2 spawnObjectDistance = new Vector2(10, 20);
        }

        [Tooltip("An array of spawn groups. These can be enemy cars, pickup items, or obstacle rocks for example")]
        public SpawnGroup[] spawnGroups;

        internal int index;

        private void Start()
        {
            // Hold some variables for easier access
            if (gameController == null) gameController = GameObject.FindObjectOfType<ECCGameController>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
		{
            // If we don't have an object to spawn around, look for it in the scene and assign it
            if ( spawnAroundObject == null && spawnAroundTag != string.Empty && GameObject.FindGameObjectWithTag(spawnAroundTag) ) spawnAroundObject = GameObject.FindGameObjectWithTag(spawnAroundTag).transform;

            // If we are not spawning, don't continue
            if (isSpawning == false) return;
            
            // Go through all the spawn groups, count down, and spawn objects
            for ( index = 0; index < spawnGroups.Length; index++ )
            {
                // If there are objects to spawn, continue
                if ( spawnGroups[index].spawnObjects.Length > 0 )
                {
                    // Count down to the next object spawn
                    if (spawnGroups[index].spawnRateCount > 0) spawnGroups[index].spawnRateCount -= Time.deltaTime;
                    else
                    {
                        // Spawn the next object in the group
                        Spawn(spawnGroups[index].spawnObjects, spawnGroups[index].spawnIndex, spawnGroups[index].spawnObjectDistance);

                        // Go to the next spawn Object in the list
                        spawnGroups[index].spawnIndex++;

                        // Reset the index if we reach the end of the list
                        if (spawnGroups[index].spawnIndex > spawnGroups[index].spawnObjects.Length - 1) spawnGroups[index].spawnIndex = 0;

                        // Reset the spawn pick up rate counter
                        spawnGroups[index].spawnRateCount = spawnGroups[index].spawnRate;
                    }

                }
            }
		}

        /// <summary>
        /// Spawns an object based on the index chosen from the array
        /// </summary>
        /// <param name="spawnArray"></param>
        /// <param name="spawnIndex"></param>
        /// <param name="spawnGap"></param>
        public void Spawn( Transform[] spawnArray, int spawnIndex, Vector2 spawnGap )
        {
            // If the array is empty, don't continue
            if (spawnArray[spawnIndex] == null) return;

            // Create a new Object spawn based on the index which loops in the list
            Transform newSpawn = Instantiate(spawnArray[spawnIndex]) as Transform;
            
            // Spawn an Object at the target position
            if (spawnAroundObject) newSpawn.position = spawnAroundObject.transform.position;// + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

            // Rotate the object randomly, and then move it forward to a random distance from the spawn point
            newSpawn.eulerAngles = Vector3.up * Random.Range(0, 360);
            newSpawn.Translate(Vector3.forward * Random.Range(spawnGap.x, spawnGap.y), Space.Self);

            // Then rotate it back to face the spawn point
            newSpawn.eulerAngles += Vector3.up * 180;

            // Position the object at the same height as the target spawn point
            //if (spawnAroundObject) newSpawn.position += Vector3.up * spawnAroundObject.position.y;

            RaycastHit hit;

            if (Physics.Raycast(newSpawn.position + Vector3.up * 5, -10 * Vector3.up, out hit, 100, gameController.groundLayer)) newSpawn.position = hit.point;


        }
    }
}
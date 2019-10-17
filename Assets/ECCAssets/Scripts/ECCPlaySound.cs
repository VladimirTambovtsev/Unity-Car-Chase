using UnityEngine;
using UnityEngine.UI;

namespace EndlessCarChase
{
    /// <summary>
    /// Plays a sound from an audio source.
    /// </summary>
    public class ECCPlaySound : MonoBehaviour
    {
        [Tooltip("The sound to play")]
        public AudioClip sound;

        [Tooltip("The tag of the sound source")]
        public string soundSourceTag = "Sound";

        [Tooltip("Play the sound immediately when the object is activated")]
        public bool playOnStart = true;

        [Tooltip("Play the sound when clicking on this button")]
        public bool playOnClick = false;

        [Tooltip("A random range for the pitch of the audio source, to make the sound more varied")]
        public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

        /// <summary>
        /// Start is only called once in the lifetime of the behaviour.
        /// The difference between Awake and Start is that Start is only called if the script instance is enabled.
        /// This allows you to delay any initialization code, until it is really needed.
        /// Awake is always called before any Start functions.
        /// This allows you to order initialization of scripts
        /// </summary>
        void Start()
        {
            if (playOnStart == true) PlayCurrentSound();

            // Listen for a click to play a sound
            if (playOnClick && GetComponent<Button>()) GetComponent<Button>().onClick.AddListener(delegate { PlayCurrentSound(); });

        }

        /// <summary>
        /// Plays the sound
        /// </summary>
        public void PlaySound(AudioClip sound)
        {
            // If there is a sound source tag and audio to play, play the sound from the audio source based on its tag
            if (soundSourceTag != string.Empty && sound)
            {
                // Give the sound a random pitch limited by the time scale of the game
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

                // Play the sound
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(sound);
            }
            else if (GetComponent<AudioSource>())
            {
                // Give the sound a random pitch limited by the time scale of the game
                GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

                // Play the sound
                GetComponent<AudioSource>().PlayOneShot(sound);
            }

        }


        /// <summary>
        /// Plays the sound
        /// </summary>
        public void PlayCurrentSound()
        {
            // If there is a sound source tag and audio to play, play the sound from the audio source based on its tag
            if (soundSourceTag != string.Empty && sound)
            {
                // Give the sound a random pitch limited by the time scale of the game
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

                // Play the sound
                GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(sound);
            }
            else if (GetComponent<AudioSource>())
            {
                // Give the sound a random pitch limited by the time scale of the game
                GetComponent<AudioSource>().pitch = Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;

                // Play the sound
                GetComponent<AudioSource>().PlayOneShot(sound);
            }
        }
    }
}
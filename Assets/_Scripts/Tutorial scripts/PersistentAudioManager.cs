using UnityEngine;

namespace MyGame
{
    public class PersistentAudioManager : MonoBehaviour
    {
        private static PersistentAudioManager instance;

        public static PersistentAudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("PersistentAudioManager");
                    instance = obj.AddComponent<PersistentAudioManager>();
                    DontDestroyOnLoad(obj);
                }
                return instance;
            }
        }

        private AudioSource audioSource;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySound(AudioClip clip, bool loop = false)
        {
            if (audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.loop = loop;
                audioSource.Play();
            }
        }

        public void StopSound()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
    }
}

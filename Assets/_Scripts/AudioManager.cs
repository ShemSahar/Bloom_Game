using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public bool loop;
    }

    public Sound[] sounds;

    private Dictionary<string, AudioSource> soundDictionary;

    private void Awake()
    {
        // Ensure there is only one instance of AudioManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        soundDictionary = new Dictionary<string, AudioSource>();

        foreach (Sound sound in sounds)
        {
            GameObject soundGameObject = new GameObject("Sound_" + sound.name);
            soundGameObject.transform.SetParent(transform);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = sound.clip;
            audioSource.loop = sound.loop;
            soundDictionary[sound.name] = audioSource;
        }
    }

    public void Play(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            soundDictionary[soundName].Play();
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }

    public void Stop(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            soundDictionary[soundName].Stop();
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }
}

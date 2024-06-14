using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public List<Sound> sounds;

    public Dictionary<string, AudioClip> soundDictionary;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    private void Awake()
    {
        
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
        // Initialize the dictionary
        soundDictionary = new Dictionary<string, AudioClip>();
        foreach (var sound in sounds)
        {
            soundDictionary.Add(sound.name, sound.clip);
        }

    }

    public void PlaySound(string name)
    {
      
        if (soundDictionary.ContainsKey(name))
        {
            sfxSource.PlayOneShot(soundDictionary[name]);
        }
        else
        {
            Debug.LogWarning("Sound with name " + name + " not found!");
        }
    }

    public void PlayMusic(string name)
    {
       
        foreach (var sound in sounds)
        {
            if (sound.name == name)
            {
                musicSource.clip = sound.clip;
                musicSource.Play();
                return;
            }
        }
        Debug.LogWarning("Music with name " + name + " not found!");
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}

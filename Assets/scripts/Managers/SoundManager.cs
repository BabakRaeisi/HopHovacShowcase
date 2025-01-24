using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // Singleton instance

    [Header("Audio Sources")]
    public AudioSource musicSource;    // For music
    public AudioSource ambientSource;  // For ambient sounds
    public AudioSource windSource;     // For wind sound

    [Header("Audio Settings")]
    public int poolSize = 10;           // Number of AudioSources in the pool for one-shot sounds
    public List<AudioClip> audioClips;  // Add all your audio clips here in the Inspector

    [Header("Volume Settings")]
    public float defaultVolume = 1.0f;  // Default volume for one-shot effects

    private Dictionary<string, AudioClip> clipDictionary; // To quickly access clips by name
    private Queue<AudioSource> audioSourcePool;           // Pool of AudioSources for one-shot sounds

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the clip dictionary
        clipDictionary = new Dictionary<string, AudioClip>();
        foreach (var clip in audioClips)
        {
            if (clip != null)
            {
                clipDictionary[clip.name] = clip; // Use the clip's name as a key
            }
        }

        // Initialize AudioSource pool
        audioSourcePool = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            audioSourcePool.Enqueue(source);
        }
    }

    // Play a looping sound
    public void PlayLoopingSound(AudioSource source, string clipName)
    {
        if (clipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            source.clip = clip;
            source.loop = true;
            source.Play();
        }
        else
        {
            Debug.LogWarning($"Clip {clipName} not found!");
        }
    }

    // Play a sound once using a specific AudioSource
    public void PlaySoundOnce(AudioSource source, string clipName)
    {
        if (clipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            source.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound '{clipName}' not found!");
        }
    }

    // Stop a looping sound
    public void StopLoopingSound(AudioSource source)
    {
        source.Stop();
    }

    // Play a one-shot effect using the pool
    public void PlayOneShotSound(string clipName, Vector3 position, float volume = 1.0f)
    {
        if (clipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            AudioSource source = GetAvailableAudioSource();
            source.transform.position = position;
            source.volume = Mathf.Clamp(volume, 0f, 1f);
            source.clip = clip;
            source.Play();
        }
        else
        {
            Debug.LogError($"Clip {clipName} not found!");
        }
    }
    public void PlayOneShotSoundClip(string clipName, Vector3 position)
    {
        if (clipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            AudioSource.PlayClipAtPoint(clip, position);

        }
        else
        {
            Debug.LogError($"Clip {clipName} not found!");
        }
    }
    // Get an available AudioSource from the pool
    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = audioSourcePool.Dequeue();
        audioSourcePool.Enqueue(source); // Re-add it to the end of the queue
        return source;
    }
}

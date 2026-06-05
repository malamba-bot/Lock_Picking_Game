using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [Header("1. Pick Movement Clacks")]
    public List<AudioClip> pickMoveClacks; 
     public float pickMoveVol = 0.8f;
    public float pickMovePitchMin = 0.95f;
    public float pickMovePitchMax = 1.05f;
    private int lastClackIndex = -1; 



    [Header("2. Lock Error / Fail Sound")]
    public AudioClip lockErrorClip;
    public float errorVol = 0.5f;


    [Header("3. Sweet Spot Unlock")]
    public AudioClip unlockClip;
    public float unlockVol = 1f;


    [Header("4. Victory / Success")]
    public AudioClip victoryClip;
    public float victoryVol = 1f;


    public int maxSources = 8;
    private List<AudioSource> pool = new List<AudioSource>();

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        // loop and add components
        for (int i = 0; i < maxSources; i++) 
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            pool.Add(src);
            Debug.Log("AudioManager: Added AudioSource " + (i+1));
        }
    }


    // Helper method to look for a free source
    private AudioSource GetFreeSource() 
    {
        for (int i = 0; i < pool.Count; i++) {
            if (pool[i].isPlaying == false) { // manual check
                return pool[i];
            }
        }
        return pool[0]; // full? just grab first one lol
    }


    // Call this when pick moves
    public void PlayPickMove() {
        if (pickMoveClacks == null || pickMoveClacks.Count == 0) {
            Debug.LogWarning("AudioManager: No pick move clack clips assigned!");
            return;
        }

        AudioSource src = GetFreeSource();
        
        int randIdx = 0;
        if (pickMoveClacks.Count > 1) 
        {
            randIdx = Random.Range(0, pickMoveClacks.Count);
            
            if (randIdx == lastClackIndex) {
                randIdx = (randIdx + 1) % pickMoveClacks.Count;
            }
            lastClackIndex = randIdx;
        }

        src.clip = pickMoveClacks[randIdx];
        src.volume = pickMoveVol;
        src.pitch = Random.Range(pickMovePitchMin, pickMovePitchMax);
        src.Play();
    }



    public void PlayLockError() 
    {
        if (lockErrorClip == null) { 
            Debug.LogWarning("AudioManager: No lock error clip assigned!");
            return; 
        }
        
        AudioSource src = GetFreeSource();
        src.clip = lockErrorClip;
        src.volume = errorVol;
        src.pitch = 1f; 
        src.Play();

    }



    public void PlayUnlockSweetSpot() {
        if (unlockClip == null) return;

        AudioSource src = GetFreeSource();
        src.clip = unlockClip;
        src.volume = unlockVol;
        src.pitch = 1f;
        src.Play();
    }

    // victory fan fare
    public void PlayVictory() {
        if (victoryClip == null)
        {
            Debug.LogWarning("AudioManager: No victory clip assigned!");
            return;
        }

        AudioSource src = GetFreeSource();
        src.clip = victoryClip;
        src.volume = victoryVol;
        src.pitch = 1f;
        src.Play();
    }


    public void StopEverything() {
        foreach (var s in pool) {
            s.Stop();
        }
    }
}
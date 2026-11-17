using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.ShaderGraph.Legacy;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PoolManager))]
public class SystemSound : MonoBehaviour
{
    [Header("PoolManagers : ")]
    [Tooltip("PoolManager handeling Ambient sound layer")]
    public PoolManager poolManagerAmbient;
    [Tooltip("PoolManager handeling Ambient sound layer")]
    public PoolManager poolManagerFXs;
    [Tooltip("PoolManager handeling Ambient sound layer")]
    public PoolManager poolManagerVoices;
    private AudioSource _musicSource;
    public static SystemSound Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Reset the component needed in the gameObject.
    /// </summary>
    public virtual void Reset()
    {
        InitPoolManagers(SetListPoolManagers());

        InitMusicAudioSource();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private List<PoolManager> SetListPoolManagers()
    {
        List<PoolManager> poolManagers = GetComponents<PoolManager>().ToList<PoolManager>();

        if (poolManagers.Count < 3)
        {
            int difference = 3 - poolManagers.Count;
            for (int i = 0; i < difference; i++)
            {
                PoolManager instance = gameObject.AddComponent<PoolManager>();
                poolManagers.Add(instance);
            }
        }
        else if (poolManagers.Count > 3)
        {
            int difference = poolManagers.Count - 3;
            for (int i = 0; i <= difference; i++)
            {
                PoolManager instance = poolManagers[poolManagers.Count - 1];
                poolManagers.Remove(instance);
                Destroy(instance);
            }
        }
        return poolManagers;
    }

    /// <summary>
    /// Initialize the 3 different PoolManager.
    /// </summary>
    private void InitPoolManagers(List<PoolManager> poolManagers)
    {
        poolManagerAmbient = poolManagers[0];
        poolManagerAmbient.poolSize = 5;
        
        poolManagerFXs = poolManagers[1];
        poolManagerFXs.poolSize = 10;
    
        poolManagerVoices = poolManagers[2];
        poolManagerVoices.poolSize = 4;
    }

    /// <summary>
    /// Initialise Music AudioSource
    /// </summary>
    private void InitMusicAudioSource()
    {
        if (TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            _musicSource = audioSource;
        }
        else
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
        }
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
    }

    /// <summary>
    /// Global Function to Play a new sound with more parameters.
    /// </summary>
    /// <param name="group">Layer Sound Enum (AudioMixerGroup)</param>
    /// <param name="audioClip">Audio Source to play</param>
    /// <param name="volume">Volume simulating the distance</param>
    /// <param name="stereoValue">Play Sound : (Left -> -1) (Middle -> 0) (Right -> 1)</param>
    public void PlayNewSound(SoundGroup group,AudioClip audioClip, float volume = 1, float stereoValue = 0f)
    {
        switch(group)
        {
            case SoundGroup.FXs:
                {
                    PlaySound(audioClip, poolManagerFXs, volume, Mathf.Clamp01(stereoValue));
                    break;
                }

            case SoundGroup.Voices:
                {
                    PlaySound(audioClip, poolManagerVoices,volume, Mathf.Clamp01(stereoValue));
                    break;
                }
            case SoundGroup.Ambient:
                {
                    PlaySound(audioClip, poolManagerAmbient);
                    break;
                }

            case SoundGroup.Music:
                {
                    PlayMusic(audioClip);
                    break;
                }
        }
    }

    /// <summary>
    /// Convert distance to Volume (float [0,1]).
    /// </summary>
    /// <param name="distance">Distance between closest listener and sender.</param>
    /// <param name="maxDistanceInArena">Distance max reachable in the arena.</param>
    /// <returns>Float value between [0.1f, 1f]</returns>
    private static float DistanceToVolume(float distance, float maxDistanceInArena)
    {
        return Mathf.Lerp(0.1f, 1f, (100f*distance/maxDistanceInArena)/100f);
    }

    /// <summary>
    /// Play a sound using the PoolManager of the MixerGroup.
    /// </summary>
    /// <param name="audioClip">Audio Source to play</param>
    /// <param name="poolManager">PoolManager's MixerGroup.</param>
    /// <param name="volume">Volume simulating the distance</param>
    /// <param name="stereoValue">Play Sound : (Left -> -1) (Middle -> 0) (Right -> 1)</param>
    private void PlaySound(AudioClip audioClip, PoolManager poolManager, float volume = 1, float stereoValue = 0f)
    {
        GameObject instance = poolManager.PoolInstance();
        instance.transform.SetParent(transform);
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.resource = audioClip;
        audioSource.panStereo = stereoValue;
        audioSource.volume = volume;
        audioSource.PlayOneShot(audioClip);
        StartCoroutine(LifeTimeSound(instance, audioClip.length));
    }

    /// <summary>
    /// Play Music sound using the Music MixerGroup.
    /// </summary>
    /// <param name="audioClip">Audio Source to play</param>
    private void PlayMusic(AudioClip audioClip)
    {
        _musicSource.resource = audioClip;
        _musicSource.Play();
    }

    /// <summary>
    /// Waiting for the sound to end before unable the audio source instance.
    /// </summary>
    /// <param name="instance">audio source instance</param>
    /// <param name="duration">sound duration / length</param>
    /// <returns>Null</returns>
    private IEnumerator LifeTimeSound(GameObject instance, float duration)
    {
        yield return new WaitForSeconds(duration);
        instance.SetActive(false);
    }
}

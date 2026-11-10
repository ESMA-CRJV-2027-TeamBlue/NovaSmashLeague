using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PoolManager))]
public class SystemSound : MonoBehaviour
{
    [Header("PoolManagers : ")]
    [Tooltip("PoolManager handeling Ambient sound layer")]
    public PoolManager _poolManagerAmbient;
    [Tooltip("PoolManager handeling Ambient sound layer")]
    public PoolManager _poolManagerFXs;
    [Tooltip("PoolManager handeling Ambient sound layer")]
    public PoolManager _poolManagerVoices;
    private AudioSource _musicSource;

    /// <summary>
    /// Reset the component needed in the gameObject.
    /// </summary>
    public virtual void Reset()
    {
        PoolManager[] poolManagers = GetComponents<PoolManager>();

        if (poolManagers.Length < 3)
        {
            int difference = 3 - poolManagers.Length;
            for (int i = 0; i <= difference; i++)
            {
                InitPoolManager();
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    _poolManagerAmbient = poolManagers[i];
                }
                else if(i == 1)
                {
                    _poolManagerFXs = poolManagers[i];
                }
                else if( i == 2)
                {
                    _poolManagerVoices = poolManagers[i];
                }
            }
        }

        if (TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            _musicSource = audioSource;
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
        }
        else
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
        }
    }

    /// <summary>
    /// Initialize the 3 different PoolManager.
    /// </summary>
    private void InitPoolManager()
    {
        if (_poolManagerAmbient == null)
        {
            _poolManagerAmbient = gameObject.AddComponent<PoolManager>();
            _poolManagerAmbient.poolSize = 5;
        }
        else if (_poolManagerFXs == null)
        {
            _poolManagerFXs = gameObject.AddComponent<PoolManager>();
            _poolManagerFXs.poolSize = 10;
        }
        else if (_poolManagerVoices == null)
        {
            _poolManagerVoices = gameObject.AddComponent<PoolManager>();
            _poolManagerVoices.poolSize = 4;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _musicSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Global Function to Play a new sound with more parameters.
    /// </summary>
    /// <param name="group">Layer Sound Enum (AudioMixerGroup)</param>
    /// <param name="audioClip">Audio Source to play</param>
    /// <param name="volume">Volume simulating the distance</param>
    /// <param name="stereoValue">Play Sound : (Left -> -1) (Middle -> 0) (Right -> 1)</param>
    public void PlayNewSound(SoundGroup group,AudioClip audioClip, float volume, float stereoValue)
    {
        switch(group)
        {
            case SoundGroup.FXs:
                {
                    PlayFXsSound(audioClip, volume, Mathf.Clamp01(stereoValue));
                    break;
                }

            case SoundGroup.Voices:
                {
                    PlayVoices(audioClip, volume, Mathf.Clamp01(stereoValue));
                    break;
                }
        }
    }

    /// <summary>
    /// Global Function to Play a new sound.
    /// </summary>
    /// <param name="group">Layer Sound Enum (AudioMixerGroup)</param>
    /// <param name="audioClip">Audio Source to play</param>
    public void PlayNewSound(SoundGroup group, AudioClip audioClip)
    {
        switch (group)
        {
            case SoundGroup.Ambient:
                {
                    PlayAmbientSound(audioClip);
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
    /// Play an Ambient sound using the poolManagerAmbient and the Ambient MixerGroup.
    /// </summary>
    /// <param name="audioClip">Audio Source to play</param>
    private void PlayAmbientSound(AudioClip audioClip)
    {
        GameObject instance = _poolManagerAmbient.PoolInstance();
        instance.transform.SetParent(transform);
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.resource = audioClip;
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
    /// Play Voice lane sound using the poolManagerVoices and the Voices MixerGroup.
    /// </summary>
    /// <param name="audioClip">Audio Source to play</param>
    /// <param name="volume">Volume simulating the distance</param>
    /// <param name="stereoValue">Play Sound : (Left -> -1) (Middle -> 0) (Right -> 1)</param>
    private void PlayVoices(AudioClip audioClip, float volume, float stereoValue)
    {
        GameObject instance = _poolManagerVoices.PoolInstance();
        instance.transform.SetParent(transform);
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.resource = audioClip;
        audioSource.panStereo = stereoValue;
        audioSource.volume = volume;
        audioSource.PlayOneShot(audioClip);
        StartCoroutine(LifeTimeSound(instance, audioClip.length));
    }
    /// <summary>
    /// Play FX sound using the poolManagerFXs and the Effects MixerGroup.
    /// </summary>
    /// <param name="audioClip">Audio Source to play</param>
    /// <param name="volume">Volume simulating the distance</param>
    /// <param name="stereoValue">Play Sound : (Left -> -1) (Middle -> 0) (Right -> 1)</param>
    private void PlayFXsSound(AudioClip audioClip, float volume, float stereoValue)
    {
        GameObject instance = _poolManagerFXs.PoolInstance();
        instance.transform.SetParent(transform);
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.resource = audioClip;
        audioSource.panStereo = stereoValue;
        audioSource.volume = volume;
        audioSource.PlayOneShot(audioClip);
        StartCoroutine(LifeTimeSound(instance, audioClip.length));
    }

    /// <summary>
    /// Waiting for the sound to end before unable the audio source instance.
    /// </summary>
    /// <param name="instance">audio source instance</param>
    /// <param name="duration">sound duration / length</param>
    /// <returns></returns>
    private IEnumerator LifeTimeSound(GameObject instance, float duration)
    {
        yield return new WaitForSeconds(duration);
        instance.SetActive(false);
    }
}

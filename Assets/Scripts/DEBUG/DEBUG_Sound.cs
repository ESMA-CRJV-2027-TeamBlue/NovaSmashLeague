using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[RequireComponent(typeof(SystemSound))]
public class DEBUG_Sound : MonoBehaviour
{
    public AudioClip audioMusic;
    public List<AudioClip> audioFX;
    public List<AudioClip> audioAmbient;
    public List<AudioClip> audioVoices;
    private SystemSound _systemSound;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _systemSound = GetComponent<SystemSound>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _systemSound.PlayNewSound(SoundGroup.FXs,audioFX[Random.Range(0,audioFX.Count-1)], Random.Range(0.1f,1f), Random.Range(-1f, 1f));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _systemSound.PlayNewSound(SoundGroup.Voices,audioVoices[Random.Range(0,audioVoices.Count-1)],Random.Range(0.1f,1f),Random.Range(-1f,1f));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _systemSound.PlayNewSound(SoundGroup.Ambient,audioAmbient[Random.Range(0,audioAmbient.Count-1)]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _systemSound.PlayNewSound(SoundGroup.Music,audioMusic);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
        }
    }
}

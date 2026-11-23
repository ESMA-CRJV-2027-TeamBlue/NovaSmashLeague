using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[RequireComponent(typeof(SystemSound))]
public class DEBUG_Sound : MonoBehaviour
{
    [Tooltip("Music to Play")]
    public AudioClip audioMusic;
    [Tooltip("List FXs to play")]
    public List<AudioClip> audioFX;
    [Tooltip("List ambient sound to play")]
    public List<AudioClip> audioAmbient;
    [Tooltip("List Voices lane to play")]
    public List<AudioClip> audioVoices;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SystemSound.Instance.PlayNewSound(SoundGroup.FXs,audioFX[Random.Range(0,audioFX.Count-1)], Random.Range(0.1f,1f), Random.Range(-1f, 1f));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SystemSound.Instance.PlayNewSound(SoundGroup.Voices,audioVoices[Random.Range(0,audioVoices.Count-1)],Random.Range(0.1f,1f),Random.Range(-1f,1f));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SystemSound.Instance.PlayNewSound(SoundGroup.Ambient,audioAmbient[Random.Range(0,audioAmbient.Count-1)]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SystemSound.Instance.PlayNewSound(SoundGroup.Music,audioMusic);
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

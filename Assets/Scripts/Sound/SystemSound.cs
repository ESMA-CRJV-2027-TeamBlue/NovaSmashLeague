using Unity.VisualScripting;
using UnityEngine;

public class SystemSound : MonoBehaviour
{
    private AudioSource _audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void PlaySoundLeft(AudioClip audioClip)
    {
        _audioSource.panStereo = -1;
        _audioSource.PlayOneShot(audioClip);
    }

    private void PlaySoundRight(AudioClip audioClip)
    {
        _audioSource.panStereo = 1;
        _audioSource.PlayOneShot(audioClip);
    }

    public void PlaySoundBoth(AudioClip audioClip)
    {
        _audioSource.volume = 1f;

        _audioSource.panStereo = 0;
        _audioSource.PlayOneShot(audioClip);
    }

    public void PlayTeamSound(Team team, AudioClip audioClip, float distance)
    {
        _audioSource.volume = Mathf.Clamp01(distance);
        
        if (team == Team.TeamA) PlaySoundLeft(audioClip);
        else if (team == Team.TeamB) PlaySoundRight(audioClip);
    }
}

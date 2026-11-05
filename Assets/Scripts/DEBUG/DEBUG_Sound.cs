using UnityEngine;

[RequireComponent(typeof(SystemSound))]
public class DEBUG_Sound : MonoBehaviour
{
    public AudioClip audioClip;
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
            _systemSound.PlayTeamSound(Team.TeamA, audioClip, 0.25f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _systemSound.PlayTeamSound(Team.TeamA,audioClip, 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _systemSound.PlayTeamSound(Team.TeamA,audioClip, 0.75f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _systemSound.PlayTeamSound(Team.TeamA,audioClip, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _systemSound.PlaySoundBoth(audioClip);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _systemSound.PlaySoundBoth(audioClip);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _systemSound.PlayTeamSound(Team.TeamB, audioClip, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _systemSound.PlayTeamSound(Team.TeamB, audioClip, 0.75f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _systemSound.PlayTeamSound(Team.TeamB, audioClip, 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _systemSound.PlayTeamSound(Team.TeamB, audioClip, 0.25f);
        }
    }
}

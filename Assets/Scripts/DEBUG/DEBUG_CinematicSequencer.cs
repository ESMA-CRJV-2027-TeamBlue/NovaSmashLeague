using System;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_CinematicSequencer : MonoBehaviour
{
    [Tooltip("The cinematic player to debug")]
    public CinematicPlayer cinematicPlayer;
    [Tooltip("The cinematics to play")]
    public List<Cinematic> cinematics;

    private void Start()
    {
        cinematicPlayer.OnCinematicQueueDepleted += () => Debug.Log("Cinematic queue depleted");
        cinematicPlayer.OnCinematicEnded += () => Debug.Log("Cinematic ended");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Debug1"))
        {
            cinematicPlayer.PlayCinematic(cinematics[0]);
        }
        else if (Input.GetButtonDown("Debug2"))
        {
            cinematicPlayer.PlayCinematic(cinematics[1]);
        }
        else if (Input.GetButtonDown("Debug3"))
        {
            cinematicPlayer.EnqueueCinematics(cinematics);
        }
    }
}

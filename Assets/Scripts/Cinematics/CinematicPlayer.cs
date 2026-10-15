using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class Cinematic
{
    public GameObject prefab;
    public Transform pivot;
}

public class CinematicPlayer : MonoBehaviour
{
    public event Action OnCinematicQueueDepleted;
    public event Action OnCinematicEnded;
    
    private PlayableDirector _currentCinematic;
    private Queue<Cinematic> _cinematicQueue = new Queue<Cinematic>();
    
    /// <summary>
    /// Uses a Cinematic to instantiate a prefab that has a PlayableDirector component, and plays it from the pivot point.
    /// If the cinematic has no pivot, use this script's gameobject instead
    /// If a cinematic is already playing, enqueues the cinematic instead.
    /// </summary>
    /// <param name="cinematic">The Cinematic to play</param>
    public void PlayCinematic(Cinematic cinematic)
    {
        // Don't interrupt current cinematic
        if (_currentCinematic)
        {
            _cinematicQueue.Enqueue(cinematic);
            return;
        }

        if (cinematic.pivot == null) cinematic.pivot = transform;
        
        _currentCinematic = Instantiate(cinematic.prefab, cinematic.pivot).GetComponent<PlayableDirector>();
        if (_currentCinematic == null)
        {
            Debug.LogError($"Cinematic prefab '{cinematic.prefab.name}' has no PlayableDirector component!");
            return;
        }
        _currentCinematic.stopped += OnCinematicStopped;
        _currentCinematic.Play();
    }

    /// <summary>
    /// Handle the end of a cinematic. Destroy related gameobject and start the next in queue. Emits an OnCinematicEnded event
    /// </summary>
    /// <param name="_playableDirector">Required param from api, not used</param>
    private void OnCinematicStopped(PlayableDirector _playableDirector)
    {
        // Check in case it triggered from editor close 
        if (_currentCinematic)
        {
            Destroy(_currentCinematic.gameObject);
            _currentCinematic = null;
        }
        OnCinematicEnded?.Invoke();
        PlayNextCinematicInQueue();
    }

    /// <summary>
    /// Set the cinematic queue to play a list of cinematics from prefabs with a PlayableDirector component.
    /// Automatically starts to play the first cinematic.
    /// </summary>
    /// <param name="cinematicPrefabs">The list of prefabs that need to contain a PlayableDirector component</param>
    public void EnqueueCinematics(List<Cinematic> cinematics)
    {
        _cinematicQueue = new Queue<Cinematic>(cinematics);
        PlayNextCinematicInQueue();
    }

    /// <summary>
    /// Try to launch the next cinematic in queue.
    /// If there are no cinematic left, emits a CinematicQueueDepleted event instead
    /// </summary>
    private void PlayNextCinematicInQueue()
    {
        if (_cinematicQueue.Count == 0)
        {
            OnCinematicQueueDepleted?.Invoke();
            return;
        }
        
        PlayCinematic(_cinematicQueue.Dequeue());
    }
}

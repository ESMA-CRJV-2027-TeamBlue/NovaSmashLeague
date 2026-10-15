using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicPlayer : MonoBehaviour
{
    public event Action currentCinematicStopped;
    public event Action cinematicQueueDepleted;
    
    private PlayableDirector _currentCinematic;
    private Queue<GameObject> _cinematicPrefabQueue;
    
    /// <summary>
    /// Instantiates a prefab that has a PlayableDirector component, and plays it.
    /// Emits a signal when the cinematic ends
    /// </summary>
    /// <param name="prefab">The prefab that has a PlayableDirector component</param>
    public void PlayCinematicFromPrefab(GameObject prefab)
    {
        _currentCinematic = Instantiate(prefab, transform).GetComponent<PlayableDirector>();
        _currentCinematic.stopped += OnCinematicStopped;
        _currentCinematic.Play();
    }

    /// <summary>
    /// Handle the end of a cinematic
    /// </summary>
    /// <param name="_playableDirector"></param>
    public void OnCinematicStopped(PlayableDirector _playableDirector)
    {
        currentCinematicStopped?.Invoke();
        Destroy(_currentCinematic.gameObject);
    }

    public void EnqueueCinematics(List<GameObject> cinematicPrefabs)
    {
        _cinematicPrefabQueue = new Queue<GameObject>(cinematicPrefabs);
        PlayNextCinematicInQueue();
        PlayCinematicFromPrefab(_cinematicPrefabQueue.Dequeue());
    }

    /// <summary>
    /// Try to launch the next cinematic in queue.
    /// If there are no cinematic left, emits a CinematicQueueDepleted event
    /// </summary>
    public void PlayNextCinematicInQueue()
    {
        if (_cinematicPrefabQueue.Count == 0)
        {
            cinematicQueueDepleted?.Invoke();
            return;
        }
        
        PlayCinematicFromPrefab(_cinematicPrefabQueue.Dequeue());
    }
}

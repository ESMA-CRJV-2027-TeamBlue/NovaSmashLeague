using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public class SpawnsManager : MonoBehaviour
{
    #region Players Spawn
    [Header("Players Spawn :")]
    [Tooltip("List of Players Gameobject playing.")]
    private List<GameObject> _players = new List<GameObject>();
    [Tooltip("List of Spawning points fo Players.")]
    [SerializeField] private List<Transform> _spawnsPlayer;
    #endregion

    #region Ball Spawn
    [Header("Ball Spawn :")]
    [Tooltip("Ball Gameobject.")]
    [SerializeField] private GameObject _Ball;
    [Tooltip("Spawning Point of the Ball.")]
    [SerializeField] private Transform _spawnBall;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindPlayers();
        SpawnPlayers();
        SpawnBall();
    }

    #region Players
    /// <summary>
    /// Find all the players playing into the scene
    /// </summary>
    private void FindPlayers()
    {
        for (int i = 1; i < 5; i++)
        {
            string tag = "Player " + i.ToString();
            _players.Add(GameObject.FindWithTag(tag));
        }
    }

    /// <summary>
    /// Give a new Position Randomly between all spawns in the scene.
    /// </summary>
    private void SpawnPlayers()
    {
        List<Transform> tmpListSpawn = _spawnsPlayer;
        foreach (GameObject player in _players)
        {
            int randomindex = Random.Range(0, tmpListSpawn.Count);
            player.transform.position = tmpListSpawn[randomindex].position;
            player.transform.rotation = tmpListSpawn[randomindex].rotation;
            tmpListSpawn.Remove(tmpListSpawn[randomindex]);
        }
    }
    #endregion

    #region Ball
    /// <summary>
    /// Spawn the ball
    /// </summary>
    private void SpawnBall()
    {
        GameObject.Instantiate(_Ball, _spawnBall.position, _spawnBall.rotation);
    }
    #endregion
}

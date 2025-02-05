using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("Race Settings")]
    public Transform[] checkpoints; // Array of all checkpoints in order
    public int totalLaps = 3; // Number of laps to complete the race

    [Header("Player Tracking")]
    private Dictionary<GameObject, PlayerProgress> playerProgress = new Dictionary<GameObject, PlayerProgress>();

    // Structure to track player progress
    private class PlayerProgress
    {
        public int currentCheckpoint = 0;
        public int currentLap = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only handle objects tagged as "Player"
        if (!other.CompareTag("Player")) return;

        GameObject player = other.gameObject;

        // Initialize progress if the player is new
        if (!playerProgress.ContainsKey(player))
        {
            playerProgress[player] = new PlayerProgress();
        }

        // Get the player's current progress
        PlayerProgress progress = playerProgress[player];

        // Check if the player reached the correct checkpoint
        if (other.transform == checkpoints[progress.currentCheckpoint])
        {
            progress.currentCheckpoint++;

            // If the player clears all checkpoints in the lap
            if (progress.currentCheckpoint >= checkpoints.Length)
            {
                progress.currentCheckpoint = 0; // Reset checkpoint to the start
                progress.currentLap++;

                if (progress.currentLap >= totalLaps)
                {
                    Debug.Log($"{player.name} has finished the race!");
                }
                else
                {
                    Debug.Log($"{player.name} completed Lap {progress.currentLap}!");
                }
            }
        }
    }

    public int GetPlayerLap(GameObject player)
    {
        if (playerProgress.ContainsKey(player))
        {
            return playerProgress[player].currentLap;
        }
        return 0;
    }

    public int GetPlayerCheckpoint(GameObject player)
    {
        if (playerProgress.ContainsKey(player))
        {
            return playerProgress[player].currentCheckpoint;
        }
        return 0;
    }

    public bool HasPlayerFinished(GameObject player)
    {
        if (playerProgress.ContainsKey(player))
        {
            return playerProgress[player].currentLap >= totalLaps;
        }
        return false;
    }
}

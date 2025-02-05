using System.Collections.Generic;
using UnityEngine;

public class TrackCheckPointHolder : MonoBehaviour
{
    public static TrackCheckPointHolder instance;

    [Header("Checkpoints & Laps")]
    public GameObject[] CheckPoint; // Array of checkpoints in the track
    public int TrackTotalLap; // Total laps required to complete the race

    [Header("Players")]
    public List<GameObject> playerBatch = new List<GameObject>(); // List of players

    private void Awake()
    {
        // Singleton pattern for global access
        instance = this;

        // Find all players with the "Player" tag and add them to the playerBatch list
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        playerBatch.AddRange(players);
    }

    private void Update()
    {
        // Sort players by lap count, then by distance to the next checkpoint
        playerBatch.Sort((a, b) =>
        {
            playerLapCounter playerA = a.GetComponent<playerLapCounter>();
            playerLapCounter playerB = b.GetComponent<playerLapCounter>();

            // Compare by lap count (descending)
            int lapComparison = playerB.currentLap.CompareTo(playerA.currentLap);
            if (lapComparison != 0)
            {
                return lapComparison;
            }

            // Compare by checkpoint count (descending)
            int checkpointComparison = playerB.currentCheckpoint.CompareTo(playerA.currentCheckpoint);
            if (checkpointComparison != 0)
            {
                return checkpointComparison;
            }

            // Compare by distance to the next checkpoint (ascending)
            Transform nextCheckpointA = CheckPoint[playerA.currentCheckpoint].transform;
            Transform nextCheckpointB = CheckPoint[playerB.currentCheckpoint].transform;

            float distanceA = Vector3.Distance(a.transform.position, nextCheckpointA.position);
            float distanceB = Vector3.Distance(b.transform.position, nextCheckpointB.position);

            return distanceA.CompareTo(distanceB);
        });

        // Debugging: Print sorted players' order
        foreach (GameObject player in playerBatch)
        {
            playerLapCounter playerInfo = player.GetComponent<playerLapCounter>();
            Debug.Log($"Player {player.name}: Lap {playerInfo.currentLap}, Checkpoint {playerInfo.currentCheckpoint}");
        }

        // Update player positions
        UpdatePlayerPositions();
    }

    private void UpdatePlayerPositions()
    {
        for (int i = 0; i < playerBatch.Count; i++)
        {
            GameObject player = playerBatch[i];
            playerLapCounter playerCode = player.GetComponent<playerLapCounter>();

            // Update player's current position in the race
            playerCode.playerCurrentPlace = i + 1;

            // Debugging: Log the player's position
            Debug.Log($"Player {player.name}: Position {playerCode.playerCurrentPlace}");
        }
    }
}

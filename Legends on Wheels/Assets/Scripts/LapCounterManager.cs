using UnityEngine;

public class LapCounter : MonoBehaviour
{
    public int currentLap = 0;
    public int totalLaps = 3;
    private int checkpointIndex = 0;
    public Transform[] checkpoints; // Set these in the Inspector
    private bool lapComplete = false;

    void OnTriggerEnter(Collider other)
    {
        // Start/Finish Line Logic
        if (other.CompareTag("StartFinish"))
        {
            if (lapComplete && checkpointIndex == checkpoints.Length)
            {
                currentLap++;
                checkpointIndex = 0; // Reset checkpoint index
                lapComplete = false;

                Debug.Log($"Lap {currentLap} complete!");
                if (currentLap > totalLaps)
                {
                    Debug.Log("Race Finished!");
                }
            }
        }

        // Checkpoint Logic
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (other.transform == checkpoints[i])
            {
                if (i == checkpointIndex) // Correct checkpoint order
                {
                    checkpointIndex++;
                    Debug.Log($"Checkpoint {checkpointIndex} passed!");

                    if (checkpointIndex == checkpoints.Length)
                    {
                        lapComplete = true; // All checkpoints passed
                    }
                }
                break;
            }
        }
    }
}

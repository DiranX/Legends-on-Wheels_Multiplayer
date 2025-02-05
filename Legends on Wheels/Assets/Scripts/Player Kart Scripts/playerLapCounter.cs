using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerLapCounter : MonoBehaviour
{
    public int currentCheckpoint, currentLap, totalLap;

    public GameObject[] checkPoint;

    public TextMeshProUGUI lapCounter;

    private void Awake()
    {
        GameObject TrackCheckPoint = GameObject.Find("CheckPoint");

        if(TrackCheckPoint != null)
        {
            checkPoint = TrackCheckPoint.GetComponent<TrackCheckPointHolder>().CheckPoint;
            totalLap = TrackCheckPoint.GetComponent<TrackCheckPointHolder>().TrackTotalLap;
        }
    }

    private void Start()
    {
        lapCounter.text = currentLap.ToString() + "/" + totalLap.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == checkPoint[currentCheckpoint])
        {
            currentCheckpoint++;
            if (currentCheckpoint == checkPoint.Length)
            {
                currentCheckpoint = 0;
                currentLap++;
                lapCounter.text = currentLap.ToString() + "/" + totalLap.ToString();
            }

            if (currentLap >= totalLap)
            {
                Debug.Log("Win");
            }
        }
    }
}

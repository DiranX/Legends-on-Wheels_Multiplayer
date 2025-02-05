using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class playerLapCounter : MonoBehaviour
{
    [Header("Lap and CheckPoint")]
    public int currentCheckpoint;
    public int currentLap;
    public int totalLap;
    public GameObject[] checkPoint;
    public TextMeshProUGUI lapCounter;
    public TextMeshProUGUI PlacementCounter;
    public int playerCurrentPlace;

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
    private void Update()
    {
        PlacementCounter.text = playerCurrentPlace.ToString();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == checkPoint[currentCheckpoint])
        {
            this.currentCheckpoint++;
            if (currentCheckpoint == checkPoint.Length)
            {
                this.currentCheckpoint = 0;
                this.currentLap++;
                this.lapCounter.text = currentLap.ToString() + "/" + totalLap.ToString();
            }

            if (currentLap >= totalLap)
            {
                Debug.Log("Win");
            }
        }
    }
}

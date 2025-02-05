using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckPointHolder : MonoBehaviour
{
    public static TrackCheckPointHolder instance;
    public GameObject[] CheckPoint;
    public int TrackTotalLap;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
}

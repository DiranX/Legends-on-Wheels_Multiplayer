using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KartAnimation : MonoBehaviour
{
    Animator anim;
    PlayerKartController playerKart;

    public Transform kartModel;
    public Transform frontWheels;
    public Transform steeringWheel;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        playerKart = GetComponent<PlayerKartController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update acceleration and backward animation
        if (playerKart.moveForward)
        {
            anim.SetBool("Accelerate", true);
            anim.SetBool("Backward", false);
        }
        else
        {
            anim.SetBool("Accelerate", false);
        }

        if (playerKart.moveBackward)
        {
            anim.SetBool("Accelerate", false);
            anim.SetBool("Backward", true);
        }
        else
        {
            anim.SetBool("Backward", false);
        }

        if(playerKart.drifting && Input.GetAxis("Horizontal") != 0)
        {
            float control = (playerKart.driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, .5f, 2) : ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 2, .5f);
            kartModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(kartModel.parent.localEulerAngles.y, (control * 15) * playerKart.driftDirection, .2f), 0);
        }

        // Update front wheels and steering wheel based on input
        frontWheels.localEulerAngles = new Vector3(0, (Input.GetAxis("Horizontal") * 20), frontWheels.localEulerAngles.z);
        steeringWheel.localEulerAngles = new Vector3(-25, 90, ((Input.GetAxis("Horizontal") * 45)));
    }
}

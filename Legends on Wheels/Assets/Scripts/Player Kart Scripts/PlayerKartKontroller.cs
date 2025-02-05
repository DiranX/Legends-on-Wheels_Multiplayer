using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerKartController : MonoBehaviour
{
    PlayerKartInput playerKartInput;
    public bool moveForward;
    public bool moveBackward;
    public bool drift;
    public bool drifting;
    public float kartYPosition;
    public Transform kartNormal;
    public Transform kartModel;

    public Rigidbody sphere;
    float speed, currentSpeed;
    float rotate, currentRotate;
    public int driftDirection;
    float driftPower;
    int driftMode = 0;
    bool first, second, third;

    [Header("Parameters")]
    public float acceleration;
    public float steering;
    public float gravity;
    public LayerMask layerMask;

    [Header("Drift Parameters")]
    public float level1Threshold = 50f;
    public float level2Threshold = 100f;
    public float level3Threshold = 150f;

    public float level1Boost = 20f;
    public float level2Boost = 40f;
    public float level3Boost = 60f;

    public float boostDuration = 2f;
    private float boostTimer = 0f;

    public float driftAssistStrength = 0.5f; // Strength of steering assist during drift
    public float driftAssistAngle = 15f; // Angle threshold for drift assist activation

    [Header("Drift Particle")]
    public GameObject[] wheelsParticle;
    public ParticleSystem[] particleSystem;
    public ParticleSystem[] flareParticle;
    public ParticleSystem[] boostParticle;
    public Color newColor;
    public Color[] driftColor;

    void Awake()
    {
        playerKartInput = new PlayerKartInput();

        playerKartInput.PlayerKart.Forward.started += MoveForward;
        playerKartInput.PlayerKart.Forward.canceled += MoveForward;
        playerKartInput.PlayerKart.Forward.performed += MoveForward;

        playerKartInput.PlayerKart.Backward.started += MoveBackWard;
        playerKartInput.PlayerKart.Backward.canceled += MoveBackWard;
        playerKartInput.PlayerKart.Backward.performed += MoveBackWard;

        playerKartInput.PlayerKart.Drift.started += Drift;
        playerKartInput.PlayerKart.Drift.canceled += Drift;
        playerKartInput.PlayerKart.Drift.performed += Drift;
    }

    void Update()
    {
        Debug.Log("Current Speed: " + currentSpeed);

        if (moveForward && !moveBackward)
        {
            speed = acceleration;
        }
        if (moveBackward && !moveForward)
        {
            speed = -acceleration;
        }

        // Steer
        if (Input.GetAxis("Horizontal") != 0)
        {
            int dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            float amount = Mathf.Abs((Input.GetAxis("Horizontal")));
            Steer(dir, amount);
        }

        if (drift && !drifting && Input.GetAxis("Horizontal") != 0)
        {
            drifting = true;
            driftDirection = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
        }

        if (drifting)
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 0, 2) : ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 2, 0);
            float powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, .2f, 1) : ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 1, .2f);
            Steer(driftDirection, control);
            driftPower += powerControl * Time.deltaTime * 100f;

            wheelsParticle[0].SetActive(true);
            wheelsParticle[1].SetActive(true);

            // Only update the color if it changes
            if (driftPower >= level3Threshold && newColor != driftColor[2])
            {
                newColor = driftColor[2];
                ChangeParticleColor();
                PlayFlareParticle();
            }
            else if (driftPower >= level2Threshold && newColor != driftColor[1] && driftPower < level3Threshold)
            {
                newColor = driftColor[1];
                ChangeParticleColor();
                PlayFlareParticle();
            }
            else if (driftPower >= level1Threshold && newColor != driftColor[0] && driftPower < level2Threshold)
            {
                newColor = driftColor[0];
                ChangeParticleColor();
                PlayFlareParticle();
            }
        }


        if (!drift && drifting)
        {
            Boost();
            wheelsParticle[0].SetActive(false);
            wheelsParticle[1].SetActive(false);
        }

        transform.position = sphere.transform.position - new Vector3(0, kartYPosition, 0);

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);
        speed = 0f;

        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f);
        rotate = 0f;
    }

    private void FixedUpdate()
    {
        sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        if (moveForward || moveBackward)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);
        }

        // Apply Boost Timer
        if (boostTimer > 0)
        {
            boostTimer -= Time.fixedDeltaTime;

            if (boostTimer <= 0)
            {
                currentSpeed = Mathf.Max(currentSpeed - level3Boost, acceleration);
            }
        }

        // Apply Drift Assist
        if (drifting)
        {
            ApplyDriftAssist();
        }

        AlignKartToGround();
    }

    public void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }

    public void Boost()
    {
        drifting = false;

        if (driftPower >= level3Threshold)
        {
            driftMode = 3;
            StartBoost(level3Boost);
        }
        else if (driftPower >= level2Threshold)
        {
            driftMode = 2;
            StartBoost(level2Boost);
        }
        else if (driftPower >= level1Threshold)
        {
            driftMode = 1;
            StartBoost(level1Boost);
        }
        else
        {
            driftMode = 0; // No boost
        }

        kartModel.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);

        newColor = driftColor[3];
        ChangeParticleColor();
        PlayBoostParticle();

        driftPower = 0;
    }

    private void StartBoost(float boostAmount)
    {
        currentSpeed += boostAmount;
        boostTimer = boostDuration;
    }

    private void ApplyDriftAssist()
    {
        float driftAngle = Vector3.Angle(transform.forward, sphere.velocity);
        if (driftAngle > driftAssistAngle)
        {
            float assistForce = Mathf.Clamp(driftAssistStrength * (driftAngle / 90f), 0f, 1f);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + assistForce * steering, 0), Time.deltaTime * 4f);
        }
    }

    void MoveForward(InputAction.CallbackContext context)
    {
        moveForward = context.ReadValueAsButton();
    }

    void MoveBackWard(InputAction.CallbackContext context)
    {
        moveBackward = context.ReadValueAsButton();
    }

    void Drift(InputAction.CallbackContext context)
    {
        drift = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        playerKartInput.PlayerKart.Enable();
    }

    private void OnDisable()
    {
        playerKartInput.PlayerKart.Disable();
    }

    private void AlignKartToGround()
    {
        RaycastHit hitOn, hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, .5f, layerMask);

        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    void ChangeParticleColor()
    {
        foreach (var ps in particleSystem)
        {
            if (ps == null) continue;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(newColor, 0.0f), new GradientColorKey(newColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        }
    }

    void PlayFlareParticle()
    {
        flareParticle[0].Play();
        flareParticle[1].Play();
        flareParticle[2].Play();
        flareParticle[3].Play();
    }

    void PlayBoostParticle()
    {
        foreach (ParticleSystem boost in boostParticle)
        {
            boost.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    }
}

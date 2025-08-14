using UnityEngine;
using UnityEngine.UI; // Import the UI namespace to use UI elements
using UnityEngine.SceneManagement; // Import the SceneManagement namespace to manage scenes
using System.Collections;
using TMPro;
public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float baseAcceleration = 800f;
    public float brakingForce = 1000f;
    public float steeringSpeed = 80f;
    public float baseMaxSpeed = 50f;
    [SerializeField] float bombSpeed = 25f;
    [SerializeField] float maxBombTime = 30f;
    [SerializeField] [Range(0f, 1f)] float slowTimeScale = 0.5f;
    [SerializeField] float maxNitrousDuration = 10f;
    [SerializeField] float nitrousAccelerationBoost = 2f;
    [SerializeField] float nitrousMaxSpeed = 100f;

    [Header("UI - Analog Needle")]
    public RectTransform needleTransform; 
    public float minNeedleAngle = 225f; 
    public float maxNeedleAngle = -45f; 

    private Rigidbody rb; [SerializeField] int maxTimeOrb = 2; // This is the maximum time orb of the
    private Coroutine slowDownCoroutine;
    private Coroutine speedUpCoroutine;
    public float playerSlowSpeed = 1f; // Reduced speed of the player movement
    public float horizontalSlowSpeed = 1.5f;

    //[SerializeField] TMP_Text timeOrbText;
    //[SerializeField] TMP_Text speedText;
    private int timeOrb = 0; // This is the time orb of the player

    [Header("UI - Digital")]
    public TextMeshProUGUI speedText;

    public Vector3 Velocity => rb.linearVelocity;

    float bombTimer;
    float currentAcceleration;
    float currentMaxSpeed;
    float nitrousTime;
    bool pressingBoost;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        bombTimer = maxBombTime;
        currentAcceleration = baseAcceleration;
        currentMaxSpeed = baseMaxSpeed;
        nitrousTime = maxNitrousDuration;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        UpdateSpeedometer();
        if (Velocity.magnitude < bombSpeed) bombTimer -= Time.fixedDeltaTime;
        if (bombTimer <= 0f) Explode();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");

        if (moveInput > 0)
        {
            rb.AddForce(transform.forward * moveInput * currentAcceleration * Time.fixedDeltaTime);
        }
        else if (moveInput < 0)
        {
            if (rb.linearVelocity.magnitude > 0.5f && Vector3.Dot(rb.linearVelocity, transform.forward) > 0)
            {
                rb.AddForce(-rb.linearVelocity.normalized * brakingForce * Time.fixedDeltaTime);
            }
            else
            {
                rb.AddForce(transform.forward * moveInput * currentAcceleration * Time.fixedDeltaTime);
            }
        }

        pressingBoost = Input.GetKey(KeyCode.Space);
        if (pressingBoost && nitrousTime > 0f)
        {
            nitrousTime -= Time.fixedDeltaTime;
            if (currentAcceleration != baseAcceleration * nitrousAccelerationBoost) currentAcceleration = baseAcceleration * nitrousAccelerationBoost;
            if (currentMaxSpeed != nitrousMaxSpeed) currentMaxSpeed = nitrousMaxSpeed;
        }
        else
        {
            if (currentAcceleration != baseAcceleration) currentAcceleration = baseAcceleration;
            if (currentMaxSpeed != baseMaxSpeed) currentMaxSpeed = baseMaxSpeed;
        }

        if (rb.linearVelocity.magnitude > currentMaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentMaxSpeed;
        }
    }
        

    void HandleSteering()
    {
        float steerInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerInput * steeringSpeed * Time.fixedDeltaTime);
    }

    void UpdateSpeedometer()
    {
        float speed = rb.linearVelocity.magnitude * 3.6f;
        if (speedText != null)
            speedText.text = Mathf.RoundToInt(speed) + " km/h";

        if (needleTransform != null)
        {
            float speedPercent = Mathf.Clamp01(speed / nitrousMaxSpeed);
            float needleAngle = Mathf.Lerp(minNeedleAngle, maxNeedleAngle, speedPercent);
            needleTransform.localEulerAngles = new Vector3(0, 0, needleAngle);
        }
    }

    public void AddNitrousTime(float nitrousTime)
    {
        this.nitrousTime = Mathf.Clamp(this.nitrousTime + nitrousTime, 0f, maxNitrousDuration);
    }

    public void AddTimeOrb(int timeOrbToAdd)
    {
        timeOrb = timeOrbToAdd; // Add time orb to the player's time orb
        Debug.Log("Time Orb: " + timeOrb); // Log the current time orb value
        // Stop any existing slowdown to avoid stacking
        if (slowDownCoroutine != null)
        {
            StopCoroutine(slowDownCoroutine);
        }
        //timeOrbText.text = "Time Orb Activated!"; // Update the time orb text
        slowDownCoroutine = StartCoroutine(SlowDownPlayer(timeOrbToAdd)); // Trigger the slowdown effect
        timeOrb = 0;
    }
    public IEnumerator SlowDownPlayer(int duration)
    {
        Time.timeScale = slowTimeScale;
        yield return new WaitForSeconds(duration); // Wait for the specified duration
        Time.timeScale = 1f;
        //Debug.Log("Time Orb Deactivated! Current Speed: " + acceleration); // Log the speed deactivation
        //timeOrbText.text = "Time Orb Deactivated!"; // Update the time orb text
        slowDownCoroutine = null; // Reset the coroutine reference
    }

    void Explode()
    {

    }
}

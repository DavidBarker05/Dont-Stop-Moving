using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float acceleration = 800f;
    public float brakingForce = 1000f;
    public float steeringSpeed = 80f;
    public float maxSpeed = 50f;

    [Header("UI - Digital")]
    public TextMeshProUGUI speedText; 

    [Header("UI - Analog Needle")]
    public RectTransform needleTransform; 
    public float minNeedleAngle = 225f; 
    public float maxNeedleAngle = -45f; 

    private Rigidbody rb;

    public Vector3 Velocity => rb.linearVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); 
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        UpdateSpeedometer();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");

        if (moveInput > 0)
        {
            rb.AddForce(transform.forward * moveInput * acceleration * Time.fixedDeltaTime);
        }
        else if (moveInput < 0)
        {
            if (rb.linearVelocity.magnitude > 0.5f && Vector3.Dot(rb.linearVelocity, transform.forward) > 0)
            {
                rb.AddForce(-rb.linearVelocity.normalized * brakingForce * Time.fixedDeltaTime);
            }
            else
            {
                rb.AddForce(transform.forward * moveInput * acceleration * Time.fixedDeltaTime);
            }
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
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
            float speedPercent = Mathf.Clamp01(speed / maxSpeed);
            float needleAngle = Mathf.Lerp(minNeedleAngle, maxNeedleAngle, speedPercent);
            needleTransform.localEulerAngles = new Vector3(0, 0, needleAngle);
        }
    }
}

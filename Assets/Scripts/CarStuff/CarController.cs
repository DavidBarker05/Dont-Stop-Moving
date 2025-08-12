using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float acceleration = 800f;
    public float brakingForce = 1000f;
    public float steeringSpeed = 80f;
    public float maxSpeed = 50f;

    [Header("UI")]
    public TextMeshProUGUI speedText; 

    private Rigidbody rb;

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
            if (rb.velocity.magnitude > 0.5f && Vector3.Dot(rb.velocity, transform.forward) > 0)
            {
                rb.AddForce(-rb.velocity.normalized * brakingForce * Time.fixedDeltaTime);
            }
            else
            {
                rb.AddForce(transform.forward * moveInput * acceleration * Time.fixedDeltaTime);
            }
        }

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void HandleSteering()
    {
        float steerInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerInput * steeringSpeed * Time.fixedDeltaTime);
    }

    void UpdateSpeedometer()
    {
        float speed = rb.velocity.magnitude * 3.6f; 
        speedText.text = Mathf.RoundToInt(speed) + " km/h";
    }
}

using UnityEngine;

public class CarController : MonoBehaviour
{
    public class ArcadeCarController : MonoBehaviour
    {
        [Header("Car Settings")]
        public float acceleration = 800f;
        public float brakingForce = 1000f;
        public float steeringSpeed = 80f;
        public float maxSpeed = 50f;

        [Header("UI - Digital")]
        public TextMeshProUGUI speedText; // Digital readout

        [Header("UI - Analog Needle")]
        public RectTransform needleTransform; // Assign your needle image
        public float minNeedleAngle = 225f; // Fully left (0 km/h)
        public float maxNeedleAngle = -45f; // Fully right (max speed)

        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = new Vector3(0, -0.5f, 0); // Stability
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
                // Accelerate forward
                rb.AddForce(transform.forward * moveInput * acceleration * Time.fixedDeltaTime);
            }
            else if (moveInput < 0)
            {
                if (rb.velocity.magnitude > 0.5f && Vector3.Dot(rb.velocity, transform.forward) > 0)
                {
                    // Brake if moving forward
                    rb.AddForce(-rb.velocity.normalized * brakingForce * Time.fixedDeltaTime);
                }
                else
                {
                    // Reverse
                    rb.AddForce(transform.forward * moveInput * acceleration * Time.fixedDeltaTime);
                }
            }

            // Limit speed
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
            // Calculate speed in km/h
            float speed = rb.velocity.magnitude * 3.6f;

            // Digital readout
            if (speedText != null)
                speedText.text = Mathf.RoundToInt(speed) + " km/h";

            // Analog needle rotation
            if (needleTransform != null)
            {
                float speedPercent = Mathf.Clamp01(speed / maxSpeed);
                float needleAngle = Mathf.Lerp(minNeedleAngle, maxNeedleAngle, speedPercent);
                needleTransform.localEulerAngles = new Vector3(0, 0, needleAngle);
            }
        }
    }

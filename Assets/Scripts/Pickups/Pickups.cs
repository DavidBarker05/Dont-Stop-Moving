using UnityEngine;

public class Pickups : MonoBehaviour
{
    public PickUpType pickUpType; // Type of the pickup
    [SerializeField] CarController playerMovement; // Reference to the PlayerMovement script

    public int speedboost = 5; // Speed boost amount
    [SerializeField] int increaseSpeed = 10; // Speed to increase when collected
    [SerializeField] int increaseTimeorb = 3; // Coal to increase when collected

    public enum PickUpType
    {

        Speed,
        TimeOrb,
    }

    void Start()
    {
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CarController playerMovement = collision.gameObject.GetComponent<CarController>();
            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement not found on collided object!");
                return;
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                switch (pickUpType)
                {
                    case PickUpType.Speed:
                        // Add speed to the player
                        playerMovement.AddSpeed(increaseSpeed, speedboost); // Call the AddSpeed method in the PlayerMovement script
                        break;
                    case PickUpType.TimeOrb:
                        // Add time to the player
                        playerMovement.AddTimeOrb(increaseTimeorb); // Call the AddTimeOrb method in the PlayerMovement script
                        break;

                }
            }
            // Update the UI text
            //playerMovement.UpdateUI();

            Destroy(gameObject); // Destroy the object
        }

    }
}

using UnityEngine;

public class Pickups : MonoBehaviour
{
    public PickUpType pickUpType; // Type of the pickup
    [SerializeField] CarController playerMovement; // Reference to the PlayerMovement script

    [SerializeField] float extraNitrousTime = 2f;
    [SerializeField] int increaseTimeorb = 3; // Coal to increase when collected

    public enum PickUpType
    {

        Nitrous,
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
                    case PickUpType.Nitrous:
                        // Add speed to the player
                        playerMovement.AddNitrousTime(extraNitrousTime);
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

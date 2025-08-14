using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject roadSection; // Prefab reference
    private GameObject oldRoad;    // Last spawned section
    private GameObject newRoad;    // Current spawned section
    public Transform spawnPoint;
    private bool Spawn = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            if (Spawn)
            {
                newRoad = Instantiate(roadSection, spawnPoint.position, spawnPoint.rotation);
                Spawn = false;
            }
        }
        // Only set oldRoad if it's null and newRoad exists
        if (oldRoad == null && newRoad != null)
        {
            oldRoad = newRoad;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (oldRoad != null && oldRoad != newRoad)
        {
            Destroy(oldRoad);
        }
        oldRoad = newRoad;
        Spawn = true;
    }
}
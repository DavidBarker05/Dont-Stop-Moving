using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject roadSection;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            Instantiate(roadSection);
        }
    }
}

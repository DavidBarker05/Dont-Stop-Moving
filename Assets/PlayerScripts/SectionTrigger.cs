using Unity.Mathematics;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject roadSection;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            Instantiate(roadSection, new Vector3(0,0,21), Quaternion.identity);
        }
    }
}

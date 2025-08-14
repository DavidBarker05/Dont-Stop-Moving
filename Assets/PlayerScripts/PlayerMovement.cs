    using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = forwardSpeed;
    }
    private void FixedUpdate()
    {
        controller.Move(direction*Time.fixedDeltaTime);
    }
}

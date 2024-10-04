using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;

    [Header("Rotation")]
    public float rotationAmount = 4f;
    public float maxRotationAmount = 5f;
    public float smoothRotation = 12f;

    [Header("Idle Sway")]
    public float idleSwayAmount = 0.01f;
    public float idleSwaySpeed = 1f;

    [Header("Movement Sway")]
    public float movementSwayAmount = 0.1f;
    public float movementSwaySpeed = 4f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private CharacterController characterController;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        characterController = GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        float movementX = -Input.GetAxis("Mouse X") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        // Add idle sway
        float idleX = Mathf.Sin(Time.time * idleSwaySpeed) * idleSwayAmount;
        float idleY = Mathf.Cos(Time.time * idleSwaySpeed * 2) * idleSwayAmount * 0.5f;

        Vector3 finalPosition = new Vector3(movementX + idleX, movementY + idleY, 0);

        // Add movement sway
        if (characterController != null)
        {
            Vector3 movement = characterController.velocity;
            float movementSwayX = Mathf.Sin(Time.time * movementSwaySpeed) * (movement.magnitude * movementSwayAmount);
            float movementSwayY = Mathf.Cos(Time.time * movementSwaySpeed * 2) * (movement.magnitude * movementSwayAmount * 0.5f);

            finalPosition += new Vector3(movementSwayX, movementSwayY, 0);
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);

        // Rotation
        float tiltY = -Input.GetAxis("Mouse X") * rotationAmount;
        float tiltX = Input.GetAxis("Mouse Y") * rotationAmount;
        
        Quaternion finalRotation = Quaternion.Euler(new Vector3(tiltX, tiltY, 0));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothRotation);
    }
}
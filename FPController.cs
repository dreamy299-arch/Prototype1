using UnityEngine; 
using UnityEngine.InputSystem; 

public class FPController : MonoBehaviour 
{ 
    [Header("Movement Settings")] 
    public float moveSpeed = 5f; 
    public float gravity = -9.81f; 
    public float jumpHeight = 1.5f; 

    [Header("Look Settings")] 
    public Transform cameraTransform; 
    public float lookSensitivity = 0.8f; 
    public float verticalLookLimit = 90f; 

    [Header("Shooting")] 
    public GameObject bulletPrefab; 
    public Transform gunPoint; 
    public float bulletForce = 500f; 

    [Header("Crouch Settings")] 
    public float crouchHeight = 1f; 
    public float standHeight = 2f; 
    public float crouchSpeed = 2.5f; 
    private float originalMoveSpeed; 

    [Header("Sprint Setting")]
    public float sprintSpeed = 8.3f;
    public int num = 34;

    [Header("Pickup Settings")] 
    public float pickupRange = 3f; 
    public Transform holdPoint; 
    public PickUpObject heldObject; 

    [Header("Throw Settings")] 
    public float throwForce = 10f; 
    public float throwUpwardBoost = 1f; 

    public CharacterController controller; 
    private Rigidbody rb; 
    public Vector2 moveInput; 
    private Vector2 lookInput; 
    private Vector3 velocity; 
    private float verticalRotation = 0f; 

    private void Awake() 
    { 
        controller = GetComponent<CharacterController>(); 
        rb = GetComponent<Rigidbody>(); 
        originalMoveSpeed = moveSpeed; 
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    } 

    private void Update() 
    { 
        // Only run movement and rotation when the game is NOT paused
        if (!PauseManuScript.isPaused)
        {
            HandleMovement(); 
            HandleLook(); 

            if (heldObject != null) 
            { 
                heldObject.MoveToHoldPoint(holdPoint.position); 
            } 
        }
    } 

    public void OnMove(InputAction.CallbackContext context) 
    { 
        moveInput = context.ReadValue<Vector2>(); 
    } 

    public void OnLook(InputAction.CallbackContext context) 
    { 
        lookInput = context.ReadValue<Vector2>(); 
    } 

    public void Throw(Vector3 impulse) 
    { 
        transform.SetParent(null); 
        if (rb != null)
        {
            rb.useGravity = true; 
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 
            rb.AddForce(impulse, ForceMode.Impulse); 
        }
    } 

    public void HandleMovement() 
    { 
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y; 
        controller.Move(move * moveSpeed * Time.deltaTime); 

        if (controller.isGrounded && velocity.y < 0) 
        { 
            velocity.y = -2f; 
        } 

        velocity.y += gravity * Time.deltaTime; 
        controller.Move(velocity * Time.deltaTime); 
    } 

    public void HandleLook() 
    { 
        float mouseX = lookInput.x * lookSensitivity; 
        float mouseY = lookInput.y * lookSensitivity; 

        verticalRotation -= mouseY; 
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit); 

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); 
        transform.Rotate(Vector3.up * mouseX); 
    } 

    public void OnJump(InputAction.CallbackContext context) 
    { 
        if (context.performed && controller.isGrounded) 
        { 
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); 
        } 
    } 

    public void OnShoot(InputAction.CallbackContext context) 
    { 
        // Ignore input actions while paused
        if (PauseManuScript.isPaused) return;

        if (context.performed) 
        { 
            Shoot(); 
        } 
    } 

    private void Shoot() 
    { 
        if (bulletPrefab != null && gunPoint != null) 
        { 
            GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation); 
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>(); 
            if (bulletRb != null) 
            { 
                bulletRb.AddForce(gunPoint.forward * bulletForce); 
            } 
        } 
    } 

    public void OnCrouch(InputAction.CallbackContext context) 
    { 
        if (PauseManuScript.isPaused) return;

        if (context.performed) 
        { 
            controller.height = crouchHeight; 
            moveSpeed = crouchSpeed; 
        } 
        else if (context.canceled) 
        { 
            controller.height = standHeight; 
            moveSpeed = originalMoveSpeed; 
        } 
    } 

    public void OnPickUp(InputAction.CallbackContext context) 
    { 
        if (PauseManuScript.isPaused || !context.performed) return; 

        if (heldObject == null) 
        { 
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward); 
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange)) 
            { 
                PickUpObject pickUp = hit.collider.GetComponent<PickUpObject>(); 
                if (pickUp != null) 
                { 
                    pickUp.PickUp(holdPoint); 
                    heldObject = pickUp; 
                } 
            } 
        } 
        else 
        { 
            heldObject.Drop(); 
            heldObject = null; 
        } 
    } 

    public void OnThrow(InputAction.CallbackContext context) 
    { 
        if (PauseManuScript.isPaused || !context.performed) return; 
        if (heldObject == null) return; 

        Vector3 dir = cameraTransform.forward; 
        Vector3 impulse = dir * throwForce + Vector3.up * throwUpwardBoost; 
        heldObject.Throw(impulse); 
        heldObject = null; 
    } 
}
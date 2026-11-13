using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isOpen = false;
    private bool isAnimating = false;

    [Header("Door Settingss")]
    public float openAngle = 90f;
    public float speed = 5f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, 0, openAngle);
    }

    void Update()
    {
        if (isAnimating)
        {
            TargetRotation();
        }
    }

    // Called by your FPController raycast
    public void Interact()
    {
        if (!isAnimating)
        {
            isOpen = !isOpen;
            isAnimating = true;
        }
    }

    void TargetRotation()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target, speed * Time.deltaTime * 100f);

        if (Quaternion.Angle(transform.localRotation, target) < 0.1f)
        {
            transform.localRotation = target;
            isAnimating = false;
        }
    }
}
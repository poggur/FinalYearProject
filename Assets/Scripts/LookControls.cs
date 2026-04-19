using UnityEngine;
using UnityEngine.InputSystem;

public class LookControls : MonoBehaviour
{
    [Header("Variables for Camera")]
    [SerializeField] private float mouseSens = 0.1f;

    [Header("Input action references")]
    [SerializeField] private InputActionReference look;

    private Vector2 mousePos;
    public bool mouseEnabled = true;

    public Transform lockTarget;

    private void MouseControls()
    {
        mousePos = look.action.ReadValue<Vector2>();

        float mouseXRotation = mousePos.x * mouseSens;
        float mouseYRotation = mousePos.y * mouseSens;

        transform.Rotate(0, mouseXRotation, 0);

    }

    private void LockedControls(Transform target)
    {
        transform.LookAt(target, Vector3.up);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lockTarget = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseEnabled)
        {
            MouseControls();
        }
        else
        {
            LockedControls(lockTarget);
        }
    }
}

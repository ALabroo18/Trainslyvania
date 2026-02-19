using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    private TouchControls touchControls;

    // Before starting, new Touch control map is created
    private void Awake()
    {
        touchControls = new TouchControls();
    }
    
    // Enables touch controls
    private void OnEnable()
    {
        touchControls.Enable();
    }

    // Disables touch controls

    private void OnDisable()
    {
        touchControls.Disable();
    }

        private void Start()
    {
        // When input is started, then debug line is printed
        touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);

        // When input is released, then debug line is printed
        touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        Debug.Log("Hi");
    }
    private void EndTouch (InputAction.CallbackContext context)
    {
        Debug.Log("Bye");
    }
}

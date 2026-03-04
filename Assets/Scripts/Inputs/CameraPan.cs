
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraPan : MonoBehaviour
{
    public GameObject cameraParent;
    public Camera camera;


    // Inputs
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    public float panSpeed = 0.5f;

    private Vector2 originPoint;
    private Vector3 _difference;

    private bool isDragging;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
        camera = Camera.main;
    }
     // Enables touch controls
    private void OnEnable()
    {
        
        touchPositionAction.Enable();
        touchPressAction.Enable();
        touchPressAction.performed += OnDrag;
        touchPressAction.canceled += OnDrag;

        
    }

    private void OnDisable()
    {        
        touchPressAction.performed -= OnDrag;
        touchPressAction.canceled -= OnDrag;
        touchPositionAction.Disable();
        touchPressAction.Disable();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isDragging)
        {
            Debug.Log("Not dragging");
            return;
            
        }
        Debug.Log(GetTouchPosition);
        _difference = new Vector3(GetTouchPosition.x, GetTouchPosition.y, 0) - camera.transform.position;
        camera.transform.position = new Vector3 (originPoint.x, originPoint.y, 0) - _difference;
    }

    public void OnDrag(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            Debug.Log("started");
            originPoint = GetTouchPosition;
        }
        if(ctx.canceled)
            Debug.Log("Canceled");
            
        isDragging = ctx.performed;

        Debug.Log(isDragging);
    }
    private Vector2 GetTouchPosition => camera.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());


}

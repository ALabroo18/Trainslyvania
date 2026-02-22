
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    private bool isIn = false;
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    public GameObject playerCharacter;
    public GameObject PressedImage;

    public Camera camera;
    // Before starting, new Touch control map is created
    private void Awake()
    {
        isIn = false;
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
    }
    
    // Enables touch controls
    private void OnEnable()
    {
       touchPressAction.performed += TouchPressed;
       Debug.Log("Hi");
    }

    // Disables touch controls

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
        
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        Debug.Log(touchPositionAction.ReadValue<Vector2>());
        Ray ray = camera.ScreenPointToRay(touchPositionAction.ReadValue<Vector2>());
        Debug.Log("Went through");
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit) && isIn == false)
        {
            Instantiate(playerCharacter, hit.point, Quaternion.identity);
            isIn = true;
        }
        // Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
        // position.z = playerCharacter.transform.position.z;
        // playerCharacter.transform.position = position;        
    }

    private IEnumerator DelayTime()
    {
        yield return new WaitForSeconds(1f);
        
    }
}

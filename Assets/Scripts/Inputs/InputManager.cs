using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public int maxTurrets = 6;
    public int maxTurretsPerCar = 2;

    private int turretsPlaced = 0;
    private Dictionary<Collider, int> turretsOnCar = new Dictionary<Collider, int>();

    public LayerMask trainCarLayer;

    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    public GameObject playerCharacter;
    public GameObject PressedImage;

    public Camera camera;
    // Before starting, new Touch control map is created
    private void Awake()
    {
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

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, trainCarLayer))
        {
            Collider carCollider = hit.collider;

            if (turretsPlaced >= maxTurrets)
                return;

            if (!turretsOnCar.ContainsKey(carCollider))
                turretsOnCar[carCollider] = 0;

            if (turretsOnCar[carCollider] >= maxTurretsPerCar)
                return;

            GameObject turret = Instantiate(playerCharacter, hit.point, Quaternion.identity);

            mediumTurret turretScript = turret.GetComponent<mediumTurret>();
            trainHealth carHealth = hit.collider.GetComponent<trainHealth>();

            if (turretScript != null && carHealth != null)
            {
                turretScript.owningCar = carHealth;
                turretsPlaced++;
                turretsOnCar[carCollider]++;
            }
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

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//declare consumable type
public enum ConsumableType
{
    None,
    Fireball,
    HolyWater,
    Caltrops
}

public class InputManager : MonoBehaviour
{
    public int maxTurrets = 6;
    public int maxTurretsPerCar = 2;

    private int turretsPlaced = 0;
    private Dictionary<Collider, int> turretsOnCar = new Dictionary<Collider, int>();

    public LayerMask trainCarLayer;
    public LayerMask groundMask;
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    public GameObject playerCharacter;
    public GameObject PressedImage;

    //added the holywater consumable
    [SerializeField] private HolyWater holyWater;
    public LayerMask turretLayer;

    [SerializeField] private FireBomb FireBomb;

    //consumabletype.none default, sets a type from the enum by a button press, which then lets you use it
    public ConsumableType activeConsumable = ConsumableType.None;

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

    public void SelectFireball()
    {
        if (ItemManager.Instance.FireballUses <= 0)
        {
            Debug.Log("No Fireballs left!");
            return;
        }
        activeConsumable = activeConsumable == ConsumableType.Fireball
            ? ConsumableType.None  // pressing again deselects
            : ConsumableType.Fireball;
        Debug.Log("Active consumable: " + activeConsumable);
    }

    public void SelectHolyWater()
    {
        if (ItemManager.Instance.HolyWaterUses <= 0)
        {
            Debug.Log("No Holy Water left!");
            return;
        }
        activeConsumable = activeConsumable == ConsumableType.HolyWater
            ? ConsumableType.None  // pressing again deselects
            : ConsumableType.HolyWater;
        Debug.Log("Active consumable: " + activeConsumable);
    }

    public void Deselect()
    {
        activeConsumable = ConsumableType.None;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        // Debug.Log(touchPositionAction.ReadValue<Vector2>());
        Ray ray = camera.ScreenPointToRay(touchPositionAction.ReadValue<Vector2>());
        Debug.Log("Went through");

        //changed to a switch case
        // Normal Turret Dropping
        switch (activeConsumable)
        {
            case ConsumableType.None:
                if (Physics.Raycast(ray, out RaycastHit turretHit, Mathf.Infinity, trainCarLayer))
                {
                    Collider carCollider = turretHit.collider;
                    if (turretsPlaced >= maxTurrets) return;
                    if (!turretsOnCar.ContainsKey(carCollider))
                        turretsOnCar[carCollider] = 0;
                    if (turretsOnCar[carCollider] >= maxTurretsPerCar) return;

                    GameObject turret = Instantiate(playerCharacter, turretHit.point, Quaternion.identity);
                    mediumTurret turretScript = turret.GetComponent<mediumTurret>();
                    trainHealth carHealth = turretHit.collider.GetComponent<trainHealth>();
                    if (turretScript != null && carHealth != null)
                    {
                        turretScript.owningCar = carHealth;
                        turretsPlaced++;
                        turretsOnCar[carCollider]++;
                    }
                }
                break;

            case ConsumableType.Fireball:
                if (Physics.Raycast(ray, out RaycastHit fireballHit, Mathf.Infinity, groundMask))
                {
                    Debug.Log("Fireball placed at: " + fireballHit.point);
                    FireBomb.FireRadius(fireballHit.point);
                    ItemManager.Instance.ConsumeFireball();
                    activeConsumable = ConsumableType.None;
                }
                break;

            case ConsumableType.HolyWater:
                RaycastHit holyWaterHit;
                if (Physics.Raycast(ray, out holyWaterHit, Mathf.Infinity))
                {
                    Debug.Log("Hit: " + holyWaterHit.collider.gameObject.name + " on layer: " + LayerMask.LayerToName(holyWaterHit.collider.gameObject.layer));

                    // check if what we hit has a turret script on it or its parent
                    mediumTurret turret = holyWaterHit.collider.GetComponentInParent<mediumTurret>();
                    if (turret == null)
                        turret = holyWaterHit.collider.GetComponentInChildren<mediumTurret>();

                    if (turret != null)
                    {
                        Debug.Log("Blessing turret: " + turret.gameObject.name);
                        holyWater.BlessTurret(turret, holyWaterHit.point);
                        ItemManager.Instance.ConsumeHolyWater();
                        activeConsumable = ConsumableType.None;
                    }
                    else
                    {
                        Debug.Log("No turret found, splashing at: " + holyWaterHit.point);
                        holyWater.SplashArea(holyWaterHit.point);
                        ItemManager.Instance.ConsumeHolyWater();
                        activeConsumable = ConsumableType.None;
                    }
                }
                break;

             case ConsumableType.Caltrops:
                // Caltrops logic here

                activeConsumable = ConsumableType.None;
                break;
            
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

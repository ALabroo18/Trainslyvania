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
    Firebomb,
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

    public void SelectFirebomb()
    {
        bool hasCharges = ModeSelector.SelectedMode == GameMode.Infinite
               ? InfiniteConsumableManager.Instance != null && InfiniteConsumableManager.Instance.FirebombCharges > 0
               : ItemManager.Instance.FirebombUses > 0;

        if (!hasCharges)
        {
            Debug.Log("No Firebombs!");
            return;
        }
        activeConsumable = activeConsumable == ConsumableType.Firebomb
            ? ConsumableType.None
            : ConsumableType.Firebomb;
    }

    public void SelectHolyWater()
    {
        bool hasCharges = ModeSelector.SelectedMode == GameMode.Infinite
        ? InfiniteConsumableManager.Instance != null && InfiniteConsumableManager.Instance.HolyWaterCharges > 0
        : ItemManager.Instance.HolyWaterUses > 0;

        if (!hasCharges)
        {
            Debug.Log("No Holy Water!");
            return;
        }
        activeConsumable = activeConsumable == ConsumableType.HolyWater
            ? ConsumableType.None
            : ConsumableType.HolyWater;
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

            case ConsumableType.Firebomb:
                if (Physics.Raycast(ray, out RaycastHit firebombHit, Mathf.Infinity, groundMask))
                {
                    bool hasCharge = ModeSelector.SelectedMode == GameMode.Infinite
                                ? InfiniteConsumableManager.Instance.UseFirebomb()
                                : ItemManager.Instance.FirebombUses > 0;

                    if (hasCharge)
                    {
                        if (ModeSelector.SelectedMode == GameMode.Normal)
                            ItemManager.Instance.ConsumeFirebomb();
                        FireBomb.FireRadius(firebombHit.point);
                        activeConsumable = ConsumableType.None;
                    }
                }
                break;

            case ConsumableType.HolyWater:
                if (Physics.Raycast(ray, out RaycastHit turretBlessHit, Mathf.Infinity, turretLayer))
                {
                    // check if what we hit has a turret script on it or its parent
                    mediumTurret turret = turretBlessHit.collider.GetComponentInParent<mediumTurret>();
                    if (turret == null)
                        turret = turretBlessHit.collider.GetComponentInChildren<mediumTurret>();

                    if (turret != null)
                    {
                        bool hasCharge = ModeSelector.SelectedMode == GameMode.Infinite
                    ? InfiniteConsumableManager.Instance.UseHolyWater()
                    : ItemManager.Instance.HolyWaterUses > 0;
                        if (hasCharge)
                        {
                            if (ModeSelector.SelectedMode == GameMode.Normal)
                                ItemManager.Instance.ConsumeHolyWater();
                            holyWater.BlessTurret(turret, turretBlessHit.point);
                            activeConsumable = ConsumableType.None;
                            break;
                        }
                    }
                }
                if (Physics.Raycast(ray, out RaycastHit splashHit, Mathf.Infinity, groundMask))
                {
                    bool hasCharge = ModeSelector.SelectedMode == GameMode.Infinite
                        ? InfiniteConsumableManager.Instance.UseHolyWater()
                        : ItemManager.Instance.HolyWaterUses > 0;

                    if (hasCharge)
                    {
                        if (ModeSelector.SelectedMode == GameMode.Normal)
                            ItemManager.Instance.ConsumeHolyWater();
                        holyWater.SplashArea(splashHit.point);
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

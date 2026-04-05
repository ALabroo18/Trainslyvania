using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class FireBombButton : MonoBehaviour
{

    private InputAction touchPosition;
    [SerializeField] private PlayerInput playerInput;    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputManager InputManager;

    public void SetConsumable()
    {
       InputManager.isConsumable = true;
       Debug.Log("Consumable is set to " + InputManager.isConsumable);
        
    }
}

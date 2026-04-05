using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class CaltropsButton : MonoBehaviour
{

    private InputAction touchPosition;
    [SerializeField] private PlayerInput playerInput;    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputManager InputManager;

    InputManager inputManager;

    public void SetConsumable()
    {
       InputManager.isConsumable = true;
        
    }

    public void SetGUI(Vector3 position)
    {
        
    }
}

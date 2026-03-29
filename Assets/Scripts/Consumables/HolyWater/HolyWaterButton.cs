using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HolyWaterButton : MonoBehaviour
{
    public HolyWater holyWater;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI usesText;

    private bool isAiming = false;

    void Start()
    {
        UpdateUsesText();
        UpdateButtonText();

        if (HolyWaterManager.Instance != null)
            HolyWaterManager.Instance.OnUsesChanged += OnUsesChanged;
    }

    void OnDestroy()
    {
        if (HolyWaterManager.Instance != null)
            HolyWaterManager.Instance.OnUsesChanged -= OnUsesChanged;
    }

    void OnUsesChanged(int newUses)
    {
        UpdateUsesText();
    }

    void Update()
    {
        if (!isAiming) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, holyWater.turretLayer))
            {
                mediumTurret turret = hit.collider.GetComponent<mediumTurret>();
                if (turret != null)
                {
                    holyWater.BlessTurret(turret, hit.point);
                    EndAiming();
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                holyWater.SplashArea(hit.point);
                EndAiming();
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelAiming();
        }
    }

    public void OnHolyWaterButtonPressed()
    {
        if (HolyWaterManager.Instance.Uses <= 0)
        {
            Debug.Log("No Holy Water left!");
            return;
        }

        if (isAiming)
        {
            CancelAiming();
            return;
        }

        isAiming = true;
        UpdateButtonText();
        Debug.Log("Click anywhere to throw Holy Water, right click to cancel");
    }

    void EndAiming()
    {
        isAiming = false;
        HolyWaterManager.Instance.ConsumeUse();
        UpdateButtonText();
    }

    void CancelAiming()
    {
        isAiming = false;
        UpdateButtonText();
        Debug.Log("Holy Water throw cancelled");
    }

    void UpdateUsesText()
    {
        if (usesText != null)
            usesText.text = "x" + HolyWaterManager.Instance.Uses;
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
            buttonText.text = isAiming ? "Cancel" : "Holy Water";
    }
}
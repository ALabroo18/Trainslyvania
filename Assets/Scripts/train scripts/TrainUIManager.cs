using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TrainUIManager : MonoBehaviour
{
    [System.Serializable]
    public class TrainUIEntry
    {
        public Button cartButton;
        public Image buttonImage;
        public TMP_Text alertText;
        public Image carTypeImage;
        public Sprite passengerSprite;
        public Sprite defensiveSprite;
    }

    [Header("UI slots")]
    public List<TrainUIEntry> uiSlots = new List<TrainUIEntry>();

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color damageColor = Color.red;
    public Color breachedColor = new Color(0.5f, 0f, 0f);
    public float flashDuration = 0.2f;

    [Header("Total Health Display")]
    public TMP_Text totalHealthText;
    public trainHealth trainHeadHealth;

    private trainHealth[] trains;

    public void SetTrains(trainHealth[] spawnedTrains)
    {
        trains = spawnedTrains;

        for (int i = 0; i < trains.Length; i++)
        {
            if (trains[i] == null) continue;
            if (i >= uiSlots.Count)
            {
                continue;
            }

            SetNormalState(i);

            CarData car = TrainManager.Instance.GetCar(i);
            if (car != null && uiSlots[i].carTypeImage != null)
            {
                if (car.carType == CarType.Passenger)
                    uiSlots[i].carTypeImage.sprite = uiSlots[i].passengerSprite;
                else if (car.carType == CarType.Defensive)
                    uiSlots[i].carTypeImage.sprite = uiSlots[i].defensiveSprite;
            }

            int index = i;
            trains[i].OnHealthChanged += (current, max) => OnHealthChanged(index, current, max);
            trains[i].OnBreached += () => OnCartBreached(index);
        }

        for (int i = trains.Length; i < uiSlots.Count; i++)
        {
            if (uiSlots[i].cartButton != null)
                uiSlots[i].cartButton.gameObject.SetActive(false);
        }

        UpdateTotalHealth();
    }

    void UpdateTotalHealth()
    {
        if (totalHealthText == null) return;

        int current = 0;
        int max = 0;

        if (trainHeadHealth != null)
        {
            current += trainHeadHealth.currentHealth;
            max += trainHeadHealth.maxHealth;
        }

        for (int i = 0; i < trains.Length; i++)
        {
            if (trains[i] == null) continue;
            current += trains[i].currentHealth;
            max += trains[i].maxHealth;
        }

        totalHealthText.text = "Train health: " + current;
    }

    void OnHealthChanged(int index, int current, int max)
    {
        if (index >= uiSlots.Count) return;
        if (trains[index].isBreached) return;
        StartCoroutine(FlashDamage(index));
        UpdateTotalHealth();
    }

    IEnumerator FlashDamage(int index)
    {
        TrainUIEntry slot = uiSlots[index];
        if (slot.alertText != null) slot.alertText.gameObject.SetActive(true);
        if (slot.buttonImage != null) slot.buttonImage.color = damageColor;

        yield return new WaitForSeconds(flashDuration);

        if (slot.buttonImage != null) slot.buttonImage.color = normalColor;
        if (slot.alertText != null) slot.alertText.gameObject.SetActive(false);
    }

    void OnCartBreached(int index)
    {
        if (index >= uiSlots.Count) return;
        TrainUIEntry slot = uiSlots[index];
        if (slot.buttonImage != null) slot.buttonImage.color = breachedColor;
        if (slot.alertText != null) slot.alertText.gameObject.SetActive(true);
        if (slot.cartButton != null) slot.cartButton.interactable = false;
        UpdateTotalHealth();
    }

    void SetNormalState(int index)
    {
        if (index >= uiSlots.Count) return;
        TrainUIEntry slot = uiSlots[index];
        if (slot.buttonImage != null) slot.buttonImage.color = normalColor;
        if (slot.alertText != null) slot.alertText.gameObject.SetActive(false);
        if (slot.cartButton != null) slot.cartButton.interactable = true;
    }
}
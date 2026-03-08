using UnityEngine;
using TMPro;

public class FullTrainHealthUI : MonoBehaviour
{
    public trainHealth trainCar1;
    public trainHealth trainCar2;

    public trainHealth trainCar3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int fullHealth = 0;

    public TMP_Text healthText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fullHealth = trainCar1.currentHealth + trainCar2.currentHealth + trainCar3.currentHealth;
        healthText.text = "Train Health: " + fullHealth;
    }
}

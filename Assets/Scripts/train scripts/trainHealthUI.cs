using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class trainHealthUI : MonoBehaviour
{
    //Mason Kuhn

    [Header("References")]
    [SerializeField] private trainHealth trainHealth;
    [SerializeField] private Button cartButton;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text alertText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color breachedColor = new Color(0.5f, 0f, 0f);

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.2f;

    Coroutine flashRoutine;

    void OnEnable()
    {
        trainHealth.OnHealthChanged += OnHealthChanged;
        trainHealth.OnBreached += OnCartBreached;

        SetNormalState();
    }

    void OnDisable()
    {
        trainHealth.OnHealthChanged -= OnHealthChanged;
        trainHealth.OnBreached -= OnCartBreached;
    }

    void OnHealthChanged(int current, int max)
    {
        if (trainHealth.isBreached)
            return;

        //flash red on damage
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashDamage());
    }

    IEnumerator FlashDamage()
    {
        alertText.gameObject.SetActive(true);
        buttonImage.color = damageColor;

        yield return new WaitForSeconds(flashDuration);

        buttonImage.color = normalColor;
        alertText.gameObject.SetActive(false);
    }

    void OnCartBreached()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        buttonImage.color = breachedColor;
        alertText.gameObject.SetActive(true);
        cartButton.interactable = false;
    }

    void SetNormalState()
    {
        buttonImage.color = normalColor;
        alertText.gameObject.SetActive(false);
        cartButton.interactable = true;
    }
}
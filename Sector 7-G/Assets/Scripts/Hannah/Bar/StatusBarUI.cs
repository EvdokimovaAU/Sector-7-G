using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text valueText;


    public void SetValue(int value)
    {
        value = Mathf.Clamp(value, 0, 100);

        if (fillImage != null)
        {
            fillImage.fillAmount = value / 100f;
        }

        if (valueText != null)
        {
            valueText.text = value + "%";
        }
    }
}
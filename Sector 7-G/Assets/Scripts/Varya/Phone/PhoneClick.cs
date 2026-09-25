using Station;
using TMPro;
using UnityEngine;

public class PhoneClick : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject phonePanel;
    [SerializeField] private GameObject callNotification;

    [Header("Notification")]
    [SerializeField] private TMP_Text notificationText;

    private void Awake()
    {
        if (phonePanel != null) phonePanel.SetActive(false);
        if (callNotification != null) callNotification.SetActive(false);
    }

    private void OnEnable()
    {
        StationEvents.SectorCalled += OnCalled;
    }

    private void OnDisable()
    {
        StationEvents.SectorCalled -= OnCalled;
    }

    private void OnMouseDown()
    {
        if (UIWindowManager.Instance != null)
        {
            UIWindowManager.Instance.OpenWindow(
                phonePanel
            );
        }
        else
        {
            phonePanel.SetActive(true);
        }
    }

    private void OnCalled(StationSector sector)
    {
        if (callNotification != null) callNotification.SetActive(true);
        if (notificationText != null)
            notificationText.text = $"Позвонили: {sector}";
    }
}
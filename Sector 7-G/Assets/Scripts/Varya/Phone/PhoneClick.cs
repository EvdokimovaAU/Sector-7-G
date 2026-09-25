using Station;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneClick : MonoBehaviour
{
    // ==================================================
    // GLOBAL STATE
    // ==================================================

    public static bool IsPhoneOpen { get; private set; }


    // ==================================================
    // PANELS
    // ==================================================

    [Header("Panels")]
    [SerializeField] private GameObject phonePanel;

    [SerializeField] private RectTransform phoneWindow;

    [SerializeField] private GameObject callNotification;


    // ==================================================
    // NOTIFICATION
    // ==================================================

    [Header("Notification")]
    [SerializeField] private TMP_Text notificationText;


    private bool justOpened;


    // ==================================================
    // UNITY
    // ==================================================

    private void Awake()
    {
        IsPhoneOpen = false;

        if (phonePanel != null)
            phonePanel.SetActive(false);

        if (callNotification != null)
            callNotification.SetActive(false);
    }


    private void OnEnable()
    {
        StationEvents.SectorCalled += OnCalled;
    }


    private void OnDisable()
    {
        StationEvents.SectorCalled -= OnCalled;
    }


    private void LateUpdate()
    {
        if (!IsPhoneOpen)
            return;

        if (phonePanel == null)
            return;

        if (!phonePanel.activeSelf)
        {
            IsPhoneOpen = false;
            return;
        }


        // Не закрываем телефон тем же кликом,
        // которым его открыли.
        if (justOpened)
        {
            justOpened = false;
            return;
        }


        if (Mouse.current == null)
            return;


        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;


        Vector2 mousePosition =
            Mouse.current.position.ReadValue();


        // Если нажали внутри телефона —
        // телефон остаётся открытым.
        if (phoneWindow != null)
        {
            bool insidePhone =
                RectTransformUtility.RectangleContainsScreenPoint(
                    phoneWindow,
                    mousePosition,
                    GetUICamera()
                );

            if (insidePhone)
                return;
        }


        // Нажали за пределами телефона.
        ClosePhone();
    }


    // ==================================================
    // OPEN
    // ==================================================

    private void OnMouseDown()
    {
        OpenPhone();
    }


    public void OpenPhone()
    {
        if (phonePanel == null)
            return;


        // СНАЧАЛА ставим блокировку.
        IsPhoneOpen = true;
        justOpened = true;


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


        Debug.Log("[PHONE] OPEN - other windows locked");
    }


    // ==================================================
    // CLOSE
    // ==================================================

    public void ClosePhone()
    {
        if (phonePanel == null)
            return;


        if (UIWindowManager.Instance != null)
        {
            UIWindowManager.Instance.CloseWindow(
                phonePanel
            );
        }
        else
        {
            phonePanel.SetActive(false);
        }


        IsPhoneOpen = false;
        justOpened = false;


        Debug.Log("[PHONE] CLOSED - other windows unlocked");
    }


    // ==================================================
    // UI CAMERA
    // ==================================================

    private Camera GetUICamera()
    {
        Canvas canvas =
            phonePanel.GetComponentInParent<Canvas>();

        if (canvas == null)
            return null;


        if (canvas.renderMode ==
            RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }


        return canvas.worldCamera;
    }


    // ==================================================
    // CALL
    // ==================================================

    private void OnCalled(StationSector sector)
    {
        if (callNotification != null)
        {
            callNotification.SetActive(true);
        }


        if (notificationText != null)
        {
            notificationText.text =
                $"Позвонили: {sector}";
        }
    }
}
using UnityEngine;

public class UIWindowManager : MonoBehaviour
{
    public static UIWindowManager Instance { get; private set; }

    // Окно, которое сейчас открыто.
    private GameObject currentWindow;

    public bool HasOpenWindow =>
        currentWindow != null &&
        currentWindow.activeSelf;

    public GameObject CurrentWindow => currentWindow;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // ==================================================
    // OPEN
    // ==================================================

    public void OpenWindow(GameObject window)
    {
        if (window == null)
            return;


        // Уже открыто какое-то другое окно.
        if (HasOpenWindow &&
            currentWindow != window)
        {
            Debug.Log(
                $"[UI] Нельзя открыть {window.name}. " +
                $"Сейчас открыто {currentWindow.name}."
            );

            return;
        }


        window.SetActive(true);

        currentWindow = window;


        Debug.Log(
            $"[UI] Open: {window.name}"
        );
    }


    // ==================================================
    // CLOSE
    // ==================================================

    public void CloseWindow(GameObject window)
    {
        if (window == null)
            return;


        window.SetActive(false);


        if (currentWindow == window)
        {
            currentWindow = null;
        }


        Debug.Log(
            $"[UI] Close: {window.name}"
        );
    }


    // ==================================================
    // CLOSE CURRENT
    // ==================================================

    public void CloseCurrentWindow()
    {
        if (!HasOpenWindow)
            return;


        GameObject window = currentWindow;

        currentWindow = null;

        window.SetActive(false);


        Debug.Log(
            $"[UI] Close: {window.name}"
        );
    }
}
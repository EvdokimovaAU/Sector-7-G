using UnityEngine;

public class UIWindowManager : MonoBehaviour
{
    public static UIWindowManager Instance { get; private set; }

    [Header("Windows")]
    [SerializeField] private GameObject phoneWindow;
    [SerializeField] private GameObject journalWindow;
    [SerializeField] private GameObject noteWindow;


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

        // Сначала закрываем остальные окна.
        CloseAll();

        // Затем открываем нужное.
        window.SetActive(true);

        // Поднимаем его поверх остальных UI.
        window.transform.SetAsLastSibling();
    }


    // ==================================================
    // CLOSE
    // ==================================================

    public void CloseWindow(GameObject window)
    {
        if (window == null)
            return;

        window.SetActive(false);
    }


    public void CloseAll()
    {
        if (phoneWindow != null)
            phoneWindow.SetActive(false);

        if (journalWindow != null)
            journalWindow.SetActive(false);

        if (noteWindow != null)
            noteWindow.SetActive(false);
    }


    // ==================================================
    // CHECK
    // ==================================================

    public bool IsAnyWindowOpen()
    {
        return
            IsOpen(phoneWindow) ||
            IsOpen(journalWindow) ||
            IsOpen(noteWindow);
    }


    private bool IsOpen(GameObject window)
    {
        return
            window != null &&
            window.activeSelf;
    }
}
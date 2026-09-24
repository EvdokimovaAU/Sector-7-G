using UnityEngine;

public class ClickInfoObject : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject infoPanel;

    [Header("Settings")]
    [SerializeField] private bool closeOnSecondClick = true;

    private void Start()
    {
        // При запуске окно скрыто.
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (infoPanel == null)
        {
            Debug.LogWarning(
                $"[{name}] Info Panel не назначен."
            );

            return;
        }

        if (closeOnSecondClick)
        {
            // Повторный клик переключает окно.
            infoPanel.SetActive(!infoPanel.activeSelf);
        }
        else
        {
            // Окно только открывается.
            infoPanel.SetActive(true);
        }
    }

    // Можно привязать к отдельной кнопке "Закрыть".
    public void ClosePanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}
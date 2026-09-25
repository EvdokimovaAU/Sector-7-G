using UnityEngine;

public class ClickInfoObject : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject infoPanel;


    private void Start()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }


    private void OnMouseDown()
    {
        OpenPanel();
    }


    public void OpenPanel()
    {
        if (infoPanel == null)
        {
            Debug.LogWarning(
                $"[{name}] Info Panel не назначен."
            );

            return;
        }


        if (UIWindowManager.Instance != null)
        {
            UIWindowManager.Instance.OpenWindow(
                infoPanel
            );
        }
        else
        {
            // Запасной вариант, если менеджер
            // случайно не добавлен на сцену.
            infoPanel.SetActive(true);
        }
    }


    public void ClosePanel()
    {
        if (infoPanel == null)
            return;


        if (UIWindowManager.Instance != null)
        {
            UIWindowManager.Instance.CloseWindow(
                infoPanel
            );
        }
        else
        {
            infoPanel.SetActive(false);
        }
    }
}
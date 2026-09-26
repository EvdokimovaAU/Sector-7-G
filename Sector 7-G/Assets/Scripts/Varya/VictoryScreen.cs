using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    // ==================================================
    // REFERENCES
    // ==================================================

    [Header("References")]

    [SerializeField]
    private TaskManager taskManager;

    [SerializeField]
    private TaskTriggeredEmergency emergency;

    [SerializeField]
    private GameState gameState;

    [SerializeField]
    private GameObject victoryWindow;


    // ==================================================
    // SETTINGS
    // ==================================================

    [Header("Victory Settings")]

    [Tooltip("Для победы стабильность станции должна быть ВЫШЕ этого значения.")]
    [Range(0, 100)]
    [SerializeField]
    private int minimumStationStability = 50;

    [Tooltip("Задержка перед появлением экрана победы.")]
    [Min(0f)]
    [SerializeField]
    private float victoryDelay = 2f;


    [Header("Scene")]

    [Tooltip("Название сцены главного меню.")]
    [SerializeField]
    private string menuSceneName = "MaineMenu";


    // ==================================================
    // RUNTIME
    // ==================================================

    private bool victoryStarted;
    private bool victoryShown;


    // ==================================================
    // UNITY
    // ==================================================

    private void Awake()
    {
        victoryStarted = false;
        victoryShown = false;

        if (victoryWindow != null)
        {
            victoryWindow.SetActive(false);
        }
    }


    private void Update()
    {
        if (victoryStarted || victoryShown)
            return;

        CheckVictory();
    }


    // ==================================================
    // VICTORY CHECK
    // ==================================================

    private void CheckVictory()
    {
        if (taskManager == null)
            return;

        if (gameState == null)
            return;

        if (victoryWindow == null)
            return;


        // Все задания должны быть выполнены.
        if (!taskManager.AreAllTasksCompleted())
            return;


        // Стабильность станции должна быть выше 50.
        if (gameState.StationStability <= minimumStationStability)
            return;


        // Не должно быть активной аварии.
        if (emergency != null &&
            emergency.IsActive)
        {
            return;
        }


        victoryStarted = true;

        StartCoroutine(
            ShowVictoryAfterDelay()
        );
    }


    // ==================================================
    // VICTORY DELAY
    // ==================================================

    private IEnumerator ShowVictoryAfterDelay()
    {
        yield return new WaitForSeconds(victoryDelay);


        // За время задержки могла начаться авария.
        if (emergency != null &&
            emergency.IsActive)
        {
            victoryStarted = false;
            yield break;
        }


        // За время задержки могла упасть стабильность.
        if (gameState.StationStability <=
            minimumStationStability)
        {
            victoryStarted = false;
            yield break;
        }


        ShowVictory();
    }


    // ==================================================
    // SHOW VICTORY
    // ==================================================

    private void ShowVictory()
    {
        if (victoryShown)
            return;


        victoryShown = true;

        victoryWindow.SetActive(true);

        // После появления окна останавливаем игру.
        Time.timeScale = 0f;
    }


    // ==================================================
    // MENU BUTTON
    // ==================================================

    public void ExitToMenu()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(menuSceneName))
            return;

        SceneManager.LoadScene(
            menuSceneName
        );
    }
}
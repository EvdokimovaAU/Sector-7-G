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


    [Header("Scene")]

    [Tooltip("Название сцены главного меню.")]
    [SerializeField]
    private string menuSceneName = "MaineMenu";


    // ==================================================
    // RUNTIME
    // ==================================================

    private bool victoryShown;


    // ==================================================
    // UNITY
    // ==================================================

    private void Awake()
    {
        victoryShown = false;

        // В начале игры окно победы скрыто.
        if (victoryWindow != null)
        {
            victoryWindow.SetActive(false);
        }
    }


    private void Start()
    {
        ValidateReferences();
    }


    private void Update()
    {
        // После победы больше ничего не проверяем.
        if (victoryShown)
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


        // ----------------------------------------------
        // 1. ВСЕ ЗАДАНИЯ ВЫПОЛНЕНЫ
        // ----------------------------------------------

        if (!taskManager.AreAllTasksCompleted())
            return;


        // ----------------------------------------------
        // 2. СТАБИЛЬНОСТЬ СТАНЦИИ > 50
        // ----------------------------------------------

        if (gameState.StationStability <=
            minimumStationStability)
        {
            return;
        }


        // ----------------------------------------------
        // 3. НЕТ АКТИВНОЙ АВАРИИ
        // ----------------------------------------------

        if (emergency != null &&
            emergency.IsActive)
        {
            return;
        }


        // Все условия выполнены.
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


        Debug.Log(
            "[VICTORY] Смена успешно завершена! " +
            $"Station Stability = {gameState.StationStability}"
        );


        victoryWindow.SetActive(true);


        // Останавливаем игровое время,
        // чтобы после победы ничего больше не происходило.
        Time.timeScale = 0f;
    }


    // ==================================================
    // MENU BUTTON
    // ==================================================

    public void ExitToMenu()
    {
        // Иначе после загрузки меню
        // Time.timeScale останется равным 0.
        Time.timeScale = 1f;


        if (string.IsNullOrWhiteSpace(menuSceneName))
        {
            Debug.LogError(
                "[VictoryScreen] Menu Scene Name не указан!"
            );

            return;
        }


        SceneManager.LoadScene(
            menuSceneName
        );
    }


    // ==================================================
    // VALIDATION
    // ==================================================

    private void ValidateReferences()
    {
        if (taskManager == null)
        {
            Debug.LogError(
                "[VictoryScreen] Task Manager не назначен!"
            );
        }


        if (emergency == null)
        {
            Debug.LogError(
                "[VictoryScreen] Emergency не назначен!"
            );
        }


        if (gameState == null)
        {
            Debug.LogError(
                "[VictoryScreen] Game State не назначен!"
            );
        }


        if (victoryWindow == null)
        {
            Debug.LogError(
                "[VictoryScreen] Victory Window не назначен!"
            );
        }
    }


    // ==================================================
    // DEBUG
    // ==================================================

    [ContextMenu("DEBUG Victory State")]
    private void DebugVictoryState()
    {
        Debug.Log(
            "========== VICTORY STATE ==========\n" +
            $"All Tasks Completed: " +
            $"{(taskManager != null && taskManager.AreAllTasksCompleted())}\n" +
            $"Station Stability: " +
            $"{(gameState != null ? gameState.StationStability : -1)}\n" +
            $"Emergency Active: " +
            $"{(emergency != null && emergency.IsActive)}\n" +
            "==================================="
        );
    }
}
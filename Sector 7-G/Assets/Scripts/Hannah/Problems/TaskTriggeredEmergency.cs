using System.Collections.Generic;
using UnityEngine;

public class TaskTriggeredEmergency : MonoBehaviour
{
    // ==================================================
    // REFERENCES
    // ==================================================

    [Header("References")]

    [SerializeField]
    private TaskManager taskManager;

    [SerializeField]
    private GameState gameState;


    // ==================================================
    // TRIGGER SETTINGS
    // ==================================================

    [Header("Trigger")]

    [Tooltip(
        "После какого количества выполненных " +
        "ежедневных заданий запускается авария."
    )]
    [Min(1)]
    [SerializeField]
    private int triggerAfterCompletedTasks = 2;


    // ==================================================
    // STABILITY
    // ==================================================

    [Header("Stability")]

    [Tooltip(
        "Изменение стабильности при начале аварии. " +
        "Для падения укажи отрицательное значение."
    )]
    [SerializeField]
    private int stabilityChangeOnStart = -35;


    [Tooltip(
        "Изменение стабильности после устранения аварии."
    )]
    [SerializeField]
    private int stabilityChangeOnResolved = 35;


    // ==================================================
    // SOLUTION
    // ==================================================

    [Header("Emergency Solution")]

    [Tooltip(
        "Последовательность действий, которую игрок " +
        "должен выполнить на панели."
    )]
    [SerializeField]
    private List<EmergencyStep> solution = new();


    // ==================================================
    // CURRENT STATE
    // ==================================================

    [Header("Runtime State")]

    [SerializeField]
    private bool isActive;

    [SerializeField]
    private bool isResolved;

    [SerializeField]
    private int currentStep;


    public bool IsActive => isActive;

    public bool IsResolved => isResolved;

    public int CurrentStep => currentStep;


    // ==================================================
    // UNITY
    // ==================================================

    private void OnEnable()
    {
        if (taskManager != null)
        {
            taskManager.OnTaskCompleted +=
                HandleTaskCompleted;
        }


        Panel.PanelEvents.OnElementChanged +=
            HandlePanelElementChanged;
    }


    private void Start()
    {
        ValidateReferences();
    }


    private void OnDisable()
    {
        if (taskManager != null)
        {
            taskManager.OnTaskCompleted -=
                HandleTaskCompleted;
        }


        Panel.PanelEvents.OnElementChanged -=
            HandlePanelElementChanged;
    }


    // ==================================================
    // TASK TRIGGER
    // ==================================================

    private void HandleTaskCompleted(
        int completedTaskCount)
    {
        // Уже запущенная авария повторно
        // не запускается.
        if (isActive)
            return;


        // Уже устранённая авария также
        // не запускается повторно.
        if (isResolved)
            return;


        // Ждём нужное количество заданий.
        if (completedTaskCount <
            triggerAfterCompletedTasks)
        {
            return;
        }


        StartEmergency();
    }


    // ==================================================
    // START EMERGENCY
    // ==================================================

    private void StartEmergency()
    {
        if (gameState == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "GameState is not assigned."
            );

            return;
        }


        isActive = true;

        currentStep = 0;


        // Например:
        //
        // стабильность = 100
        // stabilityChangeOnStart = -35
        //
        // результат = 65

        gameState.ChangeStationStability(
            stabilityChangeOnStart
        );


        Debug.Log(
            "[EMERGENCY STARTED] " +
            $"Stability changed by " +
            $"{stabilityChangeOnStart}."
        );


        if (solution == null ||
            solution.Count == 0)
        {
            Debug.LogWarning(
                "[TaskTriggeredEmergency] " +
                "Emergency started, but Solution is empty."
            );
        }
    }


    // ==================================================
    // PANEL INPUT
    // ==================================================

    private void HandlePanelElementChanged(
        Panel.PanelElementID elementID,
        int value)
    {
        // Пока аварии нет, действия игрока
        // нас не интересуют.
        if (!isActive)
            return;


        if (solution == null ||
            solution.Count == 0)
        {
            return;
        }


        if (currentStep < 0 ||
            currentStep >= solution.Count)
        {
            return;
        }


        EmergencyStep requiredStep =
            solution[currentStep];


        // ----------------------------------------------
        // CORRECT ACTION
        // ----------------------------------------------

        if (requiredStep.Matches(
                elementID,
                value))
        {
            currentStep++;


            Debug.Log(
                "[EMERGENCY] Correct action. " +
                $"Step {currentStep}/{solution.Count}"
            );


            // Все необходимые действия выполнены.
            if (currentStep >= solution.Count)
            {
                ResolveEmergency();
            }


            return;
        }


        // ----------------------------------------------
        // WRONG / UNRELATED ACTION
        // ----------------------------------------------

        Debug.Log(
            "[EMERGENCY] Action does not match " +
            $"current step: {elementID} = {value}"
        );
    }


    // ==================================================
    // RESOLVE
    // ==================================================

    private void ResolveEmergency()
    {
        if (!isActive)
            return;


        isActive = false;

        isResolved = true;


        if (gameState != null)
        {
            gameState.ChangeStationStability(
                stabilityChangeOnResolved
            );
        }


        Debug.Log(
            "[EMERGENCY RESOLVED] " +
            $"Stability changed by " +
            $"{stabilityChangeOnResolved}."
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
                "[TaskTriggeredEmergency] " +
                "TaskManager is not assigned."
            );
        }


        if (gameState == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "GameState is not assigned."
            );
        }
    }
}
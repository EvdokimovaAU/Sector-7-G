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
    // TRIGGER
    // ==================================================

    [Header("Trigger")]

    [Min(1)]
    [SerializeField]
    private int triggerAfterCompletedTasks = 2;


    // ==================================================
    // STABILITY
    // ==================================================

    [Header("Stability")]

    [SerializeField]
    private int stabilityChangeOnStart = -35;

    [SerializeField]
    private int stabilityChangeOnResolved = 35;


    // ==================================================
    // SOLUTION
    // ==================================================

    [Header("Emergency Solution")]

    [SerializeField]
    private List<EmergencyStep> solution = new();


    // ==================================================
    // STATE
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
        if (isActive)
            return;

        if (isResolved)
            return;


        if (completedTaskCount <
            triggerAfterCompletedTasks)
        {
            return;
        }


        StartEmergency();
    }


    // ==================================================
    // START
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
                "Emergency Solution is empty."
            );
        }
    }


    // ==================================================
    // CHECK EMERGENCY ACTION
    // ==================================================

    /// <summary>
    /// ѕровер€ет, €вл€етс€ ли действие текущим
    /// необходимым шагом устранени€ аварии.
    /// </summary>
    public bool IsActionRequiredByEmergency(
        Panel.PanelElementID elementID,
        int value)
    {
        if (!isActive)
            return false;


        if (solution == null ||
            solution.Count == 0)
        {
            return false;
        }


        if (currentStep < 0 ||
            currentStep >= solution.Count)
        {
            return false;
        }


        EmergencyStep requiredStep =
            solution[currentStep];


        return requiredStep.Matches(
            elementID,
            value
        );
    }


    // ==================================================
    // PANEL
    // ==================================================

    private void HandlePanelElementChanged(
        Panel.PanelElementID elementID,
        int value)
    {
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


        if (requiredStep.Matches(
                elementID,
                value))
        {
            currentStep++;


            Debug.Log(
                "[EMERGENCY] Correct action. " +
                $"Step {currentStep}/{solution.Count}"
            );


            if (currentStep >= solution.Count)
            {
                ResolveEmergency();
            }
        }
        else
        {
            Debug.Log(
                "[EMERGENCY] Wrong action: " +
                $"{elementID} = {value}"
            );
        }
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Station;

public class TaskTriggeredEmergency : MonoBehaviour
{
    // ==================================================
    // REFERENCES
    // ==================================================

    [Header("References")]
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameState gameState;
    [SerializeField] private StationMonitor stationMonitor;
    [SerializeField] private PhoneController phoneController;


    // ==================================================
    // EMERGENCY START
    // ==================================================

    [Header("Level 1 Emergency")]

    [Min(1)]
    [SerializeField] private int triggerAfterCompletedTasks = 1;

    [Min(0f)]
    [SerializeField] private float emergencyDelay = 3f;

    [SerializeField]
    private StationSector emergencySector =
        StationSector.ReactorShop;

    [SerializeField]
    private int stabilityLoss = 25;


    // ==================================================
    // JOURNAL / RESOLUTION
    // ==================================================

    [Header("Journal")]

    [Tooltip("ID должен совпадать с Emergency ID страницы журнала")]
    [SerializeField]
    private string emergencyID = "level1_emergency";


    [Header("Emergency Resolution")]

    [Tooltip("Действия, которые нужно выполнить по порядку")]
    [SerializeField]
    private List<EmergencyStep> emergencySteps = new();


    // ==================================================
    // PHONE
    // ==================================================

    [Header("Phone Responses")]

    [TextArea]
    [SerializeField]
    private string confirmedEmergencyText =
        "Тревога подтверждена. В цехе действительно аварийная ситуация.";

    [TextArea]
    [SerializeField]
    private string wrongSectorText =
        "В этом цехе всё в норме. Тревога не подтверждается.";


    // ==================================================
    // RUNTIME
    // ==================================================

    [Header("Runtime State")]

    [SerializeField]
    private bool waitingForEmergency;

    [SerializeField]
    private bool isActive;

    [SerializeField]
    private bool isConfirmed;

    [SerializeField]
    private bool hasTriggered;

    [SerializeField]
    private int currentStepIndex;


    // ==================================================
    // PUBLIC
    // ==================================================

    public bool IsActive => isActive;

    public bool IsConfirmed => isConfirmed;

    public StationSector EmergencySector =>
        emergencySector;

    public string EmergencyID =>
        emergencyID;

    public int CompletedStepCount =>
        currentStepIndex;

    public int TotalStepCount =>
        emergencySteps != null
            ? emergencySteps.Count
            : 0;


    // ==================================================
    // EVENTS FOR JOURNAL
    // ==================================================

    public event Action OnEmergencyStarted;

    public event Action OnEmergencyProgressChanged;

    public event Action OnEmergencyResolved;


    // ==================================================
    // UNITY
    // ==================================================

    private void OnEnable()
    {
        if (taskManager != null)
        {
            // ВАЖНО:
            // OnTaskCompleted = Action<int>
            taskManager.OnTaskCompleted +=
                HandleTaskCompleted;
        }

        StationEvents.SectorCalled +=
            HandleSectorCalled;

        // ВАЖНО:
        // используем именно Panel.PanelEvents
        Panel.PanelEvents.OnElementChanged +=
            HandlePanelElementChanged;
    }


    private void OnDisable()
    {
        if (taskManager != null)
        {
            taskManager.OnTaskCompleted -=
                HandleTaskCompleted;
        }

        StationEvents.SectorCalled -=
            HandleSectorCalled;

        Panel.PanelEvents.OnElementChanged -=
            HandlePanelElementChanged;
    }


    private void Start()
    {
        ValidateReferences();
    }


    // ==================================================
    // DAILY TASK
    // ==================================================

    private void HandleTaskCompleted(
        int completedTaskCount)
    {
        // Авария первого уровня запускается
        // только один раз.
        if (hasTriggered)
            return;

        if (waitingForEmergency)
            return;

        if (isActive)
            return;

        if (completedTaskCount <
            triggerAfterCompletedTasks)
        {
            return;
        }


        StartCoroutine(
            EmergencyDelayRoutine()
        );
    }


    private IEnumerator EmergencyDelayRoutine()
    {
        waitingForEmergency = true;

        Debug.Log(
            $"[EMERGENCY] Авария начнётся через " +
            $"{emergencyDelay} сек."
        );


        yield return new WaitForSeconds(
            emergencyDelay
        );


        waitingForEmergency = false;

        StartEmergency();
    }


    // ==================================================
    // START EMERGENCY
    // ==================================================

    private void StartEmergency()
    {
        if (hasTriggered)
            return;


        hasTriggered = true;

        isActive = true;

        isConfirmed = false;

        currentStepIndex = 0;


        // Подсвечиваем проблемный цех.
        if (stationMonitor != null)
        {
            stationMonitor.SetSectorProblem(
                emergencySector,
                true
            );
        }


        // Снижаем стабильность станции.
        if (gameState != null)
        {
            gameState.ChangeStationStability(
                -Mathf.Abs(stabilityLoss)
            );
        }


        Debug.Log(
            $"[EMERGENCY STARTED] " +
            $"Цех: {emergencySector}. " +
            $"Стабильность снижена на " +
            $"{Mathf.Abs(stabilityLoss)}%."
        );


        // Сообщаем журналу,
        // что авария появилась.
        OnEmergencyStarted?.Invoke();

        OnEmergencyProgressChanged?.Invoke();
    }


    // ==================================================
    // PHONE
    // ==================================================

    private void HandleSectorCalled(
        StationSector calledSector)
    {
        if (!isActive)
            return;


        // Позвонили в правильный цех.
        if (calledSector == emergencySector)
        {
            ConfirmEmergency();
            return;
        }


        // Позвонили не в тот цех.
        if (phoneController != null)
        {
            phoneController.ShowResponse(
                wrongSectorText
            );
        }


        Debug.Log(
            $"[EMERGENCY] Игрок позвонил в " +
            $"{calledSector}, но авария находится " +
            $"в {emergencySector}."
        );
    }


    private void ConfirmEmergency()
    {
        if (isConfirmed)
            return;


        isConfirmed = true;


        if (phoneController != null)
        {
            phoneController.ShowResponse(
                confirmedEmergencyText
            );
        }


        Debug.Log(
            $"[EMERGENCY CONFIRMED] " +
            $"Авария в {emergencySector} подтверждена."
        );
    }


    // ==================================================
    // EMERGENCY STEPS
    // ==================================================

    private void HandlePanelElementChanged(
        Panel.PanelElementID elementID,
        int value)
    {
        // Если аварии нет,
        // действия панели не проверяем.
        if (!isActive)
            return;


        if (emergencySteps == null)
            return;


        if (emergencySteps.Count == 0)
            return;


        // Все шаги уже выполнены.
        if (currentStepIndex >=
            emergencySteps.Count)
        {
            return;
        }


        EmergencyStep currentStep =
            emergencySteps[currentStepIndex];


        // Проверяем ТОЛЬКО текущий шаг.
        //
        // Например:
        // 1. Pump2 = 1
        // 2. Valve1 = 0
        //
        // Valve1 раньше Pump2
        // второй шаг не выполнит.
        if (!currentStep.Matches(
                elementID,
                value))
        {
            return;
        }


        // Текущий шаг выполнен.
        currentStepIndex++;


        Debug.Log(
            $"[EMERGENCY STEP] " +
            $"{currentStepIndex}/" +
            $"{emergencySteps.Count}"
        );


        // Журнал обновит галочки.
        OnEmergencyProgressChanged?.Invoke();


        // Выполнен последний шаг.
        if (currentStepIndex >=
            emergencySteps.Count)
        {
            ResolveEmergency();
        }
    }


    // ==================================================
    // RESOLVE
    // ==================================================

    public void ResolveEmergency()
    {
        if (!isActive)
            return;


        isActive = false;


        // Убираем красный цех.
        if (stationMonitor != null)
        {
            stationMonitor.SetSectorProblem(
                emergencySector,
                false
            );
        }


        Debug.Log(
            $"[EMERGENCY RESOLVED] " +
            $"Авария в {emergencySector} устранена."
        );


        // Сначала обновляем UI шагов.
        OnEmergencyProgressChanged?.Invoke();

        // Потом сообщаем об устранении.
        OnEmergencyResolved?.Invoke();
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
                "TaskManager не назначен."
            );
        }


        if (gameState == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "GameState не назначен."
            );
        }


        if (stationMonitor == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "StationMonitor не назначен."
            );
        }


        if (phoneController == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "PhoneController не назначен."
            );
        }
    }
}
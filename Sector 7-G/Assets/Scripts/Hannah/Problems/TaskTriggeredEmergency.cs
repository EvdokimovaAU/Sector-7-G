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

    [SerializeField]
    private TaskManager taskManager;

    [SerializeField]
    private GameState gameState;

    [SerializeField]
    private StationMonitor stationMonitor;

    [SerializeField]
    private PhoneController phoneController;

    [SerializeField]
    private EmergencyJournal emergencyJournal;


    // ==================================================
    // DAILY TASK TRIGGER
    // ==================================================

    [Header("Daily Task Trigger")]

    [Min(1)]
    [SerializeField]
    private int triggerAfterCompletedTasks = 1;

    [Min(0f)]
    [SerializeField]
    private float emergencyDelay = 3f;


    // ==================================================
    // LOW STABILITY TRIGGER
    // ==================================================

    [Header("Low Stability Trigger")]

    [Range(0, 100)]
    [SerializeField]
    private int lowStabilityThreshold = 50;


    // ==================================================
    // EMERGENCY DAMAGE
    // ==================================================

    [Header("Emergency Damage")]

    [SerializeField]
    private int stabilityLoss = 25;


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
    // CURRENT EMERGENCY DATA
    // ==================================================

    [Header("Current Emergency")]

    [SerializeField]
    private string emergencyID;

    [SerializeField]
    private StationSector emergencySector;

    [SerializeField]
    private List<EmergencyStep> emergencySteps = new();


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
    private bool dailyEmergencyTriggered;

    [SerializeField]
    private bool lowStabilityTriggered;

    [SerializeField]
    private int currentStepIndex;


    // Запоминаем, снимала ли именно текущая
    // авария стабильность АЭС.
    //
    // Авария после ежедневного задания: ДА.
    // Авария из-за уже низкой стабильности: НЕТ.
    private bool currentEmergencyAppliedDamage;


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
    // EVENTS
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
            taskManager.OnTaskCompleted +=
                HandleTaskCompleted;
        }


        if (gameState != null)
        {
            gameState.OnStationStabilityChanged +=
                HandleStationStabilityChanged;
        }


        StationEvents.SectorCalled +=
            HandleSectorCalled;


        Panel.PanelEvents.OnElementChanged +=
            HandlePanelElementChanged;
    }


    private void Start()
    {
        ValidateReferences();


        // На случай, если ссылки были назначены
        // автоматически в ValidateReferences().
        Resubscribe();


        // Проверяем начальную стабильность.
        if (gameState != null)
        {
            CheckLowStability(
                gameState.StationStability
            );
        }
    }


    private void OnDisable()
    {
        if (taskManager != null)
        {
            taskManager.OnTaskCompleted -=
                HandleTaskCompleted;
        }


        if (gameState != null)
        {
            gameState.OnStationStabilityChanged -=
                HandleStationStabilityChanged;
        }


        StationEvents.SectorCalled -=
            HandleSectorCalled;


        Panel.PanelEvents.OnElementChanged -=
            HandlePanelElementChanged;
    }


    private void Resubscribe()
    {
        if (taskManager != null)
        {
            taskManager.OnTaskCompleted -=
                HandleTaskCompleted;

            taskManager.OnTaskCompleted +=
                HandleTaskCompleted;
        }


        if (gameState != null)
        {
            gameState.OnStationStabilityChanged -=
                HandleStationStabilityChanged;

            gameState.OnStationStabilityChanged +=
                HandleStationStabilityChanged;
        }
    }


    // ==================================================
    // DAILY TASK TRIGGER
    // ==================================================

    private void HandleTaskCompleted(
        int completedTaskCount)
    {
        // После первого выполненного задания
        // авария создаётся только один раз.
        if (dailyEmergencyTriggered)
            return;


        if (completedTaskCount <
            triggerAfterCompletedTasks)
        {
            return;
        }


        dailyEmergencyTriggered = true;


        Debug.Log(
            $"[EMERGENCY TRIGGER] " +
            $"Выполнено заданий: {completedTaskCount}. " +
            $"Через {emergencyDelay} сек. будет случайная авария."
        );


        StartCoroutine(
            DailyEmergencyDelayRoutine()
        );
    }


    private IEnumerator DailyEmergencyDelayRoutine()
    {
        waitingForEmergency = true;


        yield return new WaitForSeconds(
            emergencyDelay
        );


        waitingForEmergency = false;


        // Если за эти 3 секунды уже появилась
        // другая авария, вторую не создаём.
        if (isActive)
        {
            Debug.Log(
                "[EMERGENCY] Уже есть активная авария. " +
                "Авария после задания не запускается."
            );

            yield break;
        }


        StartRandomEmergency(
            true
        );
    }


    // ==================================================
    // LOW STABILITY
    // ==================================================

    private void HandleStationStabilityChanged(
        int stability)
    {
        CheckLowStability(
            stability
        );
    }


    private void CheckLowStability(
        int stability)
    {
        // Пользователь просил именно НИЖЕ 50.
        if (stability >= lowStabilityThreshold)
        {
            // Стабильность восстановилась.
            // Разрешаем условию сработать снова,
            // если она потом опять упадёт ниже 50.
            lowStabilityTriggered = false;

            return;
        }


        // Уже обработали текущее падение ниже 50.
        if (lowStabilityTriggered)
            return;


        lowStabilityTriggered = true;


        Debug.LogWarning(
            $"[LOW STABILITY] " +
            $"Стабильность АЭС = {stability}. " +
            $"Запускаем случайную проблему."
        );


        // Если авария уже идёт —
        // вторую поверх неё не создаём.
        if (isActive || waitingForEmergency)
        {
            Debug.Log(
                "[LOW STABILITY] Авария уже активна " +
                "или ожидает запуска."
            );

            return;
        }


        // ВАЖНО:
        // сама низкая стабильность уже является причиной
        // появления проблемы, поэтому дополнительные
        // -25 здесь НЕ снимаем.
        StartRandomEmergency(
            false
        );
    }


    // ==================================================
    // RANDOM EMERGENCY
    // ==================================================

    private void StartRandomEmergency(
        bool applyStabilityDamage)
    {
        if (isActive)
            return;


        if (emergencyJournal == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "Emergency Journal не назначен."
            );

            return;
        }


        EmergencyJournalPage page =
            emergencyJournal.GetRandomEmergencyPage();


        if (page == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "Не удалось получить случайную " +
                "страницу аварии."
            );

            return;
        }


        StartEmergencyFromPage(
            page,
            applyStabilityDamage
        );
    }


    // ==================================================
    // START FROM JOURNAL PAGE
    // ==================================================

    private void StartEmergencyFromPage(
        EmergencyJournalPage page,
        bool applyStabilityDamage)
    {
        if (page == null)
            return;


        if (isActive)
            return;


        emergencyID =
            page.EmergencyID;


        emergencySector =
            page.Sector;


        emergencySteps.Clear();


        if (page.Steps != null)
        {
            foreach (
                EmergencyStep step
                in page.Steps)
            {
                if (step != null)
                {
                    emergencySteps.Add(
                        step
                    );
                }
            }
        }


        currentStepIndex = 0;

        isActive = true;

        isConfirmed = false;

        currentEmergencyAppliedDamage =
            applyStabilityDamage;


        // Красный цех на экране.
        if (stationMonitor != null)
        {
            stationMonitor.SetSectorProblem(
                emergencySector,
                true
            );
        }


        // Если авария появилась после
        // ежедневного задания:
        // снимаем 25% стабильности.
        //
        // Если проблема появилась потому,
        // что стабильность УЖЕ < 50:
        // дополнительно ничего не снимаем.
        if (applyStabilityDamage &&
            gameState != null)
        {
            gameState.ChangeStationStability(
                -Mathf.Abs(stabilityLoss)
            );
        }


        Debug.LogWarning(
            $"[RANDOM EMERGENCY STARTED] " +
            $"ID = {emergencyID}, " +
            $"Sector = {emergencySector}, " +
            $"Steps = {emergencySteps.Count}, " +
            $"Station damage = " +
            $"{(applyStabilityDamage ? stabilityLoss : 0)}"
        );


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


        if (calledSector ==
            emergencySector)
        {
            ConfirmEmergency();

            return;
        }


        if (phoneController != null)
        {
            phoneController.ShowResponse(
                wrongSectorText
            );
        }


        Debug.Log(
            $"[EMERGENCY] Позвонили в {calledSector}, " +
            $"но авария находится в {emergencySector}."
        );
    }


    private void ConfirmEmergency()
    {
        if (!isActive)
            return;


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
            $"{emergencyID}, " +
            $"цех {emergencySector}."
        );
    }


    // ==================================================
    // EMERGENCY STEPS
    // ==================================================

    private void HandlePanelElementChanged(
        Panel.PanelElementID elementID,
        int value)
    {
        if (!isActive)
            return;


        if (emergencySteps == null ||
            emergencySteps.Count == 0)
        {
            return;
        }


        if (currentStepIndex >=
            emergencySteps.Count)
        {
            return;
        }


        EmergencyStep currentStep =
            emergencySteps[currentStepIndex];


        if (currentStep == null)
            return;


        // Шаги выполняются строго по порядку.
        if (!currentStep.Matches(
                elementID,
                value))
        {
            return;
        }


        currentStepIndex++;


        Debug.Log(
            $"[EMERGENCY STEP] " +
            $"{emergencyID}: " +
            $"{currentStepIndex}/" +
            $"{emergencySteps.Count}"
        );


        OnEmergencyProgressChanged?.Invoke();


        if (currentStepIndex >=
            emergencySteps.Count)
        {
            ResolveEmergency();
        }
    }


    // ==================================================
    // SAFE ACTION CHECK
    // ==================================================

    public bool IsValidEmergencyAction(
        Panel.PanelElementID elementID,
        int value)
    {
        if (!isActive)
            return false;


        if (emergencySteps == null ||
            emergencySteps.Count == 0)
        {
            return false;
        }


        // Любое действие из списка устранения
        // текущей аварии безопасно.
        //
        // WrongPanelActionHandler не должен
        // снимать за него шкалы.
        foreach (
            EmergencyStep step
            in emergencySteps)
        {
            if (step == null)
                continue;


            if (step.Matches(
                    elementID,
                    value))
            {
                return true;
            }
        }


        return false;
    }


    // ==================================================
    // CURRENT STEP
    // ==================================================

    public bool IsCurrentEmergencyStep(
        Panel.PanelElementID elementID,
        int value)
    {
        if (!isActive)
            return false;


        if (emergencySteps == null ||
            emergencySteps.Count == 0)
        {
            return false;
        }


        if (currentStepIndex < 0 ||
            currentStepIndex >=
            emergencySteps.Count)
        {
            return false;
        }


        EmergencyStep currentStep =
            emergencySteps[currentStepIndex];


        if (currentStep == null)
            return false;


        return currentStep.Matches(
            elementID,
            value
        );
    }


    // ==================================================
    // RESOLVE
    // ==================================================

    public void ResolveEmergency()
    {
        if (!isActive)
            return;


        // Возвращаем 25% ТОЛЬКО если именно
        // эта авария их сняла при старте.
        //
        // Для проблемы, возникшей из-за
        // StationStability < 50,
        // дополнительные 25 не начисляем.
        if (currentEmergencyAppliedDamage &&
            gameState != null)
        {
            gameState.ChangeStationStability(
                Mathf.Abs(stabilityLoss)
            );


            Debug.Log(
                $"[EMERGENCY RESTORE] " +
                $"Station +{Mathf.Abs(stabilityLoss)}%"
            );
        }


        isActive = false;

        isConfirmed = false;


        if (stationMonitor != null)
        {
            stationMonitor.SetSectorProblem(
                emergencySector,
                false
            );
        }


        Debug.Log(
            $"[EMERGENCY RESOLVED] " +
            $"{emergencyID} устранена."
        );


        OnEmergencyProgressChanged?.Invoke();

        OnEmergencyResolved?.Invoke();


        currentEmergencyAppliedDamage = false;
    }


    // ==================================================
    // VALIDATION
    // ==================================================

    private void ValidateReferences()
    {
        if (taskManager == null)
        {
            taskManager =
                FindAnyObjectByType<TaskManager>();
        }


        if (gameState == null)
        {
            gameState =
                FindAnyObjectByType<GameState>();
        }


        if (stationMonitor == null)
        {
            stationMonitor =
                FindAnyObjectByType<StationMonitor>();
        }


        if (phoneController == null)
        {
            phoneController =
                FindAnyObjectByType<PhoneController>();
        }


        if (emergencyJournal == null)
        {
            emergencyJournal =
                FindAnyObjectByType<EmergencyJournal>();
        }


        if (taskManager == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "TaskManager не найден."
            );
        }


        if (gameState == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "GameState не найден."
            );
        }


        if (stationMonitor == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "StationMonitor не найден."
            );
        }


        if (phoneController == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "PhoneController не найден."
            );
        }


        if (emergencyJournal == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] " +
                "EmergencyJournal не найден."
            );
        }
    }
}
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
    // LOW STABILITY
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
    // FALSE ALARM
    // ==================================================

    [Header("False Alarm")]

    [Tooltip("Вероятность ложной тревоги. 0.2 = 20%")]
    [Range(0f, 1f)]
    [SerializeField]
    private float falseAlarmChance = 0.2f;

    [TextArea]
    [SerializeField]
    private string falseAlarmPhoneText =
        "В цехе всё в норме. Похоже, тревога ложная. Нажмите RESET для сброса сигнала.";


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
    // CURRENT EMERGENCY
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


    // ==================================================
    // FALSE ALARM RUNTIME
    // ==================================================

    [Header("False Alarm Runtime")]

    [SerializeField]
    private bool isFalseAlarm;

    [SerializeField]
    private bool falseAlarmConfirmed;

    private int stationStabilityBeforeEmergency;
    private int panelReliabilityBeforeEmergency;

    private bool currentEmergencyAppliedDamage;


    // ==================================================
    // PUBLIC
    // ==================================================

    public bool IsActive => isActive;

    public bool IsConfirmed => isConfirmed;

    public bool IsFalseAlarm => isFalseAlarm;

    public bool IsWaitingForEmergency =>
        waitingForEmergency;

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
        Resubscribe();

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
        if (dailyEmergencyTriggered)
            return;

        if (completedTaskCount <
            triggerAfterCompletedTasks)
        {
            return;
        }

        dailyEmergencyTriggered = true;

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

        if (isActive)
            yield break;

        StartRandomEmergency(true);
    }


    // ==================================================
    // LOW STABILITY
    // ==================================================

    private void HandleStationStabilityChanged(
        int stability)
    {
        CheckLowStability(stability);
    }


    private void CheckLowStability(
        int stability)
    {
        if (stability >= lowStabilityThreshold)
        {
            lowStabilityTriggered = false;
            return;
        }

        if (lowStabilityTriggered)
            return;

        lowStabilityTriggered = true;

        if (isActive || waitingForEmergency)
            return;

        StartRandomEmergency(false);
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
            return;

        EmergencyJournalPage page =
            emergencyJournal.GetRandomEmergencyPage();

        if (page == null)
            return;

        StartEmergencyFromPage(
            page,
            applyStabilityDamage
        );
    }


    // ==================================================
    // START EMERGENCY
    // ==================================================

    private void StartEmergencyFromPage(
        EmergencyJournalPage page,
        bool applyStabilityDamage)
    {
        if (page == null)
            return;

        if (isActive)
            return;


        // --------------------------------------------------
        // ЗАПОМИНАЕМ СОСТОЯНИЕ ДО ТРЕВОГИ
        // --------------------------------------------------

        if (gameState != null)
        {
            stationStabilityBeforeEmergency =
                gameState.StationStability;

            panelReliabilityBeforeEmergency =
                gameState.PanelReliability;
        }


        // --------------------------------------------------
        // 20% ШАНС ЛОЖНОЙ ТРЕВОГИ
        // --------------------------------------------------

        isFalseAlarm =
            UnityEngine.Random.value <
            falseAlarmChance;

        falseAlarmConfirmed = false;


        // --------------------------------------------------
        // ДАННЫЕ АВАРИИ
        // --------------------------------------------------

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
                    emergencySteps.Add(step);
                }
            }
        }


        currentStepIndex = 0;

        isActive = true;
        isConfirmed = false;

        currentEmergencyAppliedDamage =
            applyStabilityDamage;


        // --------------------------------------------------
        // КРАСНЫЙ ЦЕХ
        // --------------------------------------------------

        if (stationMonitor != null)
        {
            stationMonitor.SetSectorProblem(
                emergencySector,
                true
            );
        }


        // --------------------------------------------------
        // УРОН
        // --------------------------------------------------

        if (applyStabilityDamage &&
            gameState != null)
        {
            gameState.ChangeStationStability(
                -Mathf.Abs(stabilityLoss)
            );
        }


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


        // Позвонили НЕ в тот цех.
        if (calledSector != emergencySector)
        {
            if (phoneController != null)
            {
                phoneController.ShowResponse(
                    wrongSectorText
                );
            }

            return;
        }


        // ==================================================
        // FALSE ALARM
        // ==================================================

        if (isFalseAlarm)
        {
            falseAlarmConfirmed = true;
            isConfirmed = true;

            if (phoneController != null)
            {
                phoneController.ShowResponse(
                    falseAlarmPhoneText
                );
            }

            return;
        }


        // ==================================================
        // REAL EMERGENCY
        // ==================================================

        ConfirmEmergency();
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
    }


    // ==================================================
    // PANEL ACTION
    // ==================================================

    private void HandlePanelElementChanged(
        Panel.PanelElementID elementID,
        int value)
    {
        if (!isActive)
            return;


        // ==================================================
        // RESET FALSE ALARM
        // ==================================================

        // RESET работает для ложной тревоги
        // только ПОСЛЕ звонка в правильный цех.

        if (isFalseAlarm &&
            falseAlarmConfirmed &&
            elementID == Panel.PanelElementID.Reset)
        {
            ResolveFalseAlarm();
            return;
        }


        // ==================================================
        // FALSE ALARM
        // ==================================================

        // При ложной тревоге никакие шаги
        // из журнала выполнять не нужно.
        //
        // Сначала звонок, затем RESET.

        if (isFalseAlarm)
            return;


        // ==================================================
        // REAL EMERGENCY STEPS
        // ==================================================

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

        if (!currentStep.Matches(
                elementID,
                value))
        {
            return;
        }

        currentStepIndex++;

        OnEmergencyProgressChanged?.Invoke();

        if (currentStepIndex >=
            emergencySteps.Count)
        {
            ResolveEmergency();
        }
    }


    // ==================================================
    // FALSE ALARM RESOLVE
    // ==================================================

    private void ResolveFalseAlarm()
    {
        if (!isActive)
            return;

        if (!isFalseAlarm)
            return;

        if (!falseAlarmConfirmed)
            return;


        // ==================================================
        // ВОССТАНАВЛИВАЕМ ОБЕ ШКАЛЫ
        // ТОЧНО К СОСТОЯНИЮ ДО ТРЕВОГИ
        // ==================================================

        if (gameState != null)
        {
            int stationDifference =
                stationStabilityBeforeEmergency -
                gameState.StationStability;

            int panelDifference =
                panelReliabilityBeforeEmergency -
                gameState.PanelReliability;


            if (stationDifference != 0)
            {
                gameState.ChangeStationStability(
                    stationDifference
                );
            }

            if (panelDifference != 0)
            {
                gameState.ChangePanelReliability(
                    panelDifference
                );
            }
        }


        // Убираем красный цех.
        if (stationMonitor != null)
        {
            stationMonitor.SetSectorProblem(
                emergencySector,
                false
            );
        }


        // Сбрасываем тревогу.
        isActive = false;
        isConfirmed = false;

        isFalseAlarm = false;
        falseAlarmConfirmed = false;

        currentStepIndex = 0;

        currentEmergencyAppliedDamage = false;


        OnEmergencyProgressChanged?.Invoke();
        OnEmergencyResolved?.Invoke();
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


        // RESET подтверждённой ложной тревоги
        // считается безопасным действием.
        if (isFalseAlarm &&
            falseAlarmConfirmed &&
            elementID == Panel.PanelElementID.Reset)
        {
            return true;
        }


        // При ложной тревоге остальные
        // действия панели не являются
        // действиями устранения аварии.
        if (isFalseAlarm)
            return false;


        if (emergencySteps == null ||
            emergencySteps.Count == 0)
        {
            return false;
        }


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

        if (isFalseAlarm)
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
    // REAL EMERGENCY RESOLVE
    // ==================================================

    public void ResolveEmergency()
    {
        if (!isActive)
            return;


        // Для ложной тревоги используется
        // ResolveFalseAlarm().
        if (isFalseAlarm)
            return;


        // Возвращаем 25 только если
        // эта авария сама их сняла.
        if (currentEmergencyAppliedDamage &&
            gameState != null)
        {
            gameState.ChangeStationStability(
                Mathf.Abs(stabilityLoss)
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
    }
}
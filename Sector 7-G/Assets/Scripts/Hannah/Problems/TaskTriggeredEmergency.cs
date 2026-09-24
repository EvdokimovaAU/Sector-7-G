using System.Collections;
using UnityEngine;
using Station;

public class TaskTriggeredEmergency : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameState gameState;
    [SerializeField] private StationMonitor stationMonitor;
    [SerializeField] private PhoneController phoneController;


    [Header("Level 1 Emergency")]

    // После какого выполненного ежедневного задания
    // должна начаться авария.
    [Min(1)]
    [SerializeField] private int triggerAfterCompletedTasks = 1;

    // Задержка после выполнения задания.
    [Min(0f)]
    [SerializeField] private float emergencyDelay = 3f;

    // Цех, в котором произойдет авария.
    [SerializeField]
    private StationSector emergencySector =
        StationSector.ReactorShop;

    // Насколько падает стабильность АЭС.
    [SerializeField] private int stabilityLoss = 25;


    [Header("Phone Responses")]

    [TextArea]
    [SerializeField]
    private string confirmedEmergencyText =
        "Тревога подтверждена. В цехе действительно аварийная ситуация.";

    [TextArea]
    [SerializeField]
    private string wrongSectorText =
        "В этом цехе всё в норме. Тревога не подтверждается.";


    [Header("Runtime State")]

    [SerializeField] private bool waitingForEmergency;
    [SerializeField] private bool isActive;
    [SerializeField] private bool isConfirmed;
    [SerializeField] private bool hasTriggered;


    public bool IsActive => isActive;
    public bool IsConfirmed => isConfirmed;
    public StationSector EmergencySector => emergencySector;


    private void OnEnable()
    {
        if (taskManager != null)
        {
            taskManager.OnTaskCompleted += HandleTaskCompleted;
        }

        StationEvents.SectorCalled += HandleSectorCalled;
    }


    private void OnDisable()
    {
        if (taskManager != null)
        {
            taskManager.OnTaskCompleted -= HandleTaskCompleted;
        }

        StationEvents.SectorCalled -= HandleSectorCalled;
    }


    private void Start()
    {
        ValidateReferences();
    }


    // ==================================================
    // DAILY TASK
    // ==================================================

    private void HandleTaskCompleted(int completedTaskCount)
    {
        // Авария первого уровня должна произойти
        // только один раз.
        if (hasTriggered)
            return;

        if (waitingForEmergency)
            return;

        if (isActive)
            return;

        if (completedTaskCount < triggerAfterCompletedTasks)
            return;


        StartCoroutine(EmergencyDelayRoutine());
    }


    private IEnumerator EmergencyDelayRoutine()
    {
        waitingForEmergency = true;

        Debug.Log(
            $"[EMERGENCY] Авария начнётся через {emergencyDelay} сек."
        );

        yield return new WaitForSeconds(emergencyDelay);

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


        // Подсвечиваем аварийный цех.
        if (stationMonitor != null)
        {
            stationMonitor.SetSectorProblem(
                emergencySector,
                true
            );
        }


        // Снижаем стабильность АЭС.
        if (gameState != null)
        {
            gameState.ChangeStationStability(
                -Mathf.Abs(stabilityLoss)
            );
        }


        Debug.Log(
            $"[EMERGENCY STARTED] Цех: {emergencySector}. " +
            $"Стабильность АЭС снижена на {Mathf.Abs(stabilityLoss)}%."
        );
    }


    // ==================================================
    // PHONE
    // ==================================================

    private void HandleSectorCalled(StationSector calledSector)
    {
        // Если авария ещё не началась,
        // звонок к ней отношения не имеет.
        if (!isActive)
            return;


        // Игрок позвонил именно в аварийный цех.
        if (calledSector == emergencySector)
        {
            ConfirmEmergency();
            return;
        }


        // Игрок позвонил не туда.
        if (phoneController != null)
        {
            phoneController.ShowResponse(
                wrongSectorText
            );
        }


        Debug.Log(
            $"[EMERGENCY] Игрок позвонил в {calledSector}, " +
            $"но авария находится в {emergencySector}."
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
            $"Авария в {emergencySector} подтверждена по телефону."
        );
    }


    // ==================================================
    // FUTURE RESOLVE
    // ==================================================

    // Этот метод потом можно будет вызвать,
    // когда игрок действительно устранит аварию.
    public void ResolveEmergency()
    {
        if (!isActive)
            return;


        isActive = false;


        if (stationMonitor != null)
        {
            stationMonitor.SetSectorProblem(
                emergencySector,
                false
            );
        }


        Debug.Log(
            $"[EMERGENCY RESOLVED] Авария в {emergencySector} устранена."
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
                "[TaskTriggeredEmergency] TaskManager не назначен."
            );
        }

        if (gameState == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] GameState не назначен."
            );
        }

        if (stationMonitor == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] StationMonitor не назначен."
            );
        }

        if (phoneController == null)
        {
            Debug.LogError(
                "[TaskTriggeredEmergency] PhoneController не назначен."
            );
        }
    }
}
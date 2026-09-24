using UnityEngine;

public class WrongPanelActionHandler : MonoBehaviour
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


    // ==================================================
    // NORMAL WRONG ACTION
    // ==================================================

    [Header("Normal Wrong Action")]

    [Tooltip("Минимальный штраф за обычную неправильную кнопку.")]
    [Min(0)]
    [SerializeField]
    private int minStabilityPenalty = 5;

    [Tooltip("Максимальный штраф за обычную неправильную кнопку.")]
    [Min(0)]
    [SerializeField]
    private int maxStabilityPenalty = 20;


    // ==================================================
    // DANGEROUS BUTTONS
    // ==================================================

    [Header("Dangerous Buttons")]

    [Tooltip("Штраф стабильности за опасную кнопку.")]
    [Min(0)]
    [SerializeField]
    private int dangerousStabilityPenalty = 50;


    [Header("SOS")]

    [Tooltip("Штраф надежности панели за неправильное нажатие SOS.")]
    [Min(0)]
    [SerializeField]
    private int sosReliabilityPenalty = 50;


    // Одно физическое нажатие мыши =
    // максимум один штраф.
    private int lastPenaltyFrame = -1;


    // ==================================================
    // UNITY
    // ==================================================

    private void OnEnable()
    {
        Panel.PanelEvents.OnElementChanged +=
            HandlePanelElementChanged;
    }


    private void OnDisable()
    {
        Panel.PanelEvents.OnElementChanged -=
            HandlePanelElementChanged;
    }


    private void Start()
    {
        ValidateReferences();
    }


    // ==================================================
    // PANEL ACTION
    // ==================================================

    private void HandlePanelElementChanged(
        Panel.PanelElementID elementID,
        int value)
    {
        // --------------------------------------------------
        // ЕЖЕДНЕВНОЕ ЗАДАНИЕ
        // --------------------------------------------------

        if (taskManager != null)
        {
            if (taskManager.IsActionRequiredByTask(
                    elementID,
                    value))
            {
                // Эта кнопка сейчас нужна заданию.
                // Штрафа нет.
                return;
            }
        }


        // --------------------------------------------------
        // УСТРАНЕНИЕ АВАРИИ
        // --------------------------------------------------

        if (emergency != null)
        {
            if (emergency.IsActionRequiredByEmergency(
                    elementID,
                    value))
            {
                // Эта кнопка сейчас нужна для аварии.
                // Штрафа нет.
                return;
            }
        }


        // --------------------------------------------------
        // НЕПРАВИЛЬНОЕ ДЕЙСТВИЕ
        // --------------------------------------------------

        ApplyWrongActionPenalty(
            elementID,
            value
        );
    }


    // ==================================================
    // PENALTY
    // ==================================================

    private void ApplyWrongActionPenalty(
        Panel.PanelElementID elementID,
        int value)
    {
        if (gameState == null)
            return;


        // Защита от нескольких PanelEvents
        // от одного клика.
        if (lastPenaltyFrame == Time.frameCount)
            return;


        lastPenaltyFrame = Time.frameCount;


        // --------------------------------------------------
        // SOS
        // --------------------------------------------------

        if (elementID == Panel.PanelElementID.EmergencyShutdown)
        {
            // -50 стабильности станции.
            gameState.ChangeStationStability(
                -dangerousStabilityPenalty
            );


            // -50 надежности панели.
            gameState.ChangePanelReliability(
                -sosReliabilityPenalty
            );


            Debug.LogWarning(
                $"[SOS WRONG ACTION] " +
                $"Station Stability -{dangerousStabilityPenalty}, " +
                $"Panel Reliability -{sosReliabilityPenalty}"
            );


            return;
        }


        // --------------------------------------------------
        // ОПАСНЫЕ КНОПКИ
        // --------------------------------------------------

        if (IsDangerousButton(elementID))
        {
            gameState.ChangeStationStability(
                -dangerousStabilityPenalty
            );


            Debug.LogWarning(
                $"[DANGEROUS WRONG ACTION] " +
                $"{elementID} = {value}. " +
                $"Station Stability -{dangerousStabilityPenalty}"
            );


            return;
        }


        // --------------------------------------------------
        // ОБЫЧНАЯ НЕПРАВИЛЬНАЯ КНОПКА
        // --------------------------------------------------

        int minPenalty = Mathf.Min(
            minStabilityPenalty,
            maxStabilityPenalty
        );

        int maxPenalty = Mathf.Max(
            minStabilityPenalty,
            maxStabilityPenalty
        );


        int randomPenalty = Random.Range(
            minPenalty,
            maxPenalty + 1
        );


        gameState.ChangeStationStability(
            -randomPenalty
        );


        Debug.LogWarning(
            $"[WRONG PANEL ACTION] " +
            $"{elementID} = {value}. " +
            $"Station Stability -{randomPenalty}"
        );
    }


    // ==================================================
    // DANGEROUS BUTTON CHECK
    // ==================================================

    private bool IsDangerousButton(
        Panel.PanelElementID elementID)
    {
        return
            elementID == Panel.PanelElementID.ReactorSectionB24 ||
            elementID == Panel.PanelElementID.EmergencyShutdown ||
            elementID == Panel.PanelElementID.Reset ||
            elementID == Panel.PanelElementID.WaterSupply;
    }


    // ==================================================
    // VALIDATION
    // ==================================================

    private void ValidateReferences()
    {
        if (taskManager == null)
        {
            Debug.LogError(
                "[WrongPanelActionHandler] " +
                "TaskManager is not assigned."
            );
        }


        if (gameState == null)
        {
            Debug.LogError(
                "[WrongPanelActionHandler] " +
                "GameState is not assigned."
            );
        }


        if (emergency == null)
        {
            Debug.LogWarning(
                "[WrongPanelActionHandler] " +
                "Emergency is not assigned."
            );
        }
    }
}
using System.Collections.Generic;
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
    // NORMAL ERROR SETTINGS
    // ==================================================

    [Header("Temporary Station Damage")]

    [Tooltip(
        "Минимальный временный урон АЭС за ошибку."
    )]
    [Min(0)]
    [SerializeField]
    private int minTemporaryDamage = 5;

    [Tooltip(
        "Максимальный временный урон АЭС за ошибку."
    )]
    [Min(0)]
    [SerializeField]
    private int maxTemporaryDamage = 20;


    // ==================================================
    // PANEL DAMAGE AFTER FIX
    // ==================================================

    [Header("Panel Damage After Fix")]

    [Tooltip(
        "Минимальное снижение надёжности панели " +
        "после исправления ошибки."
    )]
    [Min(0)]
    [SerializeField]
    private int minPanelDamage = 3;

    [Tooltip(
        "Максимальное снижение надёжности панели " +
        "после исправления ошибки."
    )]
    [Min(0)]
    [SerializeField]
    private int maxPanelDamage = 10;


    // ==================================================
    // ACCIDENT
    // ==================================================

    [Header("Accident")]

    [Tooltip(
        "Сколько одновременно неисправленных ошибок " +
        "создают настоящую аварию."
    )]
    [Min(1)]
    [SerializeField]
    private int errorsForAccident = 3;

    [Tooltip(
        "Постоянный урон обеим шкалам при аварии."
    )]
    [Min(0)]
    [SerializeField]
    private int accidentDamage = 25;


    // ==================================================
    // RUNTIME
    // ==================================================

    private readonly Dictionary<
        Panel.PanelElementID,
        WrongElementState
    > activeErrors = new();


    private readonly Dictionary<
        Panel.PanelElementID,
        int
    > lastCorrectValues = new();


    // Защита от двойного события
    // одного элемента за один кадр.
    private int lastProcessedFrame = -1;

    private Panel.PanelElementID
        lastProcessedElement;


    // ==================================================
    // ERROR DATA
    // ==================================================

    private class WrongElementState
    {
        public int PreviousValue;

        public int TemporaryDamage;


        public WrongElementState(
            int previousValue,
            int temporaryDamage)
        {
            PreviousValue =
                previousValue;

            TemporaryDamage =
                temporaryDamage;
        }
    }


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
        if (gameState == null)
            return;


        // ==================================================
        // EMERGENCY SAFE ACTION
        // ==================================================

        // Если сейчас активна авария
        // и игрок выставил значение,
        // которое является одним из действий
        // для устранения этой аварии:
        //
        // НЕ снимаем стабильность АЭС.
        // НЕ снимаем надёжность панели.
        // НЕ создаём ошибку.

        if (emergency != null &&
            emergency.IsValidEmergencyAction(
                elementID,
                value
            ))
        {
            Debug.Log(
                $"[EMERGENCY SAFE ACTION] " +
                $"{elementID} = {value}. " +
                $"Station 0, Panel 0."
            );


            RememberCorrectValue(
                elementID,
                value
            );


            return;
        }


        // ==================================================
        // DUPLICATE PROTECTION
        // ==================================================

        if (lastProcessedFrame ==
                Time.frameCount &&
            lastProcessedElement ==
                elementID)
        {
            return;
        }


        lastProcessedFrame =
            Time.frameCount;

        lastProcessedElement =
            elementID;


        // ==================================================
        // EXISTING ERROR
        // ==================================================

        if (activeErrors.TryGetValue(
                elementID,
                out WrongElementState error))
        {
            // Игрок вернул элемент
            // в состояние до ошибки.
            if (value ==
                error.PreviousValue)
            {
                FixError(
                    elementID,
                    error
                );


                return;
            }


            // Ошибка уже существует.
            // Дополнительный урон не наносим.
            Debug.LogWarning(
                $"[ERROR STILL ACTIVE] " +
                $"{elementID} = {value}. " +
                $"Return to {error.PreviousValue}"
            );


            return;
        }


        // ==================================================
        // DAILY TASK ELEMENT
        // ==================================================

        // Если элемент используется
        // текущими ежедневными заданиями,
        // это не считается ошибкой панели.

        if (taskManager != null &&
            taskManager.IsElementUsedByTask(
                elementID
            ))
        {
            Debug.Log(
                $"[NO DAMAGE] " +
                $"{elementID} относится " +
                $"к ежедневным заданиям."
            );


            RememberCorrectValue(
                elementID,
                value
            );


            return;
        }


        // ==================================================
        // NEW ERROR
        // ==================================================

        RegisterNewError(
            elementID,
            value
        );
    }


    // ==================================================
    // NEW ERROR
    // ==================================================

    private void RegisterNewError(
        Panel.PanelElementID elementID,
        int wrongValue)
    {
        int previousValue =
            GetPreviousValue(
                elementID,
                wrongValue
            );


        int temporaryDamage =
            Random.Range(
                Mathf.Min(
                    minTemporaryDamage,
                    maxTemporaryDamage
                ),
                Mathf.Max(
                    minTemporaryDamage,
                    maxTemporaryDamage
                ) + 1
            );


        WrongElementState newError =
            new WrongElementState(
                previousValue,
                temporaryDamage
            );


        activeErrors.Add(
            elementID,
            newError
        );


        // Обычная ошибка временно
        // уменьшает стабильность АЭС.
        gameState.ChangeStationStability(
            -temporaryDamage
        );


        Debug.LogWarning(
            $"[NEW PANEL ERROR] " +
            $"{elementID}: " +
            $"{previousValue} -> {wrongValue}. " +
            $"Station -{temporaryDamage}. " +
            $"Active errors: {activeErrors.Count}"
        );


        // ==================================================
        // TOO MANY ERRORS
        // ==================================================

        if (activeErrors.Count >=
            errorsForAccident)
        {
            TriggerAccident();
        }
    }


    // ==================================================
    // FIX ERROR
    // ==================================================

    private void FixError(
        Panel.PanelElementID elementID,
        WrongElementState error)
    {
        // Возвращаем временно потерянную
        // стабильность АЭС.
        gameState.ChangeStationStability(
            error.TemporaryDamage
        );


        // Но ошибка повреждает саму панель.
        int panelDamage =
            Random.Range(
                Mathf.Min(
                    minPanelDamage,
                    maxPanelDamage
                ),
                Mathf.Max(
                    minPanelDamage,
                    maxPanelDamage
                ) + 1
            );


        gameState.ChangePanelReliability(
            -panelDamage
        );


        activeErrors.Remove(
            elementID
        );


        RememberCorrectValue(
            elementID,
            error.PreviousValue
        );


        Debug.Log(
            $"[PANEL ERROR FIXED] " +
            $"{elementID}. " +
            $"Station +{error.TemporaryDamage}. " +
            $"Panel -{panelDamage}. " +
            $"Active errors: {activeErrors.Count}"
        );
    }


    // ==================================================
    // ACCIDENT
    // ==================================================

    private void TriggerAccident()
    {
        Debug.LogError(
            $"[ACCIDENT] " +
            $"{activeErrors.Count} " +
            $"active panel errors!"
        );


        // Сначала возвращаем весь временный
        // урон от отдельных ошибок.
        int temporaryDamageToRestore = 0;


        foreach (
            KeyValuePair<
                Panel.PanelElementID,
                WrongElementState
            > pair
            in activeErrors)
        {
            temporaryDamageToRestore +=
                pair.Value.TemporaryDamage;
        }


        if (temporaryDamageToRestore > 0)
        {
            gameState.ChangeStationStability(
                temporaryDamageToRestore
            );
        }


        // После этого применяется
        // постоянный штраф аварии.
        gameState.ChangeStationStability(
            -accidentDamage
        );


        gameState.ChangePanelReliability(
            -accidentDamage
        );


        activeErrors.Clear();


        Debug.LogError(
            $"[ACCIDENT CREATED] " +
            $"Station -{accidentDamage}. " +
            $"Panel -{accidentDamage}."
        );
    }


    // ==================================================
    // PREVIOUS VALUES
    // ==================================================

    private int GetPreviousValue(
        Panel.PanelElementID elementID,
        int currentWrongValue)
    {
        if (lastCorrectValues.TryGetValue(
                elementID,
                out int previousValue))
        {
            return previousValue;
        }


        // Для двухпозиционных элементов.
        if (currentWrongValue == 0)
            return 1;

        if (currentWrongValue == 1)
            return 0;


        // Для остальных элементов,
        // если старое состояние неизвестно.
        return 0;
    }


    private void RememberCorrectValue(
        Panel.PanelElementID elementID,
        int value)
    {
        lastCorrectValues[elementID] =
            value;
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
                "TaskManager не назначен."
            );
        }


        if (gameState == null)
        {
            Debug.LogError(
                "[WrongPanelActionHandler] " +
                "GameState не назначен."
            );
        }


        if (emergency == null)
        {
            Debug.LogWarning(
                "[WrongPanelActionHandler] " +
                "Emergency не назначен."
            );
        }
    }
}
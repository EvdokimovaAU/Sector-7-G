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

    [Tooltip("Минимальный временный урон АЭС за ошибку.")]
    [Min(0)]
    [SerializeField]
    private int minTemporaryDamage = 5;

    [Tooltip("Максимальный временный урон АЭС за ошибку.")]
    [Min(0)]
    [SerializeField]
    private int maxTemporaryDamage = 20;


    // ==================================================
    // PANEL DAMAGE AFTER FIX
    // ==================================================

    [Header("Panel Damage After Fix")]

    [Tooltip(
        "Минимальное снижение надежности панели " +
        "после исправления ошибки."
    )]
    [Min(0)]
    [SerializeField]
    private int minPanelDamage = 3;

    [Tooltip(
        "Максимальное снижение надежности панели " +
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

    // Все элементы панели, которые игрок сейчас
    // оставил в неправильном состоянии.
    private readonly Dictionary<
        Panel.PanelElementID,
        WrongElementState
    > activeErrors = new();


    // Последнее известное корректное состояние
    // каждого элемента.
    private readonly Dictionary<
        Panel.PanelElementID,
        int
    > lastCorrectValues = new();


    // Нужна защита от нескольких одинаковых
    // событий за один кадр.
    private int lastProcessedFrame = -1;

    private Panel.PanelElementID lastProcessedElement;


    // ==================================================
    // ERROR DATA
    // ==================================================

    private class WrongElementState
    {
        // Значение, которое было ДО ошибки.
        public int PreviousValue;

        // Сколько временной стабильности
        // потеряла АЭС из-за этой ошибки.
        public int TemporaryDamage;


        public WrongElementState(
            int previousValue,
            int temporaryDamage)
        {
            PreviousValue = previousValue;
            TemporaryDamage = temporaryDamage;
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


        // Защита от случайного двойного события
        // одного элемента в одном кадре.
        if (lastProcessedFrame == Time.frameCount &&
            lastProcessedElement == elementID)
        {
            return;
        }


        lastProcessedFrame = Time.frameCount;
        lastProcessedElement = elementID;


        // ==================================================
        // ЭТОТ ЭЛЕМЕНТ УЖЕ БЫЛ ОШИБОЧНЫМ
        // ==================================================

        if (activeErrors.TryGetValue(
                elementID,
                out WrongElementState error))
        {
            // Игрок вернул элемент именно
            // в состояние ДО своей ошибки.
            if (value == error.PreviousValue)
            {
                FixError(
                    elementID,
                    error
                );

                return;
            }


            // Игрок продолжает менять уже ошибочный
            // элемент, но пока не вернул правильное
            // исходное состояние.
            //
            // Новую ошибку не создаём.
            Debug.LogWarning(
                $"[ERROR STILL ACTIVE] " +
                $"{elementID} = {value}. " +
                $"Return to {error.PreviousValue}"
            );

            return;
        }


        // ==================================================
        // ЭЛЕМЕНТ ОТНОСИТСЯ К ЕЖЕДНЕВНЫМ ЗАДАНИЯМ
        // ==================================================

        if (taskManager != null &&
            taskManager.IsElementUsedByTask(elementID))
        {
            Debug.Log(
                $"[NO DAMAGE] {elementID} относится " +
                $"к ежедневным заданиям. " +
                $"Стабильность АЭС не уменьшается."
            );


            RememberCorrectValue(
                elementID,
                value
            );


            return;
        }


        // ==================================================
        // ПРАВИЛЬНЫЙ ШАГ АВАРИИ
        // ==================================================

        if (emergency != null &&
            emergency.IsActionRequiredByEmergency(
                elementID,
                value))
        {
            RememberCorrectValue(
                elementID,
                value
            );

            return;
        }


        // ==================================================
        // НОВАЯ ОШИБКА
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
        // Получаем состояние элемента,
        // которое было до неправильного действия.
        int previousValue = GetPreviousValue(
            elementID,
            wrongValue
        );


        // Случайный временный урон станции.
        int temporaryDamage = Random.Range(
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


        // Пока это обычная ошибка,
        // временно снижаем стабильность АЭС.
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
        // 3 ОШИБКИ = АВАРИЯ
        // ==================================================

        if (activeErrors.Count >= errorsForAccident)
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
        // ----------------------------------------------
        // Возвращаем временно потерянную
        // стабильность станции.
        // ----------------------------------------------

        gameState.ChangeStationStability(
            error.TemporaryDamage
        );


        // ----------------------------------------------
        // Но панель пострадала.
        // ----------------------------------------------

        int panelDamage = Random.Range(
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


        // Ошибка устранена.
        activeErrors.Remove(
            elementID
        );


        // Это значение снова считается
        // нормальным состоянием элемента.
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
            $"{activeErrors.Count} active panel errors!"
        );


        // --------------------------------------------------
        // Сначала отменяем временный урон.
        //
        // Например:
        //
        // 100
        // ошибка 1 -> 90
        // ошибка 2 -> 78
        // ошибка 3 -> 70
        //
        // Возвращаем временные потери,
        // чтобы потом применить единый
        // постоянный штраф аварии -25.
        // --------------------------------------------------

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


        // --------------------------------------------------
        // Постоянный штраф аварии.
        // --------------------------------------------------

        gameState.ChangeStationStability(
            -accidentDamage
        );


        gameState.ChangePanelReliability(
            -accidentDamage
        );


        // Старые ошибки больше нельзя
        // "откатить" и вернуть эти 25%.
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
        // Если мы уже знаем последнее нормальное
        // состояние этого элемента — используем его.
        if (lastCorrectValues.TryGetValue(
                elementID,
                out int previousValue))
        {
            return previousValue;
        }


        // --------------------------------------------------
        // FALLBACK
        // --------------------------------------------------
        //
        // Если элемент ещё ни разу не участвовал
        // в правильном действии, система пока
        // не знает его стартового состояния.
        //
        // Для двухпозиционных элементов предполагаем
        // противоположное состояние.
        //
        // 1 -> раньше было 0
        // 0 -> раньше было 1

        if (currentWrongValue == 0)
            return 1;

        if (currentWrongValue == 1)
            return 0;


        // Для многопозиционных элементов без
        // сохранённого состояния безопасно считаем
        // стартовым 0.
        return 0;
    }


    private void RememberCorrectValue(
        Panel.PanelElementID elementID,
        int value)
    {
        lastCorrectValues[elementID] = value;
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
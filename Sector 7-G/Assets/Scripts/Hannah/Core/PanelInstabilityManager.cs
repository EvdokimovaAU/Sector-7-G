using UnityEngine;

public class PanelInstabilityManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameState gameState;


    [Header("Runtime Debug")]
    [SerializeField] private int currentReliability = 100;

    [SerializeField] private bool level80Faults;
    [SerializeField] private bool level60Faults;
    [SerializeField] private bool level40Faults;

    [SerializeField] private int reactorShift;
    [SerializeField] private int pumpFaultType;


    // Случайные неисправности выбираются
    // только один раз.
    private bool reactorShiftGenerated;
    private bool pumpFaultGenerated;


    // ==================================================
    // PUBLIC
    // ==================================================

    public int CurrentReliability =>
        currentReliability;

    public int ReactorShift =>
        reactorShift;

    public int PumpFaultType =>
        pumpFaultType;

    public bool Level80Faults =>
        level80Faults;

    public bool Level60Faults =>
        level60Faults;

    public bool Level40Faults =>
        level40Faults;


    // ==================================================
    // UNITY
    // ==================================================

    private void OnEnable()
    {
        if (gameState != null)
        {
            gameState.OnPanelReliabilityChanged +=
                HandleReliabilityChanged;
        }
    }


    private void Start()
    {
        // Если забыли назначить GameState вручную,
        // пытаемся найти его на сцене.
        if (gameState == null)
        {
            gameState =
                FindAnyObjectByType<GameState>();
        }


        if (gameState == null)
        {
            Debug.LogError(
                "[INSTABILITY] GameState не найден!",
                this
            );

            return;
        }


        // Защита от двойной подписки.
        gameState.OnPanelReliabilityChanged -=
            HandleReliabilityChanged;

        gameState.OnPanelReliabilityChanged +=
            HandleReliabilityChanged;


        UpdateInstability(
            gameState.PanelReliability
        );
    }


    private void OnDisable()
    {
        if (gameState != null)
        {
            gameState.OnPanelReliabilityChanged -=
                HandleReliabilityChanged;
        }
    }


    // ==================================================
    // RELIABILITY
    // ==================================================

    private void HandleReliabilityChanged(
        int reliability)
    {
        UpdateInstability(reliability);
    }


    private void UpdateInstability(
        int reliability)
    {
        currentReliability = reliability;


        // 80-100
        // Всё исправно.

        level80Faults =
            reliability < 80;


        // 40-59
        // Добавляются инверсии.

        level60Faults =
            reliability < 60;


        // 0-39
        // Добавляются Valve.

        level40Faults =
            reliability < 40;


        // Первый раз упали ниже 80.
        if (level80Faults)
        {
            GenerateMappingFaults();
        }


        Debug.Log(
            "========== PANEL INSTABILITY ==========\n" +
            $"Reliability: {reliability}%\n" +
            $"<80 faults: {level80Faults}\n" +
            $"<60 faults: {level60Faults}\n" +
            $"<40 faults: {level40Faults}\n" +
            $"Reactor shift: {reactorShift}\n" +
            $"Pump fault type: {pumpFaultType}\n" +
            "========================================"
        );
    }


    // ==================================================
    // RANDOM FAULTS
    // ==================================================

    private void GenerateMappingFaults()
    {
        // ==============================================
        // REACTOR SECTION
        //
        // Возможные сдвиги:
        // -2
        // -1
        // +1
        // +2
        // ==============================================

        if (!reactorShiftGenerated)
        {
            int random =
                Random.Range(0, 4);


            switch (random)
            {
                case 0:
                    reactorShift = -2;
                    break;

                case 1:
                    reactorShift = -1;
                    break;

                case 2:
                    reactorShift = 1;
                    break;

                default:
                    reactorShift = 2;
                    break;
            }


            reactorShiftGenerated = true;


            Debug.LogWarning(
                "[FAULT GENERATED] " +
                $"ReactorSection shift = {reactorShift}"
            );
        }


        // ==============================================
        // PUMP
        //
        // Тип 1:
        // P1 <-> P4
        // P2 <-> P3
        //
        // Тип 2:
        // P1 -> P2
        // P2 -> P3
        // P3 -> P4
        // P4 -> P1
        // ==============================================

        if (!pumpFaultGenerated)
        {
            pumpFaultType =
                Random.Range(1, 3);


            pumpFaultGenerated = true;


            Debug.LogWarning(
                "[FAULT GENERATED] " +
                $"Pump fault type = {pumpFaultType}"
            );
        }
    }


    // ==================================================
    // BUTTON ID
    // ==================================================

    public Panel.PanelElementID GetActualButtonID(
        Panel.PanelElementID pressedID)
    {
        if (gameState == null)
        {
            return pressedID;
        }


        int reliability =
            gameState.PanelReliability;


        // ==============================================
        // 80-100
        //
        // НИ ОДНА КНОПКА НЕ ПЕРЕНАЗНАЧАЕТСЯ.
        // ==============================================

        if (reliability >= 80)
        {
            Debug.Log(
                $"[BUTTON NORMAL] " +
                $"Reliability={reliability} | " +
                $"{pressedID} -> {pressedID}"
            );


            return pressedID;
        }


        // ==============================================
        // ВСЕГДА СТАБИЛЬНЫЕ ЭЛЕМЕНТЫ
        // ==============================================

        if (pressedID ==
            Panel.PanelElementID.EmergencyShutdown)
        {
            return pressedID;
        }


        if (pressedID ==
            Panel.PanelElementID.ReactorMode)
        {
            return pressedID;
        }


        GenerateMappingFaults();


        Panel.PanelElementID actualID =
            pressedID;


        // ==============================================
        // 60-79
        // GENERATOR
        // ==============================================

        if (IsGenerator(pressedID))
        {
            actualID =
                GetGeneratorMapping(pressedID);
        }


        // ==============================================
        // 60-79
        // PUMP
        // ==============================================

        else if (IsPump(pressedID))
        {
            actualID =
                GetPumpMapping(pressedID);
        }


        // ==============================================
        // 60-79
        // REACTOR SECTION
        // ==============================================

        else if (IsReactorSection(pressedID))
        {
            actualID =
                GetReactorSectionMapping(
                    pressedID
                );
        }


        // ==============================================
        // НИЖЕ 60
        //
        // RESET <-> WATER SUPPLY
        // ==============================================

        else if (reliability < 60)
        {
            if (pressedID ==
                Panel.PanelElementID.Reset)
            {
                actualID =
                    Panel.PanelElementID.WaterSupply;
            }

            else if (pressedID ==
                     Panel.PanelElementID.WaterSupply)
            {
                actualID =
                    Panel.PanelElementID.Reset;
            }
        }


        Debug.LogWarning(
            "[BUTTON MAPPING] " +
            $"Reliability={reliability} | " +
            $"Pressed={pressedID} | " +
            $"Actual={actualID}"
        );


        return actualID;
    }


    // ==================================================
    // GENERATOR
    // ==================================================

    private bool IsGenerator(
        Panel.PanelElementID id)
    {
        return
            id == Panel.PanelElementID.Generator1 ||
            id == Panel.PanelElementID.Generator2 ||
            id == Panel.PanelElementID.Generator3;
    }


    private Panel.PanelElementID GetGeneratorMapping(
        Panel.PanelElementID id)
    {
        // G1 -> G2
        if (id == Panel.PanelElementID.Generator1)
        {
            return Panel.PanelElementID.Generator2;
        }


        // G2 -> G3
        if (id == Panel.PanelElementID.Generator2)
        {
            return Panel.PanelElementID.Generator3;
        }


        // G3 -> G1
        if (id == Panel.PanelElementID.Generator3)
        {
            return Panel.PanelElementID.Generator1;
        }


        return id;
    }


    // ==================================================
    // PUMP
    // ==================================================

    private bool IsPump(
        Panel.PanelElementID id)
    {
        return
            id == Panel.PanelElementID.Pump1 ||
            id == Panel.PanelElementID.Pump2 ||
            id == Panel.PanelElementID.Pump3 ||
            id == Panel.PanelElementID.Pump4;
    }


    private Panel.PanelElementID GetPumpMapping(
        Panel.PanelElementID id)
    {
        // ==============================================
        // TYPE 1
        //
        // P1 -> P4
        // P2 -> P3
        // P3 -> P2
        // P4 -> P1
        // ==============================================

        if (pumpFaultType == 1)
        {
            if (id == Panel.PanelElementID.Pump1)
                return Panel.PanelElementID.Pump4;


            if (id == Panel.PanelElementID.Pump2)
                return Panel.PanelElementID.Pump3;


            if (id == Panel.PanelElementID.Pump3)
                return Panel.PanelElementID.Pump2;


            if (id == Panel.PanelElementID.Pump4)
                return Panel.PanelElementID.Pump1;
        }


        // ==============================================
        // TYPE 2
        //
        // P1 -> P2
        // P2 -> P3
        // P3 -> P4
        // P4 -> P1
        // ==============================================

        if (pumpFaultType == 2)
        {
            if (id == Panel.PanelElementID.Pump1)
                return Panel.PanelElementID.Pump2;


            if (id == Panel.PanelElementID.Pump2)
                return Panel.PanelElementID.Pump3;


            if (id == Panel.PanelElementID.Pump3)
                return Panel.PanelElementID.Pump4;


            if (id == Panel.PanelElementID.Pump4)
                return Panel.PanelElementID.Pump1;
        }


        return id;
    }


    // ==================================================
    // REACTOR SECTION
    // ==================================================

    private bool IsReactorSection(
        Panel.PanelElementID id)
    {
        return
            id >= Panel.PanelElementID.ReactorSectionB1 &&
            id <= Panel.PanelElementID.ReactorSectionB24;
    }


    private Panel.PanelElementID
        GetReactorSectionMapping(
            Panel.PanelElementID id)
    {
        int first =
            (int)Panel.PanelElementID.ReactorSectionB1;


        int last =
            (int)Panel.PanelElementID.ReactorSectionB24;


        int count =
            last - first + 1;


        int index =
            (int)id - first;


        int shiftedIndex =
            (index + reactorShift) % count;


        // В C# отрицательное число % count
        // может остаться отрицательным.
        if (shiftedIndex < 0)
        {
            shiftedIndex += count;
        }


        return
            (Panel.PanelElementID)(
                first + shiftedIndex
            );
    }


    // ==================================================
    // TEMPERATURE / PRESSURE / POWER
    // ==================================================

    public bool ShouldInvertValueControl(
        Panel.PanelElementID id)
    {
        if (gameState == null)
        {
            return false;
        }


        // Только ниже 60.
        if (gameState.PanelReliability >= 60)
        {
            return false;
        }


        bool shouldInvert =
            id == Panel.PanelElementID.Temperature ||
            id == Panel.PanelElementID.Pressure ||
            id == Panel.PanelElementID.Power;


        if (shouldInvert)
        {
            Debug.LogWarning(
                $"[VALUE CONTROL INVERTED] {id}"
            );
        }


        return shouldInvert;
    }


    // ==================================================
    // TURBINES
    // ==================================================

    public Panel.PanelElementID GetActualTurbineID(
        Panel.PanelElementID pressedID)
    {
        if (gameState == null)
        {
            return pressedID;
        }


        // До 60% всё нормально.
        if (gameState.PanelReliability >= 60)
        {
            return pressedID;
        }


        Panel.PanelElementID actualID =
            pressedID;


        // Turbine1 ON -> Turbine1 STOP
        if (pressedID ==
            Panel.PanelElementID.Turbine1)
        {
            actualID =
                Panel.PanelElementID.Turbine1_Stop;
        }


        // Turbine1 STOP -> Turbine1 ON
        else if (pressedID ==
                 Panel.PanelElementID.Turbine1_Stop)
        {
            actualID =
                Panel.PanelElementID.Turbine1;
        }


        // Turbine2 ON -> Turbine2 STOP
        else if (pressedID ==
                 Panel.PanelElementID.Turbine2)
        {
            actualID =
                Panel.PanelElementID.Turbine2_Stop;
        }


        // Turbine2 STOP -> Turbine2 ON
        else if (pressedID ==
                 Panel.PanelElementID.Turbine2_Stop)
        {
            actualID =
                Panel.PanelElementID.Turbine2;
        }


        if (actualID != pressedID)
        {
            Debug.LogWarning(
                "[TURBINE INVERTED] " +
                $"{pressedID} -> {actualID}"
            );
        }


        return actualID;
    }


    // ==================================================
    // VALVE
    // ==================================================

    public bool ShouldInvertValve(
        Panel.PanelElementID id)
    {
        if (gameState == null)
        {
            return false;
        }


        // Только ниже 40.
        if (gameState.PanelReliability >= 40)
        {
            return false;
        }


        bool isValve =
            id == Panel.PanelElementID.Valve1 ||
            id == Panel.PanelElementID.Valve2 ||
            id == Panel.PanelElementID.Valve3;


        if (isValve)
        {
            Debug.LogWarning(
                $"[VALVE INVERTED] {id}"
            );
        }


        return isValve;
    }
}
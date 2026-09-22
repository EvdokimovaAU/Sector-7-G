public enum PanelElementID
{
    // ГЕНЕРАТОРЫ
    // 0 = OFF, 1 = ON
    Generator1,
    Generator2,
    Generator3,

    // НАСОСЫ
    // 0 = OFF, 1 = ON
    Pump1,
    Pump2,
    Pump3,
    Pump4,

    // ПАРАМЕТРЫ СТАНЦИИ
    Temperature,
    Pressure,
    Power,

    // ТУРБИНЫ
    // 0 = OFF, 1 = ON
    Turbine1,
    Turbine2,

    // РЕЖИМ РЕАКТОРА
    // 0 = Нормальный
    // 1 = Экономичный
    // 2 = Резервный
    // 3 = Останов
    ReactorMode,

    // СЕКЦИИ РЕАКТОРА
    // 0 = OFF, 1 = ON
    ReactorSectionB1,
    ReactorSectionB2,
    ReactorSectionB3,
    ReactorSectionB4,
    ReactorSectionB5,
    ReactorSectionB6,
    ReactorSectionB7,
    ReactorSectionB8,
    ReactorSectionB9,
    ReactorSectionB10,
    ReactorSectionB11,
    ReactorSectionB12,
    ReactorSectionB13,
    ReactorSectionB14,
    ReactorSectionB15,
    ReactorSectionB16,
    ReactorSectionB17,
    ReactorSectionB18,
    ReactorSectionB19,
    ReactorSectionB20,
    ReactorSectionB21,
    ReactorSectionB22,
    ReactorSectionB23,
    ReactorSectionB24,

    // АВАРИЙНАЯ СИСТЕМА
    EmergencyShutdown,
    Reset,
    WaterSupply,

    // КЛАПАНЫ
    // 0 = ОТКР
    // 1 = НОР
    // 2 = ЗАКР
    Valve1,
    Valve2,
    Valve3
}
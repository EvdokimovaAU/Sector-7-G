using System;
using UnityEngine;

[Serializable]
public class EmergencyStep
{
    [Header("Required panel action")]

    [SerializeField]
    private Panel.PanelElementID elementID;

    [SerializeField]
    private int requiredValue;


    public Panel.PanelElementID ElementID => elementID;

    public int RequiredValue => requiredValue;


    // Проверяет, соответствует ли действие игрока
    // требуемому шагу устранения аварии.
    public bool Matches(
        Panel.PanelElementID id,
        int value)
    {
        return elementID == id &&
               requiredValue == value;
    }
}
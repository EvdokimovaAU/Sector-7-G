using System;
using UnityEngine;

[Serializable]
public class EmergencyStep
{
    [Header("Journal")]
    [TextArea]
    [SerializeField] private string description;

    [Header("Required panel action")]
    [SerializeField] private Panel.PanelElementID elementID;

    [SerializeField] private int requiredValue;


    public string Description => description;
    public Panel.PanelElementID ElementID => elementID;
    public int RequiredValue => requiredValue;


    public bool Matches(
        Panel.PanelElementID id,
        int value)
    {
        return elementID == id &&
               requiredValue == value;
    }
}
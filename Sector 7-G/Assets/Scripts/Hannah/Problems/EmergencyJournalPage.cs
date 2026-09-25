using System;
using System.Collections.Generic;
using UnityEngine;
using Station;

[Serializable]
public class EmergencyJournalPage
{
    [Header("Problem Information")]

    [SerializeField] private string emergencyID;

    [SerializeField] private string title;

    [TextArea(3, 6)]
    [SerializeField] private string description;

    [SerializeField] private StationSector sector;

    [SerializeField] private string phoneNumber;


    [Header("Resolution Steps")]

    [SerializeField]
    private List<EmergencyStep> steps = new();


    public string EmergencyID => emergencyID;
    public string Title => title;
    public string Description => description;
    public StationSector Sector => sector;
    public string PhoneNumber => phoneNumber;
    public IReadOnlyList<EmergencyStep> Steps => steps;
}
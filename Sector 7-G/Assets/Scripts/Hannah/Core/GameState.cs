using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [Header("Game State")]

    [SerializeField] private int currentDay = 1;

    [Range(0, 100)]
    [SerializeField] private int stationStability = 100;

    [Range(0, 100)]
    [SerializeField] private int panelReliability = 100;


    public int CurrentDay => currentDay;

    public int StationStability => stationStability;

    public int PanelReliability => panelReliability;


    // События для UI.
    public event Action<int> OnStationStabilityChanged;
    public event Action<int> OnPanelReliabilityChanged;


    public void ChangeStationStability(int amount)
    {
        stationStability = Mathf.Clamp(
            stationStability + amount,
            0,
            100
        );

        OnStationStabilityChanged?.Invoke(stationStability);
    }


    public void ChangePanelReliability(int amount)
    {
        panelReliability = Mathf.Clamp(
            panelReliability + amount,
            0,
            100
        );

        OnPanelReliabilityChanged?.Invoke(panelReliability);
    }


    public void SetStationStability(int value)
    {
        stationStability = Mathf.Clamp(value, 0, 100);

        OnStationStabilityChanged?.Invoke(stationStability);
    }


    public void SetPanelReliability(int value)
    {
        panelReliability = Mathf.Clamp(value, 0, 100);

        OnPanelReliabilityChanged?.Invoke(panelReliability);
    }
}
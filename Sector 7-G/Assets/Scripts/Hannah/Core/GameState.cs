using UnityEngine;

public class GameState : MonoBehaviour
{
    [Header("Game State")]

    // Текущий игровой день.
    [SerializeField] private int currentDay = 1;

    // Общая стабильность станции.
    [SerializeField] private int stationStability = 100;

    // Надёжность панели управления.
    [SerializeField] private int panelReliability = 100;


    public int CurrentDay => currentDay;

    public int StationStability => stationStability;

    public int PanelReliability => panelReliability;
}
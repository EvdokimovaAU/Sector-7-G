using UnityEngine;

public class GameStateUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameState gameState;

    [Header("Bars")]
    [SerializeField] private StatusBarUI stabilityBar;
    [SerializeField] private StatusBarUI reliabilityBar;


    private void OnEnable()
    {
        if (gameState == null)
            return;

        gameState.OnStationStabilityChanged +=
            HandleStabilityChanged;

        gameState.OnPanelReliabilityChanged +=
            HandleReliabilityChanged;
    }


    private void Start()
    {
        if (gameState == null)
            return;

        // Начальные значения.
        stabilityBar?.SetValue(
            gameState.StationStability
        );

        reliabilityBar?.SetValue(
            gameState.PanelReliability
        );
    }


    private void OnDisable()
    {
        if (gameState == null)
            return;

        gameState.OnStationStabilityChanged -=
            HandleStabilityChanged;

        gameState.OnPanelReliabilityChanged -=
            HandleReliabilityChanged;
    }


    private void HandleStabilityChanged(int value)
    {
        stabilityBar?.SetValue(value);
    }


    private void HandleReliabilityChanged(int value)
    {
        reliabilityBar?.SetValue(value);
    }
}
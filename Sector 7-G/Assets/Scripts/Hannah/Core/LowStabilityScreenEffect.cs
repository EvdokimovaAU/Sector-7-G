using UnityEngine;
using UnityEngine.UI;

public class LowStabilityScreenEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameState gameState;
    [SerializeField] private Image redOverlay;

    [Header("Settings")]
    [Range(0, 100)]
    [SerializeField] private int activationThreshold = 50;

    [Range(0f, 1f)]
    [SerializeField] private float maxAlpha = 0.25f;

    [Min(0.1f)]
    [SerializeField] private float fadeSpeed = 2f;


    private float targetAlpha;


    private void OnEnable()
    {
        if (gameState != null)
            gameState.OnStationStabilityChanged += HandleStabilityChanged;
    }


    private void Start()
    {
        if (redOverlay != null)
        {
            // Красная картинка не должна блокировать
            // нажатия на UI под ней.
            redOverlay.raycastTarget = false;
        }

        if (gameState != null)
            HandleStabilityChanged(gameState.StationStability);
    }


    private void OnDisable()
    {
        if (gameState != null)
            gameState.OnStationStabilityChanged -= HandleStabilityChanged;
    }


    private void Update()
    {
        if (redOverlay == null)
            return;

        Color color = redOverlay.color;

        color.a = Mathf.MoveTowards(
            color.a,
            targetAlpha,
            fadeSpeed * Time.unscaledDeltaTime
        );

        redOverlay.color = color;
    }


    private void HandleStabilityChanged(int stability)
    {
        if (stability <= activationThreshold)
            targetAlpha = maxAlpha;
        else
            targetAlpha = 0f;
    }
}
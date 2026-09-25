using UnityEngine;
using UnityEngine.UI;

public class LowStabilityScreenEffect : MonoBehaviour
{
    // ==================================================
    // REFERENCES
    // ==================================================

    [Header("References")]

    [SerializeField]
    private GameState gameState;

    [Tooltip("Все аварии уровня")]
    [SerializeField]
    private TaskTriggeredEmergency[] emergencies;

    [SerializeField]
    private Image redOverlay;


    // ==================================================
    // ACTIVATION
    // ==================================================

    [Header("Activation")]

    [Range(0, 100)]
    [SerializeField]
    private int activationThreshold = 50;


    // ==================================================
    // FLASH SETTINGS
    // ==================================================

    [Header("Red Flash")]

    [Range(0f, 1f)]
    [SerializeField]
    private float maxAlpha = 0.25f;

    [Min(0.1f)]
    [SerializeField]
    private float fadeInDuration = 1.5f;

    [Min(0.1f)]
    [SerializeField]
    private float fadeOutDuration = 1.5f;


    // ==================================================
    // RUNTIME
    // ==================================================

    private bool effectActive;
    private bool fadingToRed = true;


    // ==================================================
    // UNITY
    // ==================================================

    private void Start()
    {
        if (redOverlay != null)
        {
            redOverlay.raycastTarget = false;

            Color color = redOverlay.color;
            color.a = 0f;

            redOverlay.color = color;
        }

        CheckEffectState();
    }


    private void Update()
    {
        if (redOverlay == null)
            return;

        CheckEffectState();

        if (effectActive)
        {
            UpdateFlashing();
        }
        else
        {
            FadeToNormal();
        }
    }


    // ==================================================
    // CHECK STATE
    // ==================================================

    private void CheckEffectState()
    {
        bool lowStability = false;
        bool emergencyActive = false;


        // Стабильность АЭС <= 50.
        if (gameState != null)
        {
            lowStability =
                gameState.StationStability
                <= activationThreshold;
        }


        // Проверяем все аварии.
        if (emergencies != null)
        {
            foreach (TaskTriggeredEmergency emergency in emergencies)
            {
                if (emergency != null &&
                    emergency.IsActive)
                {
                    emergencyActive = true;
                    break;
                }
            }
        }


        // Красный экран нужен, если:
        //
        // 1. есть активная авария
        // ИЛИ
        // 2. стабильность АЭС <= 50.

        bool shouldBeActive =
            emergencyActive ||
            lowStability;


        // Эффект только что включился.
        if (shouldBeActive && !effectActive)
        {
            fadingToRed = true;
        }


        effectActive = shouldBeActive;
    }


    // ==================================================
    // RED PULSE
    // ==================================================

    private void UpdateFlashing()
    {
        Color color = redOverlay.color;


        if (fadingToRed)
        {
            float speed =
                maxAlpha / fadeInDuration;


            color.a = Mathf.MoveTowards(
                color.a,
                maxAlpha,
                speed * Time.unscaledDeltaTime
            );


            if (Mathf.Approximately(
                color.a,
                maxAlpha))
            {
                fadingToRed = false;
            }
        }
        else
        {
            float speed =
                maxAlpha / fadeOutDuration;


            color.a = Mathf.MoveTowards(
                color.a,
                0f,
                speed * Time.unscaledDeltaTime
            );


            if (Mathf.Approximately(
                color.a,
                0f))
            {
                fadingToRed = true;
            }
        }


        redOverlay.color = color;
    }


    // ==================================================
    // RETURN TO NORMAL
    // ==================================================

    private void FadeToNormal()
    {
        Color color = redOverlay.color;


        float speed =
            maxAlpha / fadeOutDuration;


        color.a = Mathf.MoveTowards(
            color.a,
            0f,
            speed * Time.unscaledDeltaTime
        );


        redOverlay.color = color;


        if (Mathf.Approximately(color.a, 0f))
        {
            fadingToRed = true;
        }
    }
}
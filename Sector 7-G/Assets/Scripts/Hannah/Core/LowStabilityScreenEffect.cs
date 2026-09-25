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

    [SerializeField]
    private TaskTriggeredEmergency emergency;

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

    [Tooltip("Сколько секунд экран краснеет")]
    [Min(0.1f)]
    [SerializeField]
    private float fadeInDuration = 1.5f;

    [Tooltip("Сколько секунд экран возвращается в норму")]
    [Min(0.1f)]
    [SerializeField]
    private float fadeOutDuration = 1.5f;


    // ==================================================
    // RUNTIME
    // ==================================================

    private bool effectActive;

    // true  = сейчас краснеем
    // false = сейчас возвращаемся к нормальному
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
    // CHECK
    // ==================================================

    private void CheckEffectState()
    {
        bool lowStability = false;
        bool emergencyActive = false;


        // ------------------------------
        // Стабильность АЭС
        // ------------------------------

        if (gameState != null)
        {
            lowStability =
                gameState.StationStability
                <= activationThreshold;
        }


        // ------------------------------
        // Авария
        // ------------------------------

        if (emergency != null)
        {
            emergencyActive =
                emergency.IsActive;
        }


        // Эффект работает, если выполняется
        // ХОТЯ БЫ одно условие.

        bool shouldBeActive =
            lowStability ||
            emergencyActive;


        // Если эффект только что включился —
        // начинаем цикл с покраснения.

        if (shouldBeActive && !effectActive)
        {
            fadingToRed = true;
        }


        effectActive = shouldBeActive;
    }


    // ==================================================
    // FLASH
    // ==================================================

    private void UpdateFlashing()
    {
        Color color = redOverlay.color;


        if (fadingToRed)
        {
            // Плавно краснеем.

            float speed =
                maxAlpha / fadeInDuration;


            color.a = Mathf.MoveTowards(
                color.a,
                maxAlpha,
                speed * Time.unscaledDeltaTime
            );


            // Дошли до максимальной красноты.
            if (Mathf.Approximately(
                    color.a,
                    maxAlpha))
            {
                fadingToRed = false;
            }
        }
        else
        {
            // Плавно возвращаем экран
            // в нормальное состояние.

            float speed =
                maxAlpha / fadeOutDuration;


            color.a = Mathf.MoveTowards(
                color.a,
                0f,
                speed * Time.unscaledDeltaTime
            );


            // Полностью исчез красный цвет —
            // начинаем новый цикл.

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
    // NORMAL STATE
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

        fadingToRed = true;
    }
}
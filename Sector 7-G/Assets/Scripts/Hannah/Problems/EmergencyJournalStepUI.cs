using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmergencyJournalStepUI : MonoBehaviour
{
    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]

    [SerializeField]
    private TMP_Text descriptionText;

    [SerializeField]
    private Image checkmarkImage;


    // =========================================================
    // CHECKMARK SPRITES
    // =========================================================

    [Header("Checkmark Sprites")]

    [Tooltip("Квадратик для невыполненного шага")]
    [SerializeField]
    private Sprite incompleteSprite;

    [Tooltip("Квадратик с галочкой для выполненного шага")]
    [SerializeField]
    private Sprite completedSprite;


    // =========================================================
    // SETUP
    // =========================================================

    public void Setup(
        EmergencyStep step,
        bool completed)
    {
        if (step == null)
        {
            Debug.LogWarning(
                "[EmergencyJournalStepUI] EmergencyStep = null."
            );

            return;
        }


        // Текст действия.
        if (descriptionText != null)
        {
            descriptionText.text =
                step.Description;
        }


        // Картинка состояния.
        if (checkmarkImage != null)
        {
            checkmarkImage.sprite =
                completed
                    ? completedSprite
                    : incompleteSprite;
        }
    }


    // =========================================================
    // CHECKMARK VISIBILITY
    // =========================================================

    public void SetCheckmarkVisible(
        bool visible)
    {
        if (checkmarkImage == null)
            return;


        checkmarkImage.gameObject.SetActive(
            visible
        );
    }
}
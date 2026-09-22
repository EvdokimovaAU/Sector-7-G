using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image checkmarkImage;

    [Header("Checkmark Sprites")]
    [SerializeField] private Sprite incompleteSprite;
    [SerializeField] private Sprite completedSprite;

    // Заполняет строку данными задания
    public void Setup(DailyTask task)
    {
        descriptionText.text = task.Description;

        UpdateState(task);
    }

    // Обновляет только состояние галочки
    public void UpdateState(DailyTask task)
    {
        checkmarkImage.sprite = task.IsCompleted
            ? completedSprite
            : incompleteSprite;
    }
}
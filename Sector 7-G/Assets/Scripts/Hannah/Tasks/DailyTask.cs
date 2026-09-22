using System;
using UnityEngine;

[Serializable]
public class DailyTask
{
    [Header("Task")]
    [SerializeField] private string description;

    [SerializeField] private PanelElementID targetElement;

    [SerializeField] private int requiredValue;

    [Header("Runtime")]
    [SerializeField] private bool isCompleted;


    public string Description => description;

    public PanelElementID TargetElement => targetElement;

    public int RequiredValue => requiredValue;

    public bool IsCompleted => isCompleted;


    /// <summary>
    /// Проверяет, выполнено ли задание после действия игрока.
    /// </summary>
    public bool Check(PanelElementID elementID, int value)
    {
        // Уже выполненное задание повторно не выполняем.
        if (isCompleted)
            return false;

        // Игрок взаимодействовал не с тем элементом.
        if (targetElement != elementID)
            return false;

        // Значение элемента не соответствует заданию.
        if (requiredValue != value)
            return false;

        // Все условия выполнены.
        isCompleted = true;

        return true;
    }


    /// <summary>
    /// Сбрасывает выполнение задания.
    /// Пригодится при начале нового дня.
    /// </summary>
    public void ResetTask()
    {
        isCompleted = false;
    }
}
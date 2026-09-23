using System;
using UnityEngine;

[Serializable]
public class DailyTask
{
    [Header("Task")]

    // Текст задания, который увидит игрок.
    [SerializeField] private string description;


    // Какой элемент панели должен изменить игрок.
    // Используем именно PanelElementID из namespace Panel.
    [SerializeField]
    private Panel.PanelElementID targetElement;


    // Какое значение необходимо установить.
    [SerializeField] private int requiredValue;


    [Header("Runtime")]

    // Выполнено ли задание.
    [SerializeField] private bool isCompleted;


    public string Description => description;

    public Panel.PanelElementID TargetElement => targetElement;

    public int RequiredValue => requiredValue;

    public bool IsCompleted => isCompleted;


    /// <summary>
    /// Проверяет действие игрока.
    /// </summary>
    public bool Check(
        Panel.PanelElementID elementID,
        int value)
    {
        // Уже выполненное задание
        // второй раз не выполняем.
        if (isCompleted)
            return false;


        // Игрок взаимодействовал
        // не с тем элементом панели.
        if (targetElement != elementID)
            return false;


        // Элемент правильный,
        // но установлено неправильное значение.
        if (requiredValue != value)
            return false;


        // И элемент, и значение совпали.
        isCompleted = true;

        return true;
    }


    /// <summary>
    /// Сбрасывает выполнение задания.
    /// </summary>
    public void ResetTask()
    {
        isCompleted = false;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [Header("Tasks")]

    // Задания текущего игрового дня.
    [SerializeField] private List<DailyTask> currentTasks = new();


    // Другие системы могут читать задания,
    // но не могут напрямую менять сам список.
    public IReadOnlyList<DailyTask> CurrentTasks => currentTasks;


    // Вызывается, когда изменилось состояние заданий.
    public event Action OnTasksChanged;

    // Вызывается, когда выполнены ВСЕ задания.
    public event Action OnAllTasksCompleted;


    /// <summary>
    /// Вызывается элементами панели после изменения состояния.
    ///
    /// elementID — какой элемент панели изменился.
    /// value — его новое значение.
    /// </summary>
    public void CheckTask(PanelElementID elementID, int value)
    {
        // Защита на случай отсутствия списка.
        if (currentTasks == null)
        {
            Debug.LogError(
                "TaskManager: список Current Tasks отсутствует!",
                this
            );

            return;
        }


        bool taskCompletedNow = false;


        foreach (DailyTask task in currentTasks)
        {
            // Защита от пустого элемента списка.
            if (task == null)
                continue;


            if (task.Check(elementID, value))
            {
                Debug.Log(
                    $"Выполнено задание: {task.Description}"
                );

                taskCompletedNow = true;

                // Одно действие выполняет максимум одно задание.
                break;
            }
        }


        // Если задание действительно изменилось —
        // сообщаем UI.
        if (taskCompletedNow)
        {
            OnTasksChanged?.Invoke();
        }


        // Проверяем завершение всей смены.
        if (AreAllTasksCompleted())
        {
            Debug.Log("ВСЕ ЗАДАНИЯ ВЫПОЛНЕНЫ");

            OnAllTasksCompleted?.Invoke();
        }
    }


    /// <summary>
    /// Проверяет, выполнены ли все задания.
    /// </summary>
    public bool AreAllTasksCompleted()
    {
        if (currentTasks == null || currentTasks.Count == 0)
            return false;


        foreach (DailyTask task in currentTasks)
        {
            // Пустой элемент считаем ошибкой,
            // поэтому смену завершённой не считаем.
            if (task == null)
                return false;

            if (!task.IsCompleted)
                return false;
        }


        return true;
    }


    /// <summary>
    /// Сбрасывает задания.
    /// Позже понадобится при начале нового дня.
    /// </summary>
    public void ResetTasks()
    {
        if (currentTasks == null)
            return;


        foreach (DailyTask task in currentTasks)
        {
            if (task != null)
            {
                task.ResetTask();
            }
        }


        OnTasksChanged?.Invoke();
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [Header("Tasks")]

    // Задания текущего игрового дня.
    [SerializeField] private List<DailyTask> currentTasks = new();


    // Другие системы могут читать задания,
    // но не могут напрямую менять список.
    public IReadOnlyList<DailyTask> CurrentTasks => currentTasks;


    // Вызывается, когда изменилось состояние заданий.
    // Например, когда одно из заданий было выполнено.
    public event Action OnTasksChanged;


    // Вызывается, когда выполнены все задания.
    public event Action OnAllTasksCompleted;


    private void OnEnable()
    {
        // Подписываемся на реальные действия игрока на панели.
        Panel.PanelEvents.OnElementChanged += HandlePanelElementChanged;
    }


    private void OnDisable()
    {
        // Обязательно отписываемся от события.
        Panel.PanelEvents.OnElementChanged -= HandlePanelElementChanged;
    }


    /// <summary>
    /// Получает изменение состояния реального элемента панели.
    /// </summary>
    private void HandlePanelElementChanged(
        Panel.PanelElementID elementID,
        int value)
    {
        Debug.Log(
            $"[TASK MANAGER] Получено от панели: {elementID} = {value}"
        );

        CheckTask(elementID, value);
    }


    /// <summary>
    /// Проверяет задания после действия игрока на панели.
    /// elementID - какой элемент панели изменился.
    /// value - новое значение элемента.
    /// </summary>
    public void CheckTask(
        Panel.PanelElementID elementID,
        int value)
    {
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
            if (task == null)
                continue;


            // DailyTask сам проверяет:
            // 1. нужный ли это элемент;
            // 2. правильное ли значение.
            if (task.Check(elementID, value))
            {
                Debug.Log(
                    $"[TASK COMPLETED] {task.Description}"
                );

                taskCompletedNow = true;

                // Одно действие выполняет максимум одно задание.
                break;
            }
        }


        // Если задание выполнилось,
        // сообщаем UI, что нужно обновиться.
        if (taskCompletedNow)
        {
            OnTasksChanged?.Invoke();
        }


        // Проверяем, выполнены ли теперь все задания.
        if (AreAllTasksCompleted())
        {
            Debug.Log(
                "[TASK MANAGER] Все задания выполнены"
            );

            OnAllTasksCompleted?.Invoke();
        }
    }


    /// <summary>
    /// Проверяет, выполнены ли все задания текущего дня.
    /// </summary>
    public bool AreAllTasksCompleted()
    {
        if (currentTasks == null ||
            currentTasks.Count == 0)
        {
            return false;
        }


        foreach (DailyTask task in currentTasks)
        {
            if (task == null)
                return false;


            if (!task.IsCompleted)
                return false;
        }


        return true;
    }


    /// <summary>
    /// Сбрасывает задания.
    /// Позже используется при начале нового дня.
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
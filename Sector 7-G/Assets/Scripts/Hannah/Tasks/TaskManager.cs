using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [Header("Current shift tasks")]
    [SerializeField]
    private List<DailyTask> currentTasks = new();


    public event Action OnTasksChanged;
    public event Action<int> OnTaskCompleted;


    public IReadOnlyList<DailyTask> CurrentTasks =>
        currentTasks;


    public int CompletedTaskCount
    {
        get
        {
            if (currentTasks == null)
                return 0;


            int count = 0;


            foreach (DailyTask task in currentTasks)
            {
                if (task != null &&
                    task.IsCompleted)
                {
                    count++;
                }
            }


            return count;
        }
    }


    // ==================================================
    // UNITY
    // ==================================================

    private void OnEnable()
    {
        Panel.PanelEvents.OnElementChanged +=
            HandlePanelElementChanged;
    }


    private void OnDisable()
    {
        Panel.PanelEvents.OnElementChanged -=
            HandlePanelElementChanged;
    }


    private void Start()
    {
        DebugCurrentTasks();
    }


    // ==================================================
    // PANEL EVENT
    // ==================================================

    private void HandlePanelElementChanged(
        Panel.PanelElementID elementID,
        int value)
    {
        Debug.Log(
            $"[TASK ACTION] {elementID} = {value}"
        );


        if (currentTasks == null)
            return;


        foreach (DailyTask task in currentTasks)
        {
            if (task == null)
                continue;


            if (task.IsCompleted)
                continue;


            // ------------------------------------------
            // Это вообще элемент данного задания?
            // ------------------------------------------

            if (task.TargetElement != elementID)
                continue;


            Debug.Log(
                $"[TASK] Найдено задание для {elementID}. " +
                $"Нужно значение {task.RequiredValue}, " +
                $"получено {value}."
            );


            // ------------------------------------------
            // Элемент правильный,
            // но значение пока неправильное.
            //
            // Задание НЕ выполняем,
            // но это НЕ считается ошибкой.
            // ------------------------------------------

            if (task.RequiredValue != value)
            {
                Debug.Log(
                    $"[TASK] {elementID} относится к заданию, " +
                    $"но значение пока неправильное."
                );


                return;
            }


            // ------------------------------------------
            // ID + VALUE совпали.
            // ------------------------------------------

            bool completed =
                task.Check(
                    elementID,
                    value
                );


            if (!completed)
                return;


            Debug.Log(
                $"[TASK COMPLETED] " +
                $"{task.Description}"
            );


            OnTasksChanged?.Invoke();


            OnTaskCompleted?.Invoke(
                CompletedTaskCount
            );


            return;
        }


        Debug.Log(
            $"[TASK] {elementID} не выполнил задание."
        );
    }


    // ==================================================
    // EXACT ACTION
    // ==================================================

    /// <summary>
    /// Проверяет, является ли действие
    /// точным действием какого-либо задания:
    ///
    /// совпадает ID
    /// И
    /// совпадает Value.
    /// </summary>

    public bool IsActionRequiredByTask(
        Panel.PanelElementID elementID,
        int value)
    {
        if (currentTasks == null)
            return false;


        foreach (DailyTask task in currentTasks)
        {
            if (task == null)
                continue;


            if (task.IsCompleted)
                continue;


            if (task.TargetElement == elementID &&
                task.RequiredValue == value)
            {
                return true;
            }
        }


        return false;
    }


    // ==================================================
    // ELEMENT USED BY TASK
    // ==================================================

    /// <summary>
    /// Проверяет только сам элемент.
    ///
    /// Если B3 используется хотя бы в одном
    /// ежедневном задании, возвращает true
    /// независимо от текущего Value.
    ///
    /// Этот метод нужен WrongPanelActionHandler.
    /// </summary>

    public bool IsElementUsedByTask(
        Panel.PanelElementID elementID)
    {
        if (currentTasks == null)
            return false;


        foreach (DailyTask task in currentTasks)
        {
            if (task == null)
                continue;


            // Выполненные задания здесь тоже учитываем.
            //
            // Если B3 является частью списка заданий
            // текущей смены, взаимодействие с B3
            // не должно внезапно считаться
            // посторонней кнопкой.

            if (task.TargetElement == elementID)
            {
                Debug.Log(
                    $"[TASK ELEMENT CHECK] " +
                    $"{elementID} присутствует " +
                    $"в ежедневных заданиях."
                );


                return true;
            }
        }


        Debug.LogWarning(
            $"[TASK ELEMENT CHECK] " +
            $"{elementID} НЕТ " +
            $"в ежедневных заданиях."
        );


        return false;
    }


    // ==================================================
    // ALL TASKS COMPLETED
    // ==================================================

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
                continue;


            if (!task.IsCompleted)
            {
                return false;
            }
        }


        return true;
    }


    // ==================================================
    // DEBUG
    // ==================================================

    [ContextMenu("DEBUG Current Tasks")]
    public void DebugCurrentTasks()
    {
        Debug.Log(
            "========== DAILY TASKS =========="
        );


        if (currentTasks == null)
        {
            Debug.LogError(
                "Current Tasks = NULL"
            );

            return;
        }


        for (int i = 0;
             i < currentTasks.Count;
             i++)
        {
            DailyTask task =
                currentTasks[i];


            if (task == null)
            {
                Debug.Log(
                    $"Task #{i + 1}: NULL"
                );

                continue;
            }


            Debug.Log(
                $"TASK #{i + 1}\n" +
                $"Text: {task.Description}\n" +
                $"Element: {task.TargetElement}\n" +
                $"Required Value: {task.RequiredValue}\n" +
                $"Completed: {task.IsCompleted}"
            );
        }


        Debug.Log(
            "================================="
        );
    }
}
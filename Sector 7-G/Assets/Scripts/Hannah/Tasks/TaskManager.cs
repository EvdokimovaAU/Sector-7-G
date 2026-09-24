using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [Header("Current shift tasks")]
    [SerializeField]
    private List<DailyTask> currentTasks = new();


    // --------------------------------------------------
    // EVENTS
    // --------------------------------------------------

    public event Action OnTasksChanged;

    public event Action<int> OnTaskCompleted;


    // --------------------------------------------------
    // PUBLIC DATA
    // --------------------------------------------------

    public IReadOnlyList<DailyTask> CurrentTasks => currentTasks;


    public int CompletedTaskCount
    {
        get
        {
            if (currentTasks == null)
                return 0;

            int count = 0;

            foreach (DailyTask task in currentTasks)
            {
                if (task != null && task.IsCompleted)
                {
                    count++;
                }
            }

            return count;
        }
    }


    // --------------------------------------------------
    // UNITY
    // --------------------------------------------------

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


    // --------------------------------------------------
    // PANEL EVENTS
    // --------------------------------------------------

    private void HandlePanelElementChanged(
        Panel.PanelElementID elementID,
        int value)
    {
        if (currentTasks == null)
            return;


        foreach (DailyTask task in currentTasks)
        {
            if (task == null)
                continue;

            if (task.IsCompleted)
                continue;


            bool taskCompletedNow =
                task.Check(elementID, value);


            if (!taskCompletedNow)
                continue;


            Debug.Log(
                $"Task completed. Total completed: " +
                $"{CompletedTaskCount}"
            );


            OnTasksChanged?.Invoke();


            OnTaskCompleted?.Invoke(
                CompletedTaskCount
            );


            break;
        }
    }


    // --------------------------------------------------
    // CHECK ACTION
    // --------------------------------------------------

    /// <summary>
    /// Проверяет, является ли действие допустимым
    /// для какого-либо невыполненного ежедневного задания.
    ///
    /// Само задание этот метод НЕ выполняет.
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


    // --------------------------------------------------
    // ALL TASKS COMPLETED
    // --------------------------------------------------

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
}
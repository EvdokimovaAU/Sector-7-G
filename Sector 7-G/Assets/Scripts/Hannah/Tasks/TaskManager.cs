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

    // ¬ызываетс€ при изменении списка заданий.
    public event Action OnTasksChanged;

    // ¬ызываетс€ после выполнени€ одного задани€.
    // ѕередает количество выполненных заданий.
    public event Action<int> OnTaskCompleted;


    // --------------------------------------------------
    // PUBLIC DATA
    // --------------------------------------------------

    public IReadOnlyList<DailyTask> CurrentTasks => currentTasks;


    //  оличество уже выполненных заданий.
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


            // ”же выполненные задани€ пропускаем.
            if (task.IsCompleted)
                continue;


            // DailyTask сам провер€ет:
            // 1. тот ли элемент панели;
            // 2. правильное ли значение;
            // 3. не было ли задание выполнено раньше.
            //
            // ≈сли задание выполнилось именно сейчас,
            // Check вернет true.

            bool taskCompletedNow =
                task.Check(elementID, value);


            if (!taskCompletedNow)
                continue;


            Debug.Log(
                $"Task completed. Total completed: " +
                $"{CompletedTaskCount}"
            );


            // ќбновл€ем UI списка заданий.
            OnTasksChanged?.Invoke();


            // —ообщаем другим системам,
            // сколько заданий уже выполнено.
            //
            // Ќа это событие будет подписана
            // наша система аварий.
            OnTaskCompleted?.Invoke(
                CompletedTaskCount
            );


            // ќдно действие панели выполн€ет
            // максимум одно ежедневное задание.
            break;
        }
    }


    // --------------------------------------------------
    // PUBLIC METHODS
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
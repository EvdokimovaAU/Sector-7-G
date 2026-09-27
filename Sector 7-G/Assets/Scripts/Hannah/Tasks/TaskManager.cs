using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    // ==================================================
    // TASK POOL
    // ==================================================

    [Header("Task Pool")]

    [Tooltip("Все возможные задания для этой смены.")]
    [SerializeField]
    private List<DailyTask> taskPool = new();


    [Tooltip("Сколько случайных заданий выбрать на смену.")]
    [Min(1)]
    [SerializeField]
    private int tasksPerShift = 3;


    // ==================================================
    // CURRENT TASKS
    // ==================================================

    [Header("Current Shift Tasks")]

    [Tooltip("Сюда автоматически попадут выбранные задания.")]
    [SerializeField]
    private List<DailyTask> currentTasks = new();


    // ==================================================
    // EVENTS
    // ==================================================

    public event Action OnTasksChanged;

    public event Action<int> OnTaskCompleted;


    // ==================================================
    // PUBLIC
    // ==================================================

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

    private void Awake()
    {
        GenerateRandomTasks();
    }


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


    // ==================================================
    // GENERATE TASKS
    // ==================================================

    private void GenerateRandomTasks()
    {
        currentTasks.Clear();


        if (taskPool == null ||
            taskPool.Count == 0)
        {
            Debug.LogError(
                "[TaskManager] Task Pool пуст!"
            );

            return;
        }


        // Создаём временную копию пула.
        // Сам список в Inspector не изменяем.
        List<DailyTask> availableTasks =
            new List<DailyTask>();


        foreach (DailyTask task in taskPool)
        {
            if (task != null)
            {
                availableTasks.Add(task);
            }
        }


        int amount =
            Mathf.Min(
                tasksPerShift,
                availableTasks.Count
            );


        // Случайно выбираем задания.
        for (int i = 0; i < amount; i++)
        {
            int randomIndex =
                UnityEngine.Random.Range(
                    0,
                    availableTasks.Count
                );


            DailyTask selectedTask =
                availableTasks[randomIndex];


            currentTasks.Add(
                selectedTask
            );


            // Удаляем из временного списка,
            // чтобы одно задание не выпало дважды.
            availableTasks.RemoveAt(
                randomIndex
            );
        }


        Debug.Log(
            $"[TaskManager] Выбрано заданий: " +
            $"{currentTasks.Count}"
        );
    }


    // ==================================================
    // PANEL ACTION
    // ==================================================

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


            // Это не элемент текущего задания.
            if (task.TargetElement != elementID)
                continue;


            // Элемент правильный,
            // но значение неправильное.
            if (task.RequiredValue != value)
                return;


            // Проверяем задание.
            bool completed =
                task.Check(
                    elementID,
                    value
                );


            if (!completed)
                return;


            // Обновляем UI.
            OnTasksChanged?.Invoke();


            // Сообщаем системе аварий,
            // сколько заданий выполнено.
            OnTaskCompleted?.Invoke(
                CompletedTaskCount
            );


            return;
        }
    }


    // ==================================================
    // EXACT ACTION CHECK
    // ==================================================

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

    public bool IsElementUsedByTask(
        Panel.PanelElementID elementID)
    {
        if (currentTasks == null)
            return false;


        foreach (DailyTask task in currentTasks)
        {
            if (task == null)
                continue;


            if (task.TargetElement == elementID)
            {
                return true;
            }
        }


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
}
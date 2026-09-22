using System.Collections.Generic;
using UnityEngine;

public class TaskUI : MonoBehaviour
{
    [Header("References")]

    // Менеджер, в котором хранятся задания текущего дня.
    [SerializeField] private TaskManager taskManager;

    // Уже созданные на сцене строки заданий.
    // Никакие prefab больше не используются.
    [SerializeField] private List<TaskItemUI> taskItems = new();


    private void OnEnable()
    {
        // Подписываемся на событие изменения заданий.
        // Например, когда одно из заданий было выполнено.
        if (taskManager != null)
        {
            taskManager.OnTasksChanged += Refresh;
        }
    }


    private void Start()
    {
        // Проверяем ссылки из Inspector.
        if (!ValidateReferences())
            return;

        // При запуске заполняем существующие строки
        // данными из TaskManager.
        SetupTaskList();
    }


    private void OnDisable()
    {
        // Отписываемся от события,
        // когда объект выключается.
        if (taskManager != null)
        {
            taskManager.OnTasksChanged -= Refresh;
        }
    }


    /// <summary>
    /// Проверяет необходимые ссылки.
    /// </summary>
    private bool ValidateReferences()
    {
        if (taskManager == null)
        {
            Debug.LogError(
                "TaskUI: не назначен TaskManager!",
                this
            );

            return false;
        }

        if (taskItems == null || taskItems.Count == 0)
        {
            Debug.LogError(
                "TaskUI: не назначены TaskItem!",
                this
            );

            return false;
        }

        return true;
    }


    /// <summary>
    /// Передаёт задания существующим строкам UI.
    /// Ничего не создаёт и ничего не удаляет.
    /// </summary>
    private void SetupTaskList()
    {
        int count = Mathf.Min(
            taskItems.Count,
            taskManager.CurrentTasks.Count
        );

        for (int i = 0; i < count; i++)
        {
            TaskItemUI item = taskItems[i];
            DailyTask task = taskManager.CurrentTasks[i];

            if (item == null || task == null)
                continue;

            item.Setup(task);

            // Показываем используемую строку.
            item.gameObject.SetActive(true);
        }


        // Если UI-строк больше, чем заданий,
        // лишние строки скрываем.
        for (int i = count; i < taskItems.Count; i++)
        {
            if (taskItems[i] != null)
            {
                taskItems[i].gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// Обновляет галочки после выполнения заданий.
    /// Вызывается через TaskManager.OnTasksChanged.
    /// </summary>
    public void Refresh()
    {
        if (taskManager == null)
            return;

        int count = Mathf.Min(
            taskItems.Count,
            taskManager.CurrentTasks.Count
        );

        for (int i = 0; i < count; i++)
        {
            TaskItemUI item = taskItems[i];
            DailyTask task = taskManager.CurrentTasks[i];

            if (item == null || task == null)
                continue;

            item.UpdateState(task);
        }
    }
}
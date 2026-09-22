using UnityEngine;
using UnityEngine.InputSystem;

public class TaskDebug : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TaskManager taskManager;


    private void Update()
    {
        // ≈сли забыли назначить TaskManager Ч
        // просто ничего не делаем.
        if (taskManager == null)
            return;


        // ≈сли клавиатура почему-то недоступна.
        if (Keyboard.current == null)
            return;


        // 1 Ч включить насос Ќ-2.
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            taskManager.CheckTask(
                PanelElementID.Pump2,
                1
            );
        }


        // 2 Ч установить мощность 60%.
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            taskManager.CheckTask(
                PanelElementID.Power,
                60
            );
        }


        // 3 Ч закрыть клапан є1.
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            taskManager.CheckTask(
                PanelElementID.Valve1,
                2
            );
        }
    }
}
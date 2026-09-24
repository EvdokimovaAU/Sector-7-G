using TMPro;
using UnityEngine;

namespace Panel
{
    public class PanelValueControl : PanelElement
    {
        // ==================================================
        // VALUE SETTINGS
        // ==================================================

        [Header("Value Settings")]

        // Значение, которое сейчас выбирает игрок стрелочками.
        [SerializeField]
        private int currentValue = 0;

        // Значение, которое реально установлено в системе.
        [SerializeField]
        private int appliedValue = 0;

        [SerializeField]
        private int min = 0;

        [SerializeField]
        private int max = 100;

        [SerializeField]
        private int step = 10;


        // ==================================================
        // BUTTONS
        // ==================================================

        [Header("Buttons")]

        // Стрелка вверх.
        [SerializeField]
        private Collider2D increaseButton;

        // Стрелка вниз.
        [SerializeField]
        private Collider2D decreaseButton;

        // Отдельная кнопка "Установить".
        [SerializeField]
        private Collider2D applyButton;


        // ==================================================
        // VISUAL
        // ==================================================

        [Header("Visual")]

        // Показывает выбранное значение.
        [SerializeField]
        private TMP_Text valueText;


        // ==================================================
        // PUBLIC
        // ==================================================

        // Для игровой логики возвращаем именно
        // установленное значение, а не временно выбранное.
        public override int GetValue()
        {
            return appliedValue;
        }


        // У этого элемента взаимодействие происходит
        // через три отдельных Collider2D.
        public override void Interact()
        {
        }


        // ==================================================
        // INPUT
        // ==================================================

        private void Update()
        {
            if (UnityEngine.InputSystem.Mouse.current == null)
                return;


            if (!UnityEngine.InputSystem.Mouse.current
                    .leftButton.wasPressedThisFrame)
            {
                return;
            }


            Camera camera = Camera.main;

            if (camera == null)
                return;


            Vector2 screenPosition =
                UnityEngine.InputSystem.Mouse.current
                    .position.ReadValue();


            Vector3 worldPosition =
                camera.ScreenToWorldPoint(screenPosition);


            // ----------------------------------------------
            // Стрелка вверх
            // ----------------------------------------------

            if (increaseButton != null &&
                increaseButton.OverlapPoint(worldPosition))
            {
                Increase();
                return;
            }


            // ----------------------------------------------
            // Стрелка вниз
            // ----------------------------------------------

            if (decreaseButton != null &&
                decreaseButton.OverlapPoint(worldPosition))
            {
                Decrease();
                return;
            }


            // ----------------------------------------------
            // Кнопка "Установить"
            // ----------------------------------------------

            if (applyButton != null &&
                applyButton.OverlapPoint(worldPosition))
            {
                ApplyValue();
            }
        }


        // ==================================================
        // CHANGE VALUE
        // ==================================================

        public void Increase()
        {
            int next = Mathf.Clamp(
                currentValue + step,
                min,
                max
            );


            if (next == currentValue)
                return;


            currentValue = next;


            // Только обновляем цифру.
            // PanelEvents здесь НЕ вызываем.
            RefreshVisual();


            Debug.Log(
                $"[PANEL SELECT] {elementID} = {currentValue}"
            );
        }


        public void Decrease()
        {
            int next = Mathf.Clamp(
                currentValue - step,
                min,
                max
            );


            if (next == currentValue)
                return;


            currentValue = next;


            // Только обновляем цифру.
            // PanelEvents здесь НЕ вызываем.
            RefreshVisual();


            Debug.Log(
                $"[PANEL SELECT] {elementID} = {currentValue}"
            );
        }


        // ==================================================
        // APPLY
        // ==================================================

        public void ApplyValue()
        {
            // Сохраняем выбранное значение
            // как реально установленное.
            appliedValue = currentValue;


            // Только теперь сообщаем остальной игре,
            // что игрок совершил действие.
            PanelEvents.ElementChanged(
                elementID,
                appliedValue
            );


            Debug.Log(
                $"[PANEL APPLY] {elementID} = {appliedValue}"
            );
        }


        // ==================================================
        // VISUAL
        // ==================================================

        protected override void RefreshVisual()
        {
            if (valueText != null)
            {
                valueText.text =
                    currentValue.ToString();
            }
        }


        // ==================================================
        // START
        // ==================================================

        private void Start()
        {
            // При запуске выбранное значение
            // совпадает с реально установленным.
            currentValue = appliedValue;

            RefreshVisual();
        }
    }
}
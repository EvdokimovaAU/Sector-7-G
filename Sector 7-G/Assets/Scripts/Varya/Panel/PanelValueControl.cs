using TMPro;
using UnityEngine;

namespace Panel
{
    public class PanelValueControl : PanelElement
    {
        [Header("Value Settings")]
        [SerializeField] private int currentValue = 0;
        [SerializeField] private int min = 0;
        [SerializeField] private int max = 100;
        [SerializeField] private int step = 10;


        [Header("Buttons")]
        [SerializeField] private Collider2D increaseButton;
        [SerializeField] private Collider2D decreaseButton;


        [Header("Visual")]
        [SerializeField] private TMP_Text valueText;


        public override int GetValue()
        {
            return currentValue;
        }


        // Для ValueControl обычный Interact не используется,
        // потому что у него две отдельные кнопки.
        public override void Interact()
        {
        }


        private void Update()
        {
            if (UnityEngine.InputSystem.Mouse.current == null)
                return;

            if (!UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                return;

            Camera camera = Camera.main;

            if (camera == null)
                return;

            Vector2 screenPosition =
                UnityEngine.InputSystem.Mouse.current.position.ReadValue();

            Vector3 worldPosition =
                camera.ScreenToWorldPoint(screenPosition);


            // Стрелка вверх
            if (increaseButton != null &&
                increaseButton.OverlapPoint(worldPosition))
            {
                Increase();
                return;
            }


            // Стрелка вниз
            if (decreaseButton != null &&
                decreaseButton.OverlapPoint(worldPosition))
            {
                Decrease();
            }
        }


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

            RefreshVisual();

            PanelEvents.ElementChanged(
                elementID,
                currentValue
            );

            Debug.Log($"[PANEL] {elementID} = {currentValue}");
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

            RefreshVisual();

            PanelEvents.ElementChanged(
                elementID,
                currentValue
            );

            Debug.Log($"[PANEL] {elementID} = {currentValue}");
        }


        protected override void RefreshVisual()
        {
            if (valueText != null)
            {
                valueText.text = currentValue.ToString();
            }
        }


        private void Start()
        {
            RefreshVisual();
        }
    }
}
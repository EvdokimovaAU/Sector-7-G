using UnityEngine;

namespace Panel
{
    public class PanelRotarySwitch : PanelElement
    {
        [Header("Rotary Switch")]

        // Значение, которое игрок сейчас выбирает.
        [SerializeField]
        private int currentState = 0;

        // Значение, которое реально установлено.
        [SerializeField]
        private int appliedState = 0;

        // Количество положений.
        [Min(1)]
        [SerializeField]
        private int stateCount = 4;


        [Header("Rotation")]

        // Что именно вращаем.
        [SerializeField]
        private Transform rotatingPart;

        // Угол положения 0.
        [SerializeField]
        private float startAngle = 135f;

        // Шаг между положениями.
        [SerializeField]
        private float angleStep = 90f;


        private void Start()
        {
            if (rotatingPart == null)
            {
                rotatingPart = transform;
            }

            currentState = appliedState;

            RefreshVisual();
        }


        // Возвращаем именно установленное значение.
        public override int GetValue()
        {
            return appliedState;
        }


        // При клике только выбираем положение.
        public override void Interact()
        {
            currentState++;

            if (currentState >= stateCount)
            {
                currentState = 0;
            }

            RefreshVisual();

            // Здесь PanelEvents НЕ вызываем.
            Debug.Log(
                $"[ROTARY SELECT] {elementID} = {currentState}"
            );
        }


        // Вызывается общей кнопкой "УСТАНОВИТЬ".
        public void ApplyValue()
        {
            appliedState = currentState;

            PanelEvents.ElementChanged(
                elementID,
                appliedState
            );

            Debug.Log(
                $"[ROTARY APPLY] {elementID} = {appliedState}"
            );
        }


        protected override void RefreshVisual()
        {
            if (rotatingPart == null)
                return;

            float angle =
                startAngle - currentState * angleStep;

            rotatingPart.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    angle
                );
        }
    }
}
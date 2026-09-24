using UnityEngine;

namespace Panel
{
    public class PanelRotarySwitch : PanelElement
    {
        [Header("Rotary Switch")]

        // Текущее положение: 0, 1, 2 или 3.
        [SerializeField] private int currentState = 0;

        // Количество положений переключателя.
        [SerializeField] private int stateCount = 4;


        [Header("Rotation")]

        // Объект, который физически вращается.
        // Обычно это SpriteRenderer самой ручки.
        [SerializeField] private Transform rotatingPart;

        // Угол первого положения.
        [SerializeField] private float startAngle = 135f;

        // На сколько градусов поворачиваемся
        // при переходе на следующее положение.
        [SerializeField] private float angleStep = 90f;


        private void Start()
        {
            if (rotatingPart == null)
            {
                rotatingPart = transform;
            }

            RefreshVisual();
        }


        public override int GetValue()
        {
            return currentState;
        }


        public override void Interact()
        {
            // 0 -> 1 -> 2 -> 3 -> 0
            currentState++;

            if (currentState >= stateCount)
            {
                currentState = 0;
            }

            RefreshVisual();

            // Отправляем новое положение
            // в общую систему панели.
            PanelEvents.ElementChanged(
                elementID,
                currentState
            );

            Debug.Log(
                $"[ROTARY] {elementID} = {currentState}"
            );
        }


        protected override void RefreshVisual()
        {
            if (rotatingPart == null)
                return;

            float angle =
                startAngle - currentState * angleStep;

            rotatingPart.localRotation =
                Quaternion.Euler(0f, 0f, angle);
        }
    }
}
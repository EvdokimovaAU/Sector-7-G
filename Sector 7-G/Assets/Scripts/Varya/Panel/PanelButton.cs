using UnityEngine;

namespace Panel
{
    public class PanelButton : PanelElement
    {
        [Header("Button Settings")]

        // true = кнопка имеет два состояния: ВКЛ / ВЫКЛ.
        // false = обычная командная кнопка.
        [SerializeField] private bool isTwoState = true;

        // Текущее состояние кнопки.
        [SerializeField] private bool isOn;


        [Header("Button Visual")]

        // SpriteRenderer самой кнопки.
        [SerializeField] private SpriteRenderer targetRenderer;

        // Спрайт обычной кнопки.
        [SerializeField] private Sprite spriteNormal;

        // Спрайт нажатой кнопки.
        [SerializeField] private Sprite spritePressed;


        [Header("Indicator Lamp")]

        // SpriteRenderer отдельной лампочки.
        // Если лампочки у кнопки нет — можно оставить пустым.
        [SerializeField] private SpriteRenderer indicatorRenderer;

        // Лампочка, когда кнопка выключена.
        [SerializeField] private Sprite indicatorOffSprite;

        // Лампочка, когда кнопка включена.
        [SerializeField] private Sprite indicatorOnSprite;


        public override int GetValue()
        {
            return isTwoState ? (isOn ? 1 : 0) : 1;
        }


        public override void Interact()
        {
            if (isTwoState)
            {
                // Меняем состояние кнопки.
                isOn = !isOn;

                // Обновляем кнопку и лампочку.
                RefreshVisual();

                // Сообщаем остальным системам новое значение.
                PanelEvents.ElementChanged(
                    elementID,
                    GetValue()
                );
            }
            else
            {
                // Обычная командная кнопка
                // при нажатии отправляет значение 1.
                PanelEvents.ElementChanged(
                    elementID,
                    1
                );
            }
        }


        protected override void RefreshVisual()
        {
            // Обновляем внешний вид кнопки.
            if (targetRenderer != null)
            {
                targetRenderer.sprite =
                    isOn ? spritePressed : spriteNormal;
            }


            // Обновляем отдельную лампочку.
            if (indicatorRenderer != null)
            {
                indicatorRenderer.sprite =
                    isOn
                        ? indicatorOnSprite
                        : indicatorOffSprite;
            }
        }


        private void Start()
        {
            // Если SpriteRenderer кнопки не назначен,
            // берём его с этого объекта.
            if (targetRenderer == null)
            {
                targetRenderer =
                    GetComponent<SpriteRenderer>();
            }

            // Выставляем начальное состояние
            // кнопки и лампочки.
            RefreshVisual();
        }
    }
}
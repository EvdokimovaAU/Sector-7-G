using UnityEngine;

namespace Panel
{
    public class PanelToggle : PanelElement
    {
        [Header("Toggle Settings")]

        // Текущее состояние тумблера.
        [SerializeField] private bool isOn;


        [Header("Visual")]

        // SpriteRenderer тумблера на уровне.
        [SerializeField] private SpriteRenderer targetRenderer;

        // Спрайт выключенного состояния.
        [SerializeField] private Sprite spriteOff;

        // Спрайт включенного состояния.
        [SerializeField] private Sprite spriteOn;


        public override int GetValue()
        {
            return isOn ? 1 : 0;
        }


        public override void Interact()
        {
            // Меняем состояние.
            isOn = !isOn;

            // Меняем внешний вид.
            RefreshVisual();

            // Сообщаем остальным системам,
            // какой элемент изменился и какое теперь значение.
            PanelEvents.ElementChanged(
                elementID,
                GetValue()
            );
        }


        protected override void RefreshVisual()
        {
            if (targetRenderer == null)
                return;

            targetRenderer.sprite =
                isOn ? spriteOn : spriteOff;
        }


        private void Start()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<SpriteRenderer>();
            }

            RefreshVisual();
        }
    }
}
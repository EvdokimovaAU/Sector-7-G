using UnityEngine;

namespace Panel
{
    public class PanelLever : PanelElement
    {
        [Header("Lever Settings")]

        // 0 = ОТКР
        // 1 = НОР
        // 2 = ЗАКР
        [SerializeField] private int state = 1;


        [Header("Visual")]

        // SpriteRenderer рычага на уровне.
        [SerializeField] private SpriteRenderer targetRenderer;

        [SerializeField] private Sprite spriteOpen;
        [SerializeField] private Sprite spriteNormal;
        [SerializeField] private Sprite spriteClosed;


        public override int GetValue()
        {
            return state;
        }


        public override void Interact()
        {
            // Каждый клик переключает:
            // 0 -> 1 -> 2 -> 0...
            state = (state + 1) % 3;

            RefreshVisual();

            PanelEvents.ElementChanged(
                elementID,
                GetValue()
            );
        }


        protected override void RefreshVisual()
        {
            if (targetRenderer == null)
                return;

            targetRenderer.sprite = state switch
            {
                0 => spriteOpen,
                1 => spriteNormal,
                2 => spriteClosed,
                _ => spriteNormal
            };
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
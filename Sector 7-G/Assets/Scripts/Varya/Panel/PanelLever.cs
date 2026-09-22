using UnityEngine;
using UnityEngine.UI;

namespace Panel
{
    public class PanelLever : PanelElement
    {
        // 0 = ÎÒÊÐ, 1 = ÍÎÐ, 2 = ÇÀÊÐ
        [SerializeField] private int state = 1;

        [Header("Visual")]
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite spriteOpen;
        [SerializeField] private Sprite spriteNormal;
        [SerializeField] private Sprite spriteClosed;

        public override int GetValue() => state;

        public override void Interact()
        {
            state = (state + 1) % 3;
            RefreshVisual();
            PanelEvents.ElementChanged(elementID, GetValue());
        }

        protected override void RefreshVisual()
        {
            if (targetImage == null) return;
            targetImage.sprite = state switch
            {
                0 => spriteOpen,
                1 => spriteNormal,
                2 => spriteClosed,
                _ => spriteNormal
            };
        }

        private void Start() => RefreshVisual();
    }
}
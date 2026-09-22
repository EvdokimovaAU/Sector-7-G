using UnityEngine;
using UnityEngine.UI;

namespace Panel
{
    public class PanelToggle : PanelElement
    {
        [SerializeField] private bool isOn;

        [Header("Visual")]
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite spriteOff;
        [SerializeField] private Sprite spriteOn;

        public override int GetValue() => isOn ? 1 : 0;

        public override void Interact()
        {
            isOn = !isOn;                                       // 3. State
            RefreshVisual();                                    // 4. Visual
            PanelEvents.ElementChanged(elementID, GetValue());  // 5. Event
        }

        protected override void RefreshVisual()
        {
            if (targetImage != null)
                targetImage.sprite = isOn ? spriteOn : spriteOff;
        }

        private void Start() => RefreshVisual();
    }
}
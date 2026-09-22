using UnityEngine;
using UnityEngine.UI;

namespace Panel
{
    public class PanelButton : PanelElement
    {
        [Tooltip("true = двухсостояние (0/1). false = командная кнопка.")]
        [SerializeField] private bool isTwoState = true;
        [SerializeField] private bool isOn;

        [Header("Visual")]
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite spriteNormal;
        [SerializeField] private Sprite spritePressed;

        public override int GetValue() => isTwoState ? (isOn ? 1 : 0) : 1;

        public override void Interact()
        {
            if (isTwoState)
            {
                isOn = !isOn;
                RefreshVisual();
                PanelEvents.ElementChanged(elementID, GetValue());
            }
            else
            {
                RefreshVisual();
                PanelEvents.ElementChanged(elementID, 1);
            }
        }

        protected override void RefreshVisual()
        {
            if (targetImage == null || !isTwoState) return;
            targetImage.sprite = isOn ? spritePressed : spriteNormal;
        }

        private void Start() => RefreshVisual();
    }
}
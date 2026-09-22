using TMPro;
using UnityEngine;

namespace Panel
{
    public class PanelValueControl : PanelElement
    {
        [SerializeField] private int currentValue;
        [SerializeField] private int min = 0;
        [SerializeField] private int max = 100;
        [SerializeField] private int step = 1;

        [Header("Visual")]
        [SerializeField] private TMP_Text valueText;

        public override int GetValue() => currentValue;
        public override void Interact() { }

        public void Increase()
        {
            int next = Mathf.Clamp(currentValue + step, min, max);
            if (next == currentValue) return;
            currentValue = next;
            RefreshVisual();
            PanelEvents.ElementChanged(elementID, GetValue());
        }

        public void Decrease()
        {
            int next = Mathf.Clamp(currentValue - step, min, max);
            if (next == currentValue) return;
            currentValue = next;
            RefreshVisual();
            PanelEvents.ElementChanged(elementID, GetValue());
        }

        protected override void RefreshVisual()
        {
            if (valueText != null) valueText.text = currentValue.ToString();
        }

        private void Start() => RefreshVisual();
    }
}
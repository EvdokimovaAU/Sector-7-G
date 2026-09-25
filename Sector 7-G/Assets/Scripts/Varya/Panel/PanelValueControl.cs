using TMPro;
using UnityEngine;

namespace Panel
{
    public class PanelValueControl : MonoBehaviour
    {
        [Header("Element")]
        [SerializeField] private PanelElementID elementID;


        [Header("Value")]
        [SerializeField] private int currentValue = 0;
        [SerializeField] private int minValue = 0;
        [SerializeField] private int maxValue = 100;
        [SerializeField] private int step = 1;


        [Header("UI")]
        [SerializeField] private TMP_Text valueText;


        [Header("References")]
        [SerializeField] private PanelInstabilityManager instabilityManager;


        private int appliedValue;


        public int CurrentValue => currentValue;
        public int AppliedValue => appliedValue;
        public PanelElementID ElementID => elementID;


        private void Start()
        {
            if (instabilityManager == null)
            {
                instabilityManager =
                    FindAnyObjectByType<PanelInstabilityManager>();
            }

            currentValue = Mathf.Clamp(
                currentValue,
                minValue,
                maxValue
            );

            appliedValue = currentValue;

            UpdateValueText();

            Debug.Log(
                $"[VALUE CONTROL READY] {elementID} = {currentValue}"
            );
        }


        // ==========================================
        // +
        // ==========================================

        public void Increase()
        {
            Debug.Log("[VALUE] Increase вызван");

            int amount = step;

            if (AreControlsInverted())
                amount = -step;

            ChangePreviewValue(amount);
        }


        // ==========================================
        // -
        // ==========================================

        public void Decrease()
        {
            Debug.Log("[VALUE] Decrease вызван");

            int amount = -step;

            if (AreControlsInverted())
                amount = step;

            ChangePreviewValue(amount);
        }


        // ==========================================
        // ИЗМЕНЕНИЕ ПРЕДВАРИТЕЛЬНОГО ЗНАЧЕНИЯ
        // ==========================================

        private void ChangePreviewValue(int amount)
        {
            int oldValue = currentValue;

            currentValue = Mathf.Clamp(
                currentValue + amount,
                minValue,
                maxValue
            );

            UpdateValueText();

            Debug.Log(
                $"[VALUE CHANGED] {elementID}: " +
                $"{oldValue} -> {currentValue}"
            );
        }


        // ==========================================
        // ПРИМЕНИТЬ
        // ==========================================

        public void Apply()
        {
            appliedValue = currentValue;

            Debug.Log(
                $"[VALUE APPLIED] {elementID} = {appliedValue}"
            );

            PanelEvents.ElementChanged(
                elementID,
                appliedValue
            );
        }


        // ==========================================
        // TEXT
        // ==========================================

        private void UpdateValueText()
        {
            if (valueText == null)
            {
                Debug.LogError(
                    $"[{elementID}] Value Text не назначен!"
                );

                return;
            }

            valueText.text = currentValue.ToString();
        }


        // ==========================================
        // INSTABILITY
        // ==========================================

        private bool AreControlsInverted()
        {
            if (instabilityManager == null)
                return false;

            return instabilityManager
                .ShouldInvertValueControl(elementID);
        }


        // ==========================================
        // SET VALUE
        // ==========================================

        public void SetValue(
            int value,
            bool sendEvent = true
        )
        {
            currentValue = Mathf.Clamp(
                value,
                minValue,
                maxValue
            );

            appliedValue = currentValue;

            UpdateValueText();

            if (sendEvent)
            {
                PanelEvents.ElementChanged(
                    elementID,
                    appliedValue
                );
            }
        }
    }
}
using TMPro;
using UnityEngine;

namespace Panel
{
    public class PanelValueControl : MonoBehaviour
    {
        // ==================================================
        // ELEMENT
        // ==================================================

        [Header("Element")]
        [SerializeField]
        private PanelElementID elementID;


        // ==================================================
        // VALUE
        // ==================================================

        [Header("Value")]

        [SerializeField]
        private int currentValue = 0;

        [SerializeField]
        private int minValue = 0;

        [SerializeField]
        private int maxValue = 100;

        [SerializeField]
        private int step = 1;


        // ==================================================
        // UI
        // ==================================================

        [Header("UI")]

        [SerializeField]
        private TMP_Text valueText;


        // ==================================================
        // REFERENCES
        // ==================================================

        [Header("References")]

        [SerializeField]
        private PanelInstabilityManager instabilityManager;


        // ==================================================
        // PUBLIC
        // ==================================================

        public int CurrentValue => currentValue;

        public PanelElementID ElementID => elementID;


        // ==================================================
        // UNITY
        // ==================================================

        private void Start()
        {
            if (instabilityManager == null)
            {
                instabilityManager =
                    FindAnyObjectByType<
                        PanelInstabilityManager
                    >();
            }


            currentValue =
                Mathf.Clamp(
                    currentValue,
                    minValue,
                    maxValue
                );


            UpdateValueText();
        }


        // ==================================================
        // INCREASE
        // ==================================================

        public void Increase()
        {
            // При нормальной панели:
            //
            // ↑ = увеличение
            //
            // При надежности < 60:
            //
            // ↑ = уменьшение

            if (AreControlsInverted())
            {
                Debug.LogWarning(
                    $"[CONTROL INVERTED] " +
                    $"{elementID}: UP -> DOWN"
                );


                ChangeValue(-step);
            }
            else
            {
                ChangeValue(step);
            }
        }


        // ==================================================
        // DECREASE
        // ==================================================

        public void Decrease()
        {
            // При нормальной панели:
            //
            // ↓ = уменьшение
            //
            // При надежности < 60:
            //
            // ↓ = увеличение

            if (AreControlsInverted())
            {
                Debug.LogWarning(
                    $"[CONTROL INVERTED] " +
                    $"{elementID}: DOWN -> UP"
                );


                ChangeValue(step);
            }
            else
            {
                ChangeValue(-step);
            }
        }


        // ==================================================
        // CHANGE VALUE
        // ==================================================

        private void ChangeValue(int amount)
        {
            int oldValue =
                currentValue;


            currentValue =
                Mathf.Clamp(
                    currentValue + amount,
                    minValue,
                    maxValue
                );


            // Обновляем цифру на панели.
            UpdateValueText();


            Debug.Log(
                $"[VALUE CONTROL] " +
                $"{elementID}: " +
                $"{oldValue} -> {currentValue}"
            );


            // Сообщаем остальной игре,
            // какое значение установлено.

            PanelEvents.ElementChanged(
                elementID,
                currentValue
            );
        }


        // ==================================================
        // UI
        // ==================================================

        private void UpdateValueText()
        {
            if (valueText == null)
            {
                return;
            }


            valueText.text =
                currentValue.ToString();
        }


        // ==================================================
        // INSTABILITY
        // ==================================================

        private bool AreControlsInverted()
        {
            if (instabilityManager == null)
            {
                return false;
            }


            return instabilityManager
                .ShouldInvertValueControl(
                    elementID
                );
        }


        // ==================================================
        // SET VALUE
        // ==================================================

        public void SetValue(
            int value,
            bool sendEvent = true)
        {
            currentValue =
                Mathf.Clamp(
                    value,
                    minValue,
                    maxValue
                );


            UpdateValueText();


            if (sendEvent)
            {
                PanelEvents.ElementChanged(
                    elementID,
                    currentValue
                );
            }
        }
    }
}
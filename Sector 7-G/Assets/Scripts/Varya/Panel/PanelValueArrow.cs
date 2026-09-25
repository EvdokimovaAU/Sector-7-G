using UnityEngine;

namespace Panel
{
    public class PanelValueArrow : MonoBehaviour
    {
        public enum ArrowType
        {
            Increase,
            Decrease
        }

        [Header("Control")]
        [SerializeField]
        private PanelValueControl valueControl;

        [Header("Arrow")]
        [SerializeField]
        private ArrowType arrowType;


        // Вызывается менеджером кликов
        public void Click()
        {
            Debug.Log(
                $"[ARROW CLICK] {name} -> {arrowType}"
            );

            if (valueControl == null)
            {
                Debug.LogError(
                    $"[{name}] Value Control не назначен!"
                );

                return;
            }

            if (arrowType == ArrowType.Increase)
            {
                valueControl.Increase();
            }
            else
            {
                valueControl.Decrease();
            }
        }
    }
}
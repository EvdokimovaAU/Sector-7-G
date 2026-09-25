using UnityEngine;

namespace Panel
{
    public class PanelValueButton : MonoBehaviour
    {
        public enum ButtonAction
        {
            Increase,
            Decrease,
            Apply
        }


        [Header("Target")]
        [SerializeField]
        private PanelValueControl valueControl;


        [Header("Action")]
        [SerializeField]
        private ButtonAction action;


        private void OnMouseDown()
        {
            Debug.Log(
                $"[VALUE CLICK] {name} -> {action}"
            );


            if (valueControl == null)
            {
                Debug.LogError(
                    $"[{name}] Value Control не назначен!"
                );

                return;
            }


            switch (action)
            {
                case ButtonAction.Increase:
                    valueControl.Increase();
                    break;


                case ButtonAction.Decrease:
                    valueControl.Decrease();
                    break;


                case ButtonAction.Apply:
                    valueControl.Apply();
                    break;
            }
        }
    }
}
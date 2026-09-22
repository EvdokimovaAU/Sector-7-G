using UnityEngine;

namespace Panel
{
    public class PanelDebugListener : MonoBehaviour
    {
        private void OnEnable() => PanelEvents.OnElementChanged += HandleChanged;
        private void OnDisable() => PanelEvents.OnElementChanged -= HandleChanged;

        private void HandleChanged(PanelElementID id, int value)
        {
            Debug.Log($"[Panel] {id} = {value}");
        }
    }
}
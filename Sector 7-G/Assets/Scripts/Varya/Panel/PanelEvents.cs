using System;

namespace Panel
{
    public static class PanelEvents
    {
        public static event Action<PanelElementID, int> OnElementChanged;

        public static void ElementChanged(PanelElementID id, int value)
        {
            OnElementChanged?.Invoke(id, value);
        }
    }
}
using UnityEngine;

namespace Panel
{
    public abstract class PanelElement : MonoBehaviour
    {
        [SerializeField] protected PanelElementID elementID;

        public PanelElementID ID => elementID;

        public abstract int GetValue();
        public abstract void Interact();
        protected abstract void RefreshVisual();
    }
}
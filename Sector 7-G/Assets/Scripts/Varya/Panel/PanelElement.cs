using UnityEngine;
using UnityEngine.InputSystem;

namespace Panel
{
    public abstract class PanelElement : MonoBehaviour
    {
        [Header("Panel Element")]
        [SerializeField] protected PanelElementID elementID;

        public PanelElementID ID => elementID;


        private void Update()
        {
            // Нет мыши - ничего не проверяем.
            if (Mouse.current == null)
                return;

            // Реагируем только на момент нажатия ЛКМ.
            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;


            // Позиция мыши на экране.
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

            // Основная камера.
            Camera camera = Camera.main;

            if (camera == null)
            {
                Debug.LogError("PanelElement: Main Camera не найдена!");
                return;
            }


            // Переводим координаты мыши
            // из экранных координат в координаты игрового мира.
            Vector3 mouseWorldPosition =
                camera.ScreenToWorldPoint(mouseScreenPosition);


            // Проверяем, находится ли точка клика
            // внутри Collider2D именно этого элемента.
            Collider2D collider = GetComponent<Collider2D>();

            if (collider == null)
            {
                Debug.LogWarning(
                    $"PanelElement {elementID}: отсутствует Collider2D!",
                    this
                );

                return;
            }


            if (collider.OverlapPoint(mouseWorldPosition))
            {
                Debug.Log($"[PANEL CLICK] {elementID}");

                Interact();
            }
        }


        public abstract int GetValue();

        public abstract void Interact();

        protected abstract void RefreshVisual();
    }
}
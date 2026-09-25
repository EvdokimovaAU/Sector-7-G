using UnityEngine;
using UnityEngine.InputSystem;

namespace Panel
{
    public class PanelMouseInput : MonoBehaviour
    {
        private Camera mainCamera;


        private void Awake()
        {
            mainCamera = Camera.main;
        }


        private void Update()
        {
            if (Mouse.current == null)
                return;

            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;

            if (mainCamera == null)
            {
                mainCamera = Camera.main;

                if (mainCamera == null)
                    return;
            }


            Vector2 screenPosition =
                Mouse.current.position.ReadValue();

            Vector2 worldPosition =
                mainCamera.ScreenToWorldPoint(
                    screenPosition
                );


            // Получаем ВСЕ коллайдеры под мышкой,
            // а не только первый.
            Collider2D[] hits =
                Physics2D.OverlapPointAll(
                    worldPosition
                );


            foreach (Collider2D hit in hits)
            {
                PanelValueArrow arrow =
                    hit.GetComponent<PanelValueArrow>();

                if (arrow == null)
                    continue;


                arrow.Click();

                return;
            }
        }
    }
}
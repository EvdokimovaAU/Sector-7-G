using System.Collections;
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


        // ==================================================
        // TARGET
        // ==================================================

        [Header("Target")]
        [SerializeField]
        private PanelValueControl valueControl;


        // ==================================================
        // ACTION
        // ==================================================

        [Header("Action")]
        [SerializeField]
        private ButtonAction action;


        // ==================================================
        // APPLY VISUAL
        // ==================================================

        [Header("Apply Button Visual")]

        [Tooltip("SpriteRenderer кнопки Установить")]
        [SerializeField]
        private SpriteRenderer buttonRenderer;

        [Tooltip("Обычная картинка кнопки")]
        [SerializeField]
        private Sprite normalSprite;

        [Tooltip("Картинка нажатой кнопки")]
        [SerializeField]
        private Sprite pressedSprite;

        [Tooltip("Через сколько секунд вернуть обычную картинку")]
        [SerializeField]
        private float pressedDuration = 2f;


        private Coroutine visualCoroutine;


        // ==================================================
        // CLICK
        // ==================================================

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

                    // Только у кнопки Apply
                    // меняем картинку.
                    ShowPressedVisual();

                    break;
            }
        }


        // ==================================================
        // APPLY VISUAL
        // ==================================================

        private void ShowPressedVisual()
        {
            if (buttonRenderer == null)
                return;


            if (pressedSprite != null)
            {
                buttonRenderer.sprite =
                    pressedSprite;
            }


            // Если игрок нажал ещё раз,
            // перезапускаем отсчёт двух секунд.
            if (visualCoroutine != null)
            {
                StopCoroutine(
                    visualCoroutine
                );
            }


            visualCoroutine =
                StartCoroutine(
                    ResetVisual()
                );
        }


        private IEnumerator ResetVisual()
        {
            yield return new WaitForSeconds(
                pressedDuration
            );


            if (buttonRenderer != null &&
                normalSprite != null)
            {
                buttonRenderer.sprite =
                    normalSprite;
            }


            visualCoroutine = null;
        }
    }
}
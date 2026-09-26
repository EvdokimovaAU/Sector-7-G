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

        [Header("Target")]
        [SerializeField]
        private PanelValueControl valueControl;

        [Header("Action")]
        [SerializeField]
        private ButtonAction action;


        // ==================================================
        // VISUAL
        // ==================================================

        [Header("Visual")]

        [SerializeField]
        private SpriteRenderer buttonRenderer;

        [SerializeField]
        private Sprite normalSprite;

        [SerializeField]
        private Sprite pressedSprite;

        [Min(0f)]
        [SerializeField]
        private float pressedDuration = 2f;


        private Coroutine visualCoroutine;


        // ==================================================
        // START
        // ==================================================

        private void Start()
        {
            // Если Renderer не указан,
            // пробуем взять его с этого объекта.
            if (buttonRenderer == null)
            {
                buttonRenderer =
                    GetComponent<SpriteRenderer>();
            }

            // Если normalSprite не назначен,
            // запоминаем текущую картинку.
            if (buttonRenderer != null &&
                normalSprite == null)
            {
                normalSprite =
                    buttonRenderer.sprite;
            }
        }


        // ==================================================
        // CLICK
        // ==================================================

        private void OnMouseDown()
        {
            Debug.Log(
                $"[VALUE BUTTON CLICK] {name} -> {action}"
            );


            if (valueControl == null)
            {
                Debug.LogError(
                    $"[{name}] PanelValueControl НЕ назначен!"
                );

                return;
            }


            switch (action)
            {
                case ButtonAction.Increase:

                    Debug.Log(
                        $"[{name}] Increase"
                    );

                    valueControl.Increase();

                    break;


                case ButtonAction.Decrease:

                    Debug.Log(
                        $"[{name}] Decrease"
                    );

                    valueControl.Decrease();

                    break;


                case ButtonAction.Apply:

                    Debug.Log(
                        $"[{name}] APPLY -> " +
                        $"{valueControl.CurrentValue}"
                    );

                    // СНАЧАЛА применяем значение.
                    valueControl.Apply();

                    // И только потом меняем картинку.
                    ShowPressedVisual();

                    break;
            }
        }


        // ==================================================
        // VISUAL
        // ==================================================

        private void ShowPressedVisual()
        {
            // Визуал вообще никак не влияет
            // на работу Apply.
            if (buttonRenderer == null)
            {
                Debug.LogWarning(
                    $"[{name}] Button Renderer не назначен. " +
                    $"Apply всё равно выполнен."
                );

                return;
            }


            if (pressedSprite == null)
            {
                Debug.LogWarning(
                    $"[{name}] Pressed Sprite не назначен. " +
                    $"Apply всё равно выполнен."
                );

                return;
            }


            buttonRenderer.sprite =
                pressedSprite;


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
using System.Collections;
using UnityEngine;

namespace Panel
{
    public class PanelRotaryApplyButton : MonoBehaviour
    {
        // ==================================================
        // ROTARY SWITCH
        // ==================================================

        [Header("Rotary Switch")]

        [SerializeField]
        private PanelRotarySwitch rotarySwitch;


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
        private float pressedDuration = 1f;


        private Coroutine visualCoroutine;


        // ==================================================
        // CLICK
        // ==================================================

        private void OnMouseDown()
        {
            Apply();
        }


        // ==================================================
        // APPLY
        // ==================================================

        public void Apply()
        {
            if (rotarySwitch == null)
            {
                Debug.LogError(
                    $"[{name}] PanelRotarySwitch не назначен!"
                );

                return;
            }


            // Применяем выбранное положение крутилки.
            rotarySwitch.ApplyValue();


            // Показываем нажатие кнопки.
            ShowPressedVisual();
        }


        // ==================================================
        // VISUAL
        // ==================================================

        private void ShowPressedVisual()
        {
            if (buttonRenderer == null)
                return;


            if (pressedSprite != null)
            {
                buttonRenderer.sprite = pressedSprite;
            }


            if (visualCoroutine != null)
            {
                StopCoroutine(visualCoroutine);
            }


            visualCoroutine =
                StartCoroutine(ResetVisual());
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
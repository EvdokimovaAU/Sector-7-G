using System.Collections;
using UnityEngine;

namespace Panel
{
    public class PanelValueApplyButton : MonoBehaviour
    {
        [Header("Value Control")]
        [SerializeField]
        private PanelValueControl valueControl;

        [Header("Visual")]
        [SerializeField]
        private SpriteRenderer buttonRenderer;

        [SerializeField]
        private Sprite normalSprite;

        [SerializeField]
        private Sprite pressedSprite;

        [Min(0f)]
        [SerializeField]
        private float pressedDuration = 0.2f;

        private Coroutine visualCoroutine;


        private void Start()
        {
            if (buttonRenderer == null)
            {
                buttonRenderer = GetComponent<SpriteRenderer>();
            }

            if (buttonRenderer != null && normalSprite == null)
            {
                normalSprite = buttonRenderer.sprite;
            }
        }


        public void Click()
        {
            if (valueControl == null)
            {
                Debug.LogError(
                    $"[{name}] Не назначен PanelValueControl!"
                );

                return;
            }

            Debug.Log(
                $"[VALUE APPLY BUTTON] {valueControl.ElementID} = {valueControl.CurrentValue}"
            );

            valueControl.Apply();

            ShowPressedVisual();
        }


        private void ShowPressedVisual()
        {
            if (buttonRenderer == null || pressedSprite == null)
                return;

            buttonRenderer.sprite = pressedSprite;

            if (visualCoroutine != null)
            {
                StopCoroutine(visualCoroutine);
            }

            visualCoroutine = StartCoroutine(ResetVisual());
        }


        private IEnumerator ResetVisual()
        {
            yield return new WaitForSeconds(pressedDuration);

            if (buttonRenderer != null && normalSprite != null)
            {
                buttonRenderer.sprite = normalSprite;
            }

            visualCoroutine = null;
        }
    }
}
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
                {
                    Debug.LogError("[PanelMouseInput] Main Camera ÌÂ Ì‡È‰ÂÌ‡!");
                    return;
                }
            }


            Vector2 screenPosition =
                Mouse.current.position.ReadValue();

            Vector2 worldPosition =
                mainCamera.ScreenToWorldPoint(screenPosition);


            Collider2D[] hits =
                Physics2D.OverlapPointAll(worldPosition);


            foreach (Collider2D hit in hits)
            {
                // ==========================================
                // —“–≈ÀŒ◊ » «Õ¿◊≈Õ»ﬂ
                // ==========================================

                PanelValueArrow arrow =
                    hit.GetComponent<PanelValueArrow>();

                if (arrow != null)
                {
                    arrow.Click();
                    return;
                }


                // ==========================================
                // ”—“¿ÕŒ¬»“‹ «Õ¿◊≈Õ»≈
                // ==========================================

                PanelValueApplyButton valueApply =
                    hit.GetComponent<PanelValueApplyButton>();

                if (valueApply != null)
                {
                    valueApply.Click();
                    return;
                }


                // ==========================================
                // ”—“¿ÕŒ¬»“‹  –”“»À ”
                // ==========================================

                PanelRotaryApplyButton rotaryApply =
                    hit.GetComponent<PanelRotaryApplyButton>();

                if (rotaryApply != null)
                {
                    rotaryApply.Click();
                    return;
                }
            }
        }
    }
}
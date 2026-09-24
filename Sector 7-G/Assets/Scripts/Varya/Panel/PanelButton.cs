using System.Collections;
using UnityEngine;

namespace Panel
{
    public class PanelButton : PanelElement
    {
        // ==================================================
        // BUTTON SETTINGS
        // ==================================================

        [Header("Button Settings")]

        [Tooltip("Если true — кнопка переключается между 0 и 1.")]
        [SerializeField]
        private bool isTwoState = true;

        [SerializeField]
        private bool isOn = false;


        // ==================================================
        // BUTTON VISUAL
        // ==================================================

        [Header("Button Visual")]

        [SerializeField]
        private SpriteRenderer targetRenderer;

        [SerializeField]
        private Sprite spriteNormal;

        [SerializeField]
        private Sprite spritePressed;


        // ==================================================
        // INDICATOR
        // ==================================================

        [Header("Indicator")]

        [SerializeField]
        private SpriteRenderer indicatorRenderer;

        [SerializeField]
        private Sprite indicatorOffSprite;

        [SerializeField]
        private Sprite indicatorOnSprite;


        // ==================================================
        // AUTO RESET
        // ==================================================

        [Header("Auto Reset")]

        [SerializeField]
        private bool autoReset = false;

        [SerializeField]
        private float autoResetDelay = 0.2f;


        // ==================================================
        // INSTABILITY
        // ==================================================

        [Header("Instability")]

        [SerializeField]
        private PanelInstabilityManager instabilityManager;


        private Coroutine resetCoroutine;


        // ==================================================
        // UNITY
        // ==================================================

        private void Start()
        {
            if (instabilityManager == null)
            {
                instabilityManager =
                    FindAnyObjectByType<PanelInstabilityManager>();
            }

            RefreshVisual();
        }


        private void OnDisable()
        {
            StopAutoReset();
        }


        // ==================================================
        // INTERACTION
        // ==================================================

        public override void Interact()
        {
            Debug.Log(
                $"[PANEL CLICK] {elementID}"
            );


            // ==================================================
            // ДВУХПОЗИЦИОННАЯ КНОПКА
            // ==================================================

            if (isTwoState)
            {
                isOn = !isOn;

                RefreshVisual();

                SendAction();

                return;
            }


            // ==================================================
            // ОБЫЧНАЯ КНОПКА
            // ==================================================

            isOn = true;

            RefreshVisual();

            SendAction();


            if (autoReset)
            {
                StartAutoReset();
            }
        }


        // ==================================================
        // GET VALUE
        //
        // ЭТО ABSTRACT-МЕТОД ИЗ PanelElement
        // ==================================================

        public override int GetValue()
        {
            if (isTwoState)
            {
                return isOn ? 1 : 0;
            }


            // Обычная кнопка при нажатии
            // всегда отправляет 1.
            return 1;
        }


        // ==================================================
        // REFRESH VISUAL
        //
        // ЭТО ABSTRACT-МЕТОД ИЗ PanelElement
        // ==================================================

        protected override void RefreshVisual()
        {
            // ------------------------------
            // Картинка самой кнопки
            // ------------------------------

            if (targetRenderer != null)
            {
                if (isOn)
                {
                    if (spritePressed != null)
                    {
                        targetRenderer.sprite =
                            spritePressed;
                    }
                }
                else
                {
                    if (spriteNormal != null)
                    {
                        targetRenderer.sprite =
                            spriteNormal;
                    }
                }
            }


            // ------------------------------
            // Лампочка / индикатор
            // ------------------------------

            if (indicatorRenderer != null)
            {
                if (isOn)
                {
                    if (indicatorOnSprite != null)
                    {
                        indicatorRenderer.sprite =
                            indicatorOnSprite;
                    }
                }
                else
                {
                    if (indicatorOffSprite != null)
                    {
                        indicatorRenderer.sprite =
                            indicatorOffSprite;
                    }
                }
            }
        }


        // ==================================================
        // SEND ACTION
        // ==================================================

        private void SendAction()
        {
            int value =
                GetValue();


            PanelElementID actualID =
                GetActualElementID();


            Debug.Log(
                $"[PANEL BUTTON] " +
                $"Pressed: {elementID} | " +
                $"Actual action: {actualID} | " +
                $"Value: {value}"
            );


            PanelEvents.ElementChanged(
                actualID,
                value
            );
        }


        // ==================================================
        // INSTABILITY
        // ==================================================

        private PanelElementID GetActualElementID()
        {
            // Если менеджера нет —
            // всё работает как обычно.

            if (instabilityManager == null)
            {
                return elementID;
            }


            PanelElementID actualID =
                elementID;


            // ==================================================
            // ОСНОВНЫЕ ПОЛОМКИ
            //
            // < 80:
            // Generator
            // Pump
            // ReactorSection
            //
            // < 60:
            // Reset <-> WaterSupply
            //
            // EmergencyShutdown и ReactorMode
            // менеджер не изменяет.
            // ==================================================

            actualID =
                instabilityManager
                    .GetActualButtonID(actualID);


            // ==================================================
            // TURBINE
            //
            // < 60:
            //
            // Turbine1 <-> Turbine1_Stop
            // Turbine2 <-> Turbine2_Stop
            // ==================================================

            actualID =
                instabilityManager
                    .GetActualTurbineID(actualID);


            return actualID;
        }


        // ==================================================
        // SET STATE
        // ==================================================

        public void SetState(
            bool newState,
            bool sendEvent = false)
        {
            isOn = newState;

            RefreshVisual();


            if (!sendEvent)
            {
                return;
            }


            SendAction();
        }


        // ==================================================
        // AUTO RESET
        // ==================================================

        private void StartAutoReset()
        {
            StopAutoReset();


            resetCoroutine =
                StartCoroutine(
                    AutoResetRoutine()
                );
        }


        private void StopAutoReset()
        {
            if (resetCoroutine == null)
            {
                return;
            }


            StopCoroutine(
                resetCoroutine
            );

            resetCoroutine = null;
        }


        private IEnumerator AutoResetRoutine()
        {
            yield return new WaitForSeconds(
                autoResetDelay
            );


            isOn = false;

            RefreshVisual();


            // ВАЖНО:
            // PanelEvents здесь НЕ вызываем.
            //
            // Иначе после нажатия 1
            // через 0.2 сек игра получит ещё и 0.


            resetCoroutine = null;
        }
    }
}
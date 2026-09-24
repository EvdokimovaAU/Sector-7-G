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

        // true = кнопка имеет два состояния: ВКЛ / ВЫКЛ.
        // false = обычная командная кнопка.
        [SerializeField]
        private bool isTwoState = true;

        // Текущее состояние кнопки.
        [SerializeField]
        private bool isOn;


        // ==================================================
        // AUTO RESET
        // ==================================================

        [Header("Auto Reset")]

        // Если включено, кнопка автоматически
        // вернётся в состояние OFF.
        [SerializeField]
        private bool autoReset = false;

        // Через сколько секунд вернуть кнопку в OFF.
        [Min(0f)]
        [SerializeField]
        private float autoResetDelay = 2f;


        // ==================================================
        // BUTTON VISUAL
        // ==================================================

        [Header("Button Visual")]

        // SpriteRenderer самой кнопки.
        [SerializeField]
        private SpriteRenderer targetRenderer;

        // Спрайт обычной кнопки.
        [SerializeField]
        private Sprite spriteNormal;

        // Спрайт нажатой кнопки.
        [SerializeField]
        private Sprite spritePressed;


        // ==================================================
        // INDICATOR
        // ==================================================

        [Header("Indicator Lamp")]

        // SpriteRenderer отдельной лампочки.
        [SerializeField]
        private SpriteRenderer indicatorRenderer;

        // Лампочка, когда кнопка выключена.
        [SerializeField]
        private Sprite indicatorOffSprite;

        // Лампочка, когда кнопка включена.
        [SerializeField]
        private Sprite indicatorOnSprite;


        // Запущенная корутина автоматического сброса.
        private Coroutine resetCoroutine;


        // ==================================================
        // VALUE
        // ==================================================

        public override int GetValue()
        {
            return isTwoState
                ? (isOn ? 1 : 0)
                : 1;
        }


        // ==================================================
        // INTERACTION
        // ==================================================

        public override void Interact()
        {
            // --------------------------------------------------
            // TWO STATE BUTTON
            // --------------------------------------------------

            if (isTwoState)
            {
                // Меняем состояние.
                isOn = !isOn;

                RefreshVisual();


                // Сообщаем системам именно о действии игрока.
                PanelEvents.ElementChanged(
                    elementID,
                    GetValue()
                );


                // Если кнопку включили и для неё
                // разрешён автоматический возврат.
                if (isOn && autoReset)
                {
                    StartAutoReset();
                }
                else
                {
                    StopAutoReset();
                }

                return;
            }


            // --------------------------------------------------
            // COMMAND BUTTON
            // --------------------------------------------------

            // Обычная командная кнопка отправляет 1.
            PanelEvents.ElementChanged(
                elementID,
                1
            );


            // Для командной кнопки можем визуально
            // показать нажатие.
            if (autoReset)
            {
                isOn = true;

                RefreshVisual();

                StartAutoReset();
            }
        }


        // ==================================================
        // AUTO RESET
        // ==================================================

        private void StartAutoReset()
        {
            // Если старый таймер ещё работает,
            // сначала останавливаем его.
            StopAutoReset();


            resetCoroutine =
                StartCoroutine(AutoResetRoutine());
        }


        private void StopAutoReset()
        {
            if (resetCoroutine == null)
                return;


            StopCoroutine(resetCoroutine);

            resetCoroutine = null;
        }


        private IEnumerator AutoResetRoutine()
        {
            yield return new WaitForSeconds(
                autoResetDelay
            );


            // Возвращаем кнопку в обычное состояние.
            isOn = false;


            RefreshVisual();


            resetCoroutine = null;


            // ВАЖНО:
            //
            // PanelEvents.ElementChanged здесь
            // специально НЕ вызывается.
            //
            // Это автоматическое изменение,
            // а не действие игрока.
            // Поэтому TaskManager и система штрафов
            // его не обрабатывают.

            Debug.Log(
                $"[PANEL AUTO RESET] {elementID}"
            );
        }


        // ==================================================
        // VISUAL
        // ==================================================

        protected override void RefreshVisual()
        {
            // Кнопка.
            if (targetRenderer != null)
            {
                targetRenderer.sprite =
                    isOn
                        ? spritePressed
                        : spriteNormal;
            }


            // Лампочка.
            if (indicatorRenderer != null)
            {
                indicatorRenderer.sprite =
                    isOn
                        ? indicatorOnSprite
                        : indicatorOffSprite;
            }
        }


        // ==================================================
        // START
        // ==================================================

        private void Start()
        {
            if (targetRenderer == null)
            {
                targetRenderer =
                    GetComponent<SpriteRenderer>();
            }


            RefreshVisual();
        }


        private void OnDisable()
        {
            StopAutoReset();
        }
    }
}
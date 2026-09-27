using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class PhoneController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private TMP_Text responseText;
        [SerializeField] private Button callButton;
        [SerializeField] private Button clearButton;

        [Header("Sector → Phone")]
        [SerializeField] private List<PhoneEntry> phoneBook = new List<PhoneEntry>();

        [Header("Response Lines")]
        [SerializeField] private List<string> responseLines = new List<string>();

        [Header("Call Limit")]
        [SerializeField] private bool callUsed = false;

        [Header("Call Button Flash")]
        [SerializeField] private Image callButtonImage;
        [SerializeField] private Sprite callNormalSprite;
        [SerializeField] private Sprite callGreenSprite;
        [SerializeField] private Sprite callRedSprite;
        [SerializeField] private int flashCount = 3;
        [SerializeField] private float flashInterval = 0.15f;

        private string _currentNumber = "";
        private Coroutine _flashRoutine;

        private void Awake()
        {
            if (callButton != null) callButton.onClick.AddListener(Call);
            if (clearButton != null) clearButton.onClick.AddListener(Clear);

            if (callButtonImage != null && callNormalSprite == null)
                callNormalSprite = callButtonImage.sprite;
        }

        private void Start()
        {
            UpdateNumberUI();
            if (responseText != null) responseText.text = "";
        }

        public void AddDigit(string digit)
        {
            if (digit.Length != 1 || !char.IsDigit(digit[0])) return;
            if (_currentNumber.Length >= 10) return;
            _currentNumber += digit;
            UpdateNumberUI();
        }

        public void Clear()
        {
            _currentNumber = "";
            if (responseText != null) responseText.text = "";
            UpdateNumberUI();

            AudioManager.Instance?.PlayClick();
        }

        public void Call()
        {
            // Звонок уже был использован
            if (callUsed)
            {
                if (responseText != null)
                    responseText.text = "Звонок уже использован";

                FlashCallButton(false);
                AudioManager.Instance?.PlayCallFail();

                return;
            }

            // Номер не введён
            if (string.IsNullOrEmpty(_currentNumber))
                return;


            // Ищем номер в телефонной книге
            var entry = phoneBook.Find(
                e => e.phoneNumber == _currentNumber
            );


            // Такого номера нет
            if (entry == null)
            {
                if (responseText != null)
                    responseText.text = "Нет такого номера";

                FlashCallButton(false);
                AudioManager.Instance?.PlayCallFail();

                _currentNumber = "";
                UpdateNumberUI();

                return;
            }


            // Очищаем старый ответ ПЕРЕД событием.
            // После события TaskTriggeredEmergency сам запишет
            // нужный текст через ShowResponse().
            if (responseText != null)
                responseText.text = "";


            // Сообщаем системе:
            // игрок позвонил в конкретный цех.
            StationEvents.SectorCalled?.Invoke(
                entry.sector
            );


            // ВАЖНО:
            // Здесь больше НЕ вызываем GetRandomResponse().
            //
            // TaskTriggeredEmergency сам решит:
            //
            // правильный цех:
            // "Тревога подтверждена..."
            //
            // неправильный цех:
            // "В этом цехе всё в норме..."


            FlashCallButton(true);

            AudioManager.Instance?.PlayCallSuccess();


            // Звонок использован
            callUsed = true;


            // Очищаем набранный номер
            _currentNumber = "";

            UpdateNumberUI();
        }

        public void ResetCall()
        {
            callUsed = false;
            _currentNumber = "";
            if (responseText != null) responseText.text = "";
            UpdateNumberUI();
        }

        public void ShowResponse(string text)
        {
            if (responseText != null)
                responseText.text = text;
        }

        private string GetRandomResponse()
        {
            if (responseLines == null || responseLines.Count == 0)
                return "Данные переданы";
            return responseLines[Random.Range(0, responseLines.Count)];
        }

        private void UpdateNumberUI()
        {
            if (numberText != null)
                numberText.text = _currentNumber;
        }

        private void FlashCallButton(bool success)
        {
            if (callButtonImage == null) return;

            if (_flashRoutine != null)
                StopCoroutine(_flashRoutine);

            _flashRoutine = StartCoroutine(FlashRoutine(success));
        }

        private IEnumerator FlashRoutine(bool success)
        {
            Sprite flashSprite = success ? callGreenSprite : callRedSprite;

            for (int i = 0; i < flashCount; i++)
            {
                callButtonImage.sprite = flashSprite;
                yield return new WaitForSeconds(flashInterval);

                callButtonImage.sprite = callNormalSprite;
                yield return new WaitForSeconds(flashInterval);
            }

            callButtonImage.sprite = callNormalSprite;
            _flashRoutine = null;
        }
    }
}
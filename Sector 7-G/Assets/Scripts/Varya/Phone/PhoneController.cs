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

        private string _currentNumber = "";

        private void Awake()
        {
            if (callButton != null) callButton.onClick.AddListener(Call);
            if (clearButton != null) clearButton.onClick.AddListener(Clear);
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
        }

        public void Call()
        {
            if (callUsed)
            {
                if (responseText != null)
                    responseText.text = "Звонок уже использован";
                return;
            }

            if (string.IsNullOrEmpty(_currentNumber)) return;

            var entry = phoneBook.Find(e => e.phoneNumber == _currentNumber);
            if (entry == null)
            {
                if (responseText != null)
                    responseText.text = "Нет такого номера";

                _currentNumber = "";
                UpdateNumberUI();
                return;
            }

            StationEvents.SectorCalled?.Invoke(entry.sector);

            if (responseText != null)
                responseText.text = GetRandomResponse();

            callUsed = true;
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
    }
}
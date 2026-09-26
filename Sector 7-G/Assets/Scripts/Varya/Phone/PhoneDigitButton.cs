using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    [RequireComponent(typeof(Button))]
    public class PhoneDigitButton : MonoBehaviour
    {
        [SerializeField] private string digit;
        [SerializeField] private PhoneController controller;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                controller.AddDigit(digit);
                AudioManager.Instance?.PlayDigit();
            });
        }
    }
}
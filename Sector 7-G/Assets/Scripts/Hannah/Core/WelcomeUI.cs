using UnityEngine;

public class WelcomeUI : MonoBehaviour
{
    [Header("Welcome")]
    [SerializeField] private GameObject welcomeBlocker;


    private void Awake()
    {
        // Сразу останавливаем игровую логику
        Time.timeScale = 0f;
    }


    private void Start()
    {
        if (welcomeBlocker == null)
        {
            Debug.LogError(
                "[WelcomeUI] Welcome Blocker не назначен!"
            );

            return;
        }

        // Показываем приветствие вместе
        // с блокирующей подложкой
        welcomeBlocker.SetActive(true);
    }


    public void CloseWelcome()
    {
        if (welcomeBlocker != null)
        {
            welcomeBlocker.SetActive(false);
        }

        // Только теперь запускаем игру
        Time.timeScale = 1f;

        Debug.Log("[WelcomeUI] Смена началась.");
    }


    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
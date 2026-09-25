using UnityEngine;

    public class GameOverListener : MonoBehaviour
    {
        [Header("GameState")]
        [SerializeField] private GameState gameState;

        [Header("Threshold")]
        [Tooltip("Если любая шкала упадёт на это значение или ниже — проигрыш.")]
        [SerializeField] private int threshold = 20;

        [Header("UI")]
        [SerializeField] private GameObject gameOverPanel;

        [Header("Time")]
        [SerializeField] private bool freezeTime = true;

        private bool _gameOver;

        private void OnEnable()
        {
            if (gameState == null) return;

            gameState.OnStationStabilityChanged += OnStabilityChanged;
            gameState.OnPanelReliabilityChanged += OnReliabilityChanged;
        }

        private void Start()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            CheckThreshold(gameState.StationStability);
            CheckThreshold(gameState.PanelReliability);
        }

        private void OnDisable()
        {
            if (gameState == null) return;

            gameState.OnStationStabilityChanged -= OnStabilityChanged;
            gameState.OnPanelReliabilityChanged -= OnReliabilityChanged;
        }

        private void OnStabilityChanged(int value) => CheckThreshold(value);
        private void OnReliabilityChanged(int value) => CheckThreshold(value);

        private void CheckThreshold(int value)
        {
            if (_gameOver) return;
            if (value > threshold) return;

            TriggerGameOver();
        }

        private void TriggerGameOver()
        {
            _gameOver = true;

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            if (freezeTime)
                Time.timeScale = 0f;

            Debug.Log($"[GameOver] Проигрыш: шкала упала до ≤{threshold}%");
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            _gameOver = false;
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

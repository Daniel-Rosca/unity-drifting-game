using Content.Scripts.Controller;
using Content.Scripts.Data;
using Content.Scripts.Manager;
using Content.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Mathf;

namespace Content.Scripts.View
{
    public class GameLevelView : MonoBehaviour
    {
        [SerializeField] private float timerDuration = 120f; // 2 minutes
        [SerializeField] private GameObject gameOverPopup;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private CarController carController;
        [SerializeField] private ScoreManager scoreManager;

        private float _currentTimer;
        private bool _timerActive = true;
        private PlayerData _playerData;

        private void Start()
        {
            LoadPlayerData();
            _currentTimer = timerDuration;
            gameOverPopup.SetActive(false);
        }

        private void Update()
        {
            if (_timerActive)
            {
                UpdateTimer();
            }
        }
        
        private void LoadPlayerData()
        {
            _playerData = SaveSystem.LoadPlayerData();
            
            if (_playerData.selectedCarData == null)
            {
                var data = Resources.Load<CarData>("YellowCar");
                _playerData.selectedCarData = new SerializableCarData(data.serializableData.baseSpeed, data.serializableData.baseAcceleration, data.serializableData.carName);
            }
            
            LoadUpgrades(_playerData.selectedCarData);

            if (_playerData == null)
            {
                _playerData = new PlayerData
                {
                    cash = 5000f
                };
            }
        }

        private void LoadUpgrades(SerializableCarData carData)
        {
            carController.SetCarData(carData);
            carController.ApplyUpgrades(_playerData.selectedCarData.upgradedSpeed, _playerData.selectedCarData.upgradedAcceleration);
        }

        private void UpdateTimer()
        {
            _currentTimer -= Time.deltaTime;

            if (_currentTimer <= 0f)
            {
                _currentTimer = 0f;
                _timerActive = false;
                ShowPopup();
            }

            UpdateTimerDisplay();
        }

        private void UpdateTimerDisplay()
        {
            var minutes = FloorToInt(_currentTimer / 60f);
            var seconds = FloorToInt(_currentTimer % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        private void ShowPopup()
        {
            gameOverPopup.SetActive(true);
            carController.IsInputEnabled = false;
            scoreManager.EndGame();
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void DoubleReward()
        {
        }
    }
}
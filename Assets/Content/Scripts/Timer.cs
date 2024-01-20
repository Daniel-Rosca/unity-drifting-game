using Content.Scripts.Controller;
using TMPro;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Content.Scripts
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float timerDuration = 120f; // 2 minutes
        [SerializeField] private GameObject gameOverPopup;
        [SerializeField] private TextMeshProUGUI timerText;

        private float currentTimer;
        private bool timerActive = true;

        private void Start()
        {
            currentTimer = timerDuration;
            gameOverPopup.SetActive(false);
        }

        private void Update()
        {
            if (timerActive)
            {
                UpdateTimer();
            }
        }

        private void UpdateTimer()
        {
            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0f)
            {
                currentTimer = 0f;
                timerActive = false;
                ShowPopup();
            }

            UpdateTimerDisplay();
        }

        private void UpdateTimerDisplay()
        {
            var minutes = FloorToInt(currentTimer / 60f);
            var seconds = FloorToInt(currentTimer % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        private void ShowPopup()
        {
            gameOverPopup.SetActive(true);
        }

        public void ReturnToMainMenu()
        {
            
        }

        public void DoubleReward()
        {
            
        }
    }
}
using Content.Scripts.Data;
using Content.Scripts.Utils;
using TMPro;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Content.Scripts.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        private float _scorePoints;
        private PlayerData _playerData;

        [SerializeField] private TextMeshProUGUI driftingScoreText;
        [SerializeField] private TextMeshProUGUI cashText;

        private void Start()
        {
            LoadOrCreatePlayerData();
        }

        private void LoadOrCreatePlayerData()
        {
            if (_playerData == null)
            {
                _playerData = new PlayerData();
                SaveSystem.SavePlayerData(_playerData);
            }
            else
            {
                _playerData = SaveSystem.LoadPlayerData();
            }
        }

        public void AddDriftingScore(float score)
        {
            _scorePoints += score;
            driftingScoreText.text = $"Score: {_scorePoints:F1}";
        }

        public void EndGame()
        {
            var cashAmount = FloorToInt(_scorePoints / 2f);
            cashText.text = $"Cash: ${cashAmount.ToString()}";

            _playerData.cash += cashAmount;

            SaveSystem.SavePlayerData(_playerData);
        }
    }
}

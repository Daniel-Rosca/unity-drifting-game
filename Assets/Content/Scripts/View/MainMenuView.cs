using System.Globalization;
using Content.Scripts.Data;
using Content.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Content.Scripts.View
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private GameObject levelSelectorPopup;
        
        private PlayerData _playerData;

        private void Start()
        {
            LoadOrCreatePlayerData();
            cashText.text = $"Cash: ${_playerData.cash}";
        }
        
        private void LoadOrCreatePlayerData()
        {
            _playerData = SaveSystem.LoadPlayerData();

            if (_playerData != null) return;
            _playerData = new PlayerData();
            SaveSystem.SavePlayerData(_playerData);
        }

        public void OnPlayButtonClick()
        {
            levelSelectorPopup.SetActive(true);
        }

        public void OnSettingsButtonClick()
        {
            //SceneManager.LoadScene("");
        }

        public void OnGarageButtonClick()
        {
            //SceneManager.LoadScene("");
        }

        public void OnBackButtonCLick()
        {
            levelSelectorPopup.SetActive(false);
        }

        public void OnLevelSelectButtonCLick(string level)
        {
            SceneManager.LoadScene(level);
        }
    }
}
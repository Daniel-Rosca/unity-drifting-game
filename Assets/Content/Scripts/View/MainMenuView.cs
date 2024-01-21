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
        [SerializeField] private GameObject level1PopupHost;
        [SerializeField] private GameObject level2PopupHost;
        [SerializeField] private GameObject level3PopupHost;
        
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
            _playerData = new PlayerData
            {
                cash = 5000f
            };
            SaveSystem.SavePlayerData(_playerData);
        }

        public void OnPlayButtonClick()
        {
            levelSelectorPopup.SetActive(true);
        }

        public void OnGarageButtonClick()
        {
            SceneManager.LoadScene("Garage");
        }

        public void OnLevelSelectorBackButtonClick()
        {
            levelSelectorPopup.SetActive(false);
        }

        public void OnBackButtonCLick()
        {
            level1PopupHost.SetActive(false);
            level2PopupHost.SetActive(false);
            level3PopupHost.SetActive(false);
        }

        public void OnQuitButtonClick()
        {
            Application.Quit();
        }

        public void OnLevelSelectButtonCLick(string level)
        {
            switch (level)
            {
                case "Level1":
                    level1PopupHost.SetActive(true);
                    break;
                case "Level2":
                    level2PopupHost.SetActive(true);
                    break;
                case "Level3":
                    level3PopupHost.SetActive(true);
                    break;
            }

            //SceneManager.LoadScene(level);
        }
    }
}
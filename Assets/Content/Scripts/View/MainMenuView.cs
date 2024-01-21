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
        [SerializeField] private GameObject levelPopupHost;
        
        private PlayerData _playerData;

        private void Start()
        {
            LoadOrCreatePlayerData();
            levelSelectorPopup.SetActive(false);
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
            levelPopupHost.SetActive(true);
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
            levelPopupHost.SetActive(false);
        }

        public void OnQuitButtonClick()
        {
            Application.Quit();
        }

        public void OnLevelSelectButtonCLick(string level)
        {
            levelSelectorPopup.SetActive(false);

            SceneManager.LoadScene(level);
        }
    }
}
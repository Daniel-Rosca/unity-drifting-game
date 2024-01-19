using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Content.Scripts.View
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private GameObject levelSelectorPopup;

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
using Content.Scripts.Controller;
using Content.Scripts.Data;
using Content.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Content.Scripts.FlowManagement
{
    public class Garage : MonoBehaviour
    {
        [SerializeField] private CarController carObject;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI accelerationText;
        [SerializeField] private TextMeshProUGUI carInfoText;
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private TextMeshProUGUI speedCostText;
        [SerializeField] private TextMeshProUGUI accelerationCostText;
        
        [Header("Upgrade Cost")]
        [SerializeField] private float speedUpgradePrice;
        [SerializeField] private float accelerationUpgradePrice;

        private PlayerData _playerData;
        private CarController _currentCar;

        private void Start()
        {
            LoadPlayerData();
            SelectInitialCar();
            UpdateUI();
        }
        
        public void UpgradeSpeed()
        {
            if (_currentCar != null && CanUpgradeCar("Speed"))
            {
                _playerData.selectedCarData.upgradedSpeed += 2f;

                DeductUpgradeCost("Speed");

                SavePlayerData();
                UpdateUI();
            }
        }

        public void UpgradeAcceleration()
        {
            if (_currentCar != null && CanUpgradeCar("Acceleration"))
            {
                _playerData.selectedCarData.upgradedAcceleration += 1f;

                DeductUpgradeCost("Acceleration");

                SavePlayerData();
                UpdateUI();
            }
        }

        public void OnBackToMainMenuButtonClick()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private void SelectInitialCar()
        {
            if (_playerData.selectedCarData == null)
            {
                var data = Resources.Load<CarData>("YellowCar");
                _playerData.selectedCarData = new SerializableCarData(data.serializableData.baseSpeed, data.serializableData.baseAcceleration, data.serializableData.carName);
            }

            SpawnCar(_playerData.selectedCarData);
        }

        private void SpawnCar(SerializableCarData carData)
        {
            if (carObject != null)
            {
                var newCar = Instantiate(carObject, Vector3.zero, Quaternion.identity);
                newCar.SetCarData(carData);
                _currentCar = newCar;
                _currentCar.ApplyUpgrades(_playerData.selectedCarData.upgradedSpeed, _playerData.selectedCarData.upgradedAcceleration);

                UpdateUI();
            }
            else
            {
                Debug.LogError($"Selected car {carData.carName} is not available in the garage.");
            }
        }

        private void LoadPlayerData()
        {
            _playerData = SaveSystem.LoadPlayerData();

            if (_playerData == null)
            {
                _playerData = new PlayerData
                {
                    cash = 0f
                };
            }
        }

        private void UpdateUI()
        {
            if (_currentCar != null)
            {
                var carData = _playerData.selectedCarData;
                var totalSpeed = carData.baseSpeed + carData.upgradedSpeed;
                var totalAcceleration = carData.baseAcceleration + carData.upgradedAcceleration;

                speedText.text = "Speed: " + totalSpeed.ToString("F1");
                accelerationText.text = "Acceleration: " + totalAcceleration.ToString("F1");
                carInfoText.text = "Selected Car: " + (carData != null ? carData.carName : "None");
                cashText.text = $"Cash: ${_playerData.cash}";
                speedCostText.text = $"${speedUpgradePrice}";
                accelerationCostText.text = $"${accelerationUpgradePrice}";
            }
            else
            {
                Debug.LogError("No car available in the garage.");
            }
        }
        
        private bool CanUpgradeCar(string upgradeType)
        {
            var upgradeCost = GetUpgradeCost(upgradeType);

            if (_playerData.cash >= upgradeCost)
            {
                return true;
            }

            Debug.LogError("Not enough cash to upgrade!");
            return false;
        }

        private float GetUpgradeCost(string upgradeType)
        {
            return upgradeType switch
            {
                "Speed" => speedUpgradePrice,
                "Acceleration" => accelerationUpgradePrice,
                _ => 0f
            };
        }
        
        private void DeductUpgradeCost(string upgradeType)
        {
            var upgradeCost = GetUpgradeCost(upgradeType);
            _playerData.cash -= upgradeCost;
        }

        private void SavePlayerData()
        {
            SaveSystem.SavePlayerData(_playerData);
        }
    }
}

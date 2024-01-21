using System.Collections.Generic;
using Content.Scripts.Controller;
using Content.Scripts.Data;
using Content.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Content.Scripts.FlowManagement
{
    public class Garage : MonoBehaviour
    {
        [SerializeField] private List<CarController> availableCars = new();
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI accelerationText;
        [SerializeField] private TextMeshProUGUI carInfoText;
        [SerializeField] private TextMeshProUGUI cashText;

        private PlayerData _playerData;
        private CarData _selectedCarData;
        private CarController _currentCar;

        private void Start()
        {
            LoadPlayerData();
            InitializeAvailableCars();
            SelectInitialCar();
            UpdateUI();
        }
        
        public void UpgradeSpeed()
        {
            if (_currentCar != null && CanUpgradeCar("Speed"))
            {
                _selectedCarData.serializableData.upgradedSpeed += 2f;

                DeductUpgradeCost("Speed");

                SavePlayerData();
                UpdateUI();
            }
        }

        public void UpgradeAcceleration()
        {
            if (_currentCar != null && CanUpgradeCar("Acceleration"))
            {
                _selectedCarData.serializableData.upgradedAcceleration += 1f;

                DeductUpgradeCost("Acceleration");

                SavePlayerData();
                UpdateUI();
            }
        }

        private void SpawnNewCar(string carName)
        {
            var newCarData = Resources.Load<CarData>(carName);

            if (newCarData != null)
            {
                SpawnCar(newCarData);
            }
            else
            {
                Debug.LogError($"CarData for {carName} not found.");
            }
        }

        private void InitializeAvailableCars()
        {
            var cars = Resources.LoadAll<CarController>("Cars");

            foreach (var car in cars)
            {
                availableCars.Add(car);
            }
        }

        private void SelectInitialCar()
        {
            if (_playerData.selectedCarData != null)
            {
                _selectedCarData = ScriptableObject.CreateInstance<CarData>();
                _selectedCarData.Initialize(_playerData.selectedCarData.baseSpeed,
                    _playerData.selectedCarData.baseAcceleration, _playerData.selectedCarData.carName);
            }
            else
            {
                _selectedCarData = Resources.Load<CarData>("YellowCar");
            }

            SpawnCar(_selectedCarData);
        }

        private void SpawnCar(CarData carData)
        {
            DestroyCurrentCar();

            var selectedCar = availableCars.Find(car => car.CarData == carData);

            if (selectedCar != null)
            {
                var newCar = Instantiate(selectedCar, Vector3.zero, Quaternion.identity);
                newCar.SetCarData(carData);
                _currentCar = newCar;
                _currentCar.ApplyUpgrades(_selectedCarData.serializableData.upgradedSpeed, _selectedCarData.serializableData.upgradedAcceleration);

                _selectedCarData = carData;

                UpdateUI();
            }
            else
            {
                Debug.LogError($"Selected car {carData.name} is not available in the garage.");
            }
        }

        private void DestroyCurrentCar()
        {
            if (_currentCar != null)
            {
                Destroy(_currentCar.gameObject);
                _currentCar = null;
            }
        }

        private void LoadPlayerData()
        {
            _playerData = SaveSystem.LoadPlayerData();

            if (_playerData == null)
            {
                _playerData = new PlayerData
                {
                    cash = 5000f
                };
            }
        }

        private void UpdateUI()
        {
            if (_currentCar != null)
            {
                float totalSpeed = _selectedCarData.serializableData.baseSpeed + _selectedCarData.serializableData.upgradedSpeed;
                float totalAcceleration = _selectedCarData.serializableData.baseAcceleration + _selectedCarData.serializableData.upgradedAcceleration;

                speedText.text = "Speed: " + totalSpeed.ToString("F1");
                accelerationText.text = "Acceleration: " + totalAcceleration.ToString("F1");
                carInfoText.text = "Selected Car: " + (_selectedCarData != null ? _selectedCarData.name : "None");
                cashText.text = $"Cash: ${_playerData.cash}";
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

        private static float GetUpgradeCost(string upgradeType)
        {
            return upgradeType switch
            {
                "Speed" => 30f,
                "Acceleration" => 50f,
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

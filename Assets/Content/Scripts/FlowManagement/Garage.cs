using System.Collections.Generic;
using Content.Scripts.Controller;
using Content.Scripts.Data;
using Content.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Content.Scripts.FlowManagement
{
    public class Garage : MonoBehaviour
    {
        public TextMeshProUGUI speedText;
        public TextMeshProUGUI accelerationText;
        public TextMeshProUGUI carInfoText;
        public TextMeshProUGUI cashText;

        private PlayerData _playerData;
        private CarData _selectedCarData;
        [SerializeField] public List<CarController> availableCars = new();
        private CarController _currentCar;

        private void Start()
        {
            LoadPlayerData();
            InitializeAvailableCars();
            SelectInitialCar();
            UpdateUI();
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
                    _playerData.selectedCarData.baseAcceleration);
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
                speedText.text = "Speed: " + _currentCar.CurrentSpeed.ToString("F1");
                accelerationText.text = "Acceleration: " + _currentCar.CurrentAcceleration.ToString("F1");
                carInfoText.text = "Selected Car: " + (_selectedCarData != null ? _selectedCarData.name : "None");
                cashText.text = $"Cash: ${_playerData.cash}";
            }
            else
            {
                Debug.LogError("No car available in the garage.");
            }
        }

        public void UpgradeSpeed()
        {
            if (_currentCar != null && CanUpgradeCar("Speed"))
            {
                _currentCar.ApplySpeedUpgrade(2f);
                SavePlayerData("Speed");
                UpdateUI();
            }
        }

        public void UpgradeAcceleration()
        {
            if (_currentCar != null && CanUpgradeCar("Acceleration"))
            {
                _currentCar.ApplyAccelerationUpgrade(1f);
                SavePlayerData("Acceleration");
                UpdateUI();
            }
        }

        public void SpawnNewCar(string carName)
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

        private void SavePlayerData(string upgradeType)
        {
            var upgradeCost = GetUpgradeCost(upgradeType);

            _playerData.cash -= upgradeCost;

            if (_currentCar != null)
            {
                _playerData.selectedCarData.baseSpeed = _currentCar.CarData.serializableData.baseSpeed;
                _playerData.selectedCarData.baseAcceleration = _currentCar.CarData.serializableData.baseAcceleration;
            }

            SaveSystem.SavePlayerData(_playerData);
        }
    }
}

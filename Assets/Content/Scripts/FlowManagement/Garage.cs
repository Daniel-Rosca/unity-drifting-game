using Content.Scripts.Controller;
using Content.Scripts.Data;
using Content.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Content.Scripts.FlowManagement
{
    public class Garage : MonoBehaviour
    {
        public CarController carController;
        public TextMeshProUGUI speedText;
        public TextMeshProUGUI accelerationText;

        private PlayerData _playerData;

        private void Start()
        {
            LoadPlayerData();
            UpdateUI();
        }

        private void LoadPlayerData()
        {
            _playerData = SaveSystem.LoadPlayerData();
            
            if (_playerData == null) return;
            var selectedCar = ScriptableObject.CreateInstance<CarData>();
            selectedCar.Initialize(_playerData.selectedCarData.baseSpeed, _playerData.selectedCarData.baseAcceleration);
            carController.SetCarData(selectedCar);
        }

        private void UpdateUI()
        {
            speedText.text = "Speed: " + carController.CurrentSpeed.ToString("F1");
            accelerationText.text = "Acceleration: " + carController.CurrentAcceleration.ToString("F1");
        }

        public void UpgradeSpeed()
        {
            carController.ApplySpeedUpgrade(2f);
            _playerData.selectedCarData = carController.CarData.serializableData;
            SavePlayerData();
            UpdateUI();
        }

        public void UpgradeAcceleration()
        {
            carController.ApplyAccelerationUpgrade(1f);
            _playerData.selectedCarData = carController.CarData.serializableData;
            SavePlayerData();
            UpdateUI();
        }

        private void SavePlayerData()
        {
            SaveSystem.SavePlayerData(_playerData);
        }
    }
}

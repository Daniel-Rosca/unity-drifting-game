using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Content.Scripts.Data;
using UnityEngine;

namespace Content.Scripts.Utils
{
    public static class SaveSystem
    {
        private static string _savePath = Application.persistentDataPath + "/playerData.sav";

        public static void SavePlayerData(PlayerData playerData)
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(_savePath, FileMode.Create);

            formatter.Serialize(stream, playerData);
            stream.Close();
        }

        public static PlayerData LoadPlayerData()
        {
            if (File.Exists(_savePath))
            {
                var formatter = new BinaryFormatter();
                var stream = new FileStream(_savePath, FileMode.Open);

                var playerData = formatter.Deserialize(stream) as PlayerData;
                stream.Close();

                return playerData;
            }

            Debug.LogError("Save file not found.");
            return null;
        }
    }
}

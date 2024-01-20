using UnityEngine;

namespace Content.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewCarData", menuName = "Car Data")]
    public class CarData : ScriptableObject
    {
        public SerializableCarData serializableData;

        public void Initialize(float speed, float acceleration)
        {
            serializableData = new SerializableCarData(speed, acceleration);
        }
    }
}

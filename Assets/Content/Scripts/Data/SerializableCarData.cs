using System;

namespace Content.Scripts.Data
{
    [Serializable]
    public class SerializableCarData
    {
        public float baseSpeed;
        public float baseAcceleration;
        public string carName;
        
        public float upgradedSpeed;
        public float upgradedAcceleration;

        public SerializableCarData(float speed, float acceleration, string name)
        {
            baseSpeed = speed;
            baseAcceleration = acceleration;
            carName = name;
            
            upgradedSpeed = baseSpeed;
            upgradedAcceleration = baseAcceleration;
        }
    }
}

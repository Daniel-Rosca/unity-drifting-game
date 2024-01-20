using System;
using UnityEngine;

namespace Content.Scripts.Data
{
    [Serializable]
    public class SerializableCarData
    {
        public float baseSpeed;
        public float baseAcceleration;

        public SerializableCarData(float speed, float acceleration)
        {
            baseSpeed = speed;
            baseAcceleration = acceleration;
        }
    }
}

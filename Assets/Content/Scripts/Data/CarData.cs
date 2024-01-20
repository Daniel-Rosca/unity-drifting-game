using UnityEngine;

namespace Content.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewCarData", menuName = "Car Data")]
    public class CarData : ScriptableObject
    {
        public float baseSpeed;
        public float baseAcceleration;
    }
}

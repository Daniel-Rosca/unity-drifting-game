using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.Networking
{
    public class SpawnPlayers : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;

        public float minX;
        public float maxX;
        public float minY;
        public float maxY;

        private void Awake()
        {
            var randomPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);
        }
    }
}

using Cinemachine;
using UnityEngine;

namespace Content.Scripts.Utils
{
    public class CameraFollowPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private void Start()
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }
}

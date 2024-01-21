using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Content.Scripts.Networking
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            SceneManager.LoadScene("MainMenu");
        }
    }
}

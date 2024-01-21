using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Content.Scripts.Networking
{
    public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField createInput;
        [SerializeField] private TMP_InputField joinInput;

        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(createInput.text);
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(joinInput.text);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            SceneManager.LoadScene("Level1");
        }
    }
}

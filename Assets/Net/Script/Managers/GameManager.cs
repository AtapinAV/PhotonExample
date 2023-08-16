using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Net.Managers
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        private PlayerController _player1;
        private PlayerController _player2;

        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private string _playerPrefabName;
        [SerializeField]
        private InputAction _quit;
        [SerializeField, Range(1f, 15f)]
        private float _randomInterval = 7f;

        private void Start()
        {
            _quit.Enable();
            _quit.performed += OnQuit;

            var pos = new Vector3(Random.Range(-_randomInterval, _randomInterval), 0f, Random.Range(-_randomInterval, _randomInterval));
            var GO = PhotonNetwork.Instantiate(_playerPrefabName + PhotonNetwork.NickName, pos, new Quaternion());
            _camera.transform.SetParent(GO.transform, false);

            PhotonPeer.RegisterType(typeof(PlayerData), 100, Debugger.SerializePlayerData, Debugger.DeserializePlayerData);
        }

        public void AddPlayer(PlayerController player)
        {
            if (player.name.Contains("1")) _player1 = player;
            else _player2 = player;

            if (_player1 != null && _player2 != null)
            {
                _player1.SetTarget(_player2.transform);
                _player2.SetTarget(_player1.transform);
            }
        }
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }


        private void OnQuit(InputAction.CallbackContext obj)
        {
            PhotonNetwork.LeaveRoom();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            _quit.Dispose();
        }
    }
}



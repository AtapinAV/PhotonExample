using Photon.Pun;
using UnityEngine;

namespace Net
{
    public class TriggerLevelComponent : MonoBehaviourPunCallbacks
    {
        private void OnTriggerEnter(Collider other)
        {
            Debugger.Log("One of the players has gone beyond the level, he is death");
#if UNITY_EDITOR
            PhotonNetwork.LoadLevel("NetMenuScene");
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            PhotonNetwork.LoadLevel("NetMenuScene");
#endif
        }
    }
}



using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Net
{
    public class ProjectileController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private PhotonView _photonView;
        [SerializeField, Range(1f, 10f)]
        private float _moveSpeed = 3f;
        [SerializeField, Range(1f, 10f)]
        private float _damage = 1f;
        [SerializeField, Range(1f, 15f)]
        private float _lifetime = 7f;

        public float GetDamage => _damage;
        public string Parent { get; set; }

        private void Start()
        {
            StartCoroutine(OnDie());
        }

        private void Update()
        {
             transform.position += _moveSpeed * Time.deltaTime * transform.forward;
        }

        private IEnumerator OnDie()
        {
            yield return new WaitForSeconds(_lifetime);
            Destroy(gameObject);
        }
    }
}



using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Net
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {        
        private Controls _controls;
        private Transform _bulletPool;
        private Transform _target;

        [SerializeField]
        private ProjectileController _bulletPrefab;
        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private PhotonView _photonView;

        [Space, SerializeField, Range(1f, 10f)]
        private float _moveSpeed = 2f;
        [SerializeField, Range(0.5f, 5f)]
        private float _maxSpeed = 2f;
        [SerializeField, Range(0.1f, 1f)]
        private float _attackDelay = 0.4f;
        [SerializeField, Range(0.1f, 1f)]
        private float _rotateDelay = 0.25f;
        [SerializeField]
        private Vector3 _firePoint;
        [SerializeField]
        private Vector3 _offset;

        [Range(1f, 50f)]
        public float Health = 20f;


        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bulletPool = FindObjectOfType<UnityEngine.EventSystems.EventSystem>().transform;

            _controls = new Controls();

            FindObjectOfType<Managers.GameManager>().AddPlayer(this);               
        }

        public void SetTarget(Transform target)
        {
            _target = target;

            StartCoroutine(Fire());

            if (!_photonView.IsMine) return;

            _controls.Player1.Enable();
           
            StartCoroutine(Focus());
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {         
            if (stream.IsWriting)
            {
                stream.SendNext(PlayerData.Create(this));
            }
            else
            {
                ((PlayerData)stream.ReceiveNext()).Set(this);
            }
        }

        private void FixedUpdate()
        {if (!_photonView.IsMine) return;


            var direction = _controls.Player1.Movement.ReadValue<Vector2>();

            if (direction.x == 0 && direction.y == 0) return;

            var velocity = _rigidbody.velocity;
            velocity += _moveSpeed * Time.fixedDeltaTime * new Vector3(direction.x, 0f, direction.y);

            velocity.y = 0f;
            velocity = Vector3.ClampMagnitude(velocity, _maxSpeed);
            _rigidbody.velocity = velocity;
        }

        private IEnumerator Fire()
        {
            while (true)
            {
                var bullet = Instantiate(_bulletPrefab, _bulletPool);
                bullet.transform.position = transform.TransformPoint(_firePoint);
                bullet.transform.rotation = transform.rotation;
                bullet.Parent = name;
                yield return new WaitForSeconds(_attackDelay);
            }
        }

        private IEnumerator Focus()
        {
            while (true)
            {
                transform.LookAt(_target);
                transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
                yield return new WaitForSeconds(_rotateDelay);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_firePoint, 0.2f);
        }

        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<ProjectileController>();
            

            if (bullet == null || bullet.Parent == name) return;

            Health -= bullet.GetDamage;
            Destroy(other.gameObject);
            if (Health <= 0f)
            {
                Debugger.Log($"Player with name {name} is dead");
#if UNITY_EDITOR
                PhotonNetwork.LoadLevel("NetMenuScene");
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
                PhotonNetwork.LoadLevel("NetMenuScene");
#endif
            }
        }

        private void OnDestroy()
        {
            _controls.Player1.Disable();
        }
    }
}




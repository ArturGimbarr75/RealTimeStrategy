using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private int _damageToDeal = 20;
    [SerializeField]
    private float _destroyAfterSeconds = 5f;
    [SerializeField]
    private float _launchForce = 10f;

    private void Start()
    {
        _rigidbody.velocity = transform.forward * _launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), _destroyAfterSeconds);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient)
                return;

            if (other.TryGetComponent(out Health health))
            {
                health.DealDamage(_damageToDeal);
            }

            DestroySelf();
        }
    }
}

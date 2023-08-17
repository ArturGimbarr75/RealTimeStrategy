using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField]
    private Targeter _targeter;
    [SerializeField]
    private GameObject _projectilePrefab;
    [SerializeField]
    private Transform _projectileSpawnPoint;
    [SerializeField]
    private float _fireRange = 5f;
    [SerializeField]
    private float _fireRate = 1f;
    [SerializeField]
    private float _rotationSpeed = 20f;

    private float _lastFireTime;

    [ServerCallback]
    private void Update()
    {
        if (_targeter.Target is null)
            return;

        if (Vector3.Distance(_targeter.Target.transform.position, transform.position) > _fireRange)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(_targeter.Target.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(  transform.rotation,
                                                        targetRotation,
                                                        _rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / _fireRate) + _lastFireTime)
        {
            Quaternion projectileRotation =
                Quaternion.LookRotation(_targeter.Target.AimAtPoint.position - _projectileSpawnPoint.position);
            GameObject projectile = Instantiate(_projectilePrefab,
                                                _projectileSpawnPoint.position,
                                                projectileRotation);
            NetworkServer.Spawn(projectile, connectionToClient);

            _lastFireTime = Time.time;
        }
    }
}

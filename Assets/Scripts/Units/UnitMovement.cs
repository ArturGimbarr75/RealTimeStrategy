using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField]
    private Targeter _targeter;
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private float _chaseRange;

    #region Server

    [ServerCallback]
    private void Update()
    {
        if (_targeter.Target is not null)
        {
            if (Vector3.Distance(transform.position, _targeter.Target.transform.position) > _chaseRange)
            {
                _agent.SetDestination(_targeter.Target.transform.position);
            }
            else if (_agent.hasPath)
            {
                _agent.ResetPath();
            }

            return;
        }

        if (_agent.hasPath && _agent.remainingDistance < _agent.stoppingDistance)
            _agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        _targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            return;

        _agent.SetDestination(hit.position);
    }

    #endregion
}

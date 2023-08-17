using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField]
    private Targeter _targeter;
    [SerializeField]
    private NavMeshAgent _agent;

    #region Server

    [ServerCallback]
    private void Update()
    {
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

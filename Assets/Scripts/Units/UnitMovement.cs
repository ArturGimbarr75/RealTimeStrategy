using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent = null;

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
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            return;

        _agent.SetDestination(hit.position);
    }

    #endregion
}

using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayerScript : NetworkBehaviour
{
    [SerializeField]
    private List<Unit> _myUnits = new();

    #region Server

    public override void OnStartServer()
    {
        Unit.OnServerUnitSpawned += ServerHandleUnitSpawned;
        Unit.OnServerUnitDespawned += ServerHandleUnitDespawned;
    }

    public override void OnStopServer()
    {
        Unit.OnServerUnitSpawned -= ServerHandleUnitSpawned;
        Unit.OnServerUnitDespawned -= ServerHandleUnitDespawned;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        _myUnits.Remove(unit);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        if (!isClientOnly)
            return;

        Unit.OnAuthorityUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.OnAuthorityUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly)
            return;

        Unit.OnAuthorityUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.OnAuthorityUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (!isOwned)
            return;

        _myUnits.Add(unit);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        if (!isOwned)
            return;

        _myUnits.Remove(unit);
    }

    #endregion
}

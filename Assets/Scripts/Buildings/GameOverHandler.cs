using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    private List<UnitBase> _unitBases = new();

    #region Server

    public override void OnStartServer()
    {
        UnitBase.OnServerBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.OnServerBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.OnServerBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.OnServerBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        _unitBases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        _unitBases.Remove(unitBase);

        if (_unitBases.Count != 1)
            return;

        Debug.Log("Game over");
    }

    #endregion

    #region Client

    #endregion
}

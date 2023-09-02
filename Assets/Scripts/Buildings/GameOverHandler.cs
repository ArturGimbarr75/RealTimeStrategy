using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action<string> OnClientGameOver;

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

        int winnerId = _unitBases[0].connectionToClient.connectionId;
        RpcGameOver(winnerId.ToString());
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        OnClientGameOver?.Invoke(winner);
    }

    #endregion
}

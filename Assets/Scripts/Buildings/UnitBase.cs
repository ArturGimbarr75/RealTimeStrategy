using Mirror;
using System;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    private Health _health;

    public static event Action<int> OnServerPlayerDie;
    public static event Action<UnitBase> OnServerBaseSpawned;
    public static event Action<UnitBase> OnServerBaseDespawned;

    #region Server

    public override void OnStartServer()
    {
        _health = GetComponent<Health>();
        _health.OnServerDie += ServerHandlerDie;
        
        OnServerBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        OnServerBaseDespawned.Invoke(this);

        _health.OnServerDie -= ServerHandlerDie;
    }

    private void ServerHandlerDie()
    {
        OnServerPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    #endregion
}

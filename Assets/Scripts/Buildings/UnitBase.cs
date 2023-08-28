using Mirror;
using System;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    private Health _health;

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
        NetworkServer.Destroy(gameObject);
        Debug.Log("Base died");
    }

    #endregion

    #region Client

    #endregion
}

using Mirror;
using System;
using UnityEngine;

public class Building : NetworkBehaviour
{
    public static event Action<Building> OnServerBuildingSpawned;
    public static event Action<Building> OnServerBuildingDespawned;
    
    public static event Action<Building> OnAuthorityBuildingSpawned;
    public static event Action<Building> OnAuthorityBuildingDespawned;

    [field: SerializeField]
    public Sprite Icon { get; private set; }
    [field: SerializeField]
    public int Id { get; private set; } = -1;
    [field: SerializeField, Min(0)]
    public int Price { get; private set; }

    #region Server

    public override void OnStartServer()
    {
        OnServerBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        OnServerBuildingDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        OnAuthorityBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned)
            return;

        OnAuthorityBuildingDespawned?.Invoke(this);
    }

    #endregion
}

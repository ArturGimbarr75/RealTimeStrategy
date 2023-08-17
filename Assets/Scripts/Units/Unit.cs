using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    public UnitMovement Movement { get; private set; }
    public Targeter Targeter { get; private set; }

    [SerializeField]
    private UnityEvent _onSelected;
    [SerializeField]
    private UnityEvent _onDeselected;

    public static event Action<Unit> OnServerUnitSpawned;
    public static event Action<Unit> OnServerUnitDespawned;

    public static event Action<Unit> OnAuthorityUnitSpawned;
    public static event Action<Unit> OnAuthorityUnitDespawned;

    private void Start()
    {
        Movement = GetComponent<UnitMovement>();
        Targeter = GetComponent<Targeter>();
    }

    #region Server

    public override void OnStartServer()
    {
        OnServerUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        OnServerUnitDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        if (!isClientOnly || !isOwned)
            return;

        OnAuthorityUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !isOwned)
            return;

        OnAuthorityUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if(!isOwned)
            return;

        _onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!isOwned)
            return;

        _onDeselected?.Invoke();
    }

    #endregion
}

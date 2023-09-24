using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    public UnitMovement Movement { get; private set; }
    public Targeter Targeter { get; private set; }

    [field: SerializeField]
    public int ResourceCost { get; private set; }

    private Health _health;

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
        _health = GetComponent<Health>();
        _health.OnServerDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        OnServerUnitDespawned?.Invoke(this);
        _health.OnServerDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        OnAuthorityUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned)
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

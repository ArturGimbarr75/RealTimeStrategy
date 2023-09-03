using Mirror;
using System;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public float HealthAmount => CurrentHealth / MaxHealth;

    [field: SerializeField]
    public int MaxHealth { get; private set; } = 100;

    [field:SyncVar(hook=nameof(HandleHealthUpdated))]
    public int CurrentHealth { get; private set; }

    public event Action OnServerDie;
    public event Action<int, int> OnClientHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        CurrentHealth = MaxHealth;
        UnitBase.OnServerPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer()
    {
        UnitBase.OnServerPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void ServerHandlePlayerDie(int connectionId)
    {
        if (connectionToClient.connectionId == connectionId)
            Kill();
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (CurrentHealth == 0)
            return;

        CurrentHealth = Mathf.Max(CurrentHealth - damageAmount, 0);

        if (CurrentHealth == 0)
        {
            OnServerDie?.Invoke();
            Debug.Log($"Died {gameObject}");
        }
    }

    [Server]
    public void Kill()
    {
        DealDamage(CurrentHealth);
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int _, int newHealth)
    {
        OnClientHealthUpdated?.Invoke(newHealth, MaxHealth);
    }

    #endregion
}

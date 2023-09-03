using Mirror;
using System;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField]
    private int _maxHealth = 100;

    [SyncVar(hook=nameof(HandleHealthUpdated))]
    private int _currentHealth;

    public event Action OnServerDie;
    public event Action<int, int> OnClientHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        _currentHealth = _maxHealth;
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
        if (_currentHealth == 0)
            return;

        _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

        if (_currentHealth == 0)
        {
            OnServerDie?.Invoke();
            Debug.Log($"Died {gameObject}");
        }
    }

    [Server]
    public void Kill()
    {
        DealDamage(_currentHealth);
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int _, int newHealth)
    {
        OnClientHealthUpdated?.Invoke(newHealth, _maxHealth);
    }

    #endregion
}

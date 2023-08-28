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

    #endregion

    #region Client

    private void HandleHealthUpdated(int _, int newHealth)
    {
        OnClientHealthUpdated?.Invoke(newHealth, _maxHealth);
    }

    #endregion
}

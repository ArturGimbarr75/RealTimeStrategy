using Mirror;
using System;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField]
    private int _maxHealth = 100;

    [SyncVar]
    private int _currentHealth;

    public event Action OnServerDie;

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
}

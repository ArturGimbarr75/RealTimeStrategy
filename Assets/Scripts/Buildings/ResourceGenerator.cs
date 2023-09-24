using Mirror;
using System;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField, Min(0)]
    private int _resourcesPerInterval;
    [SerializeField, Min(0.1f)]
    private float _interval;

    private RTSPlayer _player;
    private Health _health;
    private float _timer;

    public override void OnStartServer()
    {
        _player = connectionToClient.identity.GetComponent<RTSPlayer>();
        _health = GetComponent<Health>();
        _timer = _interval;

        _health.OnServerDie += ServerHandleDie;
        GameOverHandler.OnServerGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        _health.OnServerDie -= ServerHandleDie;
        GameOverHandler.OnServerGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _player.AddResources(_resourcesPerInterval);
            _timer += _interval;
        }
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }
}

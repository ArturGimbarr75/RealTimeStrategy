using Mirror;
using System;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    public Targetable Target { get; private set; }

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.OnServerGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.OnServerGameOver -= ServerHandleGameOver;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    [Command]
    public void CmdSetTarget(GameObject target)
    {
        if (!target.TryGetComponent(out Targetable newTarget))
            return;

        Target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        Target = null;
    }

    #endregion

    #region Client



    #endregion
}

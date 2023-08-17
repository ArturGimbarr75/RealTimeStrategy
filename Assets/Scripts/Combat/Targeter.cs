using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    public Targetable Target { get; private set; }

    #region Server

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

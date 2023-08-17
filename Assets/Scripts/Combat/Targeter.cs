using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable _target;

    #region Server

    [Command]
    public void CmdSetTarget(GameObject target)
    {
        if (!target.TryGetComponent(out Targetable newTarget))
            return;

        _target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        _target = null;
    }

    #endregion

    #region Client



    #endregion
}

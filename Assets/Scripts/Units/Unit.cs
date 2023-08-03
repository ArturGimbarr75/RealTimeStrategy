using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField]
    private UnityEvent _onSelected;
    [SerializeField]
    private UnityEvent _onDeselected;

    #region Client

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

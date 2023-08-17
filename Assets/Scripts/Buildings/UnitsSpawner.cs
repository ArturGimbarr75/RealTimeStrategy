using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitsSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject _unitPrefab;
    [SerializeField]
    private Transform _unitSpawnPoint;

    #region Server

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(  _unitPrefab,
                                                _unitSpawnPoint.position,
                                                _unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO: Fix left click
        if (eventData.button != PointerEventData.InputButton.Middle)
            return;
        if (!isOwned)
            return;

        CmdSpawnUnit();
    }

    #endregion
}

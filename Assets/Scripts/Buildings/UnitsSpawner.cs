using Mirror;
using System;
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

    private Health _health;

    #region Server

    public override void OnStartServer()
    {
        _health = GetComponent<Health>();
        _health.OnServerDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        _health.OnServerDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

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
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (!isOwned)
            return;

        CmdSpawnUnit();
    }

    #endregion
}

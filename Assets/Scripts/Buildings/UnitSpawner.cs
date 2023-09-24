using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject _unitPrefab;
    [SerializeField]
    private Transform _unitSpawnPoint;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    #region Server

    public override void OnStartServer()
    {
        _health.OnServerDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        _health.OnServerDie -= ServerHandleDie;
    }

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

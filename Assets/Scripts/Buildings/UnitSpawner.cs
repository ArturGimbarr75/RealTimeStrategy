using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Unit _unitPrefab;
    [SerializeField]
    private Transform _unitSpawnPoint;

    [Space(2)]
    [SerializeField]
    private TMP_Text _remainingText;
    [SerializeField]
    private Image _unitProgressImage;
    [SerializeField, Min(1)]
    private int _maxUnitQueue;
    [SerializeField, Min(0)]
    private float _spawnMoveRange;
    [SerializeField, Min(0)]
    private float _unitSpawnDuration;

    [SyncVar(hook=nameof(ClientHandleQueuedUnitsUpdated))]
    private int _queuedUnits;
    [SyncVar]
    private float _unitTimer;
    private float _progressImageVelocity;
    private Health _health;
    private RTSPlayer _player;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }
        
        if (isClient)
        {
            UpdateTimerDisplay();
        }
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

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (_queuedUnits >= _maxUnitQueue)
            return;

        if (_player.Resources < _unitPrefab.ResourceCost)
            return;

        _queuedUnits++;
        _player.TakeResources(_unitPrefab.ResourceCost);
    }

    [Server]
    private void ProduceUnits()
    {
        if (_queuedUnits <= 0)
            return;

        _unitTimer += Time.deltaTime;

        if (_unitTimer < _unitSpawnDuration)
            return;

        Unit unitInstance = Instantiate(_unitPrefab,
                                        _unitSpawnPoint.position,
                                        _unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance.gameObject, connectionToClient);
        Vector3 spawnOffset = Random.insideUnitSphere * _spawnMoveRange;
        spawnOffset.y = _unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(_unitSpawnPoint.position + spawnOffset);
        _queuedUnits--;
        _unitTimer = 0;
    }

    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        float progress = _unitTimer / _unitSpawnDuration;

        _unitProgressImage.fillAmount = progress < _unitProgressImage.fillAmount
            ? progress
            : Mathf.SmoothDamp(_unitProgressImage.fillAmount, progress, ref _progressImageVelocity, 0.1f);
    }

    public override void OnStartClient()
    {
        _player = connectionToClient.identity.GetComponent<RTSPlayer>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (!isOwned)
            return;

        CmdSpawnUnit();
    }

    private void ClientHandleQueuedUnitsUpdated(int old, int @new)
    {
        _remainingText.text = @new.ToString();
    }

    #endregion
}

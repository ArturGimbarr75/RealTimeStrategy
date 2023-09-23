using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField]
    private Building[] _buildings;

    public List<Unit> MyUnits { get; private set; } = new();
    public List<Building> MyBuildings { get; private set; } = new();

    #region Server

    public override void OnStartServer()
    {
        Unit.OnServerUnitSpawned += ServerHandleUnitSpawned;
        Unit.OnServerUnitDespawned += ServerHandleUnitDespawned;
        Building.OnServerBuildingSpawned += ServerHandleBuildingSpawned;
        Building.OnServerBuildingDespawned += ServerHandleBuildingDespawned;
    }

    public override void OnStopServer()
    {
        Unit.OnServerUnitSpawned -= ServerHandleUnitSpawned;
        Unit.OnServerUnitDespawned -= ServerHandleUnitDespawned;
        Building.OnServerBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.OnServerBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building? buildingToPlace = _buildings.FirstOrDefault(b => b.Id == buildingId);
        
        if (buildingToPlace is null)
            return;

        var buildingInstance = 
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId == connectionToClient.connectionId)
            MyUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId == connectionToClient.connectionId)
            MyUnits.Remove(unit);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId == connectionToClient.connectionId)
            MyBuildings.Add(building);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId == connectionToClient.connectionId)
            MyBuildings.Remove(building);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
            return;

        Unit.OnAuthorityUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.OnAuthorityUnitDespawned += AuthorityHandleUnitDespawned;
        Building.OnAuthorityBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.OnAuthorityBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !isOwned)
            return;

        Unit.OnAuthorityUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.OnAuthorityUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.OnAuthorityBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.OnAuthorityBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (isOwned)
            MyUnits.Add(unit);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        if (isOwned)
            MyUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        if (isOwned)
            MyBuildings.Add(building);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        if (isOwned)
            MyBuildings.Remove(building);
    }

    #endregion
}

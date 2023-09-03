using Mirror;
using System;
using System.Collections.Generic;
using static UnityEngine.UI.CanvasScaler;

public class RTSPlayer : NetworkBehaviour
{
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

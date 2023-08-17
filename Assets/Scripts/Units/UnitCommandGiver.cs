using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField]
    private UnitSelectionHandler _unitSelectionHandler;
    [SerializeField]
    private LayerMask _layerMask;

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
            return;

        if (hit.collider.TryGetComponent(out Targetable target))
        {
            if (target.isOwned)
            {
                TryMove(hit.point);
                return;
            }

            TryTarget(target);
            return;
        }
        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in _unitSelectionHandler.SelectedUnits)
            unit.Movement.CmdMove(point);
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in _unitSelectionHandler.SelectedUnits)
        {
            unit.Targeter.CmdSetTarget(target.gameObject);
        }
    }
}

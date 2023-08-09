using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UnitSelectionHandler : MonoBehaviour
{
    public List<Unit> SelectedUnits { get; } = new();

    [SerializeField]
    private RectTransform _unitSelectionArea;
    [SerializeField]
    private LayerMask _layerMask = new();

    private RTSPlayerScript _player;
    private Vector2 _startPosition;

    private void Update()
    {
        _player ??= NetworkClient.connection.identity?.GetComponent<RTSPlayerScript>();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            StartSelectionArea();            
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
            ClearSelectionArea();
        else if (Mouse.current.leftButton.isPressed)
            UpdateSelectionArea();
    }

    private void StartSelectionArea()
    {
        foreach (Unit selectedUnit in SelectedUnits)
        {
            selectedUnit.Deselect();
        }
        SelectedUnits.Clear();

        _unitSelectionArea.gameObject.SetActive(true);
        _startPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        float areaWidth = Mathf.Abs(_startPosition.x - mousePos.x);
        float areaHeight = Mathf.Abs(_startPosition.y - mousePos.y);

        var size = new Vector2(areaWidth, areaHeight);
        _unitSelectionArea.sizeDelta = size;
        _unitSelectionArea.anchoredPosition = (_startPosition + mousePos) / 2;
    }

    private void ClearSelectionArea()
    {
        _unitSelectionArea.gameObject.SetActive(false);

        if (_unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
                return;
            if (!hit.collider.TryGetComponent(out Unit unit))
                return;
            if (!unit.isOwned)
                return;

            SelectedUnits.Add(unit);
            unit.Select();

            return;
        }

        Vector2 min = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
        Vector2 max = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in _player.MyUnits)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (InRange(min, max, screenPosition))
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    private bool InRange(Vector2 min, Vector2 max, Vector3 value)
    {
        return value.x > min.x
            && value.x < max.x
            && value.y > min.y
            && value.y < max.y;
    }
}

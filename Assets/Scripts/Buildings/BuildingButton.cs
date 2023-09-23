using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Building _building;
    [SerializeField]
    private Image _iconImage;
    [SerializeField]
    private TMP_Text _priceText;
    [SerializeField]
    private LayerMask _floorMask;

    private RTSPlayer _player;
    private GameObject _buildingPreviewInstance;
    private Renderer _buildingRendererInstance;

    private void Start()
    {
        _iconImage.sprite = _building.Icon;
        _priceText.text = $"{_building.Price}$";
    }

    private void Update()
    {
        _player ??= NetworkClient.connection?.identity?.GetComponent<RTSPlayer>();

        if (_buildingPreviewInstance is null)
            return;

        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _buildingPreviewInstance = Instantiate(_building.BuildingPreview);
        _buildingRendererInstance = _buildingPreviewInstance.GetComponentInChildren<Renderer>();

        _buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_buildingPreviewInstance is null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
        {

        }

        Destroy(_buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
            return;

        _buildingPreviewInstance.transform.position = hit.point;

        if (!_buildingPreviewInstance.activeSelf)
            _buildingPreviewInstance.SetActive(true);
    }
}

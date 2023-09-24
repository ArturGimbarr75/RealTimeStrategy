using Mirror;
using System;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _resourcesText;
    [SerializeField]
    private TMP_Text _changeText;
    [SerializeField]
    private Animator _changeAnimator;

    private RTSPlayer _player;

    private void Update()
    {
        if (_player is null)
        {
            _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            if (_player is not null)
            {
                _player.OnClientResourcesUpdated += ClientHandleResourcesUpdated;
                ClientHandleResourcesUpdated(0, _player.Resources);
            }
        }
    }

    private void OnDestroy()
    {
        _player.OnClientResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int old, int @new)
    {
        _resourcesText.text = $"Currency: {@new}";
        _changeText.color = old < @new ? Color.green : Color.red;
        int diff = @new - old;
        _changeText.text = $"         {(diff > 0? " +" : " ")}{diff}";
        _changeAnimator.SetTrigger("Text");
    }
}

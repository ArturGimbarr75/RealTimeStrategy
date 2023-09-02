using Mirror;
using System;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameOverDisplayParrent;
    [SerializeField]
    private TMP_Text _winnerNameText;

    private void Start()
    {
        GameOverHandler.OnClientGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.OnClientGameOver += ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner)
    {
        _winnerNameText.text = $"{winner} has won!";
        _gameOverDisplayParrent.SetActive(true);
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}

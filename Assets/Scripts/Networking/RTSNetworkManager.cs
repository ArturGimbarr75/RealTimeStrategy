using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [Header(nameof(RTSNetworkManager))]
    [SerializeField] private GameObject _unitSpawnerPrefab;
    [SerializeField] private GameOverHandler _gameOverHandlerPrefab;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnerInstance =
            Instantiate(_unitSpawnerPrefab,
                        conn.identity.transform.position,
                        conn.identity.transform.rotation);
        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map_"))
        {
            GameOverHandler gameOverHandler = Instantiate(_gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandler.gameObject);
        }
    }
}

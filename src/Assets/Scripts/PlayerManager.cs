using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;

    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private BaseController _baseController;
    [SerializeField] private int _maxPlayers;
    [SerializeField] private SpriteRenderer _player;

    /// <summary>
    /// Playerの数だけオブジェクトプールを作った方が良いと思う。今回は一つのプレイヤーに絞って実装
    /// </summary>

    private List<PlayerController> _activePlayers = new List<PlayerController>();
    private Queue<PlayerController> _playerPool = new Queue<PlayerController>();

    // Start is called before the first frame update
    void Start()
    {
        //最初の生成
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        for (int i = 0; i < _maxPlayers; ++i)
        {
            var playerObject = Instantiate(_player);
            var playerController = playerObject.GetComponent<PlayerController>();
            playerController.OnPlayerHit = HandleHit;
            playerController.enemyManager = _enemyManager;
            playerController.baseController = _baseController;
            playerObject.gameObject.SetActive(false);
            _playerPool.Enqueue(playerController);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _activePlayers.Count; ++i)
        {
            var player = _activePlayers[i];

            if (IsOffScreen(player.transform.position))
            {
                DeactivePlayer(player);
            }
        }
    }

    public void SpawnPlayer()
    {
        if (_playerPool.Count > 0)
        {
            var player = _playerPool.Dequeue();
            player.PlayerSpawn();
            player.gameObject.SetActive(true);
            _activePlayers.Add(player);
        }
    }

    public List<PlayerController> UpdatePlayerList()
    {
        return _activePlayers;
    } 

    private void HandleHit(PlayerController player)
    {
        Debug.Log("撃破");

        DeactivePlayer(player);
    }

    void DeactivePlayer(PlayerController player)
    {
        player.gameObject.SetActive(false);
        _activePlayers.Remove(player);
        _playerPool.Enqueue(player);
    }

    bool IsOffScreen(Vector2 position)
    {
        return position.x < -11 || position.x > 11;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private BaseController _baseController;
    [SerializeField] private int _maxEnemies;
    [SerializeField] private SpriteRenderer _enemy;

    private List<EnemyController> _activeEnemies = new List<EnemyController>();
    private Queue<EnemyController> _enemyPool = new Queue<EnemyController>();

    private int _activePlayersCount = 0;
    private bool isInitCompleted = false;
    private float spawnInterval = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        InitializeEnemy();
    }

    //最初の敵の生成
    private void InitializeEnemy()
    {
        for (int i = 0; i < _maxEnemies; ++i)
        {
            var enemyObject = Instantiate(_enemy);
            var enemyController = enemyObject.GetComponent<EnemyController>();
            enemyController.OnEnemyHit = HandleHit;
            enemyController.playerManager = _playerManager;
            enemyController.baseController = _baseController;
            enemyObject.gameObject.SetActive(false);
            _enemyPool.Enqueue(enemyController);
        }

        isInitCompleted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInitCompleted)
        {
            StartCoroutine(SpawnEnemy());
            isInitCompleted = false;
        }

        for (int i = 0; i < _activeEnemies.Count; ++i)
        {
            var enemy = _activeEnemies[i];

            if (IsOffScreen(enemy.transform.position))
            {
                DeactiveEnemy(enemy);
            }
        }
    }

    void ChangeSpawnInterval()
    {
        _activePlayersCount = _playerManager.ActivePlayersCount();

        if (_activePlayersCount <= 1)
        {
            spawnInterval = 4.9f;
        }
        else if ((_activePlayersCount < 3) && (_activePlayersCount >= 2)) 
        {
            spawnInterval = 3.8f;
        }
        else if ((_activePlayersCount >= 3) && (_activePlayersCount < 4))
        {
            spawnInterval = 2.5f;
        }
        else if ((_activePlayersCount >= 4) && (_activePlayersCount < 5))
        {
            spawnInterval = 1.8f;
        }
        else if((_activePlayersCount >= 5) && (_activePlayersCount < 7))
        {
            spawnInterval = 1.5f;
        }
        else
        {
            spawnInterval = 1.3f;
        }
    }

    //敵の出現
    public IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            ChangeSpawnInterval();

            if (_enemyPool.Count > 0)
            {
                var enemy = _enemyPool.Dequeue();
                enemy.EnemySpawn();
                enemy.gameObject.SetActive(true);
                _activeEnemies.Add(enemy);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public List<EnemyController> UpdateEnemyList()
    {
        return _activeEnemies;
    }

    //Enemyが倒されたときに呼ばれる関数
    private void HandleHit(EnemyController enemy)
    {
        Debug.Log("撃破");

        DeactiveEnemy(enemy);
    }

    //オブジェクトプールに戻す
    void DeactiveEnemy(EnemyController enemy)
    {
        enemy.gameObject.SetActive(false);
        _activeEnemies.Remove(enemy);
        _enemyPool.Enqueue(enemy);
    }

    //枠外に出たか
    bool IsOffScreen(Vector2 position)
    {
        return position.x < -11 || position.x > 11;
    }
}

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

    private bool isInitCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        InitializeEnemy();
    }

    //�ŏ��̓G�̐���
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

    //�G�̏o��
    public IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            if (_enemyPool.Count > 0)
            {
                var enemy = _enemyPool.Dequeue();
                enemy.EnemySpawn();
                enemy.gameObject.SetActive(true);
                _activeEnemies.Add(enemy);
            }

            yield return new WaitForSeconds(3f);
        }
    }

    public List<EnemyController> UpdateEnemyList()
    {
        return _activeEnemies;
    }

    //Enemy���|���ꂽ�Ƃ��ɌĂ΂��֐�
    private void HandleHit(EnemyController enemy)
    {
        Debug.Log("���j");

        DeactiveEnemy(enemy);
    }

    //�I�u�W�F�N�g�v�[���ɖ߂�
    void DeactiveEnemy(EnemyController enemy)
    {
        enemy.gameObject.SetActive(false);
        _activeEnemies.Remove(enemy);
        _enemyPool.Enqueue(enemy);
    }

    //�g�O�ɏo����
    bool IsOffScreen(Vector2 position)
    {
        return position.x < -11 || position.x > 11;
    }
}

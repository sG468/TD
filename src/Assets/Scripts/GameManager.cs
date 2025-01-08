using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //インスタンス化

    public List<PlayerController> players; // プレイヤーリスト
    public List<EnemyController> enemies;  // 敵リスト

    //敵の種類
    [SerializeField] private SpriteRenderer[] EnemyType;
    [SerializeField] private Vector2 _playerBasePosition;
    [SerializeField] private Vector2 _enemyBasePosition;

    [SerializeField] private BaseController _baseController; //Base周りの処理をしているBaseControllerスクリプトの追加
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private EnemyManager _enemyManager;

    //[SerializeField] private GameObject _inGameUIPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _gameClearPanel;

    public bool isGame = false;
    public bool isGameOver = false;
    public bool isGameClear = false;

    //private string _playerBaseName = "PlayerBase";
    //private string _enemyBaseName = "EnemyBase";
    //private float maxDetectionRange = 3f;
    //private Vector2 _baseSize = new Vector2(1f, 1f);
    //private bool doGameOver = false;
    //private bool doGameClear = false;

    private void Awake()
    {
        //インスタンス化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        isGame = true;
        isGameClear = false;
        isGameOver = false;
        //_inGameUIPanel.SetActive(true);
        _gameOverPanel.SetActive(false);
        _gameClearPanel.SetActive(false);
        //StartCoroutine(_enemyManager.SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameClear)
        {
            //_inGameUIPanel.SetActive(false);
            _gameClearPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (isGameOver)
        {
            //_inGameUIPanel.SetActive(false);
            _gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    //public void Clear()
    //{
    //    doGameClear = true;
    //}

    //public void GameOver()
    //{
    //    doGameOver = true;
    //}
}

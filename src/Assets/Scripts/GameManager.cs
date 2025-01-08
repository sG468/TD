using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //�C���X�^���X��

    public List<PlayerController> players; // �v���C���[���X�g
    public List<EnemyController> enemies;  // �G���X�g

    //�G�̎��
    [SerializeField] private SpriteRenderer[] EnemyType;
    [SerializeField] private Vector2 _playerBasePosition;
    [SerializeField] private Vector2 _enemyBasePosition;

    [SerializeField] private BaseController _baseController; //Base����̏��������Ă���BaseController�X�N���v�g�̒ǉ�
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private EnemyManager _enemyManager;

    public bool isGame = false;

    //private string _playerBaseName = "PlayerBase";
    //private string _enemyBaseName = "EnemyBase";
    //private float maxDetectionRange = 3f;
    //private Vector2 _baseSize = new Vector2(1f, 1f);
    //private bool doGameOver = false;
    //private bool doGameClear = false;

    private void Awake()
    {
        //�C���X�^���X��
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
        isGame = true;
        //StartCoroutine(_enemyManager.SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
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

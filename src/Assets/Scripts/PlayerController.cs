using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;

    //Playerの状態
    public enum PlayerState
    {
        None,
        Move,
        CollisionWithEnemy,
        CollisionWithBase,
    }

    public System.Action<PlayerController> OnPlayerHit;
    public BaseController baseController;
    public EnemyManager enemyManager;
    public Vector2 size = new Vector2(1f, 1f); // プレイヤーの矩形サイズ
    public int health = 6;                  // プレイヤーの体力
    public int attackPower = 1;              // プレイヤーの攻撃力
    public bool isCollision = false;
    public bool isBaseCollision = false;

    private int healthStock = 0;
    private float direction = -1f;
    private float nextHitTime = 0f; //経過時間
    private float nextHitIntervalTime = 1.5f; //インターバル時間
    private bool isHit = false; //衝突いるかどうか
    private Vector3 pos;
    private Vector2 _enemyBasePosition = new Vector2(-8.3f, 0f);
    private EnemyController _hostEnemy; //今ロックしている相手
    PlayerState state = PlayerState.None; //現在のプレイヤーの状態の保持
    //List<EnemyController> activeEnemies; //今画面上に出ている敵を保有する場所

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHit)
        {
            CheckState(state);
        }
        else
        {
            AttackTime(state);
        }
    }

    /// <summary>
    /// パブリック関数
    /// </summary>
    //プレイヤーの出現
    public void PlayerSpawn()
    {
        float y = Random.Range(-1.0f, -2.0f);
        SpriteRenderer pl = GetComponent<SpriteRenderer>();
        pl.transform.position = new Vector3(9.6f, y, 0);
        pl.sortingOrder = (int)(-y * 10);
        state = PlayerState.Move;
        //health = healthStock;
    }

    //ダメージ
    public void TakeDamage(int damage)
    {
        if (gameObject == null)
        {
            return;
        }

        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Player defeated!");
            state = PlayerState.None;
            health = healthStock;
            OnPlayerHit?.Invoke(this);
            gameObject.SetActive(false);
        }
        else
        {
            //Invoke("TakeDamage", 0.7f);
        }
    }

    /// <summary>
    /// プライベート関数
    /// </summary>
    /// <param name="_state"></param>
   
    //もろもろのデータの初期化
    void SetUp()
    {
        isHit = false;
        healthStock = health;
        nextHitTime = Time.time + nextHitIntervalTime;
        pos = new Vector3(direction, 0, 0);
        state = PlayerState.Move;
    }

    //Playerの状態判定
    void CheckState(PlayerState _state)
    {
        switch (_state)
        {
            case PlayerState.Move: //移動
                PlayerMove();
                CheckCollision(enemyManager.UpdateEnemyList());
                break;
            case PlayerState.CollisionWithEnemy: //敵への攻撃
                Attack(_hostEnemy);
                CheckCollision(enemyManager.UpdateEnemyList());
                break;
            case PlayerState.CollisionWithBase: //拠点への攻撃
                AttackToBase();
                CheckCollision(enemyManager.UpdateEnemyList());
                break;
            case PlayerState.None: //無
                break;
            default:
                break;
        }
    }

    //ラグを設けた攻撃タイム
    void AttackTime(PlayerState _state)
    {
        if (Time.time > nextHitTime)
        {
            switch (_state)
            {
                case PlayerState.CollisionWithEnemy:
                    Attack(_hostEnemy);
                    break;
                case PlayerState.CollisionWithBase:
                    AttackToBase();
                    break;
            }

            nextHitTime = Time.time + nextHitIntervalTime;
        }

        CheckCollision(enemyManager.UpdateEnemyList());
    }

    //通常の移動
    void PlayerMove()
    {
        transform.position += pos * Time.deltaTime;
    }

    //当たり判定
    void CheckCollision(List<EnemyController> activeEnemies)
    {
        //敵との当たり判定
        if (activeEnemies != null)
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    if (CollisionDetector.CheckRectCollision(gameObject.transform.position, size, enemy.transform.position, enemy.size))
                    {
                        state = PlayerState.CollisionWithEnemy;
                        isHit = true;
                        _hostEnemy = enemy;
                        return;
                    }
                }
            }
        }

        //敵の拠点との当たり判定
        if (CollisionDetector.CheckRectCollision(gameObject.transform.position, size, _enemyBasePosition, size))  
        {
            state = PlayerState.CollisionWithBase;
            isHit = true;
            return;
        }

        isHit = false;
    }

    //Enemyへの攻撃
    void Attack(EnemyController enemy)
    {
        if (!enemy.gameObject.activeSelf)
        {
            state = PlayerState.Move;
            isHit = false;
        }

        enemy.TakeDamage(attackPower);

        state = PlayerState.Move;
    }

    //Baseへの攻撃
    void AttackToBase()
    {
        if (baseController.IsBaseDestroyed("enemy"))
        {
            state = PlayerState.None;
        }

        baseController.EnemyBaseDamage();

        state = PlayerState.Move;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyController : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;

    //Enemyの状態
    public enum EnemyState
    {
        None,
        Move,
        CollisionWithPlayer,
        CollisionWithBase,
    }

    public System.Action<EnemyController> OnEnemyHit;
    public BaseController baseController;
    public PlayerManager playerManager;
    public Vector2 size = new Vector2(1f, 1f); // 敵の矩形サイズ
    public int health = 50;                   // 敵の体力
    public int attackPower = 5;              // 敵の攻撃力
    public bool isCollision = false;
    public bool isBaseCollision = false;

    private int healthStock = 0;
    private float direction = 1f;
    private float nextHitTime = 0f; //経過時間
    private float nextHitInterval = 0.5f; //インターバル時間
    private bool isHit = false;
    private Vector3 pos;
    private Vector2 _playerBasePosition = new Vector2(8f, 0f);
    private PlayerController _hostPlayer; //今ロックしている相手
    EnemyState state = EnemyState.None; //今現在の敵の状態の保持
    //List<PlayerController> activePlayers; //画面上に出ているプレイヤー情報の保持
    

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
    //敵の出現
    public void EnemySpawn()
    {
        float y = Random.Range(-1.0f, -2.0f);
        SpriteRenderer pl = GetComponent<SpriteRenderer>();
        pl.transform.position = new Vector3(-10.0f, y, 0);
        pl.sortingOrder = (int)(-y * 10);
        state = EnemyState.Move;
        //health = healthStock;
    }

    //ダメージ関数
    public void TakeDamage(int damage)
    {
        if (gameObject == null)
        {
            return;
        }

        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Enemy defeated!");
            state = EnemyState.None;
            health = healthStock;
            OnEnemyHit?.Invoke(this);
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
        healthStock = health;
        isHit = false;
        nextHitTime = Time.time + nextHitInterval;
        pos = new Vector3(direction, 0, 0);
        state = EnemyState.Move;
    }

    //敵の状態判定
    void CheckState(EnemyState _state)
    {
        switch (_state)
        {
            case EnemyState.Move: //移動
                EnemyMove();
                CheckCollision(playerManager.UpdatePlayerList());
                break;
            case EnemyState.CollisionWithPlayer: //Playerへの攻撃
                Attack(_hostPlayer);
                CheckCollision(playerManager.UpdatePlayerList());
                break;
            case EnemyState.CollisionWithBase: //拠点への攻撃
                AttackToBase();
                CheckCollision(playerManager.UpdatePlayerList());
                break;
            case EnemyState.None:
                break;
            default:
                break;
        }
    }

    //ラグを伴った攻撃タイム
    void AttackTime(EnemyState _state)
    {
        if (Time.time > nextHitTime)
        {
            switch (_state)
            {
                case EnemyState.CollisionWithPlayer:
                    Attack(_hostPlayer);
                    break;
                case EnemyState.CollisionWithBase:
                    AttackToBase();
                    break;
            }

            nextHitTime = Time.time + nextHitInterval;
        }

        CheckCollision(playerManager.UpdatePlayerList());
    }

    //通常の移動
    void EnemyMove()
    {
        transform.position += pos * Time.deltaTime;
    }

    //当たり判定
    void CheckCollision(List<PlayerController> activePlayers)
    {
        //Playerとの当たり判定
        if (activePlayers != null)
        {
            foreach (var player in activePlayers)
            {
                if (player != null)
                {
                    if (CollisionDetector.CheckRectCollision(gameObject.transform.position, size, player.transform.position, player.size))
                    {
                        state = EnemyState.CollisionWithPlayer;
                        isHit = true;
                        _hostPlayer = player;
                        return;
                    }
                }
            }
        }

        //プレイヤーの拠点との当たり判定
        if (CollisionDetector.CheckRectCollision(gameObject.transform.position, size, _playerBasePosition, size))
        {
            state = EnemyState.CollisionWithBase;
            isHit = true;
            return;
        }

        isHit = false;
    }

    //Playerへの攻撃
    void Attack(PlayerController player)
    {
        if (!player.gameObject.activeSelf)
        {
            state = EnemyState.Move;
            isHit = false;
        }

        player.TakeDamage(attackPower);

        state = EnemyState.Move;
    }

    //Baseへの攻撃
    void AttackToBase()
    {
        if (baseController.IsBaseDestroyed("player"))
        {
            state = EnemyState.None;
        }

        baseController.PlayerBaseDamage();

        state = EnemyState.Move;
    }
}

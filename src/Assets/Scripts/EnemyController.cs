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

    //Enemy�̏��
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
    public Vector2 size = new Vector2(1f, 1f); // �G�̋�`�T�C�Y
    public int health = 50;                   // �G�̗̑�
    public int attackPower = 5;              // �G�̍U����
    public bool isCollision = false;
    public bool isBaseCollision = false;

    private int healthStock = 0;
    private float direction = 1f;
    private float nextHitTime = 0f; //�o�ߎ���
    private float nextHitInterval = 0.5f; //�C���^�[�o������
    private bool isHit = false;
    private Vector3 pos;
    private Vector2 _playerBasePosition = new Vector2(8f, 0f);
    private PlayerController _hostPlayer; //�����b�N���Ă��鑊��
    EnemyState state = EnemyState.None; //�����݂̓G�̏�Ԃ̕ێ�
    //List<PlayerController> activePlayers; //��ʏ�ɏo�Ă���v���C���[���̕ێ�
    

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
    /// �p�u���b�N�֐�
    /// </summary>
    //�G�̏o��
    public void EnemySpawn()
    {
        float y = Random.Range(-1.0f, -2.0f);
        SpriteRenderer pl = GetComponent<SpriteRenderer>();
        pl.transform.position = new Vector3(-10.0f, y, 0);
        pl.sortingOrder = (int)(-y * 10);
        state = EnemyState.Move;
        //health = healthStock;
    }

    //�_���[�W�֐�
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
    /// �v���C�x�[�g�֐�
    /// </summary>
    /// <param name="_state"></param>
    
    //�������̃f�[�^�̏�����
    void SetUp()
    {
        healthStock = health;
        isHit = false;
        nextHitTime = Time.time + nextHitInterval;
        pos = new Vector3(direction, 0, 0);
        state = EnemyState.Move;
    }

    //�G�̏�Ԕ���
    void CheckState(EnemyState _state)
    {
        switch (_state)
        {
            case EnemyState.Move: //�ړ�
                EnemyMove();
                CheckCollision(playerManager.UpdatePlayerList());
                break;
            case EnemyState.CollisionWithPlayer: //Player�ւ̍U��
                Attack(_hostPlayer);
                CheckCollision(playerManager.UpdatePlayerList());
                break;
            case EnemyState.CollisionWithBase: //���_�ւ̍U��
                AttackToBase();
                CheckCollision(playerManager.UpdatePlayerList());
                break;
            case EnemyState.None:
                break;
            default:
                break;
        }
    }

    //���O�𔺂����U���^�C��
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

    //�ʏ�̈ړ�
    void EnemyMove()
    {
        transform.position += pos * Time.deltaTime;
    }

    //�����蔻��
    void CheckCollision(List<PlayerController> activePlayers)
    {
        //Player�Ƃ̓����蔻��
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

        //�v���C���[�̋��_�Ƃ̓����蔻��
        if (CollisionDetector.CheckRectCollision(gameObject.transform.position, size, _playerBasePosition, size))
        {
            state = EnemyState.CollisionWithBase;
            isHit = true;
            return;
        }

        isHit = false;
    }

    //Player�ւ̍U��
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

    //Base�ւ̍U��
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

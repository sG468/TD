using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;

    //Player�̏��
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
    public Vector2 size = new Vector2(1f, 1f); // �v���C���[�̋�`�T�C�Y
    public int health = 6;                  // �v���C���[�̗̑�
    public int attackPower = 1;              // �v���C���[�̍U����
    public bool isCollision = false;
    public bool isBaseCollision = false;

    private int healthStock = 0;
    private float direction = -1f;
    private float nextHitTime = 0f; //�o�ߎ���
    private float nextHitIntervalTime = 1.5f; //�C���^�[�o������
    private bool isHit = false; //�Փ˂��邩�ǂ���
    private Vector3 pos;
    private Vector2 _enemyBasePosition = new Vector2(-8.3f, 0f);
    private EnemyController _hostEnemy; //�����b�N���Ă��鑊��
    PlayerState state = PlayerState.None; //���݂̃v���C���[�̏�Ԃ̕ێ�
    //List<EnemyController> activeEnemies; //����ʏ�ɏo�Ă���G��ۗL����ꏊ

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
    //�v���C���[�̏o��
    public void PlayerSpawn()
    {
        float y = Random.Range(-1.0f, -2.0f);
        SpriteRenderer pl = GetComponent<SpriteRenderer>();
        pl.transform.position = new Vector3(9.6f, y, 0);
        pl.sortingOrder = (int)(-y * 10);
        state = PlayerState.Move;
        //health = healthStock;
    }

    //�_���[�W
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
    /// �v���C�x�[�g�֐�
    /// </summary>
    /// <param name="_state"></param>
   
    //�������̃f�[�^�̏�����
    void SetUp()
    {
        isHit = false;
        healthStock = health;
        nextHitTime = Time.time + nextHitIntervalTime;
        pos = new Vector3(direction, 0, 0);
        state = PlayerState.Move;
    }

    //Player�̏�Ԕ���
    void CheckState(PlayerState _state)
    {
        switch (_state)
        {
            case PlayerState.Move: //�ړ�
                PlayerMove();
                CheckCollision(enemyManager.UpdateEnemyList());
                break;
            case PlayerState.CollisionWithEnemy: //�G�ւ̍U��
                Attack(_hostEnemy);
                CheckCollision(enemyManager.UpdateEnemyList());
                break;
            case PlayerState.CollisionWithBase: //���_�ւ̍U��
                AttackToBase();
                CheckCollision(enemyManager.UpdateEnemyList());
                break;
            case PlayerState.None: //��
                break;
            default:
                break;
        }
    }

    //���O��݂����U���^�C��
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

    //�ʏ�̈ړ�
    void PlayerMove()
    {
        transform.position += pos * Time.deltaTime;
    }

    //�����蔻��
    void CheckCollision(List<EnemyController> activeEnemies)
    {
        //�G�Ƃ̓����蔻��
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

        //�G�̋��_�Ƃ̓����蔻��
        if (CollisionDetector.CheckRectCollision(gameObject.transform.position, size, _enemyBasePosition, size))  
        {
            state = PlayerState.CollisionWithBase;
            isHit = true;
            return;
        }

        isHit = false;
    }

    //Enemy�ւ̍U��
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

    //Base�ւ̍U��
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

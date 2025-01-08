using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseController : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;

    /// <summary>
    /// Base�p�̃Q�[�W��HP�̒l�̐ݒ�
    /// </summary>
    [SerializeField] private TextMeshProUGUI _playerBaseGauge;
    [SerializeField] private TextMeshProUGUI _enemyBaseGauge;
    [SerializeField] private int _playerBaseMaxHP;
    [SerializeField] private int _enemyBaseMaxHP;

    private int _playerBaseHP = 0;
    private int _enemyBaseHP = 0;
    
    private bool isStop = false;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    //�e�l�̐ݒ�
    void SetUp()
    {
        //BaseHP�̃Z�b�g�A�b�v
        _playerBaseHP = _playerBaseMaxHP;
        _enemyBaseHP = _enemyBaseMaxHP;

        //BaseGauge�̃Z�b�g�A�b�v
        _playerBaseGauge.text = _playerBaseHP.ToString() + "/" + _playerBaseMaxHP.ToString();
        _enemyBaseGauge.text = _enemyBaseHP.ToString() + "/" + _enemyBaseMaxHP.ToString();

        isStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStop)
        {
            BaseGaugeUpdate();
        }
        else
        {

        }
    }

    //Base�Q�[�W�̍X�V
    void BaseGaugeUpdate()
    {
        _playerBaseGauge.text = _playerBaseHP.ToString() + "/" + _playerBaseMaxHP.ToString();
        _enemyBaseGauge.text = _enemyBaseHP.ToString() + "/" + _enemyBaseMaxHP.ToString();
    }

    //�����ŁA����������������
    //public IEnumerator TakeDamage(string anyBase)
    //{
    //    yield return new WaitForSeconds(0.5f);

    //    switch (anyBase)
    //    {
    //        case "PlayerBase":
    //            PlayerBaseDamage();
    //            //StartCoroutine(PlayerBaseDamage());
    //            break;
    //        case "EnemyBase":
    //            EnemyBaseDamage();
    //            //StartCoroutine(EnemyBaseDamage());
    //            break;
    //        default:
    //            break;
    //    }

    //}

    public void PlayerBaseDamage()
    {
        _playerBaseHP--;
        if (_playerBaseHP == 0)
        {
            //gameManager.GameOver();
            isStop = true;
            gameManager.isGameOver = true;
        }
    }

    public void EnemyBaseDamage()
    {
        _enemyBaseHP--;
        if (_enemyBaseHP == 0)
        {
            //gameManager.Clear();
            isStop = true;
            gameManager.isGameClear = true;
        }
    }

    public bool IsBaseDestroyed(string baseName)
    {
        if (baseName == "player")
        {
            return _playerBaseHP == 1;
        }
        else if (baseName == "enemy")
        {
            return _enemyBaseHP == 1;
        }

        return false;
    }
}

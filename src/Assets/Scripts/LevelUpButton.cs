using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour
{
    Wallet wallet => Wallet.instance;

    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] int[] price;

    int level = 1;

    Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        UpdateLevelUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLevelMax())
        {
            if (wallet.nowCoin >= price[level - 1])
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }    
    }

    public void OnClick()
    {
        LevelUp();
    }

    //LevelUp�{�^���������ꂽ�Ƃ�
    void LevelUp()
    {
        if (isLevelMax())
        {
            //MAX�ɂȂ�Ƃ��̏���
            UpdateLevelUI();
        }
        else
        {
            //�K�v�ȋ��z�̏���
            wallet.nowCoin -= price[level - 1];

            //���x���グ
            level++;

            //�R�C���̃X�s�[�h���グ��
            wallet.coinSpeed += 6;

            //text�̕\����ύX
            UpdateLevelUI();

            //���z��maxCoin�̍X�V
            wallet.coinLevel++;
        }
    }

    //Level�����UI�̍X�V
    void UpdateLevelUI()
    {
        if (!isLevelMax())
        {
            levelText.text = "Level" + level.ToString();
            priceText.text = price[level - 1].ToString() + "Yen";
        }
        else
        {
            priceText.text = "MAX";
            levelText.gameObject.SetActive(false);
            button.interactable = true;
            button.enabled = false;
        }
    }

    //Level������ɒB������
    bool isLevelMax()
    {
        return level > price.Length;
    }
}

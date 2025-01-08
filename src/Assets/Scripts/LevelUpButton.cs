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

    //LevelUpボタンが押されたとき
    void LevelUp()
    {
        if (isLevelMax())
        {
            //MAXになるときの処理
            UpdateLevelUI();
        }
        else
        {
            //必要な金額の消費
            wallet.nowCoin -= price[level - 1];

            //レベル上げ
            level++;

            //コインのスピードを上げる
            wallet.coinSpeed += 6;

            //textの表示を変更
            UpdateLevelUI();

            //財布のmaxCoinの更新
            wallet.coinLevel++;
        }
    }

    //Level周りのUIの更新
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

    //Levelが上限に達したか
    bool isLevelMax()
    {
        return level > price.Length;
    }
}

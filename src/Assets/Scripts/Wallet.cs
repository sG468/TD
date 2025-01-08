using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Wallet : MonoBehaviour
{
    public static Wallet instance;

    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] int[] maxCoin;

    public float nowCoin = 0;
    public int coinSpeed = 6;
    public int coinLevel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        coinLevel = 0;
        coinText.text = nowCoin.ToString() + "/" + maxCoin[coinLevel].ToString() + "Yen";
    }

    // Update is called once per frame
    void Update()
    {
        if (coinLevel <= maxCoin.Length)
        {
            UpdateCoin();
        }
    }

    //画面右上の獲得コインと上限コインの更新
    void UpdateCoin()
    {
        if (GameManager.Instance.isGame && nowCoin <= maxCoin[coinLevel])
        {
            nowCoin += Time.deltaTime * coinSpeed;
            coinText.text = nowCoin.ToString("F0") + "/" + maxCoin[coinLevel].ToString() + "Yen";
        }
    }
}

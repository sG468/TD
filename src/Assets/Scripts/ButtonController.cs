using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;
    Wallet wallet => Wallet.instance;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpriteRenderer player;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] int price;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] Slider gauge;

    Button button;
    bool isClicked = false;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        priceText.text = price.ToString() + "Yen";
    }

    // Update is called once per frame
    void Update()
    {
        //ボタンを開放してよいか
        if (wallet.nowCoin >= price && !isClicked)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable= false;
        }
    }

    //ボタンが押されたら
    public void OnClick()
    {
        wallet.nowCoin -= price;
        playerManager.SpawnPlayer();
        isClicked = true;
        StartCoroutine(SliderUpdate());
    }

    //自分のキャラクターの生成
    //void PlayerSpawn()
    //{
    //    float y = Random.Range(-1.0f, -2.0f);
    //    SpriteRenderer pl = Instantiate(player, new Vector3(9.6f, y, 0), Quaternion.identity);
    //    pl.sortingOrder = (int)(-y * 10);
    //    PlayerController _player = pl.GetComponent<PlayerController>();
    //    gameManager.players.Add(_player);
    //}

    //ボタン開放待ちゲージ
    IEnumerator SliderUpdate()
    {
        gauge.value = 0;
        gauge.gameObject.SetActive(true);

        while (gauge.value < gauge.maxValue)
        {
            gauge.value++;
            yield return new WaitForSeconds(0.05f);
        }

        gauge.gameObject.SetActive(false);

        isClicked = false;
    }
}

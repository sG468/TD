using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitPoint : MonoBehaviour
{
    public int hp;

    [SerializeField] UnityEvent OnDamageEvent;
    [SerializeField] UnityEvent OnDestroyEvent;

    GameManager gameManager => GameManager.Instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int damage)
    {
        if (gameManager.isGame)
        {
            hp -= damage;

            if (OnDamageEvent != null)
            {
                OnDamageEvent.Invoke();
            }

            if (hp <= 0)
            {
                if (OnDestroyEvent != null)
                {
                    OnDestroyEvent.Invoke();
                }
            }
        }
    }
}

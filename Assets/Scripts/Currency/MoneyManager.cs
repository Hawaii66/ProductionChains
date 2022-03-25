using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProduktionChains.Money
{
    public class MoneyManager : MonoBehaviour
    {
        public static MoneyManager instance;

        public int money { get; private set; }

        public UnityEvent onMoneyChange;

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void Start()
        {
            money = 10_000_000;
            onMoneyChange.Invoke();
        }

        public int ModifyMoney(int change)
        {
            money += change;
            onMoneyChange.Invoke();
            return money;
        }
    }
}
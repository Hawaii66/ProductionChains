using System.Collections;
using UnityEngine;
using ProduktionChains.Money;
using TMPro;
using ProduktionChains.Utilities;

namespace ProduktionChains.UI
{
    public class MoneyUI : MonoBehaviour
    {
        public TextMeshProUGUI textField;
        public string searchTag;

        private string defaultText;

        private void Awake()
        {
            if(textField == null) { Destroy(gameObject); return; }

            defaultText = textField.text;
        }

        public void UpdateText()
        {
            string temp = defaultText;
            int money = MoneyManager.instance.money;
            string moneyString = money.ToString().ReverseAddSpace(0, 3);
            temp = temp.Replace(searchTag, moneyString);

            textField.text = temp;
        }
    }
}
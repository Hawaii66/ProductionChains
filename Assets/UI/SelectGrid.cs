using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using ProduktionChains.Items;

namespace ProduktionChains.UI
{
    public class SelectGrid : MonoBehaviour
    {
        public ItemType[] items;
        public GameObject itemPrefab;

        public void Render(ItemType[] i, UnityAction<int> onClick)
        {
            items = i;
            UpdateUI(onClick);
        }

        public void UpdateUI(UnityAction<int> onClick)
        {
            Transform[] transforms = new Transform[transform.childCount];
            int index = 0;
            foreach(Transform t in transform)
            {
                transforms[index] = t;
                index += 1;
            }

            foreach(Transform childTransform in transforms)
            {
                DestroyImmediate(childTransform.gameObject);
            }

            index = 0;
            foreach(ItemType type in items)
            {
                GameObject temp = Instantiate(itemPrefab);
                temp.transform.SetParent(transform);

                int tempIndex = index;
                temp.GetComponent<Image>().sprite = ItemManager.instance.GetItemScriptableObj(type).icon;
                temp.GetComponent<Button>().onClick.AddListener(delegate { onClick(tempIndex); });
                index += 1;
            }
        }
    }
}
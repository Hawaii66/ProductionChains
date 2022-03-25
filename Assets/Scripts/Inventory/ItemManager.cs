using System.Collections;
using UnityEngine;

namespace ProduktionChains.Items
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager instance;

        public ItemScriptableObj[] items;

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

        public ItemScriptableObj GetItemScriptableObj(ItemType type)
        {
            foreach(ItemScriptableObj obj in items)
            {
                if(obj.type == type)
                {
                    return obj;
                }
            }

            return null;
        }

        public Item GetItem(ItemType type)
        {
            return new Item(GetItemScriptableObj(type));
        }
    }
}
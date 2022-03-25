using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProduktionChains.Items
{
    public enum ItemType { None, Wood, Stone, Table };

    [System.Serializable]
    public class Item
    {
        public ItemType type;
    
        public Item()
        {
            type = ItemType.None;
        }

        public Item(ItemScriptableObj obj)
        {
            type = obj.type;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProduktionChains.Items
{
    [System.Serializable]
    public class SingleInventory
    {
        private ItemType[] acceptedItems;
        public ItemType type;
        public int inventory;
        public int maxCount;

        public SingleInventory(int max, ItemType[] a)
        {
            maxCount = max;
            inventory = 0;
            acceptedItems = a;
            type = ItemType.None;
        }

        public void Clear()
        {
            inventory = 0;
            type = ItemType.None;
        }

        private bool IsAcceptedType(ItemType type)
        {
            for(int i = 0; i < acceptedItems.Length; i++)
            {
                if(acceptedItems[i] == type)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AddItem(Item item)
        {
            if (!IsAcceptedType(item.type)) { return false; }
            if(type == ItemType.None) { type = item.type; }
            if(item.type != type) { return false; }
            if (!IsFull()) { return false; }

            inventory += 1;

            return true;
        }

        public bool IsFull()
        {
            if(inventory < maxCount) { return true; }
            return false;
        }
    }
}
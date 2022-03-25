using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProduktionChains.Items
{
    [CreateAssetMenu(fileName="Item",menuName = "ScriptableObj/Item")]
    public class ItemScriptableObj : ScriptableObject
    {
        public ItemType type;
        public new string name;
        public Sprite icon;
    }
}
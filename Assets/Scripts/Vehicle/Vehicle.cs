using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProduktionChains.Items;
using ProduktionChains.Utilities;

namespace ProduktionChains.Vehicles
{
    public enum EndAction { None, EnterBuilding };
    public class Vehicle : MonoBehaviour
    {
        public float speed;
        public float turnSpeed;
        public float snapTurn;

        public Coord gridPos;

        public ItemType[] acceptedItems;
        public SingleInventory inventory;

        public bool enableOne;
        public Transform meshesParent;

        public virtual void Start()
        {
            inventory = new SingleInventory(10, acceptedItems);

            UpdateGridPos();
            UpdateMesh();
        }

        public void UpdateGridPos()
        {
            float x = transform.position.x - GridManager.gridSize / 2;
            float y = transform.position.y - GridManager.gridSize / 2;
            float z = transform.position.z - GridManager.gridSize / 2;

            gridPos = new Coord(Mathf.CeilToInt(x / GridManager.gridSize), Mathf.CeilToInt(y / GridManager.gridSize), Mathf.CeilToInt(z / GridManager.gridSize));
        }

        public void UpdateMesh()
        {
            int currentValMapped = Mathf.RoundToInt(((float)inventory.inventory).Remap(0, inventory.maxCount, 0, meshesParent.childCount));

            int count = 0;
            foreach(Transform t in meshesParent)
            {
                if (enableOne)
                {
                    if (count == currentValMapped)
                    {
                        t.GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        t.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
                else
                {
                    if(count < currentValMapped)
                    {
                        t.GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        t.GetComponent<MeshRenderer>().enabled = false;
                    }
                }

                count += 1;
            }
        }
    }
}
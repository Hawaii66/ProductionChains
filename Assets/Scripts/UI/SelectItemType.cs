using System.Collections;
using UnityEngine;
using ProduktionChains.Items;

namespace ProduktionChains.UI
{
    public class SelectItemType : MonoBehaviour
    {
        public GameObject selectGridPrefab;
        public Canvas canvas;
        public ItemType[] items;
        [SerializeField] private Vector2 position;

        private SelectGrid grid;

        public void Toggle()
        {
            if (grid)
            {
                Destroy(grid.gameObject);
            }
            else
            {
                GameObject temp = Instantiate(selectGridPrefab);
                temp.transform.SetParent(canvas.transform);
                temp.transform.position = position;

                grid = temp.GetComponent<SelectGrid>();
                grid.Render(items, Clicked);
            }
        }

        private void Clicked(int index)
        {
            Debug.Log(items[index].ToString());
        }
    }
}
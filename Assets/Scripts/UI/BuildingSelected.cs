using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProduktionChains.Buildings;

namespace ProduktionChains.UI.Buildings
{
    public class BuildingSelected : MonoBehaviour
    {
        public Transform buildingsParent;
        public Transform current;

        private int currentIndex;
        private CellType currentType;

        [SerializeField] private float spaceBetween = 110f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UpdateCurrent(-1);
                BuildManager.instance.StartBuild(currentType);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                UpdateCurrent(1);
                BuildManager.instance.StartBuild(currentType);
            }

            if (Input.GetMouseButtonDown(1))
            {
                BuildManager.instance.CancelAll();
                current.transform.position = new Vector3(10000, 0, 0);
            }
        }

        private void UpdateCurrent(int move)
        {
            currentIndex += move;
            if(currentIndex < 0) { currentIndex = buildingsParent.childCount - 1; }
            if(currentIndex >= buildingsParent.childCount) { currentIndex = 0; }

            current.position = buildingsParent.GetChild(0).transform.position;
            current.position += new Vector3(spaceBetween * currentIndex, 0, 0);

            currentType = buildingsParent.GetChild(currentIndex).GetComponent<UIBuilding>().type;
        }
    }
}
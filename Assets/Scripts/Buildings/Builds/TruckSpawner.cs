using ProduktionChains.Vehicles;
using System.Collections;
using UnityEngine;
using ProduktionChains.Money;

namespace ProduktionChains.Buildings.Builds
{
    public class TruckSpawner : Building
    {
        public GameObject prefab;
        public float spawnSpeed;

        private Transform truckParent;

        public override void Start()
        {
            base.Start();
            truckParent = GameObject.Find("TruckParent").transform;
            StartCoroutine(SpawnTruckCoroutine());
        }

        public override void AddVehicle(Vehicle vehicle)
        {
            Destroy(vehicle.gameObject);
        }

        IEnumerator SpawnTruckCoroutine()
        {
            yield return new WaitForSeconds(0);

            while (true)
            {
                SpawnTruck();
                MoneyManager.instance.ModifyMoney(-100);

                yield return new WaitForSeconds(spawnSpeed);
            }
        }

        void SpawnTruck()
        {
            GameObject temp = Instantiate(prefab);
            temp.transform.position = gridPos * GridManager.gridSize;
            temp.transform.SetParent(truckParent);

            AIVehicle v = temp.GetComponent<AIVehicle>();
            v.missionIndex = -1;
            v.NextMission(gridPos);
        }
    }
}
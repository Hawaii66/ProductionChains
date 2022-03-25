using ProduktionChains.Vehicles;
using System.Collections;
using UnityEngine;
using ProduktionChains.Items;
using System.Collections.Generic;

namespace ProduktionChains.Buildings.Builds
{
    public class WoodPickup : Building
    {
        public Animator animator;
        private Queue<Vehicle> vehicles;

        public override void Start()
        {
            base.Start();

            vehicles = new Queue<Vehicle>();

            StartCoroutine(FillTruck());
        }

        public override void AddVehicle(Vehicle vehicle)
        {
            if(vehicle is AIVehicle)
            {
                ((AIVehicle)vehicle).isDriving = false;
            }

            vehicles.Enqueue(vehicle);
        }

        IEnumerator FillTruck()
        {
            while (true)
            {
                if(vehicles.Count > 0)
                {
                    Vehicle current = vehicles.Dequeue();
                    yield return new WaitForSeconds(0.2f);

                    animator.SetBool("Load", true);

                    for (int i = 0; i < 10; i++)
                    {
                        bool success = current.inventory.AddItem(ItemManager.instance.GetItem(ItemType.Wood));

                        current.UpdateMesh();

                        if (success)
                        {
                            yield return new WaitForSeconds(0.2f);
                        }
                        else
                        {
                            Debug.Log("No success");
                            break;
                        }
                    }

                    animator.SetBool("Load", false);
                    yield return new WaitForSeconds(0.75f);

                    if (current is AIVehicle)
                    {
                        AIVehicle ai = (AIVehicle)current;

                        ai.NextMission(gridPos);
                    }
                }

                yield return null;
            }
        }
    }
}
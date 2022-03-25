using ProduktionChains.Vehicles;
using System.Collections;
using UnityEngine;

namespace ProduktionChains.Buildings.Builds
{
    public class Destruction : Building
    {
        public override void AddVehicle(Vehicle vehicle)
        {
            Destroy(vehicle.gameObject);
        }
    }
}
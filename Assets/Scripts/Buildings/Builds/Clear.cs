using ProduktionChains.Vehicles;
using System.Collections;
using UnityEngine;

namespace ProduktionChains.Buildings.Builds
{
    public class Clear : Building
    {
        public override void AddVehicle(Vehicle vehicle)
        {
            vehicle.inventory.Clear();
            vehicle.UpdateMesh();

            if(vehicle is AIVehicle)
            {
                ((AIVehicle)vehicle).NextMission(gridPos);
            }
        }
    }
}
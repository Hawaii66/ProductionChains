using System.Collections;
using UnityEngine;
using ProduktionChains.Items;
using ProduktionChains.Vehicles;

namespace ProduktionChains.Buildings.Builds
{
    public class TableFactory : Building
    {
        private BuildingVehicle buildingVehicle;

        public override void Start()
        {
            base.Start();
        }

        public override void AddVehicle(Vehicle vehicle, Vector3 waypoint)
        {
            buildingVehicle = new BuildingVehicle(vehicle, waypoint);
            AddVehicle(vehicle);
        }

        public override void AddVehicle(Vehicle vehicle)
        {
            if (vehicle is AIVehicle vehicle1)
            {
                vehicle1.isDriving = false;
            }

            if(vehicle.inventory.type != ItemType.Wood)
            {
                return;
            }

            vehicle.inventory.Clear();

            for(int i = 0; i < 10; i++)
            {
                vehicle.inventory.AddItem(ItemManager.instance.GetItem(ItemType.Table));
            }

            vehicle.UpdateGridPos();

            if (vehicle is AIVehicle vehicle2)
            {
                vehicle2.NextMission(gridPos);
            }
        }

        private struct BuildingVehicle
        {
            public Vehicle vehicle;
            public Vector3 enterWaypoint;

            public BuildingVehicle(Vehicle v, Vector3 enter)
            {
                vehicle = v;
                enterWaypoint = enter;
            }
        }
    }
}
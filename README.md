# ProductionChains
Inspired by Voxel Tycoon. Trying to create a strategy game where you manage a fleet of trucks delivering items around the world.

# How To Use
__A bit complicated__
Still in development.
The only thing to experiment with right now is placing buildings and roads into the world in the editor.
- Find the folder named Prefabs/Buildings and drag out buildings or roads into the world. __OBS__ Make sure the direction of the building is correct. This is __not__ acived by rotating along the y-axis but instead changing a property in the script attachet on the object.
- Drag out roads, like the already placed roads, they wont attach to each other in the editor.
- Find the folder named Prefabs/Trucks and select the WoodTruck.
- Open the wood truck prefab.
- In the script AI-vehicle the variable named Mission controls how it drives.
- Change the grid cordinates to those of buildings on the map. (run the game and look at each buildings gridpos to find the correct cordinates)
- Add or remove stops. If the last mission is not towards a X the vehicle will loop around to the first mission again ones it is finished.

# Buildings
- Clear: Removes the inventory of a truck. In the inspector each truck has a inventory variable, this tile clears that inventory so the truck can take on new materials.
- Destruction: Removes the truck from the map completely.
- Stone pickup: Since the truck only accepts Wood & Table this is not usable at the time. Maybe if you modify the woodtruck to pick up stone it will work. (Haven't tested)
- Table Factory: Turns all wood a truck enters with into tabels when it exits.
- Truck Spawn: Spawns the truck in the prefab variable field. Create multiple vehicles with different missions and instantiate them with different truck spawns.
- Wood Pickup: Small animation playing while a truck that enters the building gets wood until full (10 st).

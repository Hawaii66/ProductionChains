using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProduktionChains.Buildings;

namespace ProduktionChains.Roads
{
    public class RoadPathfinding : MonoBehaviour
    {
        private static float clearAfter = 10f;
        private static float clearTime;
        private static Dictionary<StartEnd, Coord[]> foundPaths;

        private static void Init()
        {
            if (foundPaths == null)
            {
                clearTime = Time.time;
                foundPaths = new Dictionary<StartEnd, Coord[]>();
            }
        }

        private static Coord[] CachedPath(Coord start, Coord end)
        {
            if(clearTime + clearAfter < Time.time)
            {
                clearTime = Time.time;
                foundPaths.Clear();
                return null;
            }

            foreach(KeyValuePair<StartEnd,Coord[]> e in foundPaths)
            {
                if(e.Key.start == start && e.Key.end == end)
                {
                    return e.Value;
                }
            }

            return null;
        }

        public static Coord[] GetPath(Coord start, Coord end)
        {
            Init();

            Coord[] cachedPath = CachedPath(start, end);
            if (cachedPath != null) { return cachedPath; }

            Dictionary<Coord, Node> unvisitedList = new Dictionary<Coord, Node>();
            Dictionary<Coord, Node> visitedList = new Dictionary<Coord, Node>();

            foreach (Road r in RoadManager.instance.roadNetwork.Values)
            {
                Node n = new Node(r.gridPos.x, r.gridPos.z, -1);
                n.connections = r.GetEndConnections();
                unvisitedList.Add(n.gridPos, n);
            }

            if (!unvisitedList.ContainsKey(start) || !unvisitedList.ContainsKey(end))
            {
                Debug.Log("Cant find start and end node: " + start + " : " + end);
                return null;
            }

            unvisitedList[start].count = 0;

            Node current = unvisitedList[start];
            int count = 0;
            while (unvisitedList.Count > 0)
            {
                if (count > 1000)
                {
                    Debug.Log("Path to long: " + start + " : " + end);
                    return null;
                }
                count += 1;

                visitedList.Add(current.gridPos, current);
                for (int i = 0; i < current.connections.Length; i++)
                {
                    Coord c = current.connections[i];
                    Coord offsetCoord = c + current.gridPos;

                    if (unvisitedList.TryGetValue(offsetCoord, out Node nextNode))
                    {
                        if (nextNode.gridPos == end)
                        {
                            //Found path reverse path and return
                            unvisitedList[nextNode.gridPos].count = current.count + 1;
                            visitedList.Add(nextNode.gridPos, unvisitedList[nextNode.gridPos]);
                            return ReversePath(visitedList, start, nextNode.gridPos);
                        }

                        nextNode.count = current.count + 1; // +1 means it is a direct neighbour
                    }
                }

                unvisitedList.Remove(current.gridPos);

                current = null;
                int val = -1;
                foreach (Node n in unvisitedList.Values)
                {
                    if (n.count != -1)
                    {
                        if (val == -1 || val > n.count)
                        {
                            current = n;
                        }
                    }
                }

                if (current == null)
                {
                    return null; //Disconnected grid
                }
            }

            return null;
        }
        
        private static Coord[] ReversePath(Dictionary<Coord, Node> nodes, Coord start, Coord end)
        {
            List<Coord> pathNodes = new List<Coord>();

            pathNodes.Add(end);
            Coord current = end;
            
            int count = 0;
            while (current != start)
            {
                if (count > 1000)
                {
                    Debug.Log("To long path: " + start + " : " + end);
                    return pathNodes.ToArray();
                }
                count += 1;

                Node nextNode = null;

                foreach (Coord c in nodes[current].connections)
                {
                    Coord nextCoord = c + current;
                    if (nodes.TryGetValue(nextCoord, out Node tempNode))
                    {
                        if (nextNode == null)
                        {
                            nextNode = tempNode;
                            continue;
                        }

                        if (tempNode.count < nextNode.count)
                        {
                            nextNode = tempNode;
                        }
                    }
                }
                current = nextNode.gridPos;
                pathNodes.Add(current);
            }

            pathNodes.Reverse();

            foundPaths.Add(new StartEnd(start, end), pathNodes.ToArray());

            return pathNodes.ToArray();
        }

        public class Node
        {
            public Coord gridPos;
            public int count;
            public Coord[] connections;

            public Node(int x, int z, int count)
            {
                gridPos = new Coord(x, 0, z);
                this.count = count;
            }
        }

        private class StartEnd
        {
            public Coord start;
            public Coord end;

            public StartEnd(Coord s, Coord e)
            {
                start = s;
                end = e;
            }
        }
    }
}
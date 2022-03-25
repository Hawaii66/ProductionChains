using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProduktionChains
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager instance;
        public Dictionary<Coord, Cell> grid;

        public static int gridSize = 6;

        [Header("Debug")]
        [SerializeField] private bool drawGrid = false;

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
            }
            grid = new Dictionary<Coord, Cell>();   
        }

        public Cell GetCell(Coord pos)
        {
            if(grid.TryGetValue(pos, out Cell c))
            {
                return c;
            }

            return null;
        }

        public Cell GetCellCreate(Coord pos)
        {
            if(grid.TryGetValue(pos, out Cell c))
            {
                return c;
            }

            Cell cell = new Cell(pos, CellType.None);
            grid.Add(cell.gridPos, cell);
            return cell;
        }

        private void OnDrawGizmos()
        {
            if (drawGrid)
            {
                Gizmos.color = Color.black;
                foreach(Cell c in grid.Values)
                {
                    Gizmos.DrawWireCube(GridToWorldCoord(c.gridPos), new Vector3(gridSize, 1, gridSize));
                }
            }
        }

        public static Coord WorldToGridCoord(Coord worldCoord)
        {
            return worldCoord / gridSize;
        }

        public static Coord GridToWorldCoord(Coord gridCoord)
        {
            return gridCoord * 6;
        }

        public static Coord WorldToGridCoord(Vector3 pos)
        {
            return WorldToGridCoord(new Coord(pos));
        }

        public static Coord GridToWorldCoord(Vector3 pos)
        {
            return GridToWorldCoord(new Coord(pos));
        }
    }
}
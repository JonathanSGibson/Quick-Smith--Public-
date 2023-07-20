using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[System.Serializable]
public class GridColumn
{
   public GridColumn()
    {
        gridItems = new List<GameObject>();
    }
    public List<GameObject> gridItems;
}

public class GridSystem : MonoBehaviour
{
    public GameObject gridStart; //The object which is the start of the grid


    public List<GridColumn> grid; // [column][row in that column]

    
    /// <summary>
    /// Passes a gridstart and a grid (as created by the tool controlled by the GridWindow script)
    /// </summary>
    /// <param name="_gridStart">The object which is the start of the grid</param>
    /// <param name="_grid">The grid itself</param>
    public void SpawnGrid(GameObject _gridStart, List<GridColumn> _grid)
    {
        gridStart = _gridStart;
        grid = _grid;
    }

    /// <summary>
    /// Gets the index of the grid closest to the world position given
    /// </summary>
    /// <param name="position"></param>
    /// <returns>Returns the index if the position is inside the grid, returns default if not</returns>
    public Vector2Int GetIndexFromPosition(Vector3 position)
    {
        position -= gridStart.transform.position;
        Vector2Int returnIndex = new Vector2Int();
        returnIndex.x = Mathf.RoundToInt(position.x);
        returnIndex.y = Mathf.RoundToInt(position.z);
        if (grid.Count > returnIndex.x )
            if (grid[returnIndex.x].gridItems.Count > returnIndex.y)
                return returnIndex;

        Debug.LogWarning("Position outside of grid");
        return default;
            
    }

    /// <summary>
    /// Gets the world position of the grid index given
    /// </summary>
    /// <param name="x">X index of the grid</param>
    /// <param name="y">Y index of the grid (Z in space)</param>
    /// <returns>The world position of the given index</returns>
    public Vector3 GetPositionFromIndex(int x, int y)
    {
        if (grid.Count > x && grid[x].gridItems.Count > y)
        {
            Vector3 position = new Vector3(x, 0, y);
            position += gridStart.transform.position;
            return position;
        }
        else
        {
            Debug.LogWarning("Index out of range");
            return new Vector3();
        }
    }

    /// <summary>
    /// Snaps an object to the position of the grid index given
    /// </summary>
    /// <param name="objToSnap">The object to snap</param>
    /// <param name="x">The X index to snap to</param>
    /// <param name="y">The Y index of the grid to snap to (Z in transform)</param>
    public void SnapObjectToGrid(GameObject objToSnap, int x, int y)
    {
        if (CheckIndexInRange(x, y))
            objToSnap.transform.position = (GetPositionFromIndex(x, y));
        else
            Debug.LogWarning("Cannot snap object - index out of range");
    }

    /// <summary>
    /// Returns every object contained within a grid square at the given index
    /// </summary>
    /// <param name="x">X index to get objects from</param>
    /// <param name="y">Y index to get objects from (Z in transform)</param>
    /// <param name="mask">The layer to target; if you want to target every layer input ~0</param>
    /// <returns>All objects within the given square</returns>
    public List<GameObject> GetObjectsInIndex(int x, int y, LayerMask mask)
    {
        RaycastHit[] collisions = Physics.BoxCastAll(GetPositionFromIndex(x, y), new Vector3(0.25f, 0.25f, 0.25f), transform.up, new Quaternion(), Mathf.Infinity, mask);
        List<GameObject> allObjects = new List<GameObject>();
        foreach (RaycastHit hit in collisions)
        {
            allObjects.Add(hit.collider.gameObject);
        }
        return allObjects;
    }

    /// <summary>
    /// Checks if a given index is within the bounds of the grid
    /// </summary>
    /// <param name="x">The X index</param>
    /// <param name="y">The Y index (Z in transform)</param>
    /// <returns>Whether the index is in range</returns>
    public bool CheckIndexInRange(int x, int y)
    {
        if (x < grid.Count)
            if (y < grid[x].gridItems.Count)
                return true;
        return false;
    }
}

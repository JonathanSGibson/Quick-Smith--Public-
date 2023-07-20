#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridWindow : EditorWindow
{
    /* This script is entirely made by Jonathan
     * 
     * This script is not a part of the game but is instead a tool designed to
     * create a grid and snap objects to this grid
     * (this grid can then be used in game time but this script cannot)
     */

    private int columns = 1; //The number of columns to spawn
    private int rows = 1; //The number of rows to spawn
    private Vector3 startPos; //The start position of the grid

    private int xIndex = 0; //The X index to snap to
    private int yIndex = 0; //The Y index to snap to [NOTE: despite being the Y index of the GRID it is in the Z transform of unity]
    private GameObject objToSnap; //The object to snap to the grid
    private GameObject _gridStart; //The start of the grid to be snapped to (as multiple grids can exist simultaneously)

    bool displayGenerator; //If the generator tab should be displayed
    bool displaySnapper; //If the snap tab should be displayed

    [MenuItem("EGS Tools/Grid")] //The name of the tab in the toolbar of unity

    //GUI functions
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<GridWindow>();
    }

    /// <summary>
    /// Functions which controls the display of the spawning and generating the grid, and the calling of those functions
    /// </summary>
    private void OnGUI()
    {
        

        displayGenerator = EditorGUILayout.BeginFoldoutHeaderGroup(displayGenerator, "Grid Generator");
        if (displayGenerator)
        {
            columns = (int)EditorGUILayout.IntField("Grid column count", columns);
            rows = (int)EditorGUILayout.IntField("Grid row count", rows);
            startPos = (Vector3)EditorGUILayout.Vector3Field("Grid start position", startPos);
            if (GUILayout.Button("Generate"))
            {
                if (columns <= 0 || rows <= 0)
                    ShowNotification(new GUIContent("Please create a grid of at least 1 row and 1 column"));
                else
                    GenerateGrid(startPos, columns, rows);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        displaySnapper = EditorGUILayout.BeginFoldoutHeaderGroup(displaySnapper, "SnapToGrid");
        if (displaySnapper)
        {
            objToSnap = (GameObject)EditorGUILayout.ObjectField("To snap", objToSnap, typeof(GameObject), true);
            _gridStart = (GameObject)EditorGUILayout.ObjectField("Grid start", _gridStart, typeof(GameObject), true);
            xIndex = (int)EditorGUILayout.IntField("X index of grid", xIndex);
            yIndex = (int)EditorGUILayout.IntField("Y index of grid (Z in world space)", yIndex);
            if (GUILayout.Button("Snap"))
            {
                if (objToSnap == null)
                    ShowNotification(new GUIContent("No object selected to snap"));
                else if (_gridStart == null || !_gridStart.GetComponent<GridSystem>())
                    ShowNotification(new GUIContent("Please enter a valid grid"));
                else if (yIndex < 0 || xIndex < 0)
                    ShowNotification(new GUIContent("Please enter two positive indices"));
                else if (!_gridStart.GetComponent<GridSystem>().CheckIndexInRange(xIndex, yIndex))
                    ShowNotification(new GUIContent("Index out of range"));
                else
                    SnapObjectToGrid(objToSnap, _gridStart, xIndex, yIndex);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.TextArea("To generate a grid enter the rows and columns (this is the count, not the index, so the max index is one lower than this value)\nThen enter the start position(The position of every square in the grid is offset by this position)\nThen hit generate\nThis will create a \"GridStart\" which contains the script required for other grid functions\nEvery point in the grid is a child of \"GridStart\"\n\nTo snap an object to a grid drag in the object to snap\nThen drag in the \"GridStart\" object of that grid\nThen input the index you want the object to snap to(Remember the max index is one lower than the count!)\nThen hit snap and it will move the object's world position to align with that grid coordinate\n");
    }

    /// <summary>
    /// Spawns a grid by creating a 2D array of empty game objects and setting their positions
    /// </summary>
    /// <param name="startPos">The position for the start of the grid</param>
    /// <param name="columns">The number of columns to spawn</param>
    /// <param name="rows">The number of rows to spawn</param>
    void GenerateGrid(Vector3 startPos, int columns, int rows)
    {
        GameObject gridStart = new GameObject("GridStart");
        gridStart.AddComponent<GridSystem>();
        gridStart.tag = "Grid";
        gridStart.transform.position = startPos;

        List<GridColumn> grid = new List<GridColumn>();
        
        //Create each column
        while (grid.Count < columns)
            grid.Add(new GridColumn());

        //Create each item and make them a child of the gridstart
        for (int i = 0; i < columns; i++)
        {
            for (int n = 0; n < rows; n++)
            {
                string name = "[" + i + "]" + "[" + n + "]";
                grid[i].gridItems.Add(new GameObject(name));
                grid[i].gridItems[n].transform.parent = gridStart.transform;
                grid[i].gridItems[n].transform.localPosition = new Vector3(i, 0, n);  
            }
        }
        gridStart.gameObject.GetComponent<GridSystem>().SpawnGrid(gridStart, grid);
        _gridStart = gridStart;
    }

    /// <summary>
    /// Snaps an object to a grid based on the grid position
    /// </summary>
    /// <param name="objToSnap">The object to snap to the grid</param>
    /// <param name="gridStart">The start of the grid to snap to</param>
    /// <param name="x">The X index of the grid position to snap to</param>
    /// <param name="y">The Y index of the grid position to snap to</param>
    void SnapObjectToGrid(GameObject objToSnap, GameObject gridStart, int x, int y)
    {
        if (gridStart.GetComponent<GridSystem>())
            objToSnap.transform.position = (gridStart.GetComponent<GridSystem>().GetPositionFromIndex(x, y));
        else
            ShowNotification(new GUIContent("Please enter a valid grid"));
    }
}
#endif
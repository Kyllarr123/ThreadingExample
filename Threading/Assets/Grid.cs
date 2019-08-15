using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Russell
{
    public class Grid : MonoBehaviour
    {
        public Node[,] grid;
        public Vector2Int gridSize;
        public Vector3 startPos;
        public Vector3 endPos;
        public GameObject floor;
        private float xSize;
        private float ySize;

        private void Start()
        {
            grid = new Node[gridSize.x,gridSize.y];
            GenerateGrid();
        }

        public void GenerateGrid()
        {

            startPos = floor.GetComponent<Collider>().bounds.min;
            endPos = floor.GetComponent<Collider>().bounds.max;

            float distanceX = Mathf.Abs(endPos.x - startPos.x);
            float distanceY = Mathf.Abs(endPos.z - startPos.z);
            
            xSize = distanceX / gridSize.x;
            ySize = distanceY / gridSize.y;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    
                    Vector3 newPos = new Vector3(startPos.x + xSize * y, startPos.y + 1, startPos.z + ySize * x);
                    grid[x,y] = new Node();

                    //grid[x, y].position = new Vector3( (gridSize.x * x), 1, gridSize.y * y);
                    grid[x, y].position = newPos - Vector3.up;
                    grid[x,y].gridPosition = new Vector2Int(x, y);
                    if (Physics.CheckBox(newPos, new Vector3(xSize/2, 0.5f, ySize/2), Quaternion.identity))
                    {
                        grid[x, y].isBlocked = true;
                    }
                    else grid[x, y].isBlocked = false;

                }
            }
        }

        private void OnDrawGizmos()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (grid[x, y].isBlocked)
                    {
                        Gizmos.color = Color.red;
                        
                    }else Gizmos.color = Color.white;
                    Gizmos.DrawCube(new Vector3(grid[x,y].position.x, grid[x,y].position.y,grid[x,y].position.z), Vector3.one);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Russell
{
    public class AStar : MonoBehaviour
    {
        public List<Node> closedNodes = new List<Node>();
        private Node currentNode;
  
        public Grid gridManager;

        public List<Node> openNodes = new List<Node>();

        private Node startNode;
        private Node target;
        public List<Node> finalPath = new List<Node>();
        private bool finished = false;
        System.Random random = new System.Random();
        public bool runThreads = true;
        // Start is called before the first frame update

        private void Awake()
        {
            gridManager = GetComponent<Grid>();
        }

        private void Start()
        {

            NewPath();
        }

        public void NewPath()
        {
            if (runThreads)
            {
                //StartCoroutine(ThreadPathing());
                Thread t = new Thread(ThreadNewPath);
                t.Start();
            } else if (!runThreads)
            {
                StartCoroutine(RunPathFind());
            }
            
            
        }

        int RandomInts(int x, int y)
        {
            int rand = random.Next(x, y);
            return rand;
        }

        private IEnumerator ThreadPathing()
        {
            yield return new WaitForSeconds(1);
            Thread t = new Thread(ThreadNewPath);
            t.Start();
        }

        public void Rerun()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }



        private void ThreadNewPath()
        {
            //yield return new WaitForSeconds(1);
            Searching();
            //yield return new WaitForSeconds(1);
            while (currentNode != target)
            {
                FindPath();
                //yield return new WaitForSeconds(0.0001f);
            }
            if (currentNode == target)
            {
                Debug.Log("Finished");
            }
            
            Node current = target;
            while (current.parentNode != null)
            {
                finalPath.Add(current);
                current = current.parentNode;
            }
            finalPath.Reverse();
            finished = true;

        }

        private void Searching()
        {
            startNode = gridManager.grid[RandomInts(0, gridManager.gridSize.x),
                RandomInts(0, gridManager.gridSize.y)];
            if (startNode.isBlocked)
                startNode = gridManager.grid[RandomInts(0, gridManager.gridSize.x),
                    RandomInts(0, gridManager.gridSize.y)];
            //Debug.Log("Got Start Location");
            target = gridManager.grid[RandomInts(0, gridManager.gridSize.x),
                RandomInts(0, gridManager.gridSize.y)];
            if (target.isBlocked)
                target = gridManager.grid[RandomInts(0, gridManager.gridSize.x),
                    RandomInts(0, gridManager.gridSize.y)];
            openNodes.Add(startNode);
            currentNode = startNode;
        }

        public void LowestFCost()
        {
            //Debug.Log("check next current");
            Node checkNode = new Node();
            checkNode.fCost = float.MaxValue + (Vector3.Distance(checkNode.position,target.position)*5);

            foreach (Node node in openNodes)
                if (checkNode.fCost > node.fCost)
                    checkNode = node;

            currentNode = checkNode;
        }

        public void FindPath()
        {
            Debug.Log("Find Path");
            if (openNodes.Contains(currentNode))
                openNodes.Remove(currentNode);
            for (var x = -1; x < 2; x++)
            for (var y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue;
                if ( (currentNode.gridPosition.x + x) < 0 ||
                     (currentNode.gridPosition.x + x) > gridManager.gridSize.x - 1) continue;
                if ( (currentNode.gridPosition.y + y) < 0 ||
                    (currentNode.gridPosition.y + y) > gridManager.gridSize.y - 1) continue;
                Node neighbour = gridManager.grid[(int) (currentNode.gridPosition.x + x), (int) (currentNode.gridPosition.y + y)];

                if (neighbour.isBlocked || closedNodes.Contains(neighbour)) continue;


                //Calc Costs                    
                float dist = Mathf.Abs(x) + Mathf.Abs(y) > 1 ? 1.4f : 1;
                float hCost = Vector2.Distance(neighbour.gridPosition, target.gridPosition);
                
                float gCost = currentNode.gCost + dist;
                float newFCost = hCost + gCost;
                
                if (neighbour.fCost < newFCost) continue;
                neighbour.parentNode = currentNode;
                neighbour.hCost = hCost;
                neighbour.gCost = gCost;
                neighbour.fCost = newFCost;
                
                if (!openNodes.Contains(neighbour))
                    openNodes.Add(neighbour);
            }

            if (!closedNodes.Contains(currentNode))
                closedNodes.Add(currentNode);
            LowestFCost();
        }
        
        private IEnumerator RunPathFind()
        {
            yield return new WaitForSeconds(1);
            startNode = gridManager.grid[Random.Range(0, gridManager.gridSize.x),
                Random.Range(0, gridManager.gridSize.y)];
            if (startNode.isBlocked)
                startNode = gridManager.grid[Random.Range(0, gridManager.gridSize.x),
                    Random.Range(0, gridManager.gridSize.y)];
            Debug.Log("Got Start Location");
            target = gridManager.grid[Random.Range(0, gridManager.gridSize.x),
                Random.Range(0, gridManager.gridSize.y)];
            if (target.isBlocked)
                target = gridManager.grid[Random.Range(0, gridManager.gridSize.x),
                    Random.Range(0, gridManager.gridSize.y)];
            openNodes.Add(startNode);
            currentNode = startNode;
            yield return new WaitForSeconds(1);
            while (currentNode != target)
            {
                FindPath();
                yield return new WaitForSeconds(0.0001f);
            }
            if (currentNode == target)
            {
                Debug.Log("Finished");
            }
            
            Node current = target;
            while (current.parentNode != null)
            {
                finalPath.Add(current);
                current = current.parentNode;
            }
            finalPath.Reverse();
            finished = true;

        }

        // Update is called once per frame
        private void Update()
        {
            if (finished)
            {
                StartCoroutine(WaitASec());
            }
        }

        private void OnDrawGizmos()
        {
            if (finished)
            {
                var scale = new Vector3(4, 4, 4);
                Gizmos.color = Color.blue;
                if (startNode != null) Gizmos.DrawCube(startNode.position, scale);
                Gizmos.color = Color.yellow;
                if (target != null) Gizmos.DrawCube(target.position, scale);
                Gizmos.color = Color.green;
                if (currentNode != null) Gizmos.DrawCube(currentNode.position, scale);

                Gizmos.color = Color.red;
                if (openNodes != null)
                    foreach (Node node in openNodes)
                        Gizmos.DrawCube(node.position, scale);
                Gizmos.color = Color.black;
                if (closedNodes != null)
                    foreach (Node node in closedNodes)
                        Gizmos.DrawCube(node.position, scale);
                Gizmos.color = Color.green;
                if (closedNodes != null)
                    foreach (Node node in finalPath)
                        Gizmos.DrawCube(node.position, scale);

            }

        }

        IEnumerator WaitASec()
        {
            yield return new WaitForSeconds(5);
            Rerun();
        }
    }
}
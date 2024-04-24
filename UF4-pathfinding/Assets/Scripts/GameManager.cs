using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GameManager : MonoBehaviour
{
    public GameObject token1, token2, token3;
    private int[,] GameMatrix; //0 not chosen, 1 player, 2 enemy
    private int[] startPos = new int[2];
    private int[] objectivePos = new int[2];
    private void Awake()
    {
        GameMatrix = new int[Calculator.length, Calculator.length];

        for (int i = 0; i < Calculator.length; i++) //fila
            for (int j = 0; j < Calculator.length; j++) //columna
                GameMatrix[i, j] = 0;

        //randomitzar pos final i inicial;
        var rand1 = Random.Range(0, Calculator.length);
        var rand2 = Random.Range(0, Calculator.length);
        startPos[0] = rand1;
        startPos[1] = rand2;
        SetObjectivePoint(startPos);

        GameMatrix[startPos[0], startPos[1]] = 1;
        GameMatrix[objectivePos[0], objectivePos[1]] = 2;

        InstantiateToken(token1, startPos);
        InstantiateToken(token2, objectivePos);
        ShowMatrix();
        Debug.Log(startPos[0] + ", " + startPos[1]);
    }
    private void InstantiateToken(GameObject token, int[] position)
    {
        Instantiate(token, Calculator.GetPositionFromMatrix(position),
            Quaternion.identity);
    }
    private void SetObjectivePoint(int[] startPos) 
    {
        var rand1 = Random.Range(0, Calculator.length);
        var rand2 = Random.Range(0, Calculator.length);
        if (rand1 != startPos[0] || rand2 != startPos[1])
        {
            objectivePos[0] = rand1;
            objectivePos[1] = rand2;
        }
    }

    private void ShowMatrix() //fa un debug log de la matriu
    {
        string matrix = "";
        for (int i = 0; i < Calculator.length; i++)
        {
            for (int j = 0; j < Calculator.length; j++)
            {
                matrix += GameMatrix[i, j] + " ";
            }
            matrix += "\n";
        }
        Debug.Log(matrix);
    }
    //EL VOSTRE EXERCICI COMENÇA AQUI
    private bool EvaluateWin()
    {
        return false;
    }

    private void Update()
    {
        if (!EvaluateWin())
        {
            List<Vector2Int> path = FindPath(startPos, objectivePos);
        }
    }

    private List<Vector2Int> FindPath(int[] startPos, int[] objectivePos)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        List<Node> openList = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        Node startNode = new Node(startPos, null, 1, Calculator.CheckDistanceToObj(startPos, objectivePos));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].totalCost < currentNode.totalCost || (openList[i].totalCost == currentNode.totalCost && openList[i].heuristicCost < currentNode.heuristicCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            InstantiateToken(token3, currentNode.position);

            closedSet.Add(currentNode);
            Debug.Log(currentNode);
            if (currentNode.position[0] == objectivePos[0] && currentNode.position[1] == objectivePos[1])
            {
                // Reconstruir el camino
                while (currentNode != null)
                {
                    path.Add(new Vector2Int(currentNode.position[0], currentNode.position[1]));
                    currentNode = currentNode.parent;
                }
                path.Reverse();
                break;
            }

            foreach (var neighbor in GetNeighbors(currentNode.position))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }


                var neighborPosition = new int[2] { neighbor.position[0], neighbor.position[1] }; 

                float gCost = currentNode.pathCost + Calculator.CheckDistanceToObj(currentNode.position, neighborPosition);
                float hCost = Calculator.CheckDistanceToObj(neighborPosition, objectivePos);
                Node neighborNode = new Node(neighborPosition, currentNode, gCost, hCost);

                if (openList.Contains(neighborNode) && gCost >= neighborNode.pathCost)
                {
                    continue;
                }

                openList.Add(neighborNode);
            }
        }

        return path;
    }

    private List<Node> GetNeighbors(int[] currentPosition)
    {
        List<Node> neighbors = new List<Node>();

        // Implementa la lógica para obtener las posiciones de los vecinos (movimientos permitidos en el ajedrez)

        return neighbors;
    }



}

public class Node
{
    public int[] position { get; private set; }
    public Node parent { get; set; }
    public float pathCost { get; set; } // Costo desde el nodo inicial hasta este nodo
    public float heuristicCost { get; set; } // Costo estimado desde este nodo hasta el nodo objetivo

    public float totalCost { get { return pathCost + heuristicCost; } } // Costo total (suma de GCost y HCost)

    public Node(int[] position, Node parent, float gCost, float hCost)
    {
        this.position = position;
        this.parent = parent;
        pathCost = gCost;
        heuristicCost = hCost;
    }
}

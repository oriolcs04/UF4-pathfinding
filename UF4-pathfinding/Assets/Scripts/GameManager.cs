using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject token1, token2, token3, token4;
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
        if (position[0] >= 0 && position[0] <= 7 && position[1] >= 0 && position[1] <= 7)
        {
            var matrixPos = Calculator.GetPositionFromMatrix(position);
            var zPos = 0f;

            if (token == token4)
            {
                zPos = -1f;
            }
            Instantiate(token, new Vector3(matrixPos.x, matrixPos.y, zPos),
                Quaternion.identity);
        } 
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
    //EL VOSTRE EXERCICI COMEN�A AQUI

    public class MyNode
    {
        public int[] position { get; private set; }
        public MyNode parent { get; set; }
        public float pathCost { get; set; } // Costo desde el nodo inicial hasta este nodo
        public float heuristicCost { get; set; } // Costo estimado desde este nodo hasta el nodo objetivo

        public float totalCost { get { return pathCost + heuristicCost; } } // Costo total (suma de GCost y HCost)

        public MyNode(int[] position, MyNode parent, int[] objectivePos)
        {
            this.position = position;
            this.parent = parent;
            pathCost = 0.2f;
            heuristicCost = Calculator.CheckDistanceToObj(position, objectivePos);
        }

        public MyNode(int[] objectivePos) 
        { 
            this.position = new int[2] {0, 0};
            this.parent = null;
            pathCost = 0.2f;
            heuristicCost = Calculator.CheckDistanceToObj(position, objectivePos);

        }


    }

    public List<MyNode> OpenList = new List<MyNode>();
    public List<MyNode> ClosedList = new List<MyNode>();
    public MyNode initialNode;
    public MyNode finalNode;

    private void Start()
    {
        initialNode = new MyNode(startPos, null, objectivePos);
        finalNode = new MyNode(objectivePos, null, objectivePos);

        DoSomethingUsefulFuckingUlgyBitch();
    }

    private bool EvaluateWin(MyNode actualNode)
    {
        return actualNode.position[0] == finalNode.position[0] && actualNode.position[1] == finalNode.position[1];
    }
    private void DoSomethingUsefulFuckingUlgyBitch()
    {
        CheckFollowingNodes(initialNode);
    }

    private void CheckFollowingNodes(MyNode parent)
    {
        MyNode NNode = new MyNode(new int[] { parent.position[0], parent.position[1] - 1 }, parent, objectivePos);
        MyNode SNode = new MyNode(new int[] { parent.position[0], parent.position[1] + 1 }, parent, objectivePos);
        MyNode WNode = new MyNode(new int[] { parent.position[0] - 1, parent.position[1] }, parent, objectivePos);
        MyNode ENode = new MyNode(new int[] { parent.position[0] + 1, parent.position[1] }, parent, objectivePos);

        OpenList.Add(NNode);
        OpenList.Add(SNode);
        OpenList.Add(WNode);
        OpenList.Add(ENode);

        

        CheckOpenList();
    }

    private void CheckOpenList()
    {
        MyNode bestNode = initialNode;

        Debug.Log("bestNodeCreated");

        foreach (MyNode node in OpenList)
        {
            InstantiateToken(token3, node.position);

            if (bestNode.totalCost > node.totalCost)
            {
                Debug.Log("Found bestNode");
                bestNode = node;
            }
        }

        OpenList.Remove(bestNode);
        ClosedList.Add(bestNode);

        if (EvaluateWin(bestNode) == true)
        {
            foreach (MyNode node in ClosedList)
            {
                InstantiateToken(token4, node.position);
            }
            return;
        } 

        CheckFollowingNodes(bestNode);
    }

    //private void Update()
    //{
    //    if (!EvaluateWin())
    //    {
    //        DoSomethingUsefulFuckingUlgyBitch();
    //    }
    //}

    //quitar el update
    //funcion lista abierta 
    //funcion lista cerrada (heu + cost menor se mete y se quita de abierta)
    //compruevas win
    //private MyNode[] AllNodes()
    //{
    //    MyNode[] myNodes = new MyNode[10];
    //    return myNodes;
    //}
}








//    private List<Vector2Int> FindPath(int[] startPos, int[] objectivePos)
//    {
//        List<Vector2Int> path = new List<Vector2Int>();

//        List<Node> openList = new List<Node>();
//        HashSet<Node> closedSet = new HashSet<Node>();

//        Node startNode = new Node(startPos, null, 1, Calculator.CheckDistanceToObj(startPos, objectivePos));
//        openList.Add(startNode);

//        while (openList.Count > 0)
//        {
//            Node currentNode = openList[0];
//            for (int i = 1; i < openList.Count; i++)
//            {
//                if (openList[i].totalCost < currentNode.totalCost || (openList[i].totalCost == currentNode.totalCost && openList[i].heuristicCost < currentNode.heuristicCost))
//                {
//                    currentNode = openList[i];
//                }
//            }

//            openList.Remove(currentNode);
//            InstantiateToken(token3, currentNode.position);

//            closedSet.Add(currentNode);
//            Debug.Log(currentNode);
//            if (currentNode.position[0] == objectivePos[0] && currentNode.position[1] == objectivePos[1])
//            {
//                // Reconstruir el camino
//                while (currentNode != null)
//                {
//                    path.Add(new Vector2Int(currentNode.position[0], currentNode.position[1]));
//                    currentNode = currentNode.parent;
//                }
//                path.Reverse();
//                break;
//            }

//            foreach (var neighbor in GetNeighbors(currentNode.position))
//            {
//                if (closedSet.Contains(neighbor))
//                {
//                    continue;
//                }


//                var neighborPosition = new int[2] { neighbor.position[0], neighbor.position[1] }; 

//                float gCost = currentNode.pathCost + Calculator.CheckDistanceToObj(currentNode.position, neighborPosition);
//                float hCost = Calculator.CheckDistanceToObj(neighborPosition, objectivePos);
//                Node neighborNode = new Node(neighborPosition, currentNode, gCost, hCost);

//                if (openList.Contains(neighborNode) && gCost >= neighborNode.pathCost)
//                {
//                    continue;
//                }

//                openList.Add(neighborNode);
//            }
//        }

//        return path;
//    }

//    private List<Node> GetNeighbors(int[] currentPosition)
//    {
//        List<Node> neighbors = new List<Node>();

//        // Implementa la l�gica para obtener las posiciones de los vecinos (movimientos permitidos en el ajedrez)

//        return neighbors;
//    }



//}


//}
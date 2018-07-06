using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameGridBuilder : MonoBehaviour
{
    public static GameGridBuilder Instance;

    [Header("Prefabs")]
    public GameObject greenSquarePrefab;
    public GameObject bunkerSquarePrefab;
    public GameObject holeSquarePrefab;
    public GameObject ballPrefab;

    [Header("Colors")]
    public List<Color> greenColors;
    public List<Color> bunkerColors;
    public Color cellOnPathColor;
    public Color selectableCellColor;
    public Color selectedCellColor;

    [Header("Hierarchy")]
    public Transform gridParent;
    public Transform ballsParent;

    [Header("Grid Attributes")]
    public int gridWidth;
    public int gridHeight;
    public float gridCellSize;

    [Header("Pointers")]
    public GameObject[,] displayedGrid;


    // Use this for initialization
    void Start ()
    {
        GameGridBuilder.Instance = this;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void BuildGrid(HerugolfGameState gridState)
    {
        gridWidth = Mathf.RoundToInt(gridState.gridDimensions.x);
        gridHeight = Mathf.RoundToInt(gridState.gridDimensions.y);
        displayedGrid = new GameObject[gridWidth, gridHeight];
        for (int xi = 0; xi < gridState.gridDimensions.x; xi++)
        {
            for (int yi = 0; yi < gridState.gridDimensions.y; yi++)
            {
                GolfCell cellData = gridState.GetCell(xi, yi);
                displayedGrid[xi,yi] = BuildSquare(xi, yi, cellData);
            }
        }
        foreach (Ball ball in gridState.balls)
        {
            AddBall(ball);
        }
    }

    public void SelectableSquare(Ball ball, int coordX, int coordY)
    {
        int dx, dy;
        dx = coordX > ball.positionOnGridX ? 1 : (coordX < ball.positionOnGridX ? -1 : 0);
        dy = coordY > ball.positionOnGridY ? 1 : (coordY < ball.positionOnGridY ? -1 : 0);

        int xc = ball.positionOnGridX;
        int yc = ball.positionOnGridY;

        while ((xc != coordX) || (yc != coordY))
        {
            xc += dx;
            yc += dy;
            displayedGrid[xc, yc].GetComponent<GridCellBehaviour>().SetHovered(true);
        }

        displayedGrid[coordX, coordY].GetComponent<GridCellBehaviour>().SetSelectable(true);
    }

    public void ResetSelectableSquares()
    {
        for (int xi = 0; xi < gridWidth; xi++)
        {
            for (int yi = 0; yi < gridHeight; yi++)
            {
                displayedGrid[xi, yi].GetComponent<GridCellBehaviour>().SetHovered(false);
                displayedGrid[xi, yi].GetComponent<GridCellBehaviour>().SetSelectable(false);
            }
        }
    }

    private GameObject BuildSquare(int coordX, int coordY, GolfCell dataCell)
    {
        GameObject cellPrefab = null;
        switch (dataCell.type)
        {
            case CellType.GREEN:
                cellPrefab = greenSquarePrefab;
                break;
            case CellType.BUNKER:
                cellPrefab = bunkerSquarePrefab;
                break;
            case CellType.HOLE_EMPTY:
                cellPrefab = holeSquarePrefab;
                break;
        }
        Vector3 cellPosition = new Vector3(coordX + 0.5f - gridWidth / 2.0f, 0, coordY + 0.5f - gridHeight / 2.0f) * gridCellSize;
        GameObject squareGo = Instantiate(cellPrefab, cellPosition, Quaternion.identity, gridParent);
        squareGo.GetComponent<GridCellBehaviour>().Init(dataCell);
        return squareGo;
    }

    private void AddBall(Ball ball)
    {
        Vector3 ballPosition = new Vector3(ball.positionOnGridX + 0.5f - gridWidth / 2.0f, 0.41f, ball.positionOnGridY + 0.5f - gridHeight / 2.0f) * gridCellSize;
        GameObject ballGo = Instantiate(ballPrefab, ballPosition, Quaternion.identity, ballsParent);
        ballGo.GetComponent<BallBehaviour>().SetBall(ball);
    }
}

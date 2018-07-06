using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    GREEN,
    BUNKER,
    LAKE,
    ROCK,
    STOP,
    HOLE_EMPTY,
    HOLE_FILLED
}

public enum Direction
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}

public class GolfCell
{
    public CellType type;

    public int x;
    public int y;

    public GolfCell(int nx, int ny, CellType t)
    {
        x = nx;
        y = ny;
        type = t;
    }

    public bool BallCanPassThrough()
    {
        bool res = false;
        switch (type)
        {
            case CellType.BUNKER:
            case CellType.GREEN:
            case CellType.LAKE:
                res = true;
                break;
            case CellType.HOLE_EMPTY:
            case CellType.HOLE_FILLED:
            case CellType.ROCK:
            case CellType.STOP:
                res = false;
                break;
        }
        return res;
    }

    public bool BallCanStopOn()
    {
        bool res = false;
        switch (type)
        {
            case CellType.GREEN:
            case CellType.HOLE_EMPTY:
                res = true;
                break;
            case CellType.HOLE_FILLED:
            case CellType.BUNKER:
            case CellType.LAKE:
            case CellType.ROCK:
            case CellType.STOP:
                res = false;
                break;
        }
        return res;
    }
}

public class Ball
{
    public int numberOfStrokes;

    public int positionOnGridX;
    public int positionOnGridY;

    private List<Vector2> positionsHistory;

    public List<Vector2> PositionHistory()
    {
        return positionsHistory;
    }
    public void AddPositionInHistory(Vector2 pos)
    {
        positionsHistory.Add(pos);
    }

    public Ball(int x, int y, int count)
    {
        numberOfStrokes = count;
        positionOnGridX = x;
        positionOnGridY = y;
        positionsHistory = new List<Vector2>();
    }
}

[System.Serializable]
public class HerugolfGameState
{
    public Vector2 gridDimensions;

    private GolfCell[,] gridArray;

    public List<Ball> balls;

    public HerugolfGameState(int width, int height)
    {
        gridDimensions = new Vector2(width, height);
        gridArray = new GolfCell[width, height];
        balls = new List<Ball>();

        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                gridArray[xi, yi] = new GolfCell(xi, yi, CellType.GREEN);
            }
        }
    }

    public Ball GetBall(int x, int y)
    {
        Ball ball = null;
        foreach (Ball b in balls)
        {
            if (b.positionOnGridX == x && b.positionOnGridY == y)
            {
                ball = b;
                break;
            }
        }
        return ball;
    }

    public GolfCell GetCell(int x, int y)
    {
        if (x < 0 || x >= gridDimensions.x || y < 0 || y >= gridDimensions.y)
        {
            throw new System.Exception("Cell (" + x + "," + y + ") doesn't exist");
        }
        return gridArray[x,y];
    }
    public GolfCell GetCell(Vector2 position)
    {
        return GetCell(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    private GolfCell ReachableCellWithBallAndDirection(Ball ball, Direction dir)
    {
        bool cellIsOkay = true;
        GolfCell cell = null;
        try
        {
            Ball b = null;
            for (int i = 1; i < ball.numberOfStrokes; i++)
            {
                int xi = ball.positionOnGridX + (dir == Direction.LEFT ? -1 : (dir == Direction.RIGHT ? 1 : 0)) * i;
                int yi = ball.positionOnGridY + (dir == Direction.UP ? 1 : (dir == Direction.DOWN ? -1 : 0)) * i;
                cell = this.GetCell(xi, yi);
                b = GetBall(xi, yi);
                cellIsOkay &= (cell.BallCanPassThrough() && b == null);
            }
            int xmax = ball.positionOnGridX + (dir == Direction.LEFT ? -1 : (dir == Direction.RIGHT ? 1 : 0)) * ball.numberOfStrokes;
            int ymax = ball.positionOnGridY + (dir == Direction.UP ? 1 : (dir == Direction.DOWN ? -1 : 0)) * ball.numberOfStrokes;
            cell = this.GetCell(xmax, ymax);
            b = GetBall(xmax, ymax);
            cellIsOkay &= (cell.BallCanStopOn() && b == null);
        }
        catch (System.Exception ex)
        {
            cellIsOkay = false;
        }
        if (cellIsOkay)
        {
            return cell;
        }
        return null;
    }

    public List<GolfCell> CellsReachableWithBall(Ball ball)
    {
        List<GolfCell> result = new List<GolfCell>();

        int x = ball.positionOnGridX;
        int y = ball.positionOnGridY;

        // down
        GolfCell cell = ReachableCellWithBallAndDirection(ball, Direction.DOWN);
        if (cell != null)
        {
            result.Add(cell);
        }

        // up
        cell = ReachableCellWithBallAndDirection(ball, Direction.UP);
        if (cell != null)
        {
            result.Add(cell);
        }

        // left
        cell = ReachableCellWithBallAndDirection(ball, Direction.LEFT);
        if (cell != null)
        {
            result.Add(cell);
        }

        // right
        cell = ReachableCellWithBallAndDirection(ball, Direction.RIGHT);
        if (cell != null)
        {
            result.Add(cell);
        }

        return result;
    }

    public static HerugolfGameState CreateEmptyGolfGrid(int width, int height)
    {
        HerugolfGameState game = new HerugolfGameState(width, height);

        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                game.gridArray[xi, yi] = new GolfCell(xi, yi, CellType.GREEN);
            }
        }

        return game;
    }

    public static HerugolfGameState CreateRandomGolfGrid(int width, int height)
    {
        HerugolfGameState game = new HerugolfGameState(width, height);

        int numberOfHoles = 0;

        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                int r = Random.Range(0, 10);
                if (r < 1)
                {
                    game.gridArray[xi, yi] = new GolfCell(xi, yi, CellType.HOLE_EMPTY);
                    numberOfHoles++;
                }
                else if (r < 3)
                {
                    game.gridArray[xi, yi] = new GolfCell(xi, yi, CellType.BUNKER);
                }
                else
                {
                    game.gridArray[xi, yi] = new GolfCell(xi, yi, CellType.GREEN);
                }
            }
        }

        int numberOfBalls = 0;
        while (numberOfBalls < numberOfHoles)
        {
            int ballPosX = Random.Range(0, Mathf.RoundToInt(game.gridDimensions.x));
            int ballPosY = Random.Range(0, Mathf.RoundToInt(game.gridDimensions.y));
            Ball ball = new Ball(ballPosX, ballPosY, Random.Range(1, 6));
            
            if (game.GetCell(ball.positionOnGridX, ball.positionOnGridY).type.Equals(CellType.GREEN))
            {
                bool alreadyABallAtThisPosition = false;
                foreach (Ball otherBall in game.balls)
                {
                    alreadyABallAtThisPosition |= ((ballPosX == otherBall.positionOnGridX) && (ballPosY == otherBall.positionOnGridY));
                }

                if (!alreadyABallAtThisPosition)
                {
                    game.balls.Add(ball);
                    numberOfBalls++;
                }
            }
        }

        return game;
    }
}

public class GameEngine : MonoBehaviour
{
    public static GameEngine Instance;

    public HerugolfGameState startGameState;
    public HerugolfGameState currentGameState;

    public GameGridBuilder gridBuilder;

    // Use this for initialization
    void Start ()
    {
        GameEngine.Instance = this;
        startGameState = HerugolfGameState.CreateRandomGolfGrid(12, 9);
        currentGameState = startGameState;

        gridBuilder.BuildGrid(currentGameState);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    public int numberOfCellsWithNextStroke;

    public TextMesh numberOfCellsTextMesh;

    public Material selectedBallMaterial;
    public Material unselectedBallMaterial;

    public Animator ballAnimator;

    public Renderer ballRenderer;

    public Ball ball;

    public bool selected;

    public static List<BallBehaviour> allBalls;

    void Start()
    {
        selected = false;
        if (allBalls == null)
        {
            allBalls = new List<BallBehaviour>();
        }
        allBalls.Add(this);
    }

    void Update()
    {
        numberOfCellsTextMesh.transform.rotation = Camera.main.transform.rotation;
    }

    public void SetBall(Ball b)
    {
        ball = b;
        numberOfCellsWithNextStroke = b.numberOfStrokes;
        numberOfCellsTextMesh.text = numberOfCellsWithNextStroke.ToString();
    }

    void OnMouseDown()
    {
        UnselectAllBalls();
        SetSelected(true);
        ballAnimator.SetBool("Pressed", true);

        GameGridBuilder.Instance.ResetSelectableSquares();

        StartCoroutine(WaitAndUpdateMouseButtonStatus(0.01f));
    }

    IEnumerator WaitAndUpdateMouseButtonStatus(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!Input.GetMouseButton(0))
        {
            ballAnimator.SetBool("Pressed", false);

            List<GolfCell> reachableCells = GameEngine.Instance.currentGameState.CellsReachableWithBall(ball);

            foreach (GolfCell cell in reachableCells)
            {
                GameGridBuilder.Instance.SelectableSquare(this.ball, cell.x, cell.y);
            }
        }
        else
        {
            StartCoroutine(WaitAndUpdateMouseButtonStatus(0.01f));
        }
    }

    private void SetSelected(bool value)
    {
        selected = value;
        ballRenderer.material = selected ? selectedBallMaterial : unselectedBallMaterial;
    }

    public static void UnselectAllBalls()
    {
        foreach (BallBehaviour ball in allBalls)
        {
            ball.SetSelected(false);
        }
    }
}

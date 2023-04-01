using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float gap = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0, 0);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0f;

    private Vector2 offset = new Vector2(0, 0);
    private List<GameObject> _gridSquares = new List<GameObject>();
    private LineIndicator _lineIndicator;


    private void OnEnable()
    {
        GameEvents.CheckIfShapeFits += CheckIfShapeFits;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeFits -= CheckIfShapeFits;
    }
    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
    }

    void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();
    }

    private void SpawnGridSquares()
    {
        // 0,1,2,3,4
        // 5,6,7,8,9

        int squareIdx = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                _gridSquares.Add(Instantiate(gridSquare));
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIdx = squareIdx;
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);

                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(squareIdx) % 2 == 0);
                squareIdx++;
            }
        }

    }

    private void SetGridSquaresPositions()
    {
        int col_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = new Vector2(0, 0);
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (col_number + 1 > columns)
            {
                square_gap_number.x = 0;
                //go to next column
                col_number = 0;
                row_number++;
                row_moved = false;
            }
            var pos_x_offset = offset.x * col_number + (square_gap_number.x * gap);
            var pos_y_offset = offset.y * row_number + (square_gap_number.y * gap);

            if (col_number > 0 && col_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += gap;
            }

            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += gap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset, 0);

            col_number++;
        }
    }

    private void CheckIfShapeFits()
    {
        var squareIndexes = new List<int>();
        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIdx);
                gridSquare.selected = false;
                //gridSquare.ActivateSquare();
            }
        }
        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();

        if (currentSelectedShape == null) return; // no selected shape
        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }

            int shapeLeft = 0;

            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

            CheckIfAnyLineIsCompleted();

        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }

    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        //columns
        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        //rows
        for (int row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (int i = 0; i < 9; i++)
            {
                data.Add(_lineIndicator.LineData[row, i]);
            }
            lines.Add(data.ToArray());
        }

        //squares
        for(int square = 0; square < 9; square++){
            List<int> data = new List<int>(9);
            for(int i = 0; i < 9; i++){
                data.Add(_lineIndicator.SquareData[square, i]);
            }
            lines.Add(data.ToArray());
        }

        int completedLines = CheckIfSquaresAreCompleted(lines);

        if(completedLines > 2){
            //TODO: Play bonus animation
        }

        int totalScores = 10 * completedLines;
        GameEvents.AddScores(totalScores);
        
    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();
        int linesCompleted = 0;
        foreach (var line in data)
        {
            bool lineCompleted = true;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }
            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach (var line in completedLines)
        {
            bool completed = false;

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if(completed){
                linesCompleted++;
            }
        }
        return linesCompleted;
    }
}

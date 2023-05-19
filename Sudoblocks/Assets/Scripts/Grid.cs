using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private ShapeStorage shapeStorage;
    [SerializeField] private int columns;
    [SerializeField] private int rows;
    public bool isHighlighted { get; private set; }
    private int squares = 9;
    [SerializeField] private float gap;
    [SerializeField] private GameObject gridSquare;
    [SerializeField] private Vector2 startPosition = new Vector2(0, 0);
    private float squareScale = 0.5f;
    private float everySquareOffset = 0.5f;

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

    private void Update()
    {
        HighlightLines();
    }

    void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();
    }

    private void SpawnGridSquares()
    {
        GameObject gridSquarePrefab = gridSquare.gameObject;
        Transform gridTransform = transform;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int squareIdx = row * columns + col;
                GameObject gridSquareObject = Instantiate(gridSquarePrefab, gridTransform);
                GridSquare gridSquareComponent = gridSquareObject.GetComponent<GridSquare>();
                gridSquareComponent.SquareIdx = squareIdx;
                gridSquareComponent.transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                gridSquareComponent.SetImage(squareIdx % 2 == 0);

                _gridSquares.Add(gridSquareComponent.gameObject);
            }
        }
    }

    private void SetGridSquaresPositions()
    {
        int colNum = 0;
        int rowNum = 0;
        Vector2 squareGapNum = new Vector2(0, 0);
        bool rowMoved = false;

        var squareRect = _gridSquares[0].GetComponent<RectTransform>();

        offset.x = squareRect.rect.width * squareRect.transform.localScale.x + everySquareOffset;
        offset.y = squareRect.rect.height * squareRect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (colNum + 1 > columns)
            {
                squareGapNum.x = 0;
                //go to next column
                colNum = 0;
                rowNum++;
                rowMoved = false;
            }
            var posXOffset = offset.x * colNum + (squareGapNum.x * gap);
            var posYOffset = offset.y * rowNum + (squareGapNum.y * gap);

            if (colNum > 0 && colNum % 3 == 0)
            {
                squareGapNum.x++;
                posXOffset += gap;
            }

            if (rowNum > 0 && rowNum % 3 == 0 && rowMoved == false)
            {
                rowMoved = true;
                squareGapNum.y++;
                posYOffset += gap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + posXOffset, startPosition.y - posYOffset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + posXOffset, startPosition.y - posYOffset, 0);

            colNum++;
        }
    }

    private void HighlightLines()
    {
        var squareIndexes = new List<int>();
        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIdx);
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;

        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            gridSquare.Unhighlight();
        }

        List<int[]> lines = new List<int[]>();
        //columns
        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        //rows
        for (int row = 0; row < rows; row++)
        {
            List<int> data = new List<int>(rows);
            for (int i = 0; i < rows; i++)
            {
                data.Add(_lineIndicator.LineData[row, i]);
            }

            lines.Add(data.ToArray());
        }

        //squares
        for (int square = 0; square < squares; square++)
        {
            List<int> data = new List<int>(squares);
            for (int i = 0; i < squares; i++)
            {
                data.Add(_lineIndicator.SquareData[square, i]);
            }
            lines.Add(data.ToArray());
        }

        List<int[]> highlightedLines = new List<int[]>();

        foreach (var line in lines)
        {
            bool lineHighlightable = true;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.ShapeBlocked || !comp.Selected || currentSelectedShape.TotalSquareNumber != squareIndexes.Count)
                {
                    lineHighlightable = false;
                }
            }
            if (lineHighlightable)
            {
                highlightedLines.Add(line);
            }
        }

        if (highlightedLines.Count != 0)
        {
            isHighlighted = true;
        }
        else
        {
            isHighlighted = false;
        }

        foreach (var line in highlightedLines)
        {
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Highlight();
            }
        }
    }

    private void CheckIfShapeFits()
    {
        var squareIndexes = new List<int>();
        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIdx);
                gridSquare.Selected = false;
            }
        }
        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();

        if (currentSelectedShape == null) return; // no selected shape
        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().ActivateGridSquare();
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

    public void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();
        //columns
        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        //rows
        for (int row = 0; row < rows; row++)
        {
            List<int> data = new List<int>(rows);
            for (int i = 0; i < rows; i++)
            {
                data.Add(_lineIndicator.LineData[row, i]);
            }

            lines.Add(data.ToArray());
        }

        //squares
        for (int square = 0; square < squares; square++)
        {
            List<int> data = new List<int>(squares);
            for (int i = 0; i < squares; i++)
            {
                data.Add(_lineIndicator.SquareData[square, i]);
            }
            lines.Add(data.ToArray());
        }

        List<int[]> linesToClear = CheckIfSquaresAreCompleted(lines); //==> this also clears the line

        //Score
        int completedLinesCount = linesToClear.Count;
        int totalScores = 10 * completedLinesCount;
        int bonusScores = 0;
        if (completedLinesCount >= 2)
        {
            bonusScores = 5 * completedLinesCount;
            GameEvents.ShowApplaudWritings();
        }

        CheckActiveShapesCondition(); // Check lose condition
        GameEvents.AddScores(totalScores + bonusScores);
    }

    private List<int[]> CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

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
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Unhighlight();
                comp.DeactivateGridSquare();
                AudioManager.instance.PlaySFX("Points");
            }

        }
        return completedLines;
    }

    private void CheckActiveShapesCondition()
    {
        int valid = 0;
        for (int i = 0; i < shapeStorage.shapeList.Count; i++)
        {
            var isShapeActive = shapeStorage.shapeList[i].IsAnyOfShapeSquareActive();
            if (isShapeActive)
            {
                if (CheckIfShapeCanBePlacedOnGrid(shapeStorage.shapeList[i]))
                {
                    foreach (var square in shapeStorage.shapeList[i].GetComponentsInChildren<ActiveSquareImageSelector>())
                    {
                        square.updateImageOnCondition = false;
                    }
                    valid++;
                }
                else
                {
                    foreach (var square in shapeStorage.shapeList[i].GetComponentsInChildren<ActiveSquareImageSelector>())
                    {
                        square.updateImageOnCondition = true; //cannot place this shape, turn tinted-blue
                    }
                }
            }
        }

        if (valid == 0)
        {
            GameEvents.GameOver();
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        int shapeColumns = currentShapeData.columns;
        int shapeRows = currentShapeData.rows;

        List<int> originalShapeFilledUpSquares = new List<int>();
        int squareIndex = 0;

        for (int rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }

        if (currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.LogError("Number of filled up squares are not equal to the original shape");
        }

        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;
        foreach (var number in squareList)
        {
            bool shapeCanBePlacedOnBoard = true;
            foreach (var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                var comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnBoard = false;
                }
            }

            if (shapeCanBePlacedOnBoard)
            {
                canBePlaced = true;
            }
        }

        return canBePlaced;
    }

    private List<int[]> GetAllSquaresCombination(int c, int r)
    {
        var squareList = new List<int[]>();
        int lastColumnIndex = 0;
        int lastRowIndex = 0;
        int safeIndex = 0;

        while (lastRowIndex + (r - 1) < squares)
        {
            var rowData = new List<int>();

            for (int row = lastRowIndex; row < lastRowIndex + r; row++)
            {
                for (int col = lastColumnIndex; col < lastColumnIndex + c; col++)
                {
                    rowData.Add(_lineIndicator.LineData[row, col]);
                }
            }

            squareList.Add(rowData.ToArray());
            lastColumnIndex++;

            if (lastColumnIndex + (c - 1) >= squares)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if (safeIndex > 100)
            {
                break;
            }
        }

        return squareList;
    }
}

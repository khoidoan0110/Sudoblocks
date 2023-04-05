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
    public SquareTextureData squareTextureData;

    private Vector2 offset = new Vector2(0, 0);
    private List<GameObject> _gridSquares = new List<GameObject>();
    private LineIndicator _lineIndicator;
    private Config.SquareColors _currentActiveSquareColor = Config.SquareColors.NotSet;
    private List<Config.SquareColors> _colorsInTheGrid = new List<Config.SquareColors>();

    private void OnEnable()
    {
        GameEvents.CheckIfShapeFits += CheckIfShapeFits;
        GameEvents.UpdateSquareColor += OnUpdateSquareColor;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeFits -= CheckIfShapeFits;
        GameEvents.UpdateSquareColor -= OnUpdateSquareColor;
    }
    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
        _currentActiveSquareColor = squareTextureData.activeSquareTextures[0].squareColors;
    }

    private void OnUpdateSquareColor(Config.SquareColors color){
        _currentActiveSquareColor = color;
    }

    private List<Config.SquareColors> GetAllSquareColorsInGrid(){
        var colors = new List<Config.SquareColors>();
        foreach (var square in _gridSquares){
            GridSquare gridSquare = square.GetComponent<GridSquare>();
            if(gridSquare.SquareOccupied){
                var color = gridSquare.GetCurrentColor();
                if(colors.Contains(color) == false){
                    colors.Add(color);
                }
            }
        }
        return colors;
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
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard(_currentActiveSquareColor);
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
        for (int square = 0; square < 9; square++)
        {
            List<int> data = new List<int>(9);
            for (int i = 0; i < 9; i++)
            {
                data.Add(_lineIndicator.SquareData[square, i]);
            }
            lines.Add(data.ToArray());
        }

        _colorsInTheGrid = GetAllSquareColorsInGrid();

        int completedLines = CheckIfSquaresAreCompleted(lines);


        int totalScores = 10 * completedLines;
        int bonusScores = 0;
        if (completedLines >= 2)
        {
            bonusScores = 5 * completedLines;
            GameEvents.ShowApplaudWritings();
        }
        
        GameEvents.AddScores(totalScores + bonusScores);
        CheckLoseCondition();
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

            if (completed)
            {
                AudioManager.instance.PlaySFX("Points");
                linesCompleted++;
            }
        }
        return linesCompleted;
    }

    private void CheckLoseCondition()
    {
        int validShapes = 0;
        for (int i = 0; i < shapeStorage.shapeList.Count; i++)
        {
            var isShapeActive = shapeStorage.shapeList[i].IsAnyOfShapeSquareActive();
            if(CheckIfShapeCanBePlacedOnGrid(shapeStorage.shapeList[i]) && isShapeActive){
                shapeStorage.shapeList[i]?.ActivateShape();
                validShapes++;
            }
        }

        if(validShapes == 0){
            GameEvents.GameOver();
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape){
        var currentShapeData = currentShape.CurrentShapeData;
        int shapeColumns = currentShapeData.columns;
        int shapeRows = currentShapeData.rows;

        List<int> originalShapeFilledUpSquares = new List<int>();
        int squareIndex = 0;

        for(int rowIndex = 0; rowIndex < shapeRows; rowIndex++){
            for(int columnIndex = 0; columnIndex < shapeColumns; columnIndex++){
                if(currentShapeData.board[rowIndex].column[columnIndex]){
                    originalShapeFilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }

        if(currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count){
            Debug.LogError("Number of filled up squares are not equal to the original shape");
        }

        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;
        foreach(var number in squareList){
            bool shapeCanBePlacedOnBoard = true;
            foreach(var squareIndexToCheck in originalShapeFilledUpSquares){
                var comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if(comp.SquareOccupied){
                    shapeCanBePlacedOnBoard = false;
                }
            }

            if(shapeCanBePlacedOnBoard){
                canBePlaced = true;
            }
        }

        return canBePlaced;
        /* x0
         * x0
         * xx
         */
    }

    private List<int[]> GetAllSquaresCombination(int columns, int rows){
        var squareList = new List<int[]>();
        int lastColumnIndex = 0;
        int lastRowIndex = 0;

        int safeIndex = 0;

        while(lastRowIndex + (rows - 1) < 9){
            var rowData = new List<int>();

            for(int row = lastRowIndex; row < lastRowIndex + rows; row++){
                for(int col = lastColumnIndex; col < lastColumnIndex + columns; col++){
                    rowData.Add(_lineIndicator.LineData[row, col]);
                }
            }

            squareList.Add(rowData.ToArray());
            lastColumnIndex++;

            if(lastColumnIndex + (columns - 1) >= 9){
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if(safeIndex > 100){
                break;
            }
        }

        return squareList;
    }
}

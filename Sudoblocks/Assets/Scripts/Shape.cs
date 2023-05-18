using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject squareShapeImage;
    [SerializeField] private Vector3 shapeSelectedScale;
    [SerializeField] private Vector2 offset = new Vector2(0f, 0f);

    [HideInInspector]
    public ShapeData CurrentShapeData;

    public int TotalSquareNumber { get; set; }

    private List<GameObject> _currentShape = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActive = true;
    private Grid _grid;

    void Awake(){
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _startPosition = _transform.localPosition;
        _shapeActive = true;
        _grid = FindObjectOfType<Grid>();
    }

    private void OnEnable(){
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }

    private void OnDisable(){
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }

    public bool IsOnStartPosition(){
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive(){
        foreach (var square in _currentShape){
            if(square.gameObject.activeSelf) return true;
        }
        return false;
    }

    public void DeactivateShape(){
        if(_shapeActive){
            foreach(var square in _currentShape){
                square?.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }
        _shapeActive = false;
    }

    private void SetShapeInactive(){
        if(IsOnStartPosition() == false && IsAnyOfShapeSquareActive()){
            foreach(var square in _currentShape){
                square.gameObject.SetActive(false);
            }
        }
    }

    public void ActivateShape(){
        if(!_shapeActive){
            foreach(var square in _currentShape){
                square?.GetComponent<ShapeSquare>().ActivateShape();
            }
        }
        _shapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData){
        _transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSquares(shapeData);

        while (_currentShape.Count <= TotalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform));
        }

        foreach (var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x, squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;

        //set positions to form final shape
        for (int row = 0; row < shapeData.rows; row++)
        {
            for (int col = 0; col < shapeData.columns; col++)
            {
                if (shapeData.board[row].column[col])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition = new Vector2(GetXPositionForShapeSquare(shapeData, col, moveDistance), GetYPositionForShapeSquare(shapeData, row, moveDistance));
                    currentIndexInList++;
                }
            }
        }
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;
        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var middleSquareIdx = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;
                if (row < middleSquareIdx)
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSquareIdx)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
            else
            {
                var middleSquareIdx2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middleSquareIdx1 = (shapeData.rows == 2) ? 0 : (shapeData.rows - 1);
                var multiplier = shapeData.rows / 2;

                if (row == middleSquareIdx1 || row == middleSquareIdx2)
                {
                    if (row == middleSquareIdx2)
                    {
                        shiftOnY = (moveDistance.y / 2) * -1;
                    }
                    if (row == middleSquareIdx1)
                    {
                        shiftOnY = moveDistance.y / 2;
                    }
                }

                if (row < middleSquareIdx1 && row < middleSquareIdx2)
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSquareIdx1 && row > middleSquareIdx2)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
        }
        return shiftOnY;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;
        if (shapeData.columns > 1)
        {
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIdx = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if (column < middleSquareIdx)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIdx)
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
            else
            {
                var middleSquareIdx2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middleSquareIdx1 = (shapeData.columns == 2) ? 0 : (shapeData.columns - 1);
                var multiplier = shapeData.columns / 2;

                if (column == middleSquareIdx1 || column == middleSquareIdx2)
                {
                    if (column == middleSquareIdx2)
                    {
                        shiftOnX = moveDistance.x / 2;
                    }
                    if (column == middleSquareIdx1)
                    {
                        shiftOnX = (moveDistance.x / 2) * -1;
                    }
                }

                if (column < middleSquareIdx1 && column < middleSquareIdx2)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIdx1 && column > middleSquareIdx2)
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
        }
        return shiftOnX;
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int count = 0;
        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckIfShapeFits();
    }

    private void MoveShapeToStartPosition(){
        _transform.transform.localPosition = _startPosition;
    }
}

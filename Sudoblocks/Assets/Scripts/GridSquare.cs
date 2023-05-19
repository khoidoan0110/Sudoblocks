using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    [SerializeField] private Image defaultImg, hoverImg, activeImg, completableImg;
    [SerializeField] private List<Sprite> defaultImgs;
    private Grid grid;

    public bool Selected { get; set; }
    public int SquareIdx { get; set; }
    public bool SquareOccupied { get; set; }
    public bool SquareHovered { get; set; }
    public bool ShapeBlocked { get; set; }
    public bool GridSquareHighlighted { get; set; }
    void Start()
    {
        Selected = false;
        SquareOccupied = false;
        grid = FindObjectOfType<Grid>();
    }

    void Update()
    {
        if (hoverImg.gameObject.activeSelf)
        {
            SquareHovered = true;
        }
        else
        {
            SquareHovered = false;
        }
    }

    public void ActivateGridSquare()
    {
        AudioManager.instance.PlaySFX("Place", 1.2f);
        hoverImg.gameObject.SetActive(false);
        activeImg.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;
    }

    public void DeactivateGridSquare()
    {
        activeImg.gameObject.SetActive(false);
        Selected = false;
        SquareOccupied = false;
    }

    public void SetImage(bool setFirstImg)
    {
        defaultImg.GetComponent<Image>().sprite = setFirstImg ? defaultImgs[1] : defaultImgs[0];
    }

    public void Highlight()
    {
        GridSquareHighlighted = true;
        completableImg.gameObject.SetActive(true);
    }

    public void Unhighlight()
    {
        GridSquareHighlighted = false;
        completableImg.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ShapeSquare shapeSquare = collision.GetComponent<ShapeSquare>();
        if (shapeSquare != null)
        {
            if (grid.isHighlighted && GridSquareHighlighted)
            {
                shapeSquare.SetHighlightedImg();
            }
            else
            {
                shapeSquare.UnsetHighlightedImg();
            }
        }

        if (SquareOccupied == false)
        {
            Selected = true;
            hoverImg.gameObject.SetActive(true);
        }
        else if (shapeSquare != null)
        {
            ShapeBlocked = true;
            GameObject parentObj = collision.transform.parent.gameObject;
            for (int i = 0; i < parentObj.transform.childCount - 1; i++)
            {
                Transform child = parentObj.transform.GetChild(i);
                ActiveSquareImageSelector activeSquareImageSelector = child.GetComponentInChildren<ActiveSquareImageSelector>();
                activeSquareImageSelector.updateImageOnGridCondition = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ShapeSquare shapeSquare = collision.GetComponent<ShapeSquare>();
        if (shapeSquare != null)
        {
            if (grid.isHighlighted && GridSquareHighlighted)
            {
                shapeSquare.SetHighlightedImg();
            }
            else
            {
                shapeSquare.UnsetHighlightedImg();
            }
        }

        if (SquareOccupied == false)
        {
            Selected = true;
            hoverImg.gameObject.SetActive(true);
        }
        else if (shapeSquare != null)
        {
            ShapeBlocked = true;

            GameObject parentObj = collision.transform.parent.gameObject;
            for (int i = 0; i < parentObj.transform.childCount - 1; i++)
            {
                Transform child = parentObj.transform.GetChild(i);
                ActiveSquareImageSelector activeSquareImageSelector = child.GetComponentInChildren<ActiveSquareImageSelector>();
                activeSquareImageSelector.updateImageOnGridCondition = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ShapeSquare shapeSquare = collision.GetComponent<ShapeSquare>();
        if (shapeSquare != null)
        {
            shapeSquare.UnsetHighlightedImg();
        }

        if (SquareOccupied == false)
        {
            Selected = false;
            hoverImg.gameObject.SetActive(false);
        }
        else if (shapeSquare != null)
        {
            ShapeBlocked = false;
            GameObject parentObj = collision.transform.parent.gameObject;
            for (int i = 0; i < parentObj.transform.childCount - 1; i++)
            {
                Transform child = parentObj.transform.GetChild(i);
                ActiveSquareImageSelector activeSquareImageSelector = child.GetComponentInChildren<ActiveSquareImageSelector>();
                activeSquareImageSelector.updateImageOnGridCondition = false;
            }
        }
    }


}

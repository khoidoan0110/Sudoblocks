using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image defaultImg, hoverImg, activeImg;
    public List<Sprite> defaultImgs;

    public bool selected { get; set; }
    public int SquareIdx { get; set; }
    public bool SquareOccupied { get; set; }

    void Start()
    {
        selected = false;
        SquareOccupied = false;
    }

    //temp
    public bool CanBePlaced()
    {
        return hoverImg.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard()
    {
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        hoverImg.gameObject.SetActive(false);
        activeImg.gameObject.SetActive(true);
        selected = true;
        SquareOccupied = true;
    }

    public void SetImage(bool setFirstImg)
    {
        defaultImg.GetComponent<Image>().sprite = setFirstImg ? defaultImgs[1] : defaultImgs[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            selected = true;
            hoverImg.gameObject.SetActive(true);
        }
        else if(collision.GetComponent<ShapeSquare>() != null){
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        selected = true;
        if (SquareOccupied == false) //&& !hoverImg.gameObject.activeSelf)
        {
            hoverImg.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            selected = false;
            hoverImg.gameObject.SetActive(false);
        }
        else if(collision.GetComponent<ShapeSquare>() != null){
            collision.GetComponent<ShapeSquare>().UnsetOccupied();
        }
    }

}

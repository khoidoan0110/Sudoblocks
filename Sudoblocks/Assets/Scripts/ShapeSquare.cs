using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    [SerializeField] private GameObject highlightedImage;
    private bool isHighlighted = false;

    void OnEnable(){
        UnsetHighlightedImg();
    }

    public void DeactivateShape()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(false);
    }

    public void ActivateShape()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(true);
    }

    public void SetHighlightedImg()
    {
        isHighlighted = true;
        UpdateHighlightState();
    }

    public void UnsetHighlightedImg()
    {
        isHighlighted = false;
        UpdateHighlightState();
    }

    private void UpdateHighlightState()
    {
        highlightedImage.SetActive(isHighlighted);
    }
}

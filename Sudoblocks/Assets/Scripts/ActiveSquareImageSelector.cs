using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveSquareImageSelector : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public bool updateImageOnReachedThreshold = false;

    private void OnEnable()
    {
        UpdateSquareOnPoints();

        if(updateImageOnReachedThreshold){
            GameEvents.UpdateSquareColor += UpdateSquareColor;
        }
    }

    private void OnDisable()
    {
        if(updateImageOnReachedThreshold){
            GameEvents.UpdateSquareColor -= UpdateSquareColor;
        }
    }

    private void UpdateSquareOnPoints()
    {
        foreach (var squareTexture in squareTextureData.activeSquareTextures)
        {
            if (squareTextureData.currentColor == squareTexture.squareColors)
            {
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }

    private void UpdateSquareColor(Config.SquareColors color)
    {
        foreach (var squareTexture in squareTextureData.activeSquareTextures)
        {
            if (color == squareTexture.squareColors)
            {
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }
}

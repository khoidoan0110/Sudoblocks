using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveSquareImageSelector : MonoBehaviour
{
    private Image defaultImage;
    public List<Sprite> sprites;
    [HideInInspector]
    public bool updateImageOnCondition, updateImageOnGridCondition = false;

    void Start()
    {
        defaultImage = GetComponent<Image>();
        UpdateShapeColor(sprites[0]);
    }

    void Update()
    {
        if(updateImageOnCondition){
            UpdateShapeColor(sprites[1]); // tinted-blue, cannot place anymore
        }
        else if(updateImageOnGridCondition){
            UpdateShapeColor(sprites[2]); // red, making contact with other blocks
        }
        else{
            UpdateShapeColor(sprites[0]); // blue
        }
    }

    public void UpdateShapeColor(Sprite sprite)
    {
        defaultImage.sprite = sprite;
    }
}

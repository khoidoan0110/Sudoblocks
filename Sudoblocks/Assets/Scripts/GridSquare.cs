using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image defaultImg;
    public List<Sprite> defaultImgs;
    void Start()
    {
        
    }

    public void SetImage(bool setFirstImg){
        defaultImg.GetComponent<Image>().sprite = setFirstImg ? defaultImgs[1] : defaultImgs[0]; 
    }
}

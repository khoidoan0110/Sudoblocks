using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class SquareTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData{
        public Sprite texture;
        public Config.SquareColors squareColors;
    }

    public int thresholdValue = 10;
    private const int startThresholdValue = 100;
    public List<TextureData> activeSquareTextures;

    public Config.SquareColors currentColor;
    private Config.SquareColors _nextColor;

    public int GetCurrentColorIndex(){
        int currentIndex = 0;
        for(int i = 0; i < activeSquareTextures.Count; i++){
            if(activeSquareTextures[i].squareColors == currentColor){
                currentIndex = i;
            }
        }
        return currentIndex;
    }

    public void UpdateColors(int currentScore){
        currentColor = _nextColor;
        int currentColorIndex = GetCurrentColorIndex();
        if(currentColorIndex == activeSquareTextures.Count - 1){
            _nextColor = activeSquareTextures[0].squareColors;
        }
        else{
            _nextColor = activeSquareTextures[currentColorIndex + 1].squareColors;
        }
        thresholdValue = startThresholdValue + currentScore;
    }

    public void SetStartColor(){
        thresholdValue = startThresholdValue;
        currentColor = activeSquareTextures[0].squareColors;
        _nextColor = activeSquareTextures[1].squareColors;
    }

    private void Awake(){
        SetStartColor();
    }

    private void OnEnable(){
        SetStartColor();
    }
}

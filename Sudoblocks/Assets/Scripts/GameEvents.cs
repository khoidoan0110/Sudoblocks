using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action GameOver;
    public static Action<int> AddScores;
    public static Action CheckIfShapeFits;
    public static Action MoveShapeToStartPosition;
    public static Action RequestNewShapes;
    public static Action SetShapeInactive;
    public static Action<int, int> UpdateBestScoreBar;
    public static Action ShowApplaudWritings;
}

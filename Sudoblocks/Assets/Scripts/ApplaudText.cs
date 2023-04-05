using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplaudText : MonoBehaviour
{
    [SerializeField] private List<GameObject> writings;
    void Start()
    {
        GameEvents.ShowApplaudWritings += ShowApplaudWritings;
    }

    private void OnDisable(){
         GameEvents.ShowApplaudWritings -= ShowApplaudWritings;
    }

    private void ShowApplaudWritings(){
        int index = Random.Range(0, writings.Count);
        writings[index].SetActive(true);
    }
}

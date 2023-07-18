using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOn : MonoBehaviour
{
    [SerializeField] private GameObject Canvas;

    private void Start()
    {
        Canvas.SetActive(true);
    }
}
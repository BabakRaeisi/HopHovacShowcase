using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Renderer tileRenderer;

    public void SetColor(Color newColor)
    {
        if (tileRenderer != null)
        {
            tileRenderer.material.color = newColor;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

 
public class Labeler : MonoBehaviour
{
    Vector2Int cords = new Vector2Int();
    [SerializeField]GridSystem grid;
    [SerializeField]TextMeshPro label;


    private void Awake()
    {
        DisplayCords();
    }
    [ContextMenu("DisplayInfo")]
    public void DisplayCords()
    {
        grid = FindObjectOfType<GridSystem>();
        label = GetComponentInChildren<TextMeshPro>();
        if (!grid) { return; }
        cords.x = Mathf.RoundToInt(transform.position.x / grid.UnityGridSize );
        cords.y = Mathf.RoundToInt(transform.position.z / grid.UnityGridSize);
        label.text = $"{cords.x},{cords.y}";
        transform.name = cords.ToString();
    }
}

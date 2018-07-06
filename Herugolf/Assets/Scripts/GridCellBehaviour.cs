using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellBehaviour : MonoBehaviour
{
    public GameObject SelectionGo;
    public GameObject SelectedGo;
    public GameObject HoverGo;
    public Renderer cellRenderer;

    public bool selected;
    public bool selectable;
    public bool hovered;

    public GolfCell data;

    public List<Color> availableColors;
    private int colorIndex;

    public void Init(GolfCell d)
    {
        data = d;
        selectable = false;
        selected = false;
        hovered = false;
        SetColor();
        UpdateAppearance();
    }

    private void SetColor()
    {
        colorIndex = Random.Range(0, availableColors.Count);
        Color color = availableColors[colorIndex];
        cellRenderer.material.color = color;
    }

    private void UpdateAppearance()
    {
        SelectedGo.SetActive(selected);
        SelectionGo.SetActive(selectable && !selected);
        HoverGo.SetActive(hovered && !selectable && !selected);
    }

    public void SetSelectable(bool value)
    {
        if (!value)
        {
            selected = false;
        }
        selectable = value;
        UpdateAppearance();
    }

    public void SetHovered(bool value)
    {
        hovered = value;
        UpdateAppearance();
    }

    void OnMouseDown()
    {
        if (selectable)
        {
            Debug.Log("click selectable");
            selected = true;
            UpdateAppearance();
        }
    }
}

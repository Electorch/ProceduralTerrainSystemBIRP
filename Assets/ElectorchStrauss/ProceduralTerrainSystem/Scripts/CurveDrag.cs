using System;
using System.Collections;
using System.Collections.Generic;
using ElectorchStrauss.ProceduralTerrainSystem.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CurveDrag : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    private bool dragging = false;
    private Vector3 dragOffset;
    private TerrainRuntimeModification terrainRuntimeModification;
    private RectTransform rt;
    private void Start()
    {
        terrainRuntimeModification = FindObjectOfType<TerrainRuntimeModification>();
        rt = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left && !dragging) return;
        Vector3 worldpoint;
        if (DragWorldPoint(eventData, out worldpoint))
        {
            if (name.Contains("Handle"))
            {
                Vector3 offsetX = worldpoint + dragOffset;
                offsetX.x = rt.position.x;
                rt.position = offsetX;
            }
            else
            {
                rt.position = worldpoint + dragOffset;
            }
        }
        terrainRuntimeModification.ModifyMeshHeightCurve();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        dragging = true;
        Vector3 worldpoint;
        if (DragWorldPoint(eventData, out worldpoint))
        {
            dragOffset = GetComponent<RectTransform>().position - worldpoint;
        }
        else
        {
            dragOffset = Vector3.zero;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        dragging = false;
    }
    // Gets the point in worldspace corresponding to where the mouse is
    private bool DragWorldPoint(PointerEventData eventData, out Vector3 worldPoint)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out worldPoint);
    }
}

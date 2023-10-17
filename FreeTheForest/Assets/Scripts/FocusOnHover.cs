using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class FocusOnHover : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;

    private Vector3 _basePosition;
    private float _scaleFactor;
    private int _baseSortOrder;

    private void Awake()
    {
        _canvas.overrideSorting = true;
        _baseSortOrder = _canvas.sortingOrder;
    }

    private void OnMouseEnter()
    {
        _basePosition = _canvas.transform.position;
        setScale(1.2f);
        _canvas.sortingOrder = _baseSortOrder + 100;
        // //move the canvas up 100f on y axis
        _canvas.transform.position = new Vector3(_canvas.transform.position.x, _canvas.transform.position.y, _canvas.transform.position.z);
    }

    private void OnMouseExit()
    {
        setScale(0.8f);
        _canvas.sortingOrder = _baseSortOrder;
        _canvas.transform.position = _basePosition;
    }

    private void OnMouseUp()
    {
        setScale(0.8f);
        _canvas.sortingOrder = _baseSortOrder;
        _canvas.transform.position = _basePosition;
    }
    private void setScale(float scale)
    {        
        transform.localScale = Vector3.one * scale;
    }
}

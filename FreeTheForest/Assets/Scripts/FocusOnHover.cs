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

    public bool canHover;

    private void Awake()
    {
        _canvas.overrideSorting = true;
        _baseSortOrder = _canvas.sortingOrder;
    }

    private void CanHover()
    {
        GameObject parent = transform.parent.gameObject;
        if(parent.name == "TargetSlot")
        {
            canHover = false;
        }
        else
        {
            canHover = true;
        }
    }
    private void OnMouseEnter()
    {
        CanHover();
        if(!canHover)
        {
            return;
        }
        _basePosition = _canvas.transform.position;
        setScale(1.2f);
        _canvas.sortingOrder = _baseSortOrder + 100;
        _canvas.transform.position = new Vector3(_canvas.transform.position.x, _canvas.transform.position.y, _canvas.transform.position.z);
    }

    private void OnMouseExit()
    {
        CanHover();
        if(!canHover)
        {
            return;
        }
        setScale(0.8f);
        _canvas.sortingOrder = _baseSortOrder;
        _canvas.transform.position = _basePosition;
    }

private void OnMouseUp()
{
        CanHover();
        if(!canHover)
        {
            return;
        }
    if (Input.GetMouseButtonUp(1)) //check if the right mouse button was released
    {
        setScale(0.8f);
        _canvas.sortingOrder = _baseSortOrder;
        _canvas.transform.position = _basePosition;
    }
}

private void OnMouseDown()
    {
        CanHover();
        if(!canHover)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            _basePosition = _canvas.transform.position;
            setScale(1.2f);
            _canvas.sortingOrder = _baseSortOrder + 100;
            _canvas.transform.position = new Vector3(_canvas.transform.position.x, _canvas.transform.position.y, _canvas.transform.position.z);
        }
        if(Input.GetMouseButtonDown(1))
        {
            setScale(0.8f);
            _canvas.sortingOrder = _baseSortOrder;
            _canvas.transform.position = _basePosition;
        }
    }
    private void setScale(float scale)
    {        
        transform.localScale = Vector3.one * scale;
    }
}

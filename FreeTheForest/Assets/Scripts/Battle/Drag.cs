using UnityEngine;

public class Drag : MonoBehaviour
{
    private Vector3 offset; //store the offset between mouse click point and the object's position.
    private Vector3 originalPos;
    private bool cancelled;
    private bool snapSuccess = false;
    private bool isDragging = false;

    private void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
        }
    }
    private void OnMouseDown()
    {
        //calculate the offset between the mouse click point and the object's position.
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        originalPos = transform.position;
        isDragging = true;
    }

    private void OnMouseUp()
    {
        trySnap();
        isDragging = false;
    }

    private void trySnap()
    {
        //additional logic to not snap if successful target is found here ****

        //otherwise snap back to original position
        snapSuccess = false;
        setCardOriginalPos();
    }
        private void setCardOriginalPos()
    {
        transform.position = originalPos;
    }
}




using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class removes duplicate event systems from the game.
/// It should be attached to the EventSystem game object.
/// </summary>
public class DestroyDuplicateEventSystems : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsOfType<EventSystem>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
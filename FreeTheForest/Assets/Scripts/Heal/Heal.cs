using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //grab 'Main Camera' and attach to canvas for player UI
        GameObject controller = GameObject.Find("PlayerInfoController");
        GameObject camera = GameObject.Find("Main Camera");
        Canvas canvasComponent = controller.GetComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceCamera;
        canvasComponent.worldCamera = camera.GetComponent<Camera>();


        PlayerInfoController.instance.HealNode(0.25f);
        Debug.Log("Healed for 25% of max health");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private Button maxHealth;
    [SerializeField] private Button strength;
    [SerializeField] private Button defense;
    // Start is called before the first frame update

    public void Start()
    {
        //grab 'Main Camera' and attach to canvas
        GameObject controller = GameObject.Find("PlayerInfoController");
        GameObject camera = GameObject.Find("Main Camera");
        Canvas canvasComponent = controller.GetComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceCamera;
        canvasComponent.worldCamera = camera.GetComponent<Camera>();
    }
    public void GainMaxHealth()
    {
        PlayerInfoController.instance.MaxHealth += 5;
        PlayerInfoPanel.Instance.UpdateStats();
    }

    public void GainStrength()
    {
        PlayerInfoController.instance.Strength += 1;
        PlayerInfoPanel.Instance.UpdateStats();
    }

    public void GainDefense()
    {
        PlayerInfoController.instance.Defense += 1;
        PlayerInfoPanel.Instance.UpdateStats();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverWorld : MonoBehaviour
{

    void Awake()
    {
        //this needs a error check
        if(PlayerInfoPanel.Instance != null)
        PlayerInfoPanel.Instance.ActivateUI();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverWorld : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("OverWorld Start");
        SceneManager.LoadSceneAsync("PlayerInfoPanel", LoadSceneMode.Additive);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

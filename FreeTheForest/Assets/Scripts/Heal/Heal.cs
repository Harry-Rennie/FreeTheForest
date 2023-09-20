using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerInfoController.instance.HealNode(0.25f);
        Debug.Log("Healed for 25% of max health");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

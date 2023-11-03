using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] public GameObject settingsPanel;
    // Start is called before the first frame update
    void Start()
    {
        Clean();
    }

    void Clean()
    {
        //if playerinfocontroller exists, destroy it
        if (PlayerInfoController.instance != null)
        {
            Destroy(PlayerInfoController.instance.gameObject);
        }
        if (TrinketManager.Instance != null)
        {
            Destroy(TrinketManager.Instance.gameObject);
        }
        if (MobManager.Instance != null)
        {
            Destroy(MobManager.Instance.gameObject);
        }

    }

}

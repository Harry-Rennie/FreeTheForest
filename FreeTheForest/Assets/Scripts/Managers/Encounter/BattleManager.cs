using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class BattleManager : MonoBehaviour
{
    [SerializeField] TMP_Text nickNameText;
    private string nickName;

    void Start()
    {
        if (PlayerInfoController.instance != null)
        {
            this.nickName = PlayerInfoController.instance.m_TextMeshPro.text;
            nickNameText.text = nickName;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
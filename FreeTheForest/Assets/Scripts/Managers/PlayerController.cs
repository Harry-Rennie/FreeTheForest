using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] TMP_Text m_TextMeshPro;
    [SerializeField] TMP_Text m_TextMeshPro2;
    private string nickName;
    // Start is called before the first frame update
    void Start()
    {
       nickName = m_TextMeshPro.text;
        m_TextMeshPro2.text = nickName;
    }
}

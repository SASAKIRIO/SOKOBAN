// ---------------------------------------------------------  
// ClearUI.cs  
//   
// 作成日:  
// 作成者:  sasaki rio
// ---------------------------------------------------------  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClearUI : MonoBehaviour
{
    [SerializeField] private Image perfectUI;
    [SerializeField] GameSystem gamesystem;
    [SerializeField, Tooltip("クリアUI出てから遷移する秒数")] private float Time;

    private void Update()
    {
        gamesystem = GameObject.FindWithTag("Player").GetComponent<GameSystem>();
        if (perfectUI.color.a == 0 && gamesystem.isGameClear)
        {
            perfectUI.color = new Color(255, 255, 255, 255);
            Invoke("SceneLoad", Time);
        }

    }

    private void SceneLoad()
    {
        SceneManager.LoadScene("Title");
    }
}

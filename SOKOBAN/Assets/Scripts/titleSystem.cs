// ---------------------------------------------------------  
// titleSystem.cs  
//   
// 作成日:  
// 作成者:  sasaki rio
// ---------------------------------------------------------  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class titleSystem : MonoBehaviour
{
    [SerializeField] private Image FadeOutObject;
    [SerializeField] private float FadeSpeed;
   
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("SOKOBAN");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}

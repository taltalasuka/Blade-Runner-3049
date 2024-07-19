using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenuUI_Manager : MonoBehaviour
{
   public void ButtonStart()
   {
      SceneManager.LoadScene(1);
   }

   public void ButtonQuit()
   {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#endif
      Application.Quit();
   }
}

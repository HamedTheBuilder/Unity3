using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Astroid");
    }


    public void QuitGame()
    {
        // إغلاق اللعبة
        Application.Quit();

        // في المحرر يتوقف اللعب
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Debug.Log("🎮 اللعبة انتهت!");
    }
}
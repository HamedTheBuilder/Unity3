using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("Levels Canvas Settings")]
    public GameObject levelsCanvas;
    public Button[] levelButtons;
    public RawImage levelPreviewImage;

    [Header("Level Preview Textures")]
    public Texture[] levelPreviewTextures;
    public Texture defaultPreviewTexture;

    [Header("Level Scenes")]
    public string[] levelSceneNames = new string[4] { "Astroid 1", "Astroid 2", "Astroid 3", "Astroid 4" };

    [Header("Object Visibility Settings")]
    public GameObject objectToHide; // ÇáÃæÈÌíßÊ ÇáĞí ÊÑíÏ ÅÎİÇÁå ÚäÏ İÊÍ ÇáãÓÊæíÇÊ

    private int currentHoveredLevel = -1;
    private bool wasObjectActive; // áÊÎÒíä ÍÇáÉ ÇáÃæÈÌíßÊ ŞÈá ÇáÅÎİÇÁ

    void Start()
    {
        // ÅÎİÇÁ ßÇäİÓ ÇáãÓÊæíÇÊ ÚäÏ ÈÏÇíÉ ÇááÚÈÉ
        if (levelsCanvas != null)
            levelsCanvas.SetActive(false);

        // ÊÚííä ÇáÕæÑÉ ÇáÇİÊÑÇÖíÉ
        SetDefaultPreview();

        // ÊİÚíá ÌãíÚ ÇáÃÒÑÇÑ (íãßä ÊÚÏíáåÇ ÍÓÈ ÇáãÓÊæíÇÊ ÇáãßÊãáÉ)
        EnableLevelButtons();
    }

    // ÊİÚíá ÇáÃÒÑÇÑ ÍÓÈ ÇáÊŞÏã İí ÇááÚÈÉ
    private void EnableLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                // åäÇ íãßä ÅÖÇİÉ ÔÑØ ÅĞÇ ßÇä ÇáãÓÊæì ãİÊæÍ Ãã áÇ
                levelButtons[i].interactable = true;
            }
        }
    }

    // ÒÑ ÇáÎÑæÌ ãä ÇááÚÈÉ
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // İÊÍ ßÇäİÓ ÇáãÓÊæíÇÊ
    public void OpenLevels()
    {
        if (levelsCanvas != null)
        {
            // ÍİÙ ÍÇáÉ ÇáÃæÈÌíßÊ ŞÈá ÅÎİÇÁå
            if (objectToHide != null)
            {
                wasObjectActive = objectToHide.activeSelf;
                objectToHide.SetActive(false);
            }

            levelsCanvas.SetActive(true);
            SetDefaultPreview();
            currentHoveredLevel = -1;
        }
    }

    // ÅÛáÇŞ ßÇäİÓ ÇáãÓÊæíÇÊ
    public void CloseLevels()
    {
        if (levelsCanvas != null)
            levelsCanvas.SetActive(false);

        // ÅÚÇÏÉ ÙåæÑ ÇáÃæÈÌíßÊ ÅĞÇ ßÇä ãÎİíÇğ
        if (objectToHide != null && wasObjectActive)
        {
            objectToHide.SetActive(true);
        }

        SetDefaultPreview();
        currentHoveredLevel = -1;
    }

    // ÊÛííÑ ÇáÕæÑÉ ÚäÏ ãÑæÑ ÇáãÇæÓ Úáì ÒÑ
    public void OnLevelButtonHover(int levelIndex)
    {
        if (levelPreviewImage != null &&
            levelIndex >= 0 &&
            levelIndex < levelPreviewTextures.Length &&
            levelPreviewTextures[levelIndex] != null)
        {
            levelPreviewImage.texture = levelPreviewTextures[levelIndex];
            currentHoveredLevel = levelIndex;
        }
    }

    // ÅÚÇÏÉ ÊÚííä ÇáÕæÑÉ Åáì ÇáÇİÊÑÇÖíÉ
    public void OnLevelButtonExit()
    {
        // İŞØ ÅĞÇ ßÇä åĞÇ åæ ÇáÒÑ ÇáĞí ßÇä ãÑæÑ Úáíå ÇáãÇæÓ
        if (currentHoveredLevel != -1)
        {
            SetDefaultPreview();
            currentHoveredLevel = -1;
        }
    }

    // ÊÚííä ÇáÕæÑÉ ÇáÇİÊÑÇÖíÉ
    private void SetDefaultPreview()
    {
        if (levelPreviewImage != null && defaultPreviewTexture != null)
        {
            levelPreviewImage.texture = defaultPreviewTexture;
        }
        else if (levelPreviewImage != null && levelPreviewTextures.Length > 0 && levelPreviewTextures[0] != null)
        {
            // ÇÓÊÎÏÇã Ãæá ÕæÑÉ ßÇİÊÑÇÖíÉ ÅĞÇ áã íÊã ÊÚííä ÕæÑÉ ÇİÊÑÇÖíÉ
            levelPreviewImage.texture = levelPreviewTextures[0];
        }
    }

    // ÊÍãíá ãÓÊæì ãÚíä - ÇáØÑíŞÉ ÇáÂãäÉ
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelSceneNames.Length && !string.IsNullOrEmpty(levelSceneNames[levelIndex]))
        {
            string sceneName = levelSceneNames[levelIndex];
            Debug.Log("ÌÇÑí ÊÍãíá ÇáãÓÊæì: " + sceneName);

            // ÇáÊÍŞŞ ÅĞÇ ßÇä ÇáãÔåÏ ãæÌæÏ İí Build Settings
            if (IsSceneInBuild(sceneName))
            {
                // ÅÚÇÏÉ ÙåæÑ ÇáÃæÈÌíßÊ ŞÈá ÊÍãíá ÇáÓíä (ÇÎÊíÇÑí)
                if (objectToHide != null && wasObjectActive)
                {
                    objectToHide.SetActive(true);
                }

                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError($"ÇáãÔåÏ '{sceneName}' ÛíÑ ãÖÇİ Åáì Build Settings!");
                ShowSceneError(sceneName);
            }
        }
        else
        {
            Debug.LogError("ãÄÔÑ ÇáãÓÊæì ÛíÑ ÕÇáÍ: " + levelIndex);
        }
    }

    // ÇáÊÍŞŞ ÅĞÇ ßÇä ÇáãÔåÏ ãÖÇİ Åáì Build Settings
    private bool IsSceneInBuild(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    // ØÑíŞÉ ÈÏíáÉ ááÊÍŞŞ ãä ÇáãÔåÏ
    private bool CanLoadScene(string sceneName)
    {
        try
        {
            // åĞå ÇáØÑíŞÉ ÊÊÍŞŞ ÅĞÇ ßÇä ÇáãÔåÏ íãßä ÊÍãíáå
            return Application.CanStreamedLevelBeLoaded(sceneName);
        }
        catch
        {
            return false;
        }
    }

    // ÚÑÖ ÑÓÇáÉ ÎØÃ ááãÓÊÎÏã
    private void ShowSceneError(string sceneName)
    {
        // íãßäß ÅÖÇİÉ UI áÚÑÖ ÇáÑÓÇáÉ ááÇÚÈ
        Debug.LogWarning($"áÇ íãßä ÊÍãíá ÇáãÓÊæì '{sceneName}'. ÊÃßÏ ãä ÅÖÇİÊå Åáì Build Settings.");

        // åäÇ íãßäß ÅÙåÇÑ äÇİĞÉ ÎØÃ ááÇÚÈ
        // ShowErrorPopup($"Level '{sceneName}' is not available!");
    }

    // ÇáÚæÏÉ Åáì ÇáŞÇÆãÉ ÇáÑÆíÓíÉ
    public void LoadMainMenu()
    {
        // ÅÚÇÏÉ ÙåæÑ ÇáÃæÈÌíßÊ ÅĞÇ ßÇä ãÎİíÇğ
        if (objectToHide != null && wasObjectActive)
        {
            objectToHide.SetActive(true);
        }

        string mainMenuScene = "MainMenu";

        if (IsSceneInBuild(mainMenuScene))
        {
            SceneManager.LoadScene(mainMenuScene);
        }
        else
        {
            Debug.LogError($"ãÔåÏ ÇáŞÇÆãÉ ÇáÑÆíÓíÉ '{mainMenuScene}' ÛíÑ ãÖÇİ Åáì Build Settings!");
            // ãÍÇæáÉ ÇÓÊÎÏÇã ÇáãÔåÏ ÇáÃæá İí Build Settings ßÈÏíá
            if (SceneManager.sceneCountInBuildSettings > 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    // ØÑíŞÉ áÊÍãíá ÇáãÔåÏ ÈÇÓãå ãÈÇÔÑÉ
    public void LoadLevelByName(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName) && IsSceneInBuild(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"ÇáãÔåÏ '{sceneName}' ÛíÑ ãÖÇİ Åáì Build Settings!");
            ShowSceneError(sceneName);
        }
    }
}
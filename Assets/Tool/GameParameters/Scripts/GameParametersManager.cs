using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameParametersManager : MonoBehaviour
{
    #region UI Items
    [Header("UI Items :")]
    [Tooltip("Dropdown selection scenes.")]
    [SerializeField] private TMP_Dropdown _dropdownSelectScene;
    #endregion

    private void Start()
    {
        InitDropdownSelectScene();
    }

    #region Dropdown Selection Scenes
    /// <summary>
    /// Initialisation of the Dropdown Selection Scene.
    /// </summary>
    private void InitDropdownSelectScene()
    {
        List<string> scenes = new List<string>(); 
        int sceneIndex = SceneManager.sceneCountInBuildSettings;
        for (int i = 1; i < sceneIndex; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            scenes.Add(sceneName);
        }

        _dropdownSelectScene.ClearOptions();
        _dropdownSelectScene.AddOptions(scenes);
    }

    /// <summary>
    /// Loading the selected scene.
    /// </summary>
    public void LoadSelectedScene()
    {
        string sceneName = _dropdownSelectScene.captionText.text;
        SceneManager.LoadSceneAsync(sceneName);
    }
    #endregion
}

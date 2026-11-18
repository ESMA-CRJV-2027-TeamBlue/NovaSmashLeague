using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DEBUG_SceneChanger : MonoBehaviour
{
    public string firstSceneToLoad;
    public List<String> sceneNames;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.LoadScene(firstSceneToLoad);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (sceneNames.Count > 0)
            {
                SceneManager.LoadScene(sceneNames[0]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            if (sceneNames.Count > 1)
            {
                SceneManager.LoadScene(sceneNames[1]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            if (sceneNames.Count > 2)
            {
                SceneManager.LoadScene(sceneNames[2]);
            }
        }
    }
}

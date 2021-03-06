﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerController : MonoBehaviour {

    private readonly string SavePath;

    public void SaveGame()
    {
        SaveFile save = CreateSaveFileObject();
        Debug.Log(save.completedLevels.Count);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/WizardingGameSave.save");
        bf.Serialize(file, save);
        file.Close();
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (File.Exists(GetSavePath()))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(GetSavePath(), FileMode.Open);
            SaveFile save = (SaveFile)bf.Deserialize(file);
            file.Close();
            CrossSceneInformation.CompletedLevels = save.completedLevels;
        }
    }

    private SaveFile CreateSaveFileObject()
    {
        SaveFile save = new SaveFile
        {
            completedLevels = CrossSceneInformation.CompletedLevels
        };

        return save;
    } 

    private string GetSavePath()
    {
        return Application.persistentDataPath + "/WizardingGameSave.save";
    }

    public void NewGame()
    {
        File.Delete(Application.persistentDataPath + "/WizardingGameSave.save");
        SceneManager.LoadScene("Level_1");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_1"));
    }

    public void LoadGameFromTitle()
    {
        SceneManager.LoadScene("MainMenu");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
    }
}

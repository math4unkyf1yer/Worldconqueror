using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string path = Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Saved to: " + path);
    }

    public static SaveData Load()
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found");
            return null;
        }
        string json = File.ReadAllText(path);

        return JsonUtility.FromJson<SaveData>(json);
    }
    public static void ClearSave()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }
}

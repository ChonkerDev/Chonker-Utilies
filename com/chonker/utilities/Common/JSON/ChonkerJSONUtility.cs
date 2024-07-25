using System.IO;
using UnityEngine;

public class ChonkerJSONUtility
{
    public static T readFile<T>(string fileName) {
        string filePath = Application.persistentDataPath + "/" + fileName;
        // Does the file exist?
        if (File.Exists(filePath)) {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(filePath);

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            return JsonUtility.FromJson<T>(fileContents);
        }

        Debug.LogWarning("Cannot find file at path: " + filePath);
        return default;
    }

    public static void writeFile<T>(string fileName, T jsonData) {
        string filePath = Application.persistentDataPath + "/" + fileName;
        // Serialize the object into JSON and save string.
        string jsonString = JsonUtility.ToJson(jsonData);

        // Write JSON to file.
        File.WriteAllText(filePath, jsonString);
    }
}
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public abstract class SaveGame
{
}

public static class SaveGameSystem
{
    /// <summary>
    /// Saves the game
    /// </summary>
    /// <param name="saveGame">The game to save</param>
    /// <param name="name">The save name</param>
    /// <returns>True if the save was a success, false if it wasn't</returns>
    public static bool Save(SaveGame saveGame, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        try
        {
            formatter.Serialize(stream, saveGame);
            File.WriteAllBytes(GetSavePath(name), stream.ToArray());
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Loads a saved game
    /// </summary>
    /// <param name="name">The name on the saved game</param>
    /// <returns>The saved game if it exists</returns>
    public static (bool isValid, SaveGame game) LoadGame(string name)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            byte[] bytes = File.ReadAllBytes(GetSavePath(name));

            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return (true, formatter.Deserialize(stream) as SaveGame);
        }
        catch (SerializationException)
        {
            return (false, null);
        }
    }

    /// <summary>
    /// Deletes a saved game
    /// </summary>
    /// <param name="name">The name on the save to delete</param>
    /// <returns>True if the save was deleted, false if it wasn't</returns>
    public static bool DeleteSaveGame(string name)
    {
        try
        {
            File.Delete(GetSavePath(name));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a saved game with the name exists
    /// </summary>
    /// <param name="name">The name on the saved game</param>
    /// <returns>True if the saved game exists, false if it doesn't</returns>
    public static bool DoesSaveGameExist(string name)
    {
        return File.Exists(GetSavePath(name));
    }

    /// <summary>
    /// Gets the path to the saved game
    /// </summary>
    /// <param name="name">The name on the saved game</param>
    /// <returns>The path of the saved game</returns>
    private static string GetSavePath(string name)
    {
        return Path.Combine(Application.persistentDataPath, $"{name}.sav");
    }
}

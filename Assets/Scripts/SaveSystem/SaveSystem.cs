using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static SavedOptions options;
    private static List<SavedHighscore> highscores;

    /// <summary>
    /// Saves the game
    /// </summary>
    /// <param name="saveGame">The game to save</param>
    /// <param name="name">The save name</param>
    /// <returns>True if the save was a success, false if it wasn't</returns>
    public static bool SaveGame(SavedGame saveGame, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        try
        {
            formatter.Serialize(stream, saveGame);
            File.WriteAllBytes(GetSavePath(name), stream.ToArray());
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Loads a saved game
    /// </summary>
    /// <param name="name">The name on the saved game</param>
    /// <returns>The saved game if it exists</returns>
    public static (bool isValid, SavedGame game) LoadGame(string name)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            byte[] bytes = File.ReadAllBytes(GetSavePath(name));

            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return (true, formatter.Deserialize(stream) as SavedGame);
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

    /// <summary>
    /// Saves the options
    /// </summary>
    /// <param name="options">The options to save</param>
    /// <returns>If saving the options was a success</returns>
    public static bool SaveOptions(SavedOptions options)
    {
        SaveSystem.options = new SavedOptions(options);
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        try
        {
            formatter.Serialize(stream, options);
            File.WriteAllBytes(GetOptionsPath(), stream.ToArray());
            return true;
        }
        catch (SerializationException)
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the saved options
    /// </summary>
    /// <returns>The saved options</returns>
    public static SavedOptions GetOptions()
    {
        if (SaveSystem.options != null)
            return new SavedOptions(SaveSystem.options);

        SavedOptions options = new SavedOptions();
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        try
        {
            if (File.Exists(GetOptionsPath()))
            {
                byte[] bytes = File.ReadAllBytes(GetOptionsPath());
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                SavedOptions loadedOptions = formatter.Deserialize(stream) as SavedOptions;

                options.BackgroundMusic = loadedOptions.BackgroundMusic;
                options.SoundEffects = loadedOptions.SoundEffects;
                options.ShakingEffect = loadedOptions.ShakingEffect;
                options.Fullscreen = loadedOptions.Fullscreen;
                options.AutoPauseOnFocusLose = loadedOptions.AutoPauseOnFocusLose;

                options.Resolution.Width = loadedOptions.Resolution.Width;
                options.Resolution.Height = loadedOptions.Resolution.Height;
                options.Resolution.RefreshRate = loadedOptions.Resolution.RefreshRate;
            }
            else
                SaveOptions(options);
        }
        catch (SerializationException)
        {
            SaveOptions(options);
        }

        SaveSystem.options = new SavedOptions(options);
        return options;
    }

    /// <summary>
    /// Gets the path for the options file
    /// </summary>
    /// <returns>The path for the options file</returns>
    private static string GetOptionsPath()
    {
        return Path.Combine(Application.persistentDataPath, "options.bin");
    }

    /// <summary>
    /// Gets the list of highscores
    /// </summary>
    /// <returns>List of highscores</returns>
    public static List<SavedHighscore> GetHighscores()
    {
        List<SavedHighscore> highscores = new List<SavedHighscore>();

        if (SaveSystem.highscores is null)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            if (File.Exists(GetHighscorePath()))
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(GetHighscorePath());
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Position = 0;
                    highscores = formatter.Deserialize(stream) as List<SavedHighscore>;
                }
                catch (SerializationException)
                {
                    SaveHighscores(highscores);
                }
            }
        }
        else
            highscores = SaveSystem.highscores;

        highscores = highscores.OrderByDescending(x => x.Score).ToList();
        return highscores;
    }

    /// <summary>
    /// Saves the highscores
    /// </summary>
    /// <param name="highscores">The highscores to save</param>
    /// <returns>If saving the highscores was a success</returns>
    public static bool SaveHighscores(List<SavedHighscore> highscores)
    {
        if (highscores.Count > 10)
            highscores = highscores.Take(10).ToList();

        SaveSystem.highscores = highscores;
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        try
        {
            formatter.Serialize(stream, highscores);
            File.WriteAllBytes(GetHighscorePath(), stream.ToArray());
            return true;
        }
        catch (SerializationException)
        {
            return false;
        }
    }

    ///// <summary>
    ///// Saves the highscore
    ///// </summary>
    ///// <param name="highscore">The highscore to save</param>
    ///// <returns>If saving the highscore was a success</returns>
    //public static bool SaveHighscore(SavedHighscore highscore)
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    MemoryStream stream = new MemoryStream();

    //    try
    //    {
    //        formatter.Serialize(stream, highscore);
    //        File.WriteAllBytes(GetHighscorePath(), stream.ToArray());
    //        return true;
    //    }
    //    catch (SerializationException)
    //    {
    //        return false;
    //    }
    //}

    ///// <summary>
    ///// Gets the saved highscore
    ///// </summary>
    ///// <returns>The saved highscore</returns>
    //public static SavedHighscore GetHighscore()
    //{
    //    SavedHighscore highscore = new SavedHighscore();
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    MemoryStream stream = new MemoryStream();

    //    try
    //    {
    //        if (File.Exists(GetHighscorePath()))
    //        {
    //            byte[] bytes = File.ReadAllBytes(GetHighscorePath());
    //            stream.Write(bytes, 0, bytes.Length);
    //            stream.Position = 0;
    //            highscore = formatter.Deserialize(stream) as SavedHighscore;
    //        }
    //        else
    //            SaveHighscore(highscore);
    //    }
    //    catch (SerializationException)
    //    {
    //        SaveHighscore(highscore);
    //    }

    //    return highscore;
    //}

    /// <summary>
    /// Gets the path for the highscore file
    /// </summary>
    /// <returns>The path for the highscore file</returns>
    private static string GetHighscorePath()
    {
        return Path.Combine(Application.persistentDataPath, "highscores.bin");
    }
}

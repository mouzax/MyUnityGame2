using UnityEngine;

public static class SaveGame
{
    const string K_HasSave   = "save_exists";
    const string K_Scene     = "save_scene";
    const string K_PosX      = "save_x";
    const string K_PosY      = "save_y";
    const string K_PosZ      = "save_z";

    public static bool HasSave() => PlayerPrefs.GetInt(K_HasSave, 0) == 1;

    public static void MarkHasSave(string sceneName)
    {
        PlayerPrefs.SetInt(K_HasSave, 1);
        PlayerPrefs.SetString(K_Scene, sceneName);
        PlayerPrefs.Save();
    }

    public static void Save(string sceneName, Vector3 position)
    {
        PlayerPrefs.SetInt(K_HasSave, 1);
        PlayerPrefs.SetString(K_Scene, sceneName);
        PlayerPrefs.SetFloat(K_PosX, position.x);
        PlayerPrefs.SetFloat(K_PosY, position.y);
        PlayerPrefs.SetFloat(K_PosZ, position.z);
        PlayerPrefs.Save();
    }

    public static bool TryGet(out string sceneName, out Vector3 position)
    {
        sceneName = PlayerPrefs.GetString(K_Scene, "");
        position = new Vector3(
            PlayerPrefs.GetFloat(K_PosX, 0),
            PlayerPrefs.GetFloat(K_PosY, 0),
            PlayerPrefs.GetFloat(K_PosZ, 0)
        );
        return HasSave() && !string.IsNullOrEmpty(sceneName);
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(K_HasSave);
        PlayerPrefs.DeleteKey(K_Scene);
        PlayerPrefs.DeleteKey(K_PosX);
        PlayerPrefs.DeleteKey(K_PosY);
        PlayerPrefs.DeleteKey(K_PosZ);
        PlayerPrefs.Save();
    }
}
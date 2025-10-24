using UnityEngine;

public static class GameDef
{
    public static class Scenes
    {
        public const string Title = "TitleScene";
        public const string Lobby = "LobbyScene";
        public const string Game = "GameScene";
    }

    public static class Hash
    {
        public static readonly int State = Animator.StringToHash("state");
    }

    public static class Formats
    {
        public const string DateTime = "yyyy-MM-dd HH:mm:ss";
    }
    // public static class Paths
    // {
    //     public const string CharacterData = "Data/character_data";
    //     public const string CharacterSlotPrefab = "Prefabs/CharacterSlotPrefab";
    //     public const string ArtsRoot = "Arts";
    //     public const string PrefabRoot = "Prefabs";
    //     public static string CharacterArt(string spriteName) => $"{ArtsRoot}/{spriteName}";
    // }

    // public static class Tags
    // {
    //     public const string Border = "Border";
    //     public const string BorderBullet = "BorderBullet";
    // }
}
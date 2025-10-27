using System.IO;
using UnityEngine;

public static class GameDef
{
    public static class Scenes
    {
        public const string Title = "TitleScene";
        public const string Lobby = "LobbyScene";
        public const string Game = "GameScene";
        public const string Rank = "RankScene";
    }

    public static class Hash
    {
        public static readonly int State = Animator.StringToHash("state");
        public static readonly int IsHit = Animator.StringToHash("isHit");
        public static readonly int IsExplosion = Animator.StringToHash("isExplosion");
        public static readonly int IsActive = Animator.StringToHash("isActive");
        public static readonly int In = Animator.StringToHash("in");
        public static readonly int Out = Animator.StringToHash("out");
    }

    public static class Formats
    {
        public const string DateTime = "yyyy-MM-dd HH:mm:ss";
    }
    
    public static class Paths
    {
        public const string Enemies = "Arts/Enemies";
        public static readonly string ScorePath = Path.Combine(Application.persistentDataPath, "score_data.json");
        // public const string CharacterData = "Data/character_data";
        // public const string CharacterSlotPrefab = "Prefabs/CharacterSlotPrefab";
        // public const string ArtsRoot = "Arts";
        // public const string PrefabRoot = "Prefabs";
        // public static string CharacterArt(string spriteName) => $"{ArtsRoot}/{spriteName}";
    }

    // public static class Tags
    // {
    //     public const string Border = "Border";
    //     public const string BorderBullet = "BorderBullet";
    // }
}
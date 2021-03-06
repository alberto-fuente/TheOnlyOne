public static class GameUtils
{
    public const int MAXHEALTH = 100;
    public const int MAXSHIELD = 100;
    public const int numberOfRarities = 4;
    public enum TypeOfItem
    {
        MELEE,
        GUN,
        THROWEABLE,
        CONSUMIBLE
    }
    public enum TypeOfPack
    {
        AMMO,
        HEALTH,
        ARMOR
    }

    public enum SceneIndex
    {
        LOGIN = 0,
        MAINMENU = 1,
        GAME = 2,
    }
}
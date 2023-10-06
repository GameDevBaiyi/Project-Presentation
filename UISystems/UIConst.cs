using System.Text;

namespace LowLevelSystems.UISystems
{
public static class UiConst
{
    public const string UICommonPackage = "UICommon";
    public const string TexturesPackage = "TexturesPackage";
    public const string CharacterPackage = "CharacterPackage";
    public const string ResIcon = "ResIcon";
    public const string ResCartoon = "ResCartoon";
    public const string ResMap = "ResMap";
    public const string ResCityMap = "ResCityMap";
    public const string DialogueHudPackage = "DialogueHudPackage";
    public const string CityHudPackage = "CityHudPackage";
    public const string LoadingPackage = "LoadingPackage";
    public const string ResTeaching = "ResTeaching";
    public const string TeachingPackage = "TeachingPackage";
    public const string CommonPromptPackage = "CommonPromptPackage";
    public const string NpcInteractionsPackage = "NpcInteractionsPackage";

    private const string _fguiUrlPrefix = "ui:/";
    private const string _folderSeparator = "/";

    public const string UrlOfUnknownHeadIcon = "Head_R999999_0";

    private static readonly StringBuilder _stringBuilder = new StringBuilder();
    public static string GetLoaderUrl(params string[] folderNames)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(_fguiUrlPrefix);
        foreach (string folderName in folderNames)
        {
            _stringBuilder.Append(_folderSeparator);
            _stringBuilder.Append(folderName);
        }
        return _stringBuilder.ToString();
    }
}
}
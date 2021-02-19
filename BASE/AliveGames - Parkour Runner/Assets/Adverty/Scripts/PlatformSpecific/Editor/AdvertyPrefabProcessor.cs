using System.Linq;
using UnityEditor;

namespace Adverty.Editor
{
    internal class AdvertyPrefabProcessor
    {
        private static string SdkLocation = EditorUtils.AdvertyDirectory;

        private const string GAME_UNIT_PREFAB_NAME = "Prefabs/GameUnit.prefab";
        private const string MENU_UNIT_PREFAB_NAME = "Prefabs/MenuUnit.prefab";

        private static readonly string[] advertyPrefabsPaths = new[]
        {
            SdkLocation + GAME_UNIT_PREFAB_NAME,
            SdkLocation + MENU_UNIT_PREFAB_NAME
        };

        [InitializeOnLoadMethod]
        public static void ValidatePrefabs()
        {
            foreach(string prefabPath in advertyPrefabsPaths)
            {
                AdvertyPrefabValidator.ValidatePrefab(prefabPath, prefabPath.Contains(MENU_UNIT_PREFAB_NAME) ? AdUnit.UnitType.MenuUnit : AdUnit.UnitType.GameUnit);
            }
        }
    }
}

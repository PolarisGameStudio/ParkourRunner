using UnityEditor;
using UnityEngine;
using System.Linq;
using Adverty.AdUnit;
using UnityEngine.UI;

namespace Adverty.Editor
{
    internal class AdvertyPrefabValidator : UnityEditor.Editor
    {
        private const string DEFAULT_GAMEOBJECT_NAME = "Unit";
        private const string DEFAULT_MESH_LOCATION = "Meshes/UnitMesh";
        private const string DEFAULT_MATERIAL_LOCATION = "Materials/UnitMaterial";

        internal static void ValidatePrefab(string assetPath, UnitType unitType)
        {
            Fit(assetPath, unitType);
        }

        private static void Fit(string inputObject, UnitType type)
        {
            GameObject instatiatedPrefab = GetInstantiatedUnitPrefab(inputObject);
            if(IsSomethingMissedInPrefab(instatiatedPrefab, type))
            {
                GameObject newPrefabInstance = CreateNewUnit(type);
                SavePrefab(newPrefabInstance, inputObject);
                DestroyImmediate(newPrefabInstance);
            }
            DestroyPrefabInstance(instatiatedPrefab);
        }

        private static void DestroyPrefabInstance(GameObject instantiatedPrefab)
        {
            PrefabUtility.UnloadPrefabContents(instantiatedPrefab);
        }

        private static void SavePrefab(GameObject instance, string targetPath)
        {
            PrefabUtility.SaveAsPrefabAsset(instance, targetPath);
        }

        private static GameObject GetInstantiatedUnitPrefab(string prefabPath)
        {
            return PrefabUtility.LoadPrefabContents(prefabPath);
        }

        private static GameObject CreateNewUnit(UnitType unitType)
        {
            GameObject unit = new GameObject(DEFAULT_GAMEOBJECT_NAME);
         
            if(unitType == UnitType.GameUnit)
            {
                unit.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load(DEFAULT_MESH_LOCATION);
                unit.AddComponent<MeshRenderer>().material = (Material)Resources.Load(DEFAULT_MATERIAL_LOCATION);
                unit.AddComponent<GameUnit>();
                return unit;
            }

            if(unitType == UnitType.MenuUnit)
            {
                unit.AddComponent<RawImage>();
                unit.AddComponent<MenuUnit>();
                unit.AddComponent<MenuUnitClickHandler>();
            }

            return unit;
        }

        private static bool IsContainsMissedComponent(GameObject childObject)
        {
            Component[] components = childObject.GetComponents<Component>();
            return components.Any(component => !component);
        }

        private static bool IsSomethingMissedInPrefab(GameObject childObject, UnitType unitType)
        {
            if(IsContainsMissedComponent(childObject))
            {
                return true;
            }

            switch(unitType)
            {
                case UnitType.GameUnit:
                    return !childObject.GetComponent<MeshRenderer>().sharedMaterial || !childObject.GetComponent<MeshFilter>().sharedMesh;
                    
                case UnitType.MenuUnit:
                    return !childObject.GetComponent<MenuUnitClickHandler>();

                default:
                    return false;
            }
        }
    }
}

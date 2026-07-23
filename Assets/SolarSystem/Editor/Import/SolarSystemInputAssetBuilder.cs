using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tanvir.SolarSystem.Editor.Import
{
    /// <summary>Creates the project-owned Input System asset from its reviewed control contract.</summary>
    internal static class SolarSystemInputAssetBuilder
    {
        private const string InputFolder = "Assets/SolarSystem/Settings/Input";
        private const string InputAssetPath = InputFolder + "/IA_SolarSystem.asset";
        private const string AssetName = "IA_SolarSystem";
        private const string ContractLabel = "SolarSystemInputContract-v2";

        internal static InputActionAsset Build()
        {
            EnsureFolder("Assets/SolarSystem/Settings");
            EnsureFolder(InputFolder);

            InputActionAsset actions =
                AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAssetPath);
            if (actions == null)
            {
                actions = ScriptableObject.CreateInstance<InputActionAsset>();
                AssetDatabase.CreateAsset(actions, InputAssetPath);
            }

            if (HasContractLabel(actions))
            {
                return actions;
            }

            while (actions.actionMaps.Count > 0)
            {
                actions.RemoveActionMap(actions.actionMaps[0]);
            }

            actions.name = AssetName;
            InputActionMap explorer = actions.AddActionMap("Explorer");
            AddMovementActions(explorer);
            AddPointerActions(explorer);
            AddDiscreteActions(explorer);
            AssetDatabase.SetLabels(actions, new[] { ContractLabel });
            EditorUtility.SetDirty(actions);
            AssetDatabase.SaveAssetIfDirty(actions);
            AssetDatabase.ImportAsset(InputAssetPath, ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAssetPath);
        }

        private static bool HasContractLabel(InputActionAsset actions)
        {
            foreach (string label in AssetDatabase.GetLabels(actions))
            {
                if (label == ContractLabel)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddMovementActions(InputActionMap explorer)
        {
            InputAction move = explorer.AddAction("Move", InputActionType.Value, expectedControlLayout: "Vector2");
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

            InputAction elevate =
                explorer.AddAction("Elevate", InputActionType.Value, expectedControlLayout: "Axis");
            elevate.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/q")
                .With("Positive", "<Keyboard>/e");
            explorer.AddAction("Boost", InputActionType.Button, "<Keyboard>/leftShift");
        }

        private static void AddPointerActions(InputActionMap explorer)
        {
            explorer.AddAction(
                "Look",
                InputActionType.Value,
                "<Mouse>/delta",
                expectedControlLayout: "Vector2");
            explorer.AddAction("LookModifier", InputActionType.Button, "<Mouse>/rightButton");
            explorer.AddAction(
                "PointerPosition",
                InputActionType.PassThrough,
                "<Mouse>/position",
                expectedControlLayout: "Vector2");
            explorer.AddAction(
                "Zoom",
                InputActionType.PassThrough,
                "<Mouse>/scroll/y",
                expectedControlLayout: "Axis");
        }

        private static void AddDiscreteActions(InputActionMap explorer)
        {
            explorer.AddAction("Select", InputActionType.Button, "<Mouse>/leftButton");
            explorer.AddAction("Focus", InputActionType.Button, "<Keyboard>/f");
            explorer.AddAction("Cancel", InputActionType.Button, "<Keyboard>/escape");
            explorer.AddAction("TogglePause", InputActionType.Button, "<Keyboard>/space");
            explorer.AddAction(
                "DecreaseTimeSpeed",
                InputActionType.Button,
                "<Keyboard>/leftBracket");
            explorer.AddAction(
                "IncreaseTimeSpeed",
                InputActionType.Button,
                "<Keyboard>/rightBracket");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            int separator = path.LastIndexOf('/');
            string parent = path.Substring(0, separator);
            string name = path.Substring(separator + 1);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}

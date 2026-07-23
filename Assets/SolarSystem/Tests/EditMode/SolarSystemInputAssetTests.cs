using NUnit.Framework;
using UnityEditor;
using UnityEngine.InputSystem;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class SolarSystemInputAssetTests
    {
        private const string AssetPath =
            "Assets/SolarSystem/Settings/Input/IA_SolarSystem.asset";

        [Test]
        public void ProjectInputAsset_DefinesReviewedExplorerContract()
        {
            InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);

            Assert.That(asset, Is.Not.Null);
            InputActionMap explorer = asset.FindActionMap("Explorer", true);
            AssertAction(explorer, "Move");
            AssertAction(explorer, "Elevate");
            AssertAction(explorer, "Look");
            AssertAction(explorer, "LookModifier");
            AssertAction(explorer, "Boost");
            AssertAction(explorer, "PointerPosition");
            AssertAction(explorer, "Zoom");
            AssertAction(explorer, "Select");
            AssertAction(explorer, "Focus");
            AssertAction(explorer, "Cancel");
        }

        [TestCase("Move", "<Keyboard>/w")]
        [TestCase("LookModifier", "<Mouse>/rightButton")]
        [TestCase("Select", "<Mouse>/leftButton")]
        [TestCase("Focus", "<Keyboard>/f")]
        [TestCase("Cancel", "<Keyboard>/escape")]
        public void ProjectInputAsset_ActionContainsExpectedBinding(
            string actionName,
            string expectedPath)
        {
            InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
            InputAction action = asset.FindActionMap("Explorer", true)
                .FindAction(actionName, true);

            Assert.That(
                action.bindings,
                Has.Some.Matches<InputBinding>(
                    binding => binding.path == expectedPath));
        }

        private static void AssertAction(InputActionMap map, string actionName)
        {
            Assert.That(map.FindAction(actionName), Is.Not.Null, actionName);
        }
    }
}

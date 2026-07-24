using NUnit.Framework;
using UnityEditor;
using UnityEngine.UIElements;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class SolarSystemHudAssetTests
    {
        private const string PanelPath =
            "Assets/SolarSystem/Settings/UI/PanelSettings_SolarSystem.asset";
        private const string VisualTreePath =
            "Assets/SolarSystem/Content/UI/SolarSystemHud.uxml";
        private const string StylePath =
            "Assets/SolarSystem/Content/UI/SolarSystemHud.uss";

        [Test]
        public void HudAssets_ProvideRequiredRuntimeContract()
        {
            PanelSettings panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelPath);
            VisualTreeAsset tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreePath);
            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);

            Assert.That(panel, Is.Not.Null);
            Assert.That(tree, Is.Not.Null);
            Assert.That(style, Is.Not.Null);

            TemplateContainer root = tree.CloneTree();
            Assert.That(root.Q<Label>("simulation-state"), Is.Not.Null);
            Assert.That(root.Q<Label>("simulation-rate"), Is.Not.Null);
            Assert.That(root.Q<Label>("selection-target"), Is.Not.Null);
            Assert.That(root.Q<Label>("scale-mode"), Is.Not.Null);
            Assert.That(root.Q<VisualElement>("control-hints"), Is.Not.Null);
            Assert.That(root.Q<Label>("control-key-click"), Is.Not.Null);
            Assert.That(root.Q<Label>("control-key-focus"), Is.Not.Null);
            Assert.That(root.Q<Label>("control-key-wheel"), Is.Not.Null);
            Assert.That(root.Q<Label>("control-key-space"), Is.Not.Null);
            Assert.That(root.Q<Label>("control-key-speed"), Is.Not.Null);
            Assert.That(root.Q<Label>("control-key-compare"), Is.Not.Null);
            Assert.That(root.Q<Label>("pause-action"), Is.Not.Null);
            Assert.That(root.Q<VisualElement>("body-information-panel"), Is.Not.Null);
            Assert.That(root.Q<VisualElement>("selection-reticle"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-name"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-category"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-summary"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-parent"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-radius"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-mass"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-rotation"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-axial-tilt"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-orbit-distance"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-orbit-period"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-scale-note"), Is.Not.Null);
            Assert.That(root.Q<Label>("body-source"), Is.Not.Null);
            Assert.That(
                root.Q<VisualElement>("scale-comparison-panel"),
                Is.Not.Null);
            Assert.That(root.Q<Label>("comparison-progress"), Is.Not.Null);
            Assert.That(root.Q<Label>("comparison-title"), Is.Not.Null);
            Assert.That(root.Q<Label>("comparison-metric"), Is.Not.Null);
            Assert.That(root.Q<Label>("comparison-description"), Is.Not.Null);
            Assert.That(root.Q<Label>("comparison-next-action"), Is.Not.Null);
        }

        [Test]
        public void PanelSettings_UseApprovedReferenceResolution()
        {
            PanelSettings panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelPath);

            Assert.That(panel.scaleMode, Is.EqualTo(PanelScaleMode.ScaleWithScreenSize));
            Assert.That(panel.referenceResolution.x, Is.EqualTo(1920));
            Assert.That(panel.referenceResolution.y, Is.EqualTo(1080));
        }
    }
}

namespace Tanvir.SolarSystem.Release
{
    /// <summary>
    /// Owns the reviewed, platform-neutral identity and output contract for release builds.
    /// </summary>
    public static class ReleaseBuildContract
    {
        /// <summary>Public creator identity serialized into players.</summary>
        public const string CompanyName = "Tanvir Ahmed Arnab";

        /// <summary>Public product title serialized into players.</summary>
        public const string ProductName = "Solar System Simulation";

        /// <summary>Approved semantic version for the first public release.</summary>
        public const string Version = "1.0.0";

        /// <summary>Shared reverse-domain application identifier.</summary>
        public const string ApplicationIdentifier =
            "com.tanvirahmedarnab.solarsystem";

        /// <summary>Sole production scene included in release players.</summary>
        public const string ProductionScenePath =
            "Assets/SolarSystem/Scenes/SolarSystem.unity";

        /// <summary>Windows first-launch width in pixels.</summary>
        public const int WindowsWidth = 1280;

        /// <summary>Windows first-launch height in pixels.</summary>
        public const int WindowsHeight = 720;

        /// <summary>WebGL first-launch canvas width in pixels.</summary>
        public const int WebWidth = 960;

        /// <summary>WebGL first-launch canvas height in pixels.</summary>
        public const int WebHeight = 540;

        /// <summary>Unity architecture value for Intel 64-bit plus Apple silicon.</summary>
        public const int MacOsUniversalArchitecture = 2;

        /// <summary>Ignored repository-relative root for release artifacts.</summary>
        public const string ReleaseRoot = "Builds/Release";

        /// <summary>Versioned Windows artifact directory.</summary>
        public const string WindowsDirectory =
            "SolarSystem-1.0.0-Windows-x86_64";

        /// <summary>Versioned Universal macOS artifact directory.</summary>
        public const string MacOsDirectory =
            "SolarSystem-1.0.0-macOS-Universal";

        /// <summary>Versioned WebGL artifact directory.</summary>
        public const string WebGlDirectory =
            "SolarSystem-1.0.0-WebGL";

        /// <summary>Windows player executable name.</summary>
        public const string WindowsExecutable = "Solar System Simulation.exe";

        /// <summary>macOS application-bundle name.</summary>
        public const string MacOsApplication = "Solar System Simulation.app";

        /// <summary>Machine-readable evidence filename written beside a build.</summary>
        public const string BuildReportFileName = "release-build-report.json";

        /// <summary>
        /// Creates the approved scene array in deterministic build order.
        /// </summary>
        public static string[] CreateScenePaths()
        {
            return new[] { ProductionScenePath };
        }
    }
}

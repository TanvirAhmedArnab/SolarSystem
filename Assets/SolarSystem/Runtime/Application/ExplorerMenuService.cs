using System;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Available pages in the unified player-facing Explorer Menu.</summary>
    public enum ExplorerMenuPage
    {
        Help = 0,
        Settings = 1,
        CreditsAndSources = 2
    }

    /// <summary>Owns modal visibility and active-page state without referencing UI Toolkit.</summary>
    public sealed class ExplorerMenuService
    {
        public event Action Changed;

        public bool IsOpen { get; private set; }
        public ExplorerMenuPage ActivePage { get; private set; } = ExplorerMenuPage.Help;

        public void Open(ExplorerMenuPage page)
        {
            Validate(page);
            if (IsOpen && ActivePage == page)
            {
                return;
            }

            IsOpen = true;
            ActivePage = page;
            Changed?.Invoke();
        }

        public void SetPage(ExplorerMenuPage page)
        {
            Validate(page);
            if (!IsOpen)
            {
                Open(page);
                return;
            }

            if (ActivePage == page)
            {
                return;
            }

            ActivePage = page;
            Changed?.Invoke();
        }

        public bool Close()
        {
            if (!IsOpen)
            {
                return false;
            }

            IsOpen = false;
            Changed?.Invoke();
            return true;
        }

        private static void Validate(ExplorerMenuPage page)
        {
            if (page < ExplorerMenuPage.Help ||
                page > ExplorerMenuPage.CreditsAndSources)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }
        }
    }
}

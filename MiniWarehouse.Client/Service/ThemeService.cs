using Microsoft.JSInterop;
using MudBlazor;

namespace MiniWarehouse.Client.Service
{
    public class ThemeService
    {
        private readonly IJSRuntime _js;
        public event Action? OnChange;
        private bool _isDarkMode;

        public ThemeService(IJSRuntime js)
        {
            _js = js;
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode == value) return;
                _isDarkMode = value;
                OnChange?.Invoke(); 
            }
        }

        // Diese Methode rufen wir beim App-Start einmal auf
        public async Task InitializeAsync()
        {
            var storedValue = await _js.InvokeAsync<string>("localStorage.getItem", "darkMode");
            if (bool.TryParse(storedValue, out var isDark))
            {
                _isDarkMode = isDark;
                OnChange?.Invoke();
            }
        }

        public void ToggleDarkMode() => IsDarkMode = !IsDarkMode;

        public MudTheme CurrentTheme { get; } = new MudTheme()
        {
            PaletteLight = new PaletteLight(),
            PaletteDark = new PaletteDark()
            {
                Black = "#27272f",
                Background = Colors.Gray.Darken4,
                Surface = "#1e1e2d",
                AppbarBackground = Colors.Gray.Darken4,
                DrawerBackground = Colors.Gray.Darken4,
                DrawerText = "rgba(255,255,255, 0.50)",
                DrawerIcon = "rgba(255,255,255, 0.50)"
            }
        };
    }
}
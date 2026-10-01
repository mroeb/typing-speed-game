using System.Windows;
using System.Windows.Input;
using WpfApp1.Controls;
using WpfApp1.ViewModels;

namespace WpfApp1;

public partial class MainWindow : Window
{
    public MainWindow() : this(CompositionRoot.CreateViewModel()) { }

    public MainWindow(GameViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();
        DataContext = viewModel;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);

        if (DataContext is not GameViewModel viewModel)
        {
            return;
        }

        if (e.Key is Key.Tab or Key.Escape)
        {
            viewModel.Restart();
            e.Handled = true;
            return;
        }

        if (viewModel.IsResultVisible)
        {
            if (e.Key is Key.Enter or Key.Space)
            {
                viewModel.Restart();
                e.Handled = true;
            }

            return;
        }

        if (e.Key == Key.Back)
        {
            viewModel.Backspace();
            e.Handled = true;
            return;
        }

        if (KeyboardMap.TryGetCharacter(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers, out var character))
        {
            viewModel.TypeCharacter(character);
            e.Handled = true;
        }
    }
}

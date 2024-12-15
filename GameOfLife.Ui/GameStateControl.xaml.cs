using System.Windows.Controls;

namespace GameOfLife.Ui;

public partial class GameStateControl : UserControl
{
    public required GameStateViewModel ViewModel { get; init; } = new();

    public GameStateControl()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }
}

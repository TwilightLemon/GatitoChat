using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GatitoChat.Views;

public partial class MessageBox : Window
{
    public static void Show(string message)
    {
        new MessageBox() { Hint = message }.ShowDialog(App.MainWindow);
    }
    public MessageBox()
    {
        InitializeComponent();
    }
    public string Hint
    {
        get => HintTb.Text;
        set => HintTb.Text = value;
    }

    private void OKBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
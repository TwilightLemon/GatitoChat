using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using GatitoChat.Services;
using GatitoChat.ViewModels;
using System;

namespace GatitoChat.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _vm;

    #if DEBUG
    public MainWindow()
    {
        InitializeComponent();
    }
    #endif
    
    public MainWindow(MainWindowViewModel  viewModel)
    {
        DataContext =_vm =  viewModel;
        InitializeComponent();
        Closing += MainWindow_Closing;
        MessageInputTb.AddHandler(TextBox.KeyDownEvent, MessageInputTb_PreviewKeyDown, RoutingStrategies.Tunnel);
    }

    private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        await App.GetRequiredService<ChatClientService>().CloseAll();
    }

    private void MessageInputTb_PreviewKeyDown(object? sender,KeyEventArgs e)
    {
        //回车发送消息，Ctrl(or Shift)+Enter换行
        if (e.Key == Key.Enter)
        {
            if (e.KeyModifiers is KeyModifiers.Control or KeyModifiers.Shift)
            {
                int caretIndex = MessageInputTb.CaretIndex;
                // 在光标位置插入换行符
                MessageInputTb.Text = MessageInputTb.Text.Insert(caretIndex, Environment.NewLine);

                // 更新光标位置到换行符之后
                MessageInputTb.CaretIndex = caretIndex + Environment.NewLine.Length;
            }
            else
            {
                _vm.SendMessageCommand.Execute(null);
            }
            e.Handled = true;
        }
    }
}
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using GatitoChat.Services;
using GatitoChat.ViewModels;
using System;
using GatitoChat.Models;

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
    
    public MainWindow(MainWindowViewModel viewModel,ChatClientService  chatClientService)
    {
        DataContext =_vm =  viewModel;
        InitializeComponent();
        Closing += MainWindow_Closing;
        PointerPressed += Window_OnPointerPressed;
        chatClientService.OnNewMessageReceived += ChatClientService_OnNewMessageReceived;
        MessageInputTb.AddHandler(TextBox.KeyDownEvent, MessageInputTb_PreviewKeyDown, RoutingStrategies.Tunnel);
    }

    private bool _isAtBottom;
    private void MsgScrollViewer_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        _isAtBottom =
            Math.Abs(MsgScrollViewer.Offset.Y + MsgScrollViewer.Viewport.Height - MsgScrollViewer.Extent.Height) < 5;
    }

    private void ChatClientService_OnNewMessageReceived(RoomModel room,MessageItem msg)
    {
        if (_isAtBottom && room == _vm.SelectedRoom)
        {
            MsgScrollViewer.ScrollToEnd();
        }
    }
    private void Window_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse) this.BeginMoveDrag(e);
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
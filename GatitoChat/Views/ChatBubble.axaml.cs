using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using GatitoChat.Models;

namespace GatitoChat.Views;

public partial class ChatBubble : UserControl
{
    public ChatBubble()
    {
        InitializeComponent();
    }

    static ChatBubble()
    {
        MessageProperty.Changed.AddClassHandler<ChatBubble>(MessageChanged);
    }

    private static void MessageChanged(ChatBubble sender, AvaloniaPropertyChangedEventArgs e)
    {
        sender.Refresh(e.NewValue as MessageItem);
    }
    
    public MessageItem Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public static readonly StyledProperty<MessageItem> MessageProperty =
        AvaloniaProperty.Register<ChatBubble, MessageItem>(nameof(Message));

    private void Refresh(MessageItem? value)
    {
        if (value == null) return;
        if (value.Type == SenderType.System)
        {
            Username.IsVisible = false;
           // MsgTb.TextAlignment = TextAlignment.Center;
            MsgContainer.Margin = new Thickness(0);
            MsgContainer.HorizontalAlignment =  HorizontalAlignment.Stretch;
        }else if (value.Type == SenderType.Self)
        {
            MsgContainer.HorizontalAlignment = Username.HorizontalAlignment = HorizontalAlignment.Right;
            MsgContainer.Margin = new Thickness(40,24,0,0);
            MsgContainer.Bind(BackgroundProperty,Resources.GetResourceObservable("AccentButtonBackground"));
            MsgTb.Foreground = Brushes.GhostWhite;
        }

        Username.Text = value.Name;
        MsgTb.MarkdownText = value.Content;
    }
}
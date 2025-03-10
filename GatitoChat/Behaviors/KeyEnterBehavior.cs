using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace GatitoChat.Behaviors;

public class KeyEnterBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<KeyEnterBehavior, Control, ICommand>("Command");

    static KeyEnterBehavior()
    {
        CommandProperty.Changed.AddClassHandler<Control>(OnCommandChanged);
    }

    public static void SetCommand(Control obj, ICommand value) => obj.SetValue(CommandProperty, value);
    public static ICommand GetCommand(Control obj) => obj.GetValue(CommandProperty);

    private static void OnCommandChanged(Control sender,AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is Control control)
        {
            if (e.OldValue!=null)
            {
                control.KeyDown -= Control_KeyDown;
            }

            if (e.NewValue != null)
            {
                control.KeyDown += Control_KeyDown;
            }
        }
    }

    private static void Control_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && sender is Control{ } control)
        {
            var command = GetCommand(control);
            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}

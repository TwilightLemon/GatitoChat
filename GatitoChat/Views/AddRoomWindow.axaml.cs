using Avalonia.Controls;
using GatitoChat.ViewModels;
using System;

namespace GatitoChat.Views;

public partial class AddRoomWindow : Window
{
    private readonly AddRoomWindowViewModel _vm;

    public AddRoomWindow(AddRoomWindowViewModel vm)
    {
        DataContext=_vm = vm;
        vm.RequestCloseWindow += Vm_AddRoomSuccess;
        InitializeComponent();
    }
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        RoomNameInputTb.Focus();
    }

    private void Vm_AddRoomSuccess()
    {
        _vm.RequestCloseWindow -= Vm_AddRoomSuccess;
        Close();
    }
}
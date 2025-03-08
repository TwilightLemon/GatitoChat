using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GatitoChat.Core.Models;
using GatitoChat.ViewModels;

namespace GatitoChat.Views;

public partial class AddRoomWindow : Window
{
    private readonly AddRoomWindowViewModel _vm;

    public AddRoomWindow(AddRoomWindowViewModel vm)
    {
        DataContext=_vm = vm;
        vm.AddRoomSuccess += Vm_AddRoomSuccess;
        InitializeComponent();
    }

    private async void Vm_AddRoomSuccess(RoomInfo info)
    {
        _vm.AddRoomSuccess -= Vm_AddRoomSuccess;
        await Task.Delay(1500);
        Close();
    }
}
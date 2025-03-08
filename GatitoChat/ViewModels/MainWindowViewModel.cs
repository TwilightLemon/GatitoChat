using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GatitoChat.Core;
using GatitoChat.Core.Models;
using GatitoChat.Services;
using GatitoChat.Views;

namespace GatitoChat.ViewModels;

public partial class MainWindowViewModel
    (UserProfileService userProfileService,
        IHttpClientFactory httpClientFactory,
        CommonClientService commonClientService) : ViewModelBase
{
    [ObservableProperty]private string _loginBtnHint= "Login";
    [RelayCommand]
    private void Login()
    {
        var vm = App.GetRequiredService<LoginWindowViewModel>();
        vm.OnLoginCallback += LoginWindowVm_OnLoginCallback;
        _loginWindowVmRef = new(vm);
        new LoginWindow(vm).Show();
    }
    private WeakReference<LoginWindowViewModel>?  _loginWindowVmRef;
    private void LoginWindowVm_OnLoginCallback()
    {
        if (_loginWindowVmRef != null && _loginWindowVmRef.TryGetTarget(out var vm))
        {
            vm.OnLoginCallback -= LoginWindowVm_OnLoginCallback;
        }
        _loginWindowVmRef = null;
        
        LoginBtnHint = userProfileService.Credential.UserName??"Login";
        commonClientService.RoomManager = null;
        commonClientService.ChatClient = null;
        _ = Connect();
    }
    
    [ObservableProperty]private string _connectBtnHint= "Connect";
    private bool _connected;
    [RelayCommand]
    private async Task Connect()
    {
        commonClientService.RoomManager ??= new(httpClientFactory.CreateClient(App.PublicClientFlag), userProfileService.ChatServerUrl)
            { UserCredential = userProfileService.Credential };
        commonClientService.ChatClient ??=new(userProfileService.ChatServerUrl, userProfileService.Credential);
        if (_connected)
        {
            if (await commonClientService.RoomManager.Disconnect() is {Success:true})
            {
                await commonClientService.ChatClient .Close([]);
                _connected = false;
                ConnectBtnHint = "Disconnected";
            }
        }
        else
        {
            if (await commonClientService.RoomManager.Connect() is { Success: true })
            {
                await commonClientService.ChatClient .Connect();
                _connected = true;
                ConnectBtnHint = "Connecting";
            }
        }
    }

    public ObservableCollection<RoomInfo> RoomsInfo { get; } = [];

    [RelayCommand]
    private void AddRoom()
    {
        var vm = App.GetRequiredService<AddRoomWindowViewModel>();
        vm.AddRoomSuccess += Vm_AddRoomSuccess;
        _addRoomWindowVmRef = new(vm);
        new AddRoomWindow(vm).Show();
    }
    private WeakReference<AddRoomWindowViewModel>?  _addRoomWindowVmRef;

    private void Vm_AddRoomSuccess(RoomInfo info)
    {
        if(_addRoomWindowVmRef?.TryGetTarget(out var vm) is true)
            vm.AddRoomSuccess-= Vm_AddRoomSuccess;
        _addRoomWindowVmRef = null;
        
        RoomsInfo.Add(info);
    }
}
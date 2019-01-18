using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CodeBrix.Prism.Observables;
using Microsoft.AspNet.SignalR.Client;
using SignalR.Models;

namespace AndroidSignalR.Services
{
    public class SignalRKeyboardService
    {
        public static readonly string SignalRBaseUrl = "http://192.168.1.19:9001/";
        public static readonly string HardwareKeyHubName = "HardwareKeyHub";
        public static readonly string MessageEventName = "SendMessage";
        public static readonly string ReceiveMessageMethodName = "ReceiveMessage";

        private HubConnection _hubConnection;
        private IHubProxy _hubProxy;
        private IDisposable _incomingMessageHandler;
        private readonly SimpleObserverNotifier<HardwareKeyMessage> _notifier;

        public bool IsConnected { get; private set; }

        public async Task Connect()
        {
            try
            {
                _hubConnection = new HubConnection(SignalRBaseUrl);
                _hubProxy = _hubConnection.CreateHubProxy(HardwareKeyHubName);
                _incomingMessageHandler = _hubProxy.On<HardwareKeyMessage>(MessageEventName, HandleIncomingMessage);
                await _hubConnection.Start();
                IsConnected = true;
            }
            catch (Exception e)
            {
                var ex = new HubException(
                    $"Error while trying to start the HardwareKey hub connection\n - is the host running?\n{e.Message}\n{e}");
                _notifier.NotifyError(ex);
            }
        }

        private void HandleIncomingMessage(HardwareKeyMessage message)
        {
            if (message != null)
            {
                _notifier.NotifyNext(message);
            }
        }

        private bool SendMessageToHost(HardwareKeyMessage message)
        {
            bool result = false;

            try
            {
                if (!IsConnected)
                {
                    throw new HubException("The SignalR hub is not connected.");
                }

                _hubProxy?.Invoke(ReceiveMessageMethodName, message);
                result = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error while trying to send temperature to host - {e.Message}\n{e}");
                Debugger.Break();
            }

            return result;
        }

        public IObservable<HardwareKeyMessage> GetObservable() => _notifier;

        public SignalRKeyboardService()
        {
            _notifier = new SimpleObserverNotifier<HardwareKeyMessage>();
        }
    }
}

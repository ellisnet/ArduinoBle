using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using SignalR.Models;
using SignalRHost.Wpf.Helpers;

namespace SignalRHost.Wpf.ViewModels
{
    public enum LedColor
    {
        None,
        Green,
        Amber,
        Red,
    }

    public interface ILedColorAware
    {
        Action<LedColor> SetLeftLedAction { get; set; }
        Action<LedColor> SetRightLedAction { get; set; }
    }

    public class MainViewModel : SimpleViewModel, ILedColorAware
    {
        public Action<LedColor> SetLeftLedAction { get; set; }
        public Action<LedColor> SetRightLedAction { get; set; }

        #region Commands

        private SimpleCommand _keyPowerCommand;
        public SimpleCommand KeyPowerCommand => _keyPowerCommand
            ?? (_keyPowerCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyPower); }));

        private SimpleCommand _keyUpCommand;
        public SimpleCommand KeyUpCommand => _keyUpCommand
            ?? (_keyUpCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyUp); }));

        private SimpleCommand _keyMenuCommand;
        public SimpleCommand KeyMenuCommand => _keyMenuCommand
            ?? (_keyMenuCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyMenu); }));

        private SimpleCommand _keyLeftCommand;
        public SimpleCommand KeyLeftCommand => _keyLeftCommand
            ?? (_keyLeftCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyLeft); }));

        private SimpleCommand _keyEnterCommand;
        public SimpleCommand KeyEnterCommand => _keyEnterCommand
            ?? (_keyEnterCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyEnter); }));

        private SimpleCommand _keyRightCommand;
        public SimpleCommand KeyRightCommand => _keyRightCommand
            ?? (_keyRightCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyRight); }));

        private SimpleCommand _keyShiftCommand;
        public SimpleCommand KeyShiftCommand => _keyShiftCommand
            ?? (_keyShiftCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyShift); }));

        private SimpleCommand _keyDownCommand;
        public SimpleCommand KeyDownCommand => _keyDownCommand
            ?? (_keyDownCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyDown); }));

        private SimpleCommand _keyBackCommand;
        public SimpleCommand KeyBackCommand => _keyBackCommand
            ?? (_keyBackCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyBackspace); }));

        private SimpleCommand _key7Command;
        public SimpleCommand Key7Command => _key7Command
            ?? (_key7Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key7); }));

        private SimpleCommand _key8Command;
        public SimpleCommand Key8Command => _key8Command
            ?? (_key8Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key8); }));

        private SimpleCommand _key9Command;
        public SimpleCommand Key9Command => _key9Command
            ?? (_key9Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key9); }));

        private SimpleCommand _key4Command;
        public SimpleCommand Key4Command => _key4Command
            ?? (_key4Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key4); }));

        private SimpleCommand _key5Command;
        public SimpleCommand Key5Command => _key5Command
            ?? (_key5Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key5); }));

        private SimpleCommand _key6Command;
        public SimpleCommand Key6Command => _key6Command
            ?? (_key6Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key6); }));

        private SimpleCommand _key1Command;
        public SimpleCommand Key1Command => _key1Command
            ?? (_key1Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key1); }));

        private SimpleCommand _key2Command;
        public SimpleCommand Key2Command => _key2Command
            ?? (_key2Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key2); }));

        private SimpleCommand _key3Command;
        public SimpleCommand Key3Command => _key3Command
            ?? (_key3Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key3); }));

        private SimpleCommand _keyDotCommand;
        public SimpleCommand KeyDotCommand => _keyDotCommand
            ?? (_keyDotCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyDot); }));

        private SimpleCommand _key0Command;
        public SimpleCommand Key0Command => _key0Command
            ?? (_key0Command = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.Key0); }));

        private SimpleCommand _keyDashCommand;
        public SimpleCommand KeyDashCommand => _keyDashCommand
            ?? (_keyDashCommand = new SimpleCommand(_ => { SendKeyStroke(HardwareKeyStroke.KeyDash); }));

        #endregion

        private void SetLeftLed(LedColor color)
        {
            if (SetLeftLedAction != null)
            {
                InvokeOnMainThread(() => SetLeftLedAction.Invoke(color));
            }
        }

        private void SetRightLed(LedColor color)
        {
            if (SetRightLedAction != null)
            {
                InvokeOnMainThread(() => SetRightLedAction.Invoke(color));
            }
        }

        private void SendKeyStroke(HardwareKeyStroke stroke) => HardwareKeyMessageHelper.SendMessage(new HardwareKeyMessage(stroke));

        public void ProcessIncomingMessage(HardwareKeyMessage message)
        {
            if (message != null)
            {
                Debug.WriteLine($"Incoming hardware key message: {message.Stroke}");
            }
        }

        public MainViewModel()
        {
            // ReSharper disable once PossibleNullReferenceException
            (Application.Current as App).MainView = this;

            //Fire-and-forget task to cycle the LED colors to make sure they work
            new Task(async () =>
            {
                //Left
                await Task.Delay(1000);
                SetLeftLed(LedColor.Green);
                await Task.Delay(1000);
                SetLeftLed(LedColor.Amber);
                await Task.Delay(1000);
                SetLeftLed(LedColor.Red);
                await Task.Delay(1000);
                SetLeftLed(LedColor.None);

                //Right
                await Task.Delay(1000);
                SetRightLed(LedColor.Green);
                await Task.Delay(1000);
                SetRightLed(LedColor.Amber);
                await Task.Delay(1000);
                SetRightLed(LedColor.Red);
                await Task.Delay(1000);
                SetRightLed(LedColor.None);
            }).Start();
        }
    }
}

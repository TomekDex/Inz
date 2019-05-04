using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToeWindow
{
    public class ButtonAwait : Button
    {
        public ButtonAwaiter GetAwaiter()
        {
            return new ButtonAwaiter(this);
        }
    }

    public class ButtonAwaiter : INotifyCompletion
    {
        public bool IsCompleted { get; } = false;
        private ButtonAwait buttonAwait;

        public ButtonAwaiter(ButtonAwait buttonAwait)
        {
            this.buttonAwait = buttonAwait;
        }

        public void GetResult()
        {

        }

        public void OnCompleted(Action continuation)
        {
            RoutedEventHandler h = null;
            h = (o, e) =>
            {
                ((ButtonAwait)o).Click -= h;
                continuation();
            };
            buttonAwait.Click += h;
        }
    }
}
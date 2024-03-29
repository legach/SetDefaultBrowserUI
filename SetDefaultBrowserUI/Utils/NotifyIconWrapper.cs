﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using Application = System.Windows.Application;

namespace SetDefaultBrowserUI.Utils
{
    public class NotifyIconWrapper : FrameworkElement, IDisposable
    {

        public static readonly DependencyProperty TextProperty =
                DependencyProperty.Register("Text", typeof(string), typeof(NotifyIconWrapper), 
                    new PropertyMetadata(
                    (d, e) =>
                    {
                        var notifyIcon = ((NotifyIconWrapper)d)._notifyIcon;
                        if (notifyIcon == null)
                            return;
                        notifyIcon.Text = (string)e.NewValue;
                    }));

        private static readonly DependencyProperty NotifyRequestProperty =
            DependencyProperty.Register("NotifyRequest", typeof(NotifyRequestRecord), typeof(NotifyIconWrapper),
                new PropertyMetadata(
                    (d, e) =>
                    {
                        var r = (NotifyRequestRecord)e.NewValue;
                        ((NotifyIconWrapper)d)._notifyIcon?.ShowBalloonTip(r.Duration, r.Title, r.Text, r.Icon);
                    }));

        public static readonly DependencyProperty ContextMenuStripProperty =
            DependencyProperty.Register("ContextMenuStrip", typeof(ContextMenuStrip), typeof(NotifyIconWrapper), 
                new PropertyMetadata(
                (d, e) =>
                {
                    var notifyIcon = ((NotifyIconWrapper)d)._notifyIcon;
                    if (notifyIcon == null)
                        return;
                    notifyIcon.ContextMenuStrip = (ContextMenuStrip)e.NewValue;
                }));

        private static readonly RoutedEvent OpenSelectedEvent = EventManager.RegisterRoutedEvent("OpenSelected",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(NotifyIconWrapper));

        private static readonly RoutedEvent ExitSelectedEvent = EventManager.RegisterRoutedEvent("ExitSelected",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(NotifyIconWrapper));

        private readonly NotifyIcon? _notifyIcon;


        public NotifyIconWrapper()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            _notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Visible = true,
            };
            _notifyIcon.DoubleClick += OpenItemOnClick;
            _notifyIcon.MouseClick += MouseClickHandler;
            Application.Current.Exit += (obj, args) => { _notifyIcon.Dispose(); };
        }

        private void MouseClickHandler(object? sender, MouseEventArgs e)
        {
            if (ContextMenu == null)
            {
                return;
            }
            ContextMenu.IsOpen = false;

            if (e.Button == MouseButtons.Right)
            {
                ContextMenu.IsOpen = true;
            }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public NotifyRequestRecord NotifyRequest
        {
            get => (NotifyRequestRecord)GetValue(NotifyRequestProperty);
            set => SetValue(NotifyRequestProperty, value);
        }

        public ContextMenuStrip ContextMenuStrip
        {
            get => (ContextMenuStrip)GetValue(ContextMenuStripProperty);
            set => SetValue(ContextMenuStripProperty, value);
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }

        public event RoutedEventHandler OpenSelected
        {
            add => AddHandler(OpenSelectedEvent, value);
            remove => RemoveHandler(OpenSelectedEvent, value);
        }

        public event RoutedEventHandler ExitSelected
        {
            add => AddHandler(ExitSelectedEvent, value);
            remove => RemoveHandler(ExitSelectedEvent, value);
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var openItem = new ToolStripMenuItem("Open");
            openItem.Click += OpenItemOnClick;
            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += ExitItemOnClick;
            var contextMenu = new ContextMenuStrip { Items = { openItem, exitItem } };
            return contextMenu;
        }

        private void OpenItemOnClick(object? sender, EventArgs eventArgs)
        {
            var args = new RoutedEventArgs(OpenSelectedEvent);
            RaiseEvent(args);
        }

        private void ExitItemOnClick(object? sender, EventArgs eventArgs)
        {
            var args = new RoutedEventArgs(ExitSelectedEvent);
            RaiseEvent(args);
        }

        public class NotifyRequestRecord
        {
            public string Title { get; set; } = "";
            public string Text { get; set; } = "";
            public int Duration { get; set; } = 1000;
            public ToolTipIcon Icon { get; set; } = ToolTipIcon.Info;
        }
    }
}

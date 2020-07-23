﻿using System;
using Windows.UI.ViewManagement;
#if NET5_0
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
using Windows.UI.Core;
#endif

namespace WindowsStateTriggers
{
    /// <summary>
    /// Trigger for switching when the User interaction mode changes (tablet mode)
    /// </summary>
#if UNO
    [Uno.NotImplemented]
#endif
    public sealed class UserInteractionModeTrigger : StateTriggerBase, ITriggerValue
    {
        private bool m_IsActive;

        /// <summary>
		/// Occurs when the <see cref="IsActive" /> property has changed.
		/// </summary>
        public event EventHandler? IsActiveChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInteractionModeTrigger"/> class.
        /// </summary>
        public UserInteractionModeTrigger()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var weakEvent =
                    new WeakEventListener<UserInteractionModeTrigger, object, WindowSizeChangedEventArgs>(this)
                    {
                        OnEventAction = UserInteractionModeTrigger_SizeChanged,
                        OnDetachAction = (_, weakEventListener) => Window.Current.SizeChanged -= weakEventListener.OnEvent
                    };
                Window.Current.SizeChanged += weakEvent.OnEvent;
                UpdateTrigger(InteractionMode);
            }
        }

        /// <summary>
		/// Gets a value indicating whether this trigger is active.
		/// </summary>
		/// <value><c>true</c> if this trigger is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get => m_IsActive;
            private set
            {
                if (m_IsActive != value)
                {
                    m_IsActive = value;
                    base.SetActive(value);
                    IsActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
		/// Gets or sets the InteractionMode to trigger on.
		/// </summary>
        public UserInteractionMode InteractionMode
        {
            get => (UserInteractionMode)GetValue(InteractionModeProperty);
            set => SetValue(InteractionModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="InteractionMode"/> parameter.
        /// </summary>
        public static readonly DependencyProperty InteractionModeProperty = 
            DependencyProperty.Register("InteractionMode", typeof(UserInteractionMode), typeof(UserInteractionModeTrigger), 
            new PropertyMetadata(UserInteractionMode.Mouse, OnInteractionModeChanged));

        private static void OnInteractionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (UserInteractionModeTrigger)d;
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var orientation = (UserInteractionMode)e.NewValue;
                obj.UpdateTrigger(orientation);
            }
        }

        private void UpdateTrigger(UserInteractionMode interactionMode)
        {
            IsActive = interactionMode == UIViewSettings.GetForCurrentView().UserInteractionMode;
        }

        private static void UserInteractionModeTrigger_SizeChanged(UserInteractionModeTrigger instance, object sender, WindowSizeChangedEventArgs e)
        {
            instance.UpdateTrigger(instance.InteractionMode);
        }
    }
}

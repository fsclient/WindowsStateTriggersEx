// Copyright (c) Morten Nielsen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.Graphics.Display;
#if NET5_0
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace WindowsStateTriggers
{
    /// <summary>
    /// Trigger for switching when the screen orientation changes
    /// </summary>
	public class OrientationStateTrigger : StateTriggerBase, ITriggerValue
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrientationStateTrigger"/> class.
		/// </summary>
		public OrientationStateTrigger()
		{
			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				var weakEvent =
					new WeakEventListener<OrientationStateTrigger, DisplayInformation, object>(this)
					{
						OnEventAction = OrientationStateTrigger_OrientationChanged,
						OnDetachAction = (_, weakEventListener) => DisplayInformation.GetForCurrentView().OrientationChanged -= weakEventListener.OnEvent
					};
				DisplayInformation.GetForCurrentView().OrientationChanged += weakEvent.OnEvent;
			}
		}

		private static void OrientationStateTrigger_OrientationChanged(OrientationStateTrigger instance, DisplayInformation sender, object args)
		{
            instance.UpdateTrigger(sender.CurrentOrientation);
		}

        private void UpdateTrigger(Windows.Graphics.Display.DisplayOrientations orientation)
        {
            if (orientation == Windows.Graphics.Display.DisplayOrientations.None)
            {
                IsActive = false;
            }
            else if (orientation == Windows.Graphics.Display.DisplayOrientations.Landscape ||
               orientation == Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped)
            {
                IsActive = Orientation == Orientations.Landscape;
            }
            else if (orientation == Windows.Graphics.Display.DisplayOrientations.Portrait ||
                    orientation == Windows.Graphics.Display.DisplayOrientations.PortraitFlipped)
            {
				IsActive = Orientation == Orientations.Portrait;
            }
        }

		/// <summary>
		/// Gets or sets the orientation to trigger on.
		/// </summary>
		public Orientations Orientation
        {
            get => (Orientations)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Orientation"/> parameter.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientations), typeof(OrientationStateTrigger), 
			new PropertyMetadata(Orientations.None, OnOrientationPropertyChanged));

		private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var obj = (OrientationStateTrigger)d;
			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				var orientation = DisplayInformation.GetForCurrentView().CurrentOrientation;
				obj.UpdateTrigger(orientation);
			}
        }

		#region ITriggerValue

		private bool m_IsActive;

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
        /// Occurs when the <see cref="IsActive" /> property has changed.
        /// </summary>
        public event EventHandler? IsActiveChanged;

		#endregion ITriggerValue

		/// <summary>
		/// Orientations
		/// </summary>
		public enum Orientations 
		{
			/// <summary>
			/// none
			/// </summary>
			None,
			/// <summary>
			/// landscape
			/// </summary>
			Landscape,
			/// <summary>
			/// portrait
			/// </summary>
			Portrait
		}
	}
}

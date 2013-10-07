// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupControlHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the PopupControlHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.PopupControls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// The pop up control helper class.
    /// </summary>
    internal class PopupControlHelper
    {
        /// <summary>
        /// The handle mouse down delegate;
        /// </summary>
        private readonly RoutedEventHandler handleMouseDown;

        /// <summary>
        /// The pop up control collection.
        /// </summary>
        private readonly Collection<WeakReference> popupControls = new Collection<WeakReference>();

        /// <summary>
        /// The popup control helper.
        /// </summary>
        private static PopupControlHelper instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="PopupControlHelper"/> class from being created.
        /// </summary>
        private PopupControlHelper()
        {
            if (Application.Current == null || Application.Current.MainWindow == null)
            {
                return;
            }

            this.handleMouseDown = this.OnHandleMouseDown;

            Application.Current.MainWindow.AddHandler(UIElement.MouseDownEvent, this.handleMouseDown, true);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static PopupControlHelper Instance
        {
            get
            {
                return instance = instance ?? new PopupControlHelper();
            }
        }

        /// <summary>
        /// Registers the popup control.
        /// </summary>
        /// <param name="popupControl">The popup control.</param>
        public void RegisterPopupControl(IPopupControl popupControl)
        {
            this.popupControls.Add(new WeakReference(popupControl));
        }

        /// <summary>
        /// Called when [handle mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnHandleMouseDown(object sender, RoutedEventArgs e)
        {
            var controls = this.popupControls.Where(wr => wr.IsAlive && wr.Target != null).Select(wr => wr.Target).OfType<IPopupControl>();

            foreach (var popupControl in controls)
            {
                popupControl.OnHandleMouseDown(sender, e);
            }

            var expiredRefs = this.popupControls.Where(wr => wr.Target == null).ToArray();

            foreach (var weakReference in expiredRefs)
            {
                this.popupControls.Remove(weakReference);
            }
        }
    }
}

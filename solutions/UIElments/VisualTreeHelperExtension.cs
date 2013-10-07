// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualTreeHelperExtension.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the VisualTreeHelperExtension type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Defines the VisualTreeHelperExtension type.
    /// </summary>
    public static class VisualTreeHelperExtension
    {
        /// <summary>
        /// Gets all disposable children.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>An enumerable of all the disposable child items.</returns>
        public static IEnumerable<IDisposable> GetAllDisposableChildren(this DependencyObject element)
        {
            var disposableChildren = new List<IDisposable>();

            if (element != null)
            {
                for (var i = 0; i < GetChildrenCount(element); i++)
                {
                    var child = GetChild(element, i);

                    var disposableChild = child as IDisposable;
                    if (disposableChild != null)
                    {
                        disposableChildren.Add(disposableChild);
                        continue;
                    }

                    disposableChildren.AddRange(GetAllDisposableChildren(child));
                }
            }

            return disposableChildren;
        }

        /// <summary>
        /// Gets all child elements of the specified type.
        /// </summary>
        /// <typeparam name="T">The child type to filter by.</typeparam>
        /// <param name="element">The element.</param>
        /// <returns>An enumerable of all matching child elements.</returns>
        public static IEnumerable<T> GetAllChildElementsOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            return GetAllChildElementsOfType(element, new List<T>());
        }

        /// <summary>
        /// Gets the first child element of the specified type.
        /// </summary>
        /// <typeparam name="T">The child type to filter by.</typeparam>
        /// <param name="element">The element.</param>
        /// <returns>The first child found of the specified type.</returns>
        public static T GetFirstChildElementOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element is T)
            {
                return (T)element;
            }

            T output = null;

            for (var i = 0; i < GetChildrenCount(element); i++)
            {
                output = GetFirstChildElementOfType<T>(GetChild(element, i));

                if (output != null)
                {
                    break;
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the parent element of the specified type.
        /// </summary>
        /// <typeparam name="T">The parent type.</typeparam>
        /// <param name="element">The element.</param>
        /// <returns>The first parent of the specified type.</returns>
        public static T GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            var parentElement = GetParent(element);

            if (parentElement is T)
            {
                return (T)parentElement;
            }

            return parentElement == null ? null : GetParentOfType<T>(parentElement);
        }

        /// <summary>
        /// Determines whether [is instance or child of] [the specified source].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// <c>true</c> if [is instance or child of] [the specified source]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInstanceOrChildOf(this DependencyObject source, DependencyObject instance)
        {
            if (instance.Equals(source))
            {
                return true;
            }

            var parent = GetParent(source);

            while (parent != null)
            {
                if (parent.Equals(instance))
                {
                    return true;
                }

                parent = GetParent(parent);
            }

            return false;
        }

        /// <summary>
        /// Gets all child elements of the specified type.
        /// </summary>
        /// <typeparam name="T">The child type.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="children">The children.</param>
        /// <returns>Enumeration of matching elements.</returns>
        private static IEnumerable<T> GetAllChildElementsOfType<T>(this DependencyObject element, ICollection<T> children)
            where T : DependencyObject
        {
            if (element is T)
            {
                children.Add((T)element);
            }

            for (var i = 0; i < GetChildrenCount(element); i++)
            {
                GetAllChildElementsOfType(GetChild(element, i), children);
            }

            return children.AsEnumerable();
        }

        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="childIndex">Index of the child.</param>
        /// <returns><c>Null</c> if the source element is not a visual or visual3d; otherwise <c>child</c> dependency object.</returns>
        private static DependencyObject GetChild(DependencyObject source, int childIndex)
        {
            try
            {
                return VisualTreeHelper.GetChild(source, childIndex);
            }
            catch (InvalidOperationException ex)
            {
                CommandLibrary.ApplicationExceptionCommand.Execute(ex, Application.Current.MainWindow);
            }

            return null;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// <c>Null</c> if the source element is not a visual or visual3d; otherwise <c>child</c> dependency object.
        /// </returns>
        private static DependencyObject GetParent(DependencyObject source)
        {
            try
            {
                return VisualTreeHelper.GetParent(source);
            }
            catch (InvalidOperationException ex)
            {
                CommandLibrary.ApplicationExceptionCommand.Execute(ex, Application.Current.MainWindow);
            }

            return null;
        }

        /// <summary>
        /// Gets the children count.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns><c>Zero</c> if the source element is not a visual or visual3d; otherwise <c>count</c> of children.</returns>
        private static int GetChildrenCount(DependencyObject source)
        {
            try
            {
                return VisualTreeHelper.GetChildrenCount(source);
            }
            catch (InvalidOperationException ex)
            {
                CommandLibrary.ApplicationExceptionCommand.Execute(ex, Application.Current.MainWindow);
            }

            return 0;
        }
    }
}
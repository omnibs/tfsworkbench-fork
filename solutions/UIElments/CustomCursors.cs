// --------------------------------------------------------------------------------------------------------------------- 
// <copyright file="CustomCursors.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the CustomCursors type.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Defines the CustomCursors type.
    /// </summary>
    public static class CustomCursors
    {
        /// <summary>
        /// Initializes static members of the <see cref="CustomCursors"/> class.
        /// </summary>
        static CustomCursors()
        {
            Hand =
                new Cursor(
                    GetResourceStream(
                        "pack://application:,,,/TfsWorkbench.UIElements;component/Resources/Hand.cur",
                        UriKind.Absolute));

            MoveHand =
                new Cursor(
                    GetResourceStream(
                        "pack://application:,,,/TfsWorkbench.UIElements;component/Resources/MoveHand.cur",
                        UriKind.Absolute));

            Question =
                new Cursor(
                    GetResourceStream(
                        "pack://application:,,,/TfsWorkbench.UIElements;component/Resources/Question.cur",
                        UriKind.Absolute));
            HandNo =
                new Cursor(
                    GetResourceStream(
                        "pack://application:,,,/TfsWorkbench.UIElements;component/Resources/HandNo.cur",
                        UriKind.Absolute));

            Rotate =
                new Cursor(
                    GetResourceStream(
                        "pack://application:,,,/TfsWorkbench.UIElements;component/Resources/Rotate.cur",
                        UriKind.Absolute));
        }

        public static Cursor Rotate { get; set; }

        /// <summary>
        /// Gets Grab Hand cursor.
        /// </summary>
        /// <value>The grab hand.</value>
        public static Cursor Hand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets HandNo.
        /// </summary>
        /// <value>
        /// The hand no.
        /// </value>
        public static Cursor HandNo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets MoveHand.
        /// </summary>
        /// <value>
        /// The move hand.
        /// </value>
        public static Cursor MoveHand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets Question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public static Cursor Question
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the resource stream.
        /// </summary>
        /// <param name="resourceAddress">The resource url.</param>
        /// <param name="uriKind">The uri kind.</param>
        /// <returns>The resource stream.</returns>
        /// <exception cref="NullReferenceException">
        /// </exception>
        private static Stream GetResourceStream(string resourceAddress, UriKind uriKind)
        {
            var cursorResource = Application.GetResourceStream(new Uri(resourceAddress, uriKind));

            if (cursorResource == null)
            {
                throw new ArgumentException(string.Concat("Uable to locate resource stream '", resourceAddress, "'"));
            }

            return cursorResource.Stream;
        }
    }
}
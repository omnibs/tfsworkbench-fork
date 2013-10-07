using System.Windows;

namespace TfsWorkbench.NotePadUI
{
    public class DragBehaviour
    {
        public enum DragOption
        {
            None,
            Move,
            Rotate,
            Resize
        }

        public enum AspectOption
        {
            Horizontal,
            Vertical,
            Both
        }

        public static readonly DependencyProperty ResizeAspectProperty =
            DependencyProperty.RegisterAttached(
                "ResizeAspect",
                typeof(AspectOption),
                typeof(DragBehaviour),
                new FrameworkPropertyMetadata(AspectOption.Horizontal, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty DragActionProperty =
            DependencyProperty.RegisterAttached(
                "DragAction",
                typeof (DragOption),
                typeof (DragBehaviour),
                new FrameworkPropertyMetadata(DragOption.None, FrameworkPropertyMetadataOptions.Inherits));

        [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = true)]
        public static AspectOption GetResizeAspect(DependencyObject d)
        {
            return (AspectOption)d.GetValue(ResizeAspectProperty);
        }

        public static void SetResizeAspect(DependencyObject d, AspectOption value)
        {
            d.SetValue(ResizeAspectProperty, value);
        }

        [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = true)]
        public static DragOption GetDragAction(DependencyObject d)
        {
            return (DragOption)d.GetValue(DragActionProperty);
        }

        public static void SetDragAction(DependencyObject d, DragOption value)
        {
            d.SetValue(DragActionProperty, value);
        }
    }
}

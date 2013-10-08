// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLibrary.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the CommandLibrary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System.Windows.Input;

    /// <summary>
    /// Initializes instance of CommandLibrary
    /// </summary>
    public static class CommandLibrary
    {
        /// <summary>
        /// The new child routed command;
        /// </summary>
        private static readonly RoutedUICommand discardItemCommand =
            new RoutedUICommand("Discard", "discardItem", typeof(CommandLibrary));

        /// <summary>
        /// The new child routed command;
        /// </summary>
        private static readonly RoutedUICommand createChildCommand =
            new RoutedUICommand("New Child", "createChild", typeof(CommandLibrary));

        /// <summary>
        /// The new parent routed command;
        /// </summary>
        private static readonly RoutedUICommand createParentCommand =
            new RoutedUICommand("New Parent", "createParent", typeof(CommandLibrary));

        /// <summary>
        /// The edit task board item routed command;
        /// </summary>
        private static readonly RoutedUICommand editItemCommand =
            new RoutedUICommand("Edit Item", "editItem", typeof(CommandLibrary));

        /// <summary>
        /// The edit task board item routed command;
        /// </summary>
        private static readonly RoutedUICommand closeDialogCommand =
            new RoutedUICommand("Close", "closeDialog", typeof(CommandLibrary));

        /// <summary>
        /// The load project routed command;
        /// </summary>
        private static readonly RoutedUICommand loadProjectCommand =
            new RoutedUICommand("Load Project", "loadProject", typeof(CommandLibrary));

        /// <summary>
        /// The save project routed command;
        /// </summary>
        private static readonly RoutedUICommand saveProjectCommand =
            new RoutedUICommand("Save Project", "saveProject", typeof(CommandLibrary));

        /// <summary>
        /// The close routed command;
        /// </summary>
        private static readonly RoutedUICommand closeProjectCommand =
            new RoutedUICommand("Close Project", "closeProject", typeof(CommandLibrary));

        /// <summary>
        /// The refresh project data command instance.
        /// </summary>
        private static readonly RoutedUICommand refreshProjectDataCommand =
            new RoutedUICommand("Refresh Project Data", "refreshProjectData", typeof(CommandLibrary));

        /// <summary>
        /// The reset project layout command instance.
        /// </summary>
        private static readonly RoutedUICommand resetProjectLayoutCommand =
            new RoutedUICommand("Reset Project Layout", "resetProjectLayout", typeof(CommandLibrary));

        /// <summary>
        /// The exit routed command;
        /// </summary>
        private static readonly RoutedUICommand exitCommand =
            new RoutedUICommand("Exit", "exit", typeof(CommandLibrary));

        /// <summary>
        /// The application message routed command;
        /// </summary>
        private static readonly RoutedUICommand applicationMessageCommand =
            new RoutedUICommand("Application Message", "applicationMessage", typeof(CommandLibrary));

        /// <summary>
        /// The application exception routed command;
        /// </summary>
        private static readonly RoutedUICommand applicationExceptionCommand =
            new RoutedUICommand("Application Exception", "applicationException", typeof(CommandLibrary));

        /// <summary>
        /// The refresh item routed command;
        /// </summary>
        private static readonly RoutedUICommand refreshItemCommand =
            new RoutedUICommand("Refresh Item", "refreshItem", typeof(CommandLibrary));

        /// <summary>
        /// The save item routed command;
        /// </summary>
        private static readonly RoutedUICommand saveItemCommand =
            new RoutedUICommand("Save Item", "saveItem", typeof(CommandLibrary));

        /// <summary>
        /// The save item routed command;
        /// </summary>
        private static readonly RoutedUICommand refreshItemAndViewChildrenCommand =
            new RoutedUICommand("Refresh Item and Children", "refrehItemAndChildren", typeof(CommandLibrary));

        /// <summary>
        /// The edit view command;
        /// </summary>
        private static readonly RoutedUICommand editViewCommand =
            new RoutedUICommand("Edit View", "editView", typeof(CommandLibrary));

        /// <summary>
        /// The add view command;
        /// </summary>
        private static readonly RoutedUICommand addViewCommand =
            new RoutedUICommand("Add View", "addView", typeof(CommandLibrary));

        /// <summary>
        /// The delete view command;
        /// </summary>
        private static readonly RoutedUICommand deleteViewCommand =
            new RoutedUICommand("Delete View", "deleteView", typeof(CommandLibrary));

        /// <summary>
        /// The delete view command;
        /// </summary>
        private static readonly RoutedUICommand showDisplayModeCommand =
            new RoutedUICommand("Show Display Element", "showDisplayMode", typeof(CommandLibrary));

        /// <summary>
        /// The show dialog command instance.
        /// </summary>
        private static readonly RoutedUICommand showDialogCommand =
            new RoutedUICommand("Show Dialog", "showDialog", typeof(CommandLibrary));

        /// <summary>
        /// The reset project command instance.
        /// </summary>
        private static readonly RoutedUICommand showAboutCommand =
            new RoutedUICommand("About", "showAbout", typeof(CommandLibrary));

        /// <summary>
        /// The system shell command instance.
        /// </summary>
        private static readonly RoutedUICommand systemShellCommand =
            new RoutedUICommand("System Shell", "systemShell", typeof(CommandLibrary));

        /// <summary>
        /// The disable user input command instance.
        /// </summary>
        private static readonly RoutedUICommand disableUserInputCommand =
            new RoutedUICommand("Disable Input", "disableUserInput", typeof(CommandLibrary));

        /// <summary>
        /// The open item in browser command instance.
        /// </summary>
        private static readonly RoutedUICommand openItemInBrowser =
            new RoutedUICommand("Open In Browser", "openInBrowser", typeof(CommandLibrary));

        /// <summary>
        /// The edit type data command.
        /// </summary>
        private static readonly RoutedUICommand editTypeDataCommand =
            new RoutedUICommand("Item Context Options", "editItemTypeOptions", typeof(CommandLibrary));

        /// <summary>
        /// The show search dialog command.
        /// </summary>
        private static readonly RoutedUICommand showSearchDialogCommand =
            new RoutedUICommand("Search", "showSearchDialog", typeof(CommandLibrary));

        /// <summary>
        /// The clear highlights command.
        /// </summary>
        private static readonly RoutedUICommand clearHighlightsCommand =
            new RoutedUICommand("Clear Hightlights", "claerHightlights", typeof(CommandLibrary));

        /// <summary>
        /// The show item in command.
        /// </summary>
        private static readonly RoutedUICommand showItemInCommand =
            new RoutedUICommand("Show Item In", "showItemIn", typeof(CommandLibrary));

        /// <summary>
        /// The close all dialogs command.
        /// </summary>
        private static readonly RoutedUICommand closeAllDialogsCommand =
            new RoutedUICommand("Close All Dialogs", "closeAllDialogs", typeof(CommandLibrary));

        /// <summary>
        /// The duplicate item command.
        /// </summary>
        private static readonly RoutedUICommand duplicateCommand =
            new RoutedUICommand("Duplicate", "duplicate", typeof(CommandLibrary));

        /// <summary>
        /// Assign to me
        /// </summary>
        private static readonly RoutedUICommand assignToMeCommand = 
            new RoutedUICommand("Assign To Me", "assignToMe", typeof(CommandLibrary));


        /// <summary>
        /// Assign to me command
        /// </summary>
        public static RoutedUICommand AssignToMeCommand
        {
            get
            {
                return assignToMeCommand;
            }
        }

        /// <summary>
        /// Gets the duplicate command.
        /// </summary>
        /// <value>The duplicate command.</value>
        public static RoutedUICommand DuplicateCommand
        {
            get
            {
                return duplicateCommand;
            }
        }

        /// <summary>
        /// Gets the discard item command.
        /// </summary>
        /// <value>The discard item command.</value>
        public static RoutedUICommand DiscardItemCommand
        {
            get
            {
                return discardItemCommand;
            }
        }

        /// <summary>
        /// Gets the new parent command.
        /// </summary>
        /// <value>The new parent command.</value>
        public static RoutedUICommand CreateParentCommand
        {
            get
            {
                return createParentCommand;
            }
        }

        /// <summary>
        /// Gets the new child command.
        /// </summary>
        /// <value>The new child.</value>
        public static RoutedUICommand CreateChildCommand
        {
            get
            {
                return createChildCommand;
            }
        }

        /// <summary>
        /// Gets the edit item command.
        /// </summary>
        /// <value>The edit item command.</value>
        public static RoutedUICommand EditItemCommand
        {
            get
            {
                return editItemCommand;
            }
        }

        /// <summary>
        /// Gets the close dialog command.
        /// </summary>
        /// <value>The close dialog.</value>
        public static RoutedUICommand CloseDialogCommand
        {
            get
            {
                return closeDialogCommand;
            }
        }

        /// <summary>
        /// Gets the load project command.
        /// </summary>
        /// <value>The load command.</value>
        public static RoutedUICommand LoadProjectCommand
        {
            get
            {
                return loadProjectCommand;
            }
        }

        /// <summary>
        /// Gets the Save Project command.
        /// </summary>
        /// <value>The SaveProject command.</value>
        public static RoutedUICommand SaveProjectCommand
        {
            get
            {
                return saveProjectCommand;
            }
        }

        /// <summary>
        /// Gets the close project command.
        /// </summary>
        /// <value>The close project command.</value>
        public static RoutedUICommand CloseProjectCommand
        {
            get
            {
                return closeProjectCommand;
            }
        }

        /// <summary>
        /// Gets the exit command.
        /// </summary>
        /// <value>The exit command.</value>
        public static RoutedUICommand ExitCommand
        {
            get
            {
                return exitCommand;
            }
        }

        /// <summary>
        /// Gets the application message.
        /// </summary>
        /// <value>The application message.</value>
        public static RoutedUICommand ApplicationMessageCommand
        {
            get
            {
                return applicationMessageCommand;
            }
        }

        /// <summary>
        /// Gets the application exception.
        /// </summary>
        /// <value>The application exception.</value>
        public static RoutedUICommand ApplicationExceptionCommand
        {
            get
            {
                return applicationExceptionCommand;
            }
        }

        /// <summary>
        /// Gets the refresh item command.
        /// </summary>
        /// <value>The refresh item command.</value>
        public static RoutedUICommand RefreshItemCommand
        {
            get
            {
                return refreshItemCommand;
            }
        }

        /// <summary>
        /// Gets the save item.
        /// </summary>
        /// <value>The save item.</value>
        public static RoutedUICommand SaveItemCommand
        {
            get
            {
                return saveItemCommand;
            }
        }

        /// <summary>
        /// Gets the refresh item and children command.
        /// </summary>
        /// <value>The refresh item and children command.</value>
        public static RoutedUICommand RefreshItemAndViewChildren
        {
            get
            {
                return refreshItemAndViewChildrenCommand;
            }
        }
        
        /// <summary>
        /// Gets the edit view command.
        /// </summary>
        /// <value>The edit view.</value>
        public static RoutedUICommand EditViewCommand
        {
            get
            {
                return editViewCommand;
            }
        }

        /// <summary>
        /// Gets the Add view command.
        /// </summary>
        /// <value>The Add view.</value>
        public static RoutedUICommand AddViewCommand
        {
            get
            {
                return addViewCommand;
            }
        }

        /// <summary>
        /// Gets the Delete view command.
        /// </summary>
        /// <value>The Delete view.</value>
        public static RoutedUICommand DeleteViewCommand
        {
            get
            {
                return deleteViewCommand;
            }
        }

        /// <summary>
        /// Gets the show display element command.
        /// </summary>
        /// <value>The show display element command.</value>
        public static RoutedUICommand ShowDisplayModeCommand
        {
            get
            {
                return showDisplayModeCommand;
            }
        }

        /// <summary>
        /// Gets the show dialog.
        /// </summary>
        /// <value>The show dialog.</value>
        public static RoutedUICommand ShowDialogCommand
        {
            get
            {
                return showDialogCommand;
            }
        }

        /// <summary>
        /// Gets the refresh project data command.
        /// </summary>
        /// <value>The reset project data command.</value>
        public static RoutedUICommand RefreshProjectDataCommand
        {
            get
            {
                return refreshProjectDataCommand;
            }
        }

        /// <summary>
        /// Gets the reset project layout command.
        /// </summary>
        /// <value>The reset project layout command.</value>
        public static RoutedUICommand ResetProjectLayoutCommand
        {
            get
            {
                return resetProjectLayoutCommand;
            }
        }
        
        /// <summary>
        /// Gets the show about command.
        /// </summary>
        /// <value>The show about.</value>
        public static RoutedUICommand ShowAboutCommand
        {
            get
            {
                return showAboutCommand;
            }
        }

        /// <summary>
        /// Gets the system shell command.
        /// </summary>
        /// <value>The system shell command.</value>
        public static RoutedUICommand SystemShellCommand
        {
            get { return systemShellCommand; }
        }

        /// <summary>
        /// Gets the disabled user input.
        /// </summary>
        /// <value>The disabled user input.</value>
        public static RoutedUICommand DisableUserInputCommand
        {
            get { return disableUserInputCommand; }
        }

        /// <summary>
        /// Gets the open item in browser command.
        /// </summary>
        /// <value>The open item in browser command.</value>
        public static RoutedUICommand OpenItemInBrowserCommand
        {
            get { return openItemInBrowser; }
        }

        /// <summary>
        /// Gets the edit type data command.
        /// </summary>
        /// <value>The edit type data command.</value>
        public static RoutedUICommand EditTypeDataCommand
        {
            get { return editTypeDataCommand; }
        }

        /// <summary>
        /// Gets the show search dialog command.
        /// </summary>
        /// <value>The show search dialog command.</value>
        public static RoutedUICommand ShowSearchDialogCommand
        {
            get { return showSearchDialogCommand; }
        }

        /// <summary>
        /// Gets the clear highlights command.
        /// </summary>
        /// <value>The clear highlights command.</value>
        public static RoutedUICommand ClearHighlightsCommand
        {
            get { return clearHighlightsCommand; }
        }

        /// <summary>
        /// Gets the show item in command.
        /// </summary>
        /// <value>The show item in command.</value>
        public static RoutedUICommand ShowItemInCommand
        {
            get { return showItemInCommand; }
        }

        /// <summary>
        /// Gets the close all dialogs command.
        /// </summary>
        /// <value>The close all dialogs command.</value>
        public static RoutedUICommand CloseAllDialogsCommand
        {
            get { return closeAllDialogsCommand; }
        }
    }
}

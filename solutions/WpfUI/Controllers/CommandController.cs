// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initialises and instance of TfsWorkbench.WpfUI.CommandController
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.UIElements;
    using TfsWorkbench.WpfUI.Controls;
    using TfsWorkbench.WpfUI.Properties;

    /// <summary>
    /// Initialises and instance of TfsWorkbench.WpfUI.CommandController
    /// </summary>
    internal static class CommandController
    {
        /// <summary>
        /// Gets the project data service.
        /// </summary>
        /// <value>The project data service.</value>
        private static IProjectDataService ProjectDataService
        {
            get
            {
                return ServiceManager.Instance.GetService<IProjectDataService>();
            }
        }

        /// <summary>
        /// Wires up the commands.
        /// </summary>
        public static void SetupCommandBindings()
        {
            ApplicationController.Instance.MainWindow.CommandBindings.Clear();

            RegisterCommandBinding(CommandLibrary.AssignToMeCommand, OnAssignToMe, CanAssignToMeExecute);

            RegisterCommandBinding(CommandLibrary.EditViewCommand, OnEditView, CanExecute);
            RegisterCommandBinding(CommandLibrary.AddViewCommand, OnAddView, CanProjectActionExecute);
            RegisterCommandBinding(CommandLibrary.DeleteViewCommand, OnDeleteView);

            RegisterCommandBinding(CommandLibrary.CreateChildCommand, OnCreateChild, CanExecute);
            RegisterCommandBinding(CommandLibrary.CreateParentCommand, OnCreateParent);
            RegisterCommandBinding(CommandLibrary.EditItemCommand, OnEditItem, CanEditItemExecute);
            RegisterCommandBinding(CommandLibrary.DuplicateCommand, OnDuplicate, CanDuplicateExecute);
            RegisterCommandBinding(CommandLibrary.DiscardItemCommand, OnDiscardItem, CanDiscardExecute);
            RegisterCommandBinding(CommandLibrary.RefreshItemCommand, OnRefreshItem, CanRefreshItemExecute);
            RegisterCommandBinding(CommandLibrary.SaveItemCommand, OnSaveItem, CanSaveItemExecute);
            RegisterCommandBinding(CommandLibrary.RefreshItemAndViewChildren, OnRefreshItemAndViewChildren);
            RegisterCommandBinding(CommandLibrary.OpenItemInBrowserCommand, OnOpenInBrowser, CanOpenInBrowserExecute);
            RegisterCommandBinding(CommandLibrary.ShowItemInCommand, OnShowItemIn);

            RegisterCommandBinding(CommandLibrary.LoadProjectCommand, OnLoadProject);
            RegisterCommandBinding(CommandLibrary.SaveProjectCommand, OnSaveProject, CanProjectActionExecute, new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
            RegisterCommandBinding(CommandLibrary.CloseProjectCommand, OnCloseProject, CanProjectActionExecute);
            RegisterCommandBinding(CommandLibrary.RefreshProjectDataCommand, OnRefreshProjectData, CanProjectActionExecute, new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Shift));
            RegisterCommandBinding(CommandLibrary.ResetProjectLayoutCommand, OnResetProjectLayout, CanProjectActionExecute);
            RegisterCommandBinding(CommandLibrary.EditTypeDataCommand, OnEditTypeData, CanProjectActionExecute);
            RegisterCommandBinding(CommandLibrary.ShowSearchDialogCommand, OnShowSearchDialog, CanProjectActionExecute, new KeyGesture(Key.F, ModifierKeys.Control));
            RegisterCommandBinding(CommandLibrary.ClearHighlightsCommand, OnClearHightlights, null, new KeyGesture(Key.Z, ModifierKeys.Control | ModifierKeys.Shift));

            RegisterCommandBinding(CommandLibrary.DisableUserInputCommand, OnDisableUserInput);
            RegisterCommandBinding(CommandLibrary.ShowDialogCommand, OnShowDialog);
            RegisterCommandBinding(CommandLibrary.CloseDialogCommand, OnCloseDialog);
            RegisterCommandBinding(CommandLibrary.CloseAllDialogsCommand, OnCloseAllDialogs, null, new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Shift));
            RegisterCommandBinding(CommandLibrary.ShowDisplayModeCommand, OnShowDisplayMode, CanShowDisplayModeExecute);
            RegisterCommandBinding(CommandLibrary.ShowAboutCommand, OnShowAbout);

            RegisterCommandBinding(CommandLibrary.SystemShellCommand, OnSystemShell);

            RegisterCommandBinding(CommandLibrary.ExitCommand, OnExitApplication, CanExecute);
            RegisterCommandBinding(CommandLibrary.ApplicationMessageCommand, OnApplicationMessage);
            RegisterCommandBinding(CommandLibrary.ApplicationExceptionCommand, OnApplicationException);
        }

        private static void CanAssignToMeExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
        }

        /// <summary>
        /// Assign item to current user
        /// </summary>
        private static void OnAssignToMe(object sender, ExecutedRoutedEventArgs e)
        {
            var source = e.Parameter as IWorkbenchItem;
            source[WorkbenchItemHelper.GetOwnerFieldName(source.GetTypeName())] = ProjectData.CurrentUser;
        }

        /// <summary>
        /// Registers the command binding.
        /// </summary>
        /// <param name="commandBinding">The command binding.</param>
        public static void RegisterCommandBinding(CommandBinding commandBinding)
        {
            ApplicationController.Instance.MainWindow.CommandBindings.Add(commandBinding);            
        }

        /// <summary>
        /// Waits for all commands to complete.
        /// </summary>
        public static void WaitForAllCommandsToComplete()
        {
            var frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher
                .BeginInvoke(
                    DispatcherPriority.ApplicationIdle,
                    new Action(() => frame.Continue = false));

            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Applications the exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Exposed as internal to allow testing.</remarks>
        internal static void OnApplicationException(object sender, ExecutedRoutedEventArgs e)
        {
            var exception = e.Parameter as Exception;

            if (exception == null)
            {
                return;
            }

            var message = string.Empty;

            while (exception != null)
            {
                var newLineSpacer = string.IsNullOrEmpty(message)
                                        ? string.Empty
                                        : Environment.NewLine;

                message = string.Concat(message, newLineSpacer, exception.Message);

                exception = exception.InnerException;
            }

            var callback = (SendOrPostCallback)delegate
                {
                    ApplicationController.Instance.MainWindow.PART_StatusBar.Text = message;
                    ApplicationController.Instance.MainWindow.PART_StatusBar.Foreground = Brushes.Red;

                    if (!ApplicationController.Instance.MainWindow.PART_InternalGrid.IsEnabled)
                    {
                        CommandLibrary.DisableUserInputCommand.Execute(false, ApplicationController.Instance.MainWindow);
                    }
                };

            ApplicationController.Instance.MainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, callback, null);
        }

        /// <summary>
        /// Determines whether this instance [can duplicate execute] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanDuplicateExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter as IWorkbenchItem != null;
        }

        /// <summary>
        /// Called when [duplicate].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnDuplicate(object sender, ExecutedRoutedEventArgs e)
        {
            var source = e.Parameter as IWorkbenchItem;
            var projectData = ProjectDataService.CurrentProjectData;
             
            if (source == null || projectData == null)
            {
                return;
            }

            var duplicate = ProjectDataService.CreateDuplicate(source);

            if (duplicate == null)
            {
                return;
            }

            projectData.WorkbenchItems.Add(duplicate);

            ShowWorkbenchItemEditPanel(duplicate);

            var currentHighlighter =
                ProjectDataService.HighlightProviders.FirstOrDefault(
                    hp => hp.IsActive && hp.IsHighlightProvider);

            if (currentHighlighter == null)
            {
                return;
            }

            CommandLibrary.ShowItemInCommand.Execute(
                new Tuple<IWorkbenchItem, IHighlightProvider>(duplicate, currentHighlighter),
                ApplicationController.Instance.MainWindow);
        }

        /// <summary>
        /// Called when [close all dialogs].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCloseAllDialogs(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationController.Instance.DialogController.CloseAllDialogs();
        }

        /// <summary>
        /// Registers the command binding.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <param name="keyBinding">The key binding.</param>
        private static void RegisterCommandBinding(ICommand command, ExecutedRoutedEventHandler handler, CanExecuteRoutedEventHandler canExecute = null, KeyGesture keyBinding = null)
        {
            RegisterCommandBinding(new CommandBinding(command, handler, canExecute));

            if (keyBinding == null)
            {
                return;
            }

            var inputBinding = new InputBinding(command, keyBinding);
            ApplicationController.Instance.MainWindow.InputBindings.Add(inputBinding);
        }

        /// <summary>
        /// Determines whether this instance can execute.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ApplicationController.Instance.MainWindow.PART_InternalGrid.IsEnabled;
        }

        /// <summary>
        /// Determines whether this instance [can save item execute].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanSaveItemExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var item = e.Parameter as IWorkbenchItem;
            e.CanExecute = item != null && item.ValueProvider != null && item.ValueProvider.IsDirty;
        }

        /// <summary>
        /// Determines whether this instance [can refresh item execute].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanRefreshItemExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var item = e.Parameter as IWorkbenchItem;
            e.CanExecute = item != null && item.ValueProvider != null && !item.ValueProvider.IsNew;
        }

        /// <summary>
        /// Determines whether this instance [can show display mode execute].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanShowDisplayModeExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ApplicationController.Instance.MainWindow.PART_DisplayModeGrid.Children.Count != 1;
        }

        /// <summary>
        /// Determines whether this instance [can edit item execute].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanEditItemExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
        }

        /// <summary>
        /// Determines whether this instance [can discard execute].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanDiscardExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var workbenchItem = e.Parameter as IWorkbenchItem;

            e.CanExecute = workbenchItem != null && workbenchItem.ValueProvider != null && workbenchItem.ValueProvider.IsNew;
        }

        /// <summary>
        /// Determines whether this instance [can open in browser execute].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanOpenInBrowserExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var item = e.Parameter as IWorkbenchItem;
            var projectData = ProjectDataService.CurrentProjectData;

            e.CanExecute = projectData != null
                            && !string.IsNullOrEmpty(projectData.WebAccessUrl)
                            && projectData.ProjectGuid != null
                            && item != null
                            && item.ValueProvider != null
                            && !item.ValueProvider.IsNew;
        }

        /// <summary>
        /// Determines whether this instance [can project action execute].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanProjectActionExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectDataService.CurrentProjectData != null;
        }

        /// <summary>
        /// Systems the shell.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSystemShell(object sender, ExecutedRoutedEventArgs e)
        {
            var processPath = e.Parameter as string;
            if (processPath == null)
            {
                return;
            }

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo { FileName = processPath };
                process.Start();
            }
        }

        /// <summary>
        /// Clears the hightlights.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnClearHightlights(object sender, ExecutedRoutedEventArgs e)
        {
            HighlightHelper.ClearAllHighlights();
        }

        /// <summary>
        /// Shows the item in.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnShowItemIn(object sender, ExecutedRoutedEventArgs e)
        {
            var parameters = e.Parameter as Tuple<IWorkbenchItem, IHighlightProvider>;

            if (parameters == null)
            {
                return;
            }

            if (parameters.Item1 == null || parameters.Item2 == null)
            {
                return;
            }

            HighlightHelper.ClearAllHighlights();

            parameters.Item2.Highlight(parameters.Item1);

            ApplicationController.Instance.SetStatusMessage(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.String023,
                    parameters.Item1.GetTypeName(),
                    parameters.Item1.GetId(),
                    parameters.Item2.Title));
        }

        /// <summary>
        /// Shows the search dialog.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnShowSearchDialog(object sender, ExecutedRoutedEventArgs e)
        {
            if (ProjectDataService.CurrentProjectData == null)
            {
                return;
            }

            var searchDialog = new SearchControl { ProjectData = ProjectDataService.CurrentProjectData };

            ApplicationController.Instance.DialogController.ShowDialog(searchDialog);
        }

        /// <summary>
        /// Edits the context fields.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnEditTypeData(object sender, ExecutedRoutedEventArgs e)
        {
            if (ProjectDataService.CurrentProjectData == null)
            {
                return;
            }

            var dialog = new EditTypeDataControl { ProjectData = ProjectDataService.CurrentProjectData };

            ApplicationController.Instance.DialogController.ShowDialog(dialog);
        }

        /// <summary>
        /// Resets the project layout.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnResetProjectLayout(object sender, ExecutedRoutedEventArgs e)
        {
            var projectData = ProjectDataService.CurrentProjectData;
            if (projectData == null)
            {
                return;
            }

            if (projectData.WorkbenchItems.Any(tbi => tbi.ValueProvider != null && tbi.ValueProvider.IsDirty))
            {
                if (MessageBox.Show(
                    Resources.String024, 
                    Resources.String025, 
                    MessageBoxButton.OKCancel, 
                    MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            else
            {
                if (MessageBox.Show(
                    Resources.String026,
                    Resources.String027,
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            ApplicationController.Instance.SetStatusMessage(Resources.String028);

            ApplicationController.Instance.EnableInput(false);

            ApplicationController.Instance.MainWindow.Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle,
                new Action(() => ApplicationController.Instance.DataProviderController.ResetProjectLayout(projectData)));
        }

        /// <summary>
        /// Resets the project.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnRefreshProjectData(object sender, ExecutedRoutedEventArgs e)
        {
            var projectData = ProjectDataService.CurrentProjectData;
            if (projectData == null)
            {
                return;
            }

            if (projectData.WorkbenchItems.Any(tbi => tbi.ValueProvider != null && tbi.ValueProvider.IsDirty) && 
                MessageBox.Show(
                    Resources.String029, 
                    Resources.String030, 
                    MessageBoxButton.OKCancel, 
                    MessageBoxImage.Warning) == MessageBoxResult.Cancel)
            {
                return;
            }

            ApplicationController.Instance.SetStatusMessage(Resources.String031);

            ApplicationController.Instance.EnableInput(false);

            ApplicationController.Instance.MainWindow.Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle,
                new Action(() => ApplicationController.Instance.DataProviderController.RefreshProjectData(projectData)));
        }

        /// <summary>
        /// Refreshes the item and children.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnRefreshItemAndViewChildren(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as IChildCreationParameters;

            var mainAppWindow = ApplicationController.Instance.MainWindow;

            if (item == null || item.Parent == null || item.Parent.ValueProvider == null)
            {
                CommandLibrary.ApplicationExceptionCommand.Execute(new ArgumentException(Resources.String013), mainAppWindow);

                return;
            }

            var parent = item.Parent;

            if (parent.ChildLinks.Any(l => l.Child.ValueProvider != null && l.Child.ValueProvider.IsDirty))
            {
                if (MessageBox.Show(
                    Resources.String014, 
                    Resources.String015, 
                    MessageBoxButton.OKCancel, 
                    MessageBoxImage.Question) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            var childrenToUpdate = parent.ChildLinks
                .Where(l => l.Child.ValueProvider != null && Equals(item.ChildTypeName, l.Child.GetTypeName()) && Equals(item.LinkTypeName, l.LinkName))
                .Select(l => l.Child)
                .ToArray();

            Action sync = () =>
                {
                    try
                    {
                        var identityList = parent.GetId().ToString(CultureInfo.CurrentCulture);

                        foreach (var childItem in childrenToUpdate)
                        {
                            childItem.ValueProvider.SyncToLatest();
                            childItem.OnPropertyChanged();
                            identityList = string.Concat(identityList, ", ", childItem.GetId());
                        }

                        parent.ValueProvider.SyncToLatest();
                        parent.OnPropertyChanged();

                        var finalMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.String016,
                            childrenToUpdate.Any() ? Resources.String017 : string.Empty,
                            identityList);

                        ApplicationController.Instance.SetStatusMessage(finalMessage);
                    }
                    catch (Exception ex)
                    {
                        if (CommandLibrary.ApplicationExceptionCommand.CanExecute(ex, mainAppWindow))
                        {
                            CommandLibrary.ApplicationExceptionCommand.Execute(ex, mainAppWindow);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        CommandLibrary.DisableUserInputCommand.Execute(false, mainAppWindow);
                    }
                };

            var initialMessage = string.Format(
                CultureInfo.InvariantCulture,
                Resources.String018,
                childrenToUpdate.Count() + 1,
                childrenToUpdate.Any() ? Resources.String017 : string.Empty);

            ApplicationController.Instance.SetStatusMessage(initialMessage);
            CommandLibrary.DisableUserInputCommand.Execute(true, mainAppWindow);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, sync);
        }

        /// <summary>
        /// Opens the in browser.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnOpenInBrowser(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as IWorkbenchItem;

            if (item == null)
            {
                return;
            }

            var projectData = ProjectDataService.CurrentProjectData;

            if (projectData == null)
            {
                return;
            }

            var webAccessUri =
                new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.String019,
                        projectData.WebAccessUrl,
                        projectData.ProjectCollectionUrl,
                        projectData.ProjectGuid,
                        item.GetId()));

            CommandLibrary.SystemShellCommand.Execute(webAccessUri.AbsoluteUri, ApplicationController.Instance.MainWindow);
        }

        /// <summary>
        /// Disableds the input.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnDisableUserInput(object sender, ExecutedRoutedEventArgs e)
        {
            var disable = Equals(e.Parameter, true);

            ApplicationController.Instance.EnableInput(!disable);
        }

        /// <summary>
        /// Shows the about.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnShowAbout(object sender, ExecutedRoutedEventArgs e)
        {
            CommandLibrary.ShowDialogCommand.Execute(new AboutControl(), ApplicationController.Instance.MainWindow);
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnShowDialog(object sender, ExecutedRoutedEventArgs e)
        {
            var control = e.Parameter as FrameworkElement;
            if (control == null)
            {
                return;
            }

            ApplicationController.Instance.DialogController.ShowDialog(control);
        }

        /// <summary>
        /// Shows the display element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnShowDisplayMode(object sender, ExecutedRoutedEventArgs e)
        {
            var element = e.Parameter as IDisplayMode;
            if (element == null)
            {
                return;
            }

            foreach (var child in ApplicationController.Instance.MainWindow.PART_DisplayModeGrid.Children.OfType<UIElement>())
            {
                child.Visibility = child == element ? Visibility.Visible : Visibility.Hidden;
            }

            ApplicationController.Instance.SetActiveDisplayMode(element);
        }

        /// <summary>
        /// Deletes the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnDeleteView(object sender, ExecutedRoutedEventArgs e)
        {
            var view = e.Parameter as ViewMap;
            if (view == null)
            {
                return;
            }

            if (MessageBox.Show(string.Concat(Resources.String008, view.Title), Resources.String009, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            var projectData = ProjectDataService.CurrentProjectData;

            projectData.ViewMaps.Remove(view);
        }

        /// <summary>
        /// Adds the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnAddView(object sender, ExecutedRoutedEventArgs e)
        {
            var projectData = ProjectDataService.CurrentProjectData;

            if (projectData == null || projectData.ItemTypes == null || projectData.ItemTypes.Count() == 0)
            {
                return;
            }

            var linkName = projectData.LinkTypes == null || projectData.LinkTypes.Count() == 0
                               ? string.Empty
                               : projectData.LinkTypes.ElementAt(0);

            var typeName = projectData.ItemTypes.ElementAt(0).TypeName;

            var view = new ViewMap
            {
                Title = Resources.String020,
                Description = Resources.String021,
                ChildType = typeName,
                LinkName = linkName,
                DisplayOrder = projectData.ViewMaps.Count()
            };

            view.ParentTypes.Add(typeName);

            view = ViewEditorControl.SetupChildStates(projectData, view, typeName);

            projectData.ViewMaps.Add(view);

            CommandLibrary.EditViewCommand.Execute(view, ApplicationController.Instance.MainWindow);
        }

        /// <summary>
        /// Edits the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnEditView(object sender, ExecutedRoutedEventArgs e)
        {
            var view = e.Parameter as ViewMap;
            if (view == null)
            {
                return;
            }

            var editDialog = new ViewEditorControl
                {
                    ViewMap = view, 
                    ProjectData = ProjectDataService.CurrentProjectData, 
                    SaveProjectLayout = ApplicationController.Instance.SaveProjectLayout
                };

            ApplicationController.Instance.DialogController.ShowDialog(editDialog);
        }

        /// <summary>
        /// Saves the item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSaveItem(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as IWorkbenchItem;

            if (item == null || item.ValueProvider == null)
            {
                return;
            }

            var errors = item.ValueProvider.Save();

            if (errors == null || !errors.Any())
            {
                var message = string.Format(CultureInfo.InvariantCulture, Resources.String032, item.GetTypeName(), item.GetId());

                ApplicationController.Instance.SetStatusMessage(message);

                item.OnPropertyChanged();
            }
            else
            {
                ApplicationController.Instance.WorkbenchItemSaveError(item, errors);
            }
        }

        /// <summary>
        /// Refreshes the item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnRefreshItem(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as IWorkbenchItem;

            if (item == null || item.ValueProvider == null)
            {
                return;
            }

            item.ValueProvider.SyncToLatest();
            item.OnPropertyChanged();

            var message = string.Format(
                CultureInfo.InvariantCulture,
                Resources.String012,
                item.GetTypeName(),
                item.GetId());

            ApplicationController.Instance.SetStatusMessage(message);
        }

        /// <summary>
        /// Applications the message.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnApplicationMessage(object sender, ExecutedRoutedEventArgs e)
        {
            var message = e.Parameter as string;

            if (message == null)
            {
                return;
            }

            var mainAppWindow = ApplicationController.Instance.MainWindow;
            if (mainAppWindow == null)
            {
                return;
            }

            Action showMessage = () =>
                {
                    mainAppWindow.PART_StatusBar.Text = message;
                    mainAppWindow.PART_StatusBar.Foreground = Brushes.Black;
                };

            var dispatcher = mainAppWindow.Dispatcher;

            if (Thread.CurrentThread != dispatcher.Thread)
            {
                dispatcher.BeginInvoke(DispatcherPriority.Input, showMessage);
            }
            else
            {
                showMessage();
            }
        }

        /// <summary>
        /// Discards the item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnDiscardItem(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as IWorkbenchItem;

            if (item == null)
            {
                ApplicationController.Instance.SetStatusMessage(Resources.String010);
            }

            ProjectDataService.CurrentProjectData.WorkbenchItems.Remove(item);
        }

        /// <summary>
        /// Closes the project.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCloseProject(object sender, ExecutedRoutedEventArgs e)
        {
            var mainAppWindow = ApplicationController.Instance.MainWindow;
            if (mainAppWindow == null)
            {
                return;
            }

            Action action = () =>
            {
                ApplicationController.Instance.CloseProject();
                ApplicationController.Instance.EnableInput(true);
            };

            var dispatcher = mainAppWindow.Dispatcher;

            ApplicationController.Instance.EnableInput(false);
            dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnExitApplication(object sender, ExecutedRoutedEventArgs e)
        {
            if (ApplicationController.Instance.DataProviderController != null)
            {
                ApplicationController.Instance.DataProviderController.Dispose();
            }

            ApplicationController.Instance.MainWindow.Close();
        }

        /// <summary>
        /// Shows the edit panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnEditItem(object sender, ExecutedRoutedEventArgs e)
        {
            var workbenchItem = e.Parameter as IWorkbenchItem;
            if (workbenchItem == null)
            {
                ApplicationController.Instance.SetStatusMessage(Resources.String011);
                return;
            }

            ShowWorkbenchItemEditPanel(workbenchItem);
        }

        /// <summary>
        /// Saves the project.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSaveProject(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationController.Instance.DataProviderController.BeginSaveProjectData(ProjectDataService.CurrentProjectData);

            ApplicationController.Instance.SaveProjectLayout();

            ApplicationController.Instance.EnableInput(false);

            ApplicationController.Instance.SetStatusMessage(Resources.String022);
        }

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnLoadProject(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationController.Instance.DialogController.ShowDialog(ApplicationController.Instance.DataProviderController.GenerateProjectLoadControl());
        }

        /// <summary>
        /// Creates the parent.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCreateParent(object sender, ExecutedRoutedEventArgs e)
        {
            var typeName = e.Parameter as string;

            if (string.IsNullOrEmpty(typeName))
            {
                ApplicationController.Instance.SetStatusMessage(Resources.String007);

                return;
            }

            var parent = ProjectDataService.CreateNewItem(typeName);

            ProjectDataService.CurrentProjectData.WorkbenchItems.Add(parent);

            ShowWorkbenchItemEditPanel(parent); 
        }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCreateChild(object sender, ExecutedRoutedEventArgs e)
        {
            var args = e.Parameter as IChildCreationParameters;

            if (args == null)
            {
                ApplicationController.Instance.SetStatusMessage(Resources.String006);

                return;
            }

            var child = ProjectDataService.CreateNewChild(args);

            ProjectDataService.CurrentProjectData.WorkbenchItems.Add(child);

            ShowWorkbenchItemEditPanel(child);
        }

        /// <summary>
        /// Closes the edit panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCloseDialog(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = e.Parameter as FrameworkElement;

            if (dialog == null)
            {
                ApplicationController.Instance.SetStatusMessage(Resources.String005);

                return;
            }

            ApplicationController.Instance.DialogController.CloseDialog(dialog);
        }

        /// <summary>
        /// Shows the workbench item edit panel.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        private static void ShowWorkbenchItemEditPanel(IWorkbenchItem workbenchItem)
        {
            var applicationController = ApplicationController.Instance;

            FrameworkElement editDialog = new EditItemControlv2
                {
                    ProjectData = ProjectDataService.CurrentProjectData,
                    DataProvider = ProjectDataService.CurrentDataProvider
                };

            ((EditItemControlv2)editDialog).DelaySetWorkbenchItem(workbenchItem);

            applicationController.DialogController.ShowDialog(editDialog);
        }
    }
}

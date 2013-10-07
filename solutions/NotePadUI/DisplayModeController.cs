using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TfsWorkbench.Core.EventArgObjects;
using TfsWorkbench.Core.Interfaces;
using TfsWorkbench.Core.Services;
using TfsWorkbench.NotePadUI.Helpers;
using TfsWorkbench.NotePadUI.Models;
using TfsWorkbench.NotePadUI.Properties;
using TfsWorkbench.NotePadUI.Services;
using TfsWorkbench.NotePadUI.UIElements;
using TfsWorkbench.UIElements;
using TfsWorkbench.UIElements.FilterObjects;

namespace TfsWorkbench.NotePadUI
{
    public class DisplayModeController
    {
        private readonly IProjectDataService projectDataService;

        private readonly IPadLayoutService padLayoutService;
        private IFilterCollection filterCollection;
        private bool hasQueuedLayoutUpdate;
        private readonly DisplayMode displayMode;

        public DisplayModeController(DisplayMode displayMode)
            : this(displayMode, ServiceManager.Instance.GetService<IProjectDataService>(), PadLayoutService.Instance)
        {
        }

        public DisplayModeController(DisplayMode displayMode, IProjectDataService projectDataService, IPadLayoutService padLayoutService)
        {
            AssertDependenciesAreNotNull(displayMode, projectDataService, padLayoutService);

            this.displayMode = displayMode;
            this.projectDataService = projectDataService;
            this.padLayoutService = padLayoutService;

            FilterCollection = new FilterCollection(Dispatcher.CurrentDispatcher);
            PadUiItems = new ObservableCollection<UIPadItem>();
            PadUiItems.CollectionChanged += OnPadItemsChanged;

            this.projectDataService.ProjectDataChanged += OnProjectDataChanged;

            CreateNoteCommand = new RelayCommand(OnCreateNote);
            ResetCommand = new RelayCommand(OnReset);
            CreateToDoList = new RelayCommand(OnCreateToDoList);

            displayMode.CommandBindings.Add( 
                new CommandBinding(LocalCommandLibrary.DeletePadItemCommand, OnDeletePadItem));

            displayMode.CommandBindings.Add(
                new CommandBinding(LocalCommandLibrary.ChangeColourCommand, OnChangeColour));

            displayMode.CommandBindings.Add(
                new CommandBinding(LocalCommandLibrary.SaveLayoutCommand, (sender, args) => OnSave(null)));
        }

        public IFilterCollection FilterCollection
        {
            get { return filterCollection; }
            internal set
            {
                if (filterCollection == value)
                {
                    return;
                }

                if (filterCollection != null)
                {
                    filterCollection.SelectionChanged -= OnFiltersChanged;
                }

                filterCollection = value;

                if (filterCollection != null)
                {
                    filterCollection.SelectionChanged += OnFiltersChanged;
                }
            }
        }

        public ICommand CreateNoteCommand { get; private set; }

        public ICommand CreateToDoList { get; set; }

        public ICommand ResetCommand { get; private set; }

        public ObservableCollection<UIPadItem> PadUiItems { get; private set; }

        public void Highlight(IWorkbenchItem workbenchItem)
        {
            if (workbenchItem == null)
            {
                return;
            }

            var selectedItems = PadUiItems
                .Select(ui => ui.DataContext)
                .OfType<WorkbenchPadItem>()
                .Select(pi => pi.WorkbenchItem)
                .ToList();

            if (!selectedItems.Contains(workbenchItem))
            {
                selectedItems.Add(workbenchItem);

                FilterCollection.SelectItems(selectedItems);
            }

            Action callback = () =>
                {
                    var uiItem = GetCorrespondingUiElement(workbenchItem);

                    if (uiItem == null)
                    {
                        return;
                    }

                    ElementDragHelper.BringToTop(uiItem, displayMode.PART_LayoutCanvas);
                    uiItem.BringIntoView();
                    HighlightHelper.ClearAllHighlights();
                    HighlightHelper.Highlight(uiItem);
                    CommandLibrary.ShowDisplayModeCommand.Execute(displayMode, displayMode);
                };

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback);
        }

        private static void AssertDependenciesAreNotNull(DisplayMode displayMode, IProjectDataService projectDataService, IPadLayoutService padLayoutService)
        {
            if (displayMode == null)
            {
                throw new ArgumentNullException("displayMode");
            }

            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            if (padLayoutService == null)
            {
                throw new ArgumentNullException("padLayoutService");
            }
        }

        private void OnReset(object obj)
        {
            var decisionControl = new DecisionControl
                {
                    Caption = "Confirm Reset All Notes",
                    Message = "Are you sure you want to reset all notes?",
                    HideDoNotShowAgain = true
                };

            decisionControl.DecisionMade += (sender, args) =>
                {
                    if (!decisionControl.IsYes)
                    {
                        return;
                    }

                    var itemsToDelte = PadUiItems.ToArray();

                    foreach (var padUiItem in itemsToDelte)
                    {
                        DeletePadItem(padUiItem);
                    }

                    padLayoutService.Save();
                };

            CommandLibrary.ShowDialogCommand.Execute(decisionControl, displayMode);
        }

        private void OnDeletePadItem(object sender, ExecutedRoutedEventArgs e)
        {
            var target = e.Parameter as UIPadItem;

            if (target == null)
            {
                return;
            }

            DeletePadItem(target);

            displayMode.PART_LayoutCanvas.ResizePanelToContent();
        }

        private void OnCreateNote(object obj)
        {
            if (projectDataService.CurrentProjectData == null || !projectDataService.CurrentProjectData.ProjectGuid.HasValue)
            {
                return;
            }

            var colourAsString = obj as string;

            if (colourAsString == null)
            {
                return;
            }

            Action<NotePadItem> initialiser = padItem =>
                {
                    padItem.Created = DateTime.Now;
                    padItem.Text = "Add text here";
                    padItem.Colour = colourAsString;
                    padItem.IsPinable = true;
                };

            var item =
                PadItemFactory.CreateInstance(
                    projectDataService.CurrentProjectData.ProjectGuid.Value,
                    initialiser);

            padLayoutService.Add(item);

            var uiPadItem = new UIPadItem {DataContext = item};

            PadUiItems.Add(uiPadItem);

            Action postBack = () => ElementDragHelper.BringToTop(uiPadItem, displayMode.PART_LayoutCanvas);

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, postBack);
        }

        private void OnCreateToDoList(object obj)
        {
            if (projectDataService.CurrentProjectData == null || !projectDataService.CurrentProjectData.ProjectGuid.HasValue)
            {
                return;
            }

            Action<ToDoList> initialiser = padItem =>
            {
                padItem.Width = 300;
                padItem.Height = 500;
                padItem.ToDoItems.Add(new ToDoItem { Text = "TODO - Add an action here..." });
                padItem.Colour = "ToDoList";
            };

            var item =
                PadItemFactory.CreateInstance(
                    projectDataService.CurrentProjectData.ProjectGuid.Value,
                    initialiser);

            padLayoutService.Add(item);

            var uiPadItem = new UIPadItem { DataContext = item };

            PadUiItems.Add(uiPadItem);

            Action postBack = () => ElementDragHelper.BringToTop(uiPadItem, displayMode.PART_LayoutCanvas);

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, postBack);
        }

        private void DeletePadItem(UIPadItem target)
        {
            var padItemBase = target.DataContext as PadItemBase;

            if (padItemBase != null)
            {
                padLayoutService.Remove(padItemBase);
            }

            PadUiItems.Remove(target);

            displayMode.PART_LayoutCanvas.Children.Remove(target);

            DeleteWorkbenchPadItem(target);
        }

        private void DeleteWorkbenchPadItem(FrameworkElement target)
        {
            var workbenchPadItem = target.DataContext as WorkbenchPadItem;

            if (workbenchPadItem == null)
            {
                return;
            }

            filterCollection.DeselectItem(workbenchPadItem.WorkbenchItem);
        }

        private void OnProjectDataChanged(object sender, ProjectDataChangedEventArgs e)
        {
            var padItems = padLayoutService.GetWorkspaceLayout(e.NewValue).ToList();

            UpdatePadItems(padItems);

            InitialiseFilterCollection(e.NewValue, padItems);

            displayMode.PART_LayoutCanvas.ResizePanelToContent();
        }

        private FrameworkElement GetCorrespondingUiElement(IWorkbenchItem workbenchItem)
        {
            var uiItem = displayMode.PART_LayoutCanvas
                .Children
                    .OfType<UIPadItem>()
                    .FirstOrDefault(ui =>
                    {
                        var padItem = ui.DataContext as WorkbenchPadItem;
                        return padItem != null && padItem.WorkbenchItem == workbenchItem;
                    });

            return uiItem;
        }

        private void InitialiseFilterCollection(IProjectData projectData, IEnumerable<PadItemBase> padItems)
        {
            FilterCollection.Initialise(projectData);
            hasQueuedLayoutUpdate = true;
            FilterCollection.SelectItems(padItems.OfType<WorkbenchPadItem>().Select(pi => pi.WorkbenchItem));
            hasQueuedLayoutUpdate = false;
        }

        private void UpdatePadItems(List<PadItemBase> padItems)
        {
            PadUiItems.Clear();
            padItems.ForEach(pi => PadUiItems.Add(new UIPadItem { DataContext = pi }));
        }

        private void OnFiltersChanged(object sender, EventArgs e)
        {
            EnqueueLayoutUpdate();
        }

        private void EnqueueLayoutUpdate()
        {
            if (hasQueuedLayoutUpdate)
            {
                return;
            }

            hasQueuedLayoutUpdate = true;

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(UpdateLayout));
        }

        private void UpdateLayout()
        {
            var padItems = padLayoutService.GetWorkspaceLayout(projectDataService.CurrentProjectData, filterCollection.IsMatch).ToList();

            UpdatePadItems(padItems);

            padLayoutService.Save();

            hasQueuedLayoutUpdate = false;
        }

        private void OnPadItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.OfType<UIElement>())
                    {
                        displayMode.PART_LayoutCanvas.Children.Add(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<UIElement>())
                    {
                        displayMode.PART_LayoutCanvas.Children.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    displayMode.PART_LayoutCanvas.Children.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private void OnSave(object obj)
        {
            padLayoutService.Save();
            Settings.Default.Save();
        }

        private void OnChangeColour(object sender, ExecutedRoutedEventArgs e)
        {
            var args = e.Parameter as List<object>;

            if (args == null || args.Count() != 2)
            {
                return;
            }

            var padItem = args[0] as PadItemBase;
            var colour = args[1] as string;

            if (padItem == null)
            {
                return;
            }

            padItem.Colour = colour;

            padItem.OnPropertyChanged("Colour");
        }
    }
}
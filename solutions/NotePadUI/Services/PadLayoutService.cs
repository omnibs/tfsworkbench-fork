using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.Core.Interfaces;
using TfsWorkbench.NotePadUI.Helpers;
using TfsWorkbench.NotePadUI.Models;
using TfsWorkbench.NotePadUI.Properties;

namespace TfsWorkbench.NotePadUI.Services
{
    public class PadLayoutService : IPadLayoutService
    {
        private static PadLayoutService instance;
        private PadItemCollection padItemCollection;
        private XmlSerializer serialiser;

        public static IPadLayoutService Instance
        {
            get { return instance = instance ?? new PadLayoutService(); }
        }

        public PadLayoutService()
        {
            DataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Settings.Default.DataFile);
        }

        public PadItemCollection PadItemCollection
        {
            get
            {
                if (padItemCollection == null)
                {
                    LoadMap();
                }

                return padItemCollection;
            }
        }

        public string DataPath { get; set; }

        public IList<PadItemBase> GetWorkspaceLayout(IProjectData projectData)
        {
            var output = new List<PadItemBase>();

            foreach (var padItemBase in GetAllProjectPadItems(projectData))
            {
                var selectableItem = padItemBase as ISelectable;

                if (selectableItem == null || selectableItem.IsSelected)
                {
                    output.Add(padItemBase);
                }
            }

            return output;
        }

        public IList<PadItemBase> GetWorkspaceLayout(IProjectData projectData, Predicate<IWorkbenchItem> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            if (!projectData.ProjectGuid.HasValue)
            {
                return new List<PadItemBase>();
            }

            var workspaceLayout = new List<PadItemBase>();

            var allProjectPadItems = GetAllProjectPadItems(projectData);
            var workbenchPadItems = allProjectPadItems.OfType<WorkbenchPadItem>();

            Func<IWorkbenchItem, WorkbenchPadItem> factory = item => 
                PadItemFactory
                    .CreateInstance<WorkbenchPadItem>(projectData.ProjectGuid.Value, 
                    padItem =>
                        { 
                            padItem.WorkbenchItem = item;
                            padItem.WorkbenchItemId = item.GetId();
                            padItem.Width = 500;
                            padItem.Height = 300;
                            padItem.Colour = "White";
                        });

            var requiredItems = projectData.WorkbenchItems.Where(i => filter(i)).ToArray();
            var missingItems = requiredItems.Where(i => workbenchPadItems.All(pi => pi.WorkbenchItem != i)).Select(factory);

            foreach (var item in missingItems)
            {
                workspaceLayout.Add(item);
                Add(item);
            }

            foreach (var padItem in allProjectPadItems)
            {
                var workbenchPadItem = padItem as WorkbenchPadItem;

                if (workbenchPadItem != null)
                {
                    workbenchPadItem.IsSelected = filter(workbenchPadItem.WorkbenchItem);
                }

                if (workbenchPadItem == null || workbenchPadItem.IsSelected)
                {
                    workspaceLayout.Add(padItem);
                }
            }

            return workspaceLayout;
        }

        public void Add(PadItemBase itemToAdd)
        {
            if (PadItemCollection.Any())
            {
                itemToAdd.Id = PadItemCollection.Max(pi => pi.Id) + 1;
            }
            else
            {
                itemToAdd.Id = 1;
            }

            PadItemCollection.Add(itemToAdd);

            Save();
        }

        public void Save()
        {
            try
            {
                var directoryName = Path.GetDirectoryName(DataPath);

                if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                using (var sw = new StreamWriter(DataPath))
                {
                    Serialiser.Serialize(sw, PadItemCollection);
                }
            }
            catch (Exception)
            {
                // Ignore persistence errors.
            }
        }

        public void Remove(PadItemBase padItemToRemove)
        {
            PadItemCollection.Remove(padItemToRemove);
            Save();
        }

        private XmlSerializer Serialiser
        {
            get
            {
                return serialiser = serialiser ?? new XmlSerializer(typeof(PadItemCollection));
            }
        }

        private void LoadMap()
        {
            if (!File.Exists(DataPath))
            {
                padItemCollection = new PadItemCollection();
            }
            else
                try
                {
                    using (var sr = new StreamReader(DataPath))
                    {
                        padItemCollection = (PadItemCollection)Serialiser.Deserialize(sr);
                    }
                }
                catch (Exception)
                {
                    padItemCollection = new PadItemCollection();
                }
        }

        private IList<PadItemBase> GetAllProjectPadItems(IProjectData projectData)
        {
            var output = new List<PadItemBase>();

            if (projectData == null)
            {
                return output;
            }

            var projectMap =
                PadItemCollection.Where(im => im.ProjectGuid == projectData.ProjectGuid.ToString()).ToArray();

            foreach (var map in projectMap)
            {
                var workbenchPadItem = map as WorkbenchPadItem;

                if (workbenchPadItem == null)
                {
                    output.Add(map);
                    continue;
                }

                var workbenchItem =
                    projectData.WorkbenchItems.FirstOrDefault(wbi => wbi.GetId() == workbenchPadItem.WorkbenchItemId);

                if (workbenchItem == null)
                {
                    continue;
                }

                workbenchPadItem.WorkbenchItem = workbenchItem;

                output.Add(map);
            }

            return output;
        }
    }
}
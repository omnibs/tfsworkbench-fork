using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using NUnit.Framework;
using Rhino.Mocks;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.Core.Interfaces;
using TfsWorkbench.NotePadUI;
using TfsWorkbench.NotePadUI.Models;
using TfsWorkbench.NotePadUI.Services;
using Settings = TfsWorkbench.Core.Properties.Settings;

namespace TfsWorkbench.Tests
{
    [TestFixture]
    public class PadLayoutServiceTests
    {
        private PadItemCollection padItemCollection;
        private string projectGuid;
        private IWorkbenchItemRepository workbenchItemRepository;
        private IProjectData projectData;
        private string dataPath;
        private XmlSerializer serialiser;
        private Collection<IWorkbenchItem> items;

        [SetUp]
        public void SetUp()
        {
            serialiser = new XmlSerializer(typeof(PadItemCollection));

            projectGuid = Guid.NewGuid().ToString();

            items = new Collection<IWorkbenchItem>();

            CreateProjectData();

            CreateWorkbenchItemRepository();

            CreateTempDataFile();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(dataPath);
        }

        [Test]
        public void When_calling_instance_Then_default_instance_is_returned()
        {
            // Arrange
            
            // Act
            var result = PadLayoutService.Instance;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void When_calling_GetWorkspaceLayout_with_null_arg_then_empty_collection_is_returned()
        {
            // Arrange
            var service = new PadLayoutService();

            // Act
            var result = service.GetWorkspaceLayout(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [Test]
        public void When_constructed_then_default_data_path_is_set()
        {
            // Arrange
            var service = new PadLayoutService();

            // Act
            var result = service.DataPath;

            // Assert
            Assert.IsNotNullOrEmpty(result);
        }

        [Test]
        public void When_constructing_then_layout_data_is_loaded()
        {
            // Arrange
            var service = new PadLayoutService {DataPath = dataPath};

            // Act
            var result = service.PadItemCollection;

            // Assert
            AssertCollectionsContainSameItems(padItemCollection, result);
        }

        [Test]
        public void When_calling_GetWorkspaceLayout_with_valid_arg_then_populated_collection_returned()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };

            // Act
            var result = service.GetWorkspaceLayout(projectData);

            // Assert
            AssertCollectionsContainSameItems(padItemCollection, result);
        }

        [Test]
        public void When_calling_GetWorkspaceLayout_with_project_data_then_only_mapped_items_returned()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };

            service.PadItemCollection.Remove(service.PadItemCollection[0]);

            // Act
            var result = service.GetWorkspaceLayout(projectData);

            // Assert
            Assert.AreEqual(padItemCollection.Count() - 1, result.Count());
        }

        [Test]
        public void When_calling_GetWorkspaceLayout_with_null_arg_then_empty_collection_returned()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };

            // Act
            var result = service.GetWorkspaceLayout(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void When_data_file_is_not_valid_then_return_empty_collection()
        {
            // Arrange
            using (var sr = new StreamWriter(dataPath))
            {
                sr.WriteLine("Invalid conent");
            }

            var service = new PadLayoutService { DataPath = dataPath };

            // Act
            var result = service.PadItemCollection;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void When_calling_GetWorkspaceLayout_with_predicate_then_only_matching_workitems_are_returned()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };

            // Act
            var result = service.GetWorkspaceLayout(projectData, item => item.GetId() == 1).OfType<WorkbenchPadItem>();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(workbenchItemRepository.First(item => item.GetId() == 1), result.First().WorkbenchItem);
        }

        [Test]
        public void When_calling_GetWorkspaceLayout_with_null_predicate_then_exception_is_thrown()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };

            TestDelegate methodToTest = () => service.GetWorkspaceLayout(projectData, null);

            // Act
            var result = Assert.Throws<ArgumentNullException>(methodToTest).Message;

            // Assert
            Assert.IsTrue(result.Contains("filter"));
        }

        [Test]
        public void When_saving_then_layout_data_is_written_to_file()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };

            var collection = service.PadItemCollection;
            var padItem = new WorkbenchPadItem {LeftOffset = 100, TopOffset = 200, ProjectGuid = Guid.NewGuid().ToString(), WorkbenchItemId = 101};
            collection.Add(padItem);

            // Act
            service.Save();

            // Assert
            PadItemCollection results;
            using (var sr = new StreamReader(dataPath))
            {
                results = (PadItemCollection)serialiser.Deserialize(sr);
            }

            AssertCollectionsContainSameItems(results, collection);
        }

        [Test]
        public void When_calling_GetWorkspaceLayout_with_predicate_that_matches_new_items_then_new_pad_items_added_to_collection()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };

            var itemsBefore = service.PadItemCollection.Count();

            AddNewWorkbenchItemToRepository(4);

            // Act
            var workspace = service.GetWorkspaceLayout(projectData, item => true);
            var itemsAfter = service.PadItemCollection.Count();

            // Assert
            Assert.Greater(itemsAfter, itemsBefore);
            Assert.AreEqual(workspace.Count(), itemsAfter);
        }

        [Test]
        public void When_serialising_notes_then_values_are_included_in_workspace()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };
            service.PadItemCollection.Add(new NotePadItem { Text = "Hello", ProjectGuid = projectGuid });

            // Act
            var workspace = service.GetWorkspaceLayout(projectData);

            // Assert
            Assert.IsTrue(workspace.OfType<NotePadItem>().Any());
        }

        [Test]
        public void When_getting_workspace_with_predicate_then_matching_workbench_items_are_marked_as_selected()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };
            service.PadItemCollection.OfType<WorkbenchPadItem>().ToList().ForEach(pi => pi.IsSelected = false);
            Predicate<IWorkbenchItem> predicate = item => item.GetId() == 1;

            // Act
            service.GetWorkspaceLayout(projectData, predicate);

            // Assert
            foreach (var padItem in service.PadItemCollection.OfType<WorkbenchPadItem>())
            {
                Assert.AreEqual(padItem.IsSelected, predicate(padItem.WorkbenchItem));
            }
        }

        [Test]
        public void When_getting_workspace_then_only_selected_workbenchItems_are_returned()
        {
            // Arrange
            var service = new PadLayoutService { DataPath = dataPath };
            service.PadItemCollection.Add(new WorkbenchPadItem {IsSelected = false});

            // Act
            var results = service.GetWorkspaceLayout(projectData).OfType<WorkbenchPadItem>();

            // Assert
            Assert.IsTrue(results.All(r => r.IsSelected));
        }

        private void AddNewWorkbenchItemToRepository(int itemId)
        {
            var item = MockRepository.GenerateStub<IWorkbenchItem>();

            item[Settings.Default.IdFieldName] = itemId;

            items.Add(item);
        }

        private void CreateProjectData()
        {
            projectData = MockRepository.GenerateStub<IProjectData>();

            projectData.ProjectGuid = new Guid(projectGuid);
        }

        private void CreateWorkbenchItemRepository()
        {
            workbenchItemRepository = MockRepository.GenerateMock<IWorkbenchItemRepository>();

            for (int i = 0; i < 3; i++)
            {
                AddNewWorkbenchItemToRepository(i);
            }

            workbenchItemRepository
                .Expect(wir => wir.GetEnumerator())
                .WhenCalled(mi => mi.ReturnValue = items.GetEnumerator())
                .Return(null)
                .Repeat.Any();

            projectData
                .Expect(pd => pd.WorkbenchItems)
                .Return(workbenchItemRepository)
                .Repeat.Any();
        }

        private void CreateTempDataFile()
        {
            padItemCollection = new PadItemCollection();

            foreach (var wbi in workbenchItemRepository)
            {
                var itemToAdd = new WorkbenchPadItem
                    {
                        LeftOffset = 1,
                        TopOffset = 2,
                        ProjectGuid = projectGuid,
                        WorkbenchItemId = wbi.GetId(),
                        IsSelected = true
                    };

                padItemCollection.Add(itemToAdd);
            }

            dataPath = Path.GetTempFileName();

            using (var sw = new StreamWriter(dataPath))
            {
                serialiser.Serialize(sw, padItemCollection);
            }
        }

        private static void AssertCollectionsContainSameItems(IEnumerable<PadItemBase> collectionA, IEnumerable<PadItemBase> collectionB)
        {
            Assert.IsTrue(
                collectionA.All(
                    i =>
                    collectionB.Any(
                        r =>
                        r.LeftOffset == i.LeftOffset && r.ProjectGuid == i.ProjectGuid && r.TopOffset == i.TopOffset)));
        }
    }
}

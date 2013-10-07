namespace TfsWorkbench.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.FilterService;
    using TfsWorkbench.FilterService.Properties;
    using TfsWorkbench.Tests.Helpers;

    using Settings = TfsWorkbench.Core.Properties.Settings;

    /// <summary>
    /// The filter service test fixture class.
    /// </summary>
    [TestFixture]
    public class FilterServiceTests
    {
        private IProjectData projectData;

        [SetUp]
        public void SetUp()
        {
            this.GenerateDependencyServices();
        }

        [TearDown]
        public void TearDown()
        {
            this.projectData = null;
            ServiceManagerHelper.ClearDummyManager();
        }

        [Test]
        public void Filter_service_should_raise_changed_event_when_predicate_changed()
        {
            // Arrange
            this.projectData = CreateDummyProjectData();

            var service = GetService();

            var hasRaisedEvent = false;

            service.FiltersChanged += (s, e) =>
                {
                    hasRaisedEvent = true;
                };

            // Act
            service.FilterProvider = CreateProvider(w => w.GetTypeName() == "Type 1");

            // Assert
            hasRaisedEvent.ShouldBeTrue();
        }

        private static IFilterProvider CreateProvider(Func<IWorkbenchItem, bool> pred)
        {
            var provider = MockRepository.GenerateMock<IFilterProvider>();

            provider.Expect(p => p.IsIncluded(null)).IgnoreArguments().WhenCalled(
                mi => mi.ReturnValue = pred(mi.Arguments.First() as IWorkbenchItem)).Return(false).Repeat.Any();

            return provider;
        }

        [Test]
        public void Filter_service_should_only_list_items_that_do_not_match_specfied_filter_predicate()
        {
            // Arrange
            var items = CreateItems();

            var service = GetService();

            // Act
            service.FilterProvider = CreateProvider(w => w.GetTypeName() == "Type 1");
            var result1 = service.ApplyFilter(items);
            service.FilterProvider = CreateProvider(w => w.GetTypeName() == "Type 2");
            var result2 = service.ApplyFilter(items);

            // Assert
            result1.ShouldNotBeNull();
            result2.ShouldNotBeNull();

            result1.Count().ShouldEqual(2);
            result2.Count().ShouldEqual(2);

            result1.All(w => w.GetTypeName() == "Type 1");
            result2.All(w => w.GetTypeName() == "Type 2");
        }

        [Test]
        public void Filter_instance_should_provide_a_fluent_construction_method()
        {
            // Arrange
            
            // Act
            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.IsEqualTo, "Test");

            // Assert
            filter.ItemTypeName.ShouldEqual("Type Name 1");
            filter.FieldName.ShouldEqual("A.Field");
            filter.FilterOperator.ShouldEqual(FilterOperatorOption.IsEqualTo);
            filter.Value.ShouldEqual("Test");
        }

        [Test]
        public void Filter_should_indicate_validity()
        {
            // Arrange
            var filter = new WorkbenchFilter();

            // Act
            var result1 = filter.IsValidFilter;

            filter.FieldName = "A.Field.Name";

            var result2 = filter.IsValidFilter;

            filter.ItemTypeName = null;

            var result3 = filter.IsValidFilter;

            filter.ItemTypeName = "A type name";

            var result4 = filter.IsValidFilter;

            // Assert
            result1.ShouldBeFalse();
            result2.ShouldBeTrue();
            result3.ShouldBeFalse();
            result4.ShouldBeTrue();
        }

        [Test]
        public void Filter_should_match_specifications_against_workbenchitems()
        {
            // Arrange
            const string WorkbenchItemTypeName = "Type Name 1";
            const string Value = "Test";
            const string FieldName = "A.Field";

            var item1 = CreateDummyWorkbenchItem(WorkbenchItemTypeName);

            var filter = new WorkbenchFilter()
                .ItemsOfType(WorkbenchItemTypeName)
                .WithField(FieldName)
                .That(FilterOperatorOption.IsEqualTo, Value);

            // Act
            var result1 = filter.IsMatch(item1);

            item1.Expect(w => w[FieldName]).Return(Value).Repeat.Any();

            var result2 = filter.IsMatch(item1);

            // Assert
            result1.ShouldBeFalse();
            result2.ShouldBeTrue();
        }

        [Test]
        public void Filter_should_compare_by_equal_to()
        {
            // Arrange
            var itemToMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch.Expect(w => w["A.Field"]).Return("Test").Repeat.Any();

            var itemToNotMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToNotMatch.Expect(w => w["A.Field"]).Return("Fail").Repeat.Any();

            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.IsEqualTo, "Test");

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch);
            var result2 = filter.IsMatch(itemToNotMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_compare_by_not_equal_to()
        {
            // Arrange
            var itemToMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch.Expect(w => w["A.Field"]).Return("Test").Repeat.Any();

            var itemToNotMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToNotMatch.Expect(w => w["A.Field"]).Return("Fail").Repeat.Any();

            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.IsNotEqualTo, "Fail");

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch);
            var result2 = filter.IsMatch(itemToNotMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_compare_by_greater_than()
        {
            // Arrange
            var itemToMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch.Expect(w => w["A.Field"]).Return(10).Repeat.Any();

            var itemToNotMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToNotMatch.Expect(w => w["A.Field"]).Return(5).Repeat.Any();

            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.IsGreaterThan, 5);

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch);
            var result2 = filter.IsMatch(itemToNotMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_compare_by_less_than()
        {
            // Arrange
            var itemToMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch.Expect(w => w["A.Field"]).Return(5).Repeat.Any();

            var itemToNotMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToNotMatch.Expect(w => w["A.Field"]).Return(10).Repeat.Any();

            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.IsLessThan, 10);

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch);
            var result2 = filter.IsMatch(itemToNotMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_compare_by_greater_than_equal_to()
        {
            // Arrange
            var itemToMatch1 = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch1.Expect(w => w["A.Field"]).Return(10).Repeat.Any();

            var itemToMatch2 = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch2.Expect(w => w["A.Field"]).Return(11).Repeat.Any();

            var itemToNotMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToNotMatch.Expect(w => w["A.Field"]).Return(9).Repeat.Any();

            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.IsGreaterThanEqualTo, 10);

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch1);
            var result2 = filter.IsMatch(itemToMatch2);
            var result3 = filter.IsMatch(itemToNotMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            result3.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_compare_by_less_than_equal_to()
        {
            // Arrange
            var itemToMatch1 = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch1.Expect(w => w["A.Field"]).Return(9).Repeat.Any();

            var itemToMatch2 = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch2.Expect(w => w["A.Field"]).Return(10).Repeat.Any();

            var itemToNotMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToNotMatch.Expect(w => w["A.Field"]).Return(11).Repeat.Any();

            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.IsLessThanEqualTo, 10);

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch1);
            var result2 = filter.IsMatch(itemToMatch2);
            var result3 = filter.IsMatch(itemToNotMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            result3.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_compare_by_starts_with()
        {
            // Arrange
            var itemToMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch.Expect(w => w["A.Field"]).Return("ABCDEFGHIJK").Repeat.Any();

            var itemToNotMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToNotMatch.Expect(w => w["A.Field"]).Return("LMNOPQRSTUVWXYZ").Repeat.Any();

            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.StartsWith, "ABC");

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch);
            var result2 = filter.IsMatch(itemToNotMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_compare_by_ends_with()
        {
            // Arrange
            var itemToMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch.Expect(w => w["A.Field"]).Return("ABCDEFGHIJK").Repeat.Any();

            var itemToNotMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToNotMatch.Expect(w => w["A.Field"]).Return("LMNOPQRSTUVWXYZ").Repeat.Any();

            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.EndsWith, "IJK");

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch);
            var result2 = filter.IsMatch(itemToNotMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_compare_by_contains()
        {
            // Arrange
            var itemToMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch.Expect(w => w["A.Field"]).Return("ABCDEFGHIJK").Repeat.Any();

            var itemToNotMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToNotMatch.Expect(w => w["A.Field"]).Return("LMNOPQRSTUVWXYZ").Repeat.Any();

            var filter = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.Contains, "DEF");

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch);
            var result2 = filter.IsMatch(itemToNotMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_by_chainable()
        {
            // Arrange
            var itemToMatch = CreateDummyWorkbenchItem("Type Name 1");
            itemToMatch.Expect(w => w["A.Field"]).Return("ABC").Repeat.Any();
            itemToMatch.Expect(w => w["B.Field"]).Return(123).Repeat.Any();
            itemToMatch.Expect(w => w["C.Field"]).Return(2.5).Repeat.Any();

            var filter1 = new WorkbenchFilter()
                .ItemsOfType("Type Name 1")
                .WithField("A.Field")
                .That(FilterOperatorOption.IsEqualTo, "ABC");

            var filter2 = new WorkbenchFilter()
                .WithField("B.Field")
                .That(FilterOperatorOption.IsEqualTo, 123);

            var filter3 = new WorkbenchFilter(FilterActionOption.Exclude)
                .WithField("C.Field")
                .That(FilterOperatorOption.IsEqualTo, 2.5);

            var filtersCollection = new WorkbenchFilterCollection();

            filtersCollection.And(filter1);
            filtersCollection.And(filter2);

            // Act
            var result1 = filtersCollection.IsIncluded(itemToMatch);

            filtersCollection.And(filter3);
            System.Diagnostics.Debug.WriteLine(filtersCollection);

            var result2 = filtersCollection.IsIncluded(itemToMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
        }

        [Test]
        public void Filter_should_match_all_types_if_no_type_specfied()
        {
            // Arrange
            const string WorkItemTypeName1 = "Type Name 1";
            const string WorkItemTypeName2 = "Type Name 2";
            const string TestValue = "Test";
            const string FieldName = "A.Field";

            var itemToMatch1 = CreateDummyWorkbenchItem(WorkItemTypeName1);
            itemToMatch1.Expect(w => w[FieldName]).Return(TestValue).Repeat.Any();

            var itemTomatch2 = CreateDummyWorkbenchItem(WorkItemTypeName2);
            itemTomatch2.Expect(w => w[FieldName]).Return(TestValue).Repeat.Any();

            var filter = new WorkbenchFilter()
                .WithField(FieldName)
                .That(FilterOperatorOption.IsEqualTo, TestValue);

            System.Diagnostics.Debug.WriteLine(filter);

            // Act
            var result1 = filter.IsMatch(itemToMatch1);
            var result2 = filter.IsMatch(itemTomatch2);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
        }

        [Test]
        public void Filters_should_be_xml_serialisable()
        {
            // Arrange
            var collection = new WorkbenchFilterCollection()
                .Include("Sprint Backlog Item", "System.IterationPath", FilterOperatorOption.StartsWith, "Project\\Release 01")
                .Include("Product Backlog Item", "Scrum.V3.WorkRemaining", FilterOperatorOption.IsNotEqualTo, 0);

            var serialiser = new SerializerInstance<WorkbenchFilterCollection>();

            // Act
            var serialisedFilter = serialiser.Serialize(collection);

            System.Diagnostics.Debug.WriteLine(serialisedFilter);

            var result = serialiser.Deserialize(serialisedFilter);

            // Assert
            Equals(result.ToString(), collection.ToString());
        }

        [Test]
        public void Controller_should_load_filters_from_project_data_when_changed()
        {
            // Arrange
            var projectData = CreateDummyProjectData();

            var collection = new WorkbenchFilterCollection()
                .Include("Sprint Backlog Item", "System.IterationPath", FilterOperatorOption.StartsWith, "Project\\Release 01")
                .Include("Product Backlog Item", "Scrum.V3.WorkRemaining", FilterOperatorOption.IsNotEqualTo, 0);

            var serialiser = new SerializerInstance<WorkbenchFilterCollection>();

            projectData.Filter = serialiser.Serialize(collection);

            var filterServiceView = MockRepository.GenerateMock<IFilterServiceView>();
            filterServiceView.Expect(v => v.CommandBindings).Return(new CommandBindingCollection()).Repeat.Any();

            var projectDataService = MockRepository.GenerateStub<IProjectDataService>();
            projectDataService.CurrentProjectData = projectData;

            var controller = new FilterServiceController(filterServiceView, projectDataService, MockRepository.GenerateMock<IFilterService>());

            // Act
            this.projectData = projectData;
            projectDataService.Raise(
                pds => pds.ProjectDataChanged += null,
                projectDataService,
                new ProjectDataChangedEventArgs(null, projectData));

            var filter = controller.Filters;

            // Assert
            filter.ToString().ShouldEqual(collection.ToString());
        }

        [Test]
        public void Exclusion_filters_should_exclude_matches_from_results()
        {
            // Arrange
            const string TypeName = "Type 1";
            const string FieldName = "Field.Name.1";
            const string FieldValue1 = "ABC";
            const string FieldValue2 = "DEF";

            var itemToExclude = CreateDummyWorkbenchItem(TypeName);
            var itemToInclude = CreateDummyWorkbenchItem(TypeName);

            itemToExclude.Expect(w => w[FieldName]).Return(FieldValue1).Repeat.Any();
            itemToInclude.Expect(w => w[FieldName]).Return(FieldValue2).Repeat.Any();

            this.projectData = CreateDummyProjectData(new[] { itemToExclude, itemToInclude });

            var collection = new WorkbenchFilterCollection()
                .Exclude(TypeName, FieldName, FilterOperatorOption.IsEqualTo, FieldValue1);

            System.Diagnostics.Debug.WriteLine(collection);

            // Act
            var result1 = collection.IsIncluded(itemToExclude);
            var result2 = collection.IsIncluded(itemToInclude);

            // Assert
            result1.ShouldBeFalse();
            result2.ShouldBeTrue();
        }

        [Test]
        public void Exclusion_filters_should_override_inclusion_filters()
        {
            // Arrange
            const string TypeName = "Type 1";
            const string FieldName = "Field.Name.1";
            const string FieldValue1 = "ABC";
            const string FieldValue2 = "DEF";

            var itemToExclude = CreateDummyWorkbenchItem(TypeName);
            var itemToInclude = CreateDummyWorkbenchItem(TypeName);

            itemToExclude.Expect(w => w[FieldName]).Return(FieldValue1).Repeat.Any();
            itemToInclude.Expect(w => w[FieldName]).Return(FieldValue2).Repeat.Any();

            this.projectData = CreateDummyProjectData(new[] { itemToExclude, itemToInclude });

            var collection = new WorkbenchFilterCollection()
                .Exclude(TypeName, FieldName, FilterOperatorOption.IsEqualTo, FieldValue1)
                .Include(Resources.String002, FieldName, FilterOperatorOption.IsEqualTo, FieldValue1)
                .Include(Resources.String002, FieldName, FilterOperatorOption.IsEqualTo, FieldValue2);

            System.Diagnostics.Debug.WriteLine(collection);

            // Act
            var result1 = collection.IsIncluded(itemToExclude);
            var result2 = collection.IsIncluded(itemToInclude);

            // Assert
            result1.ShouldBeFalse();
            result2.ShouldBeTrue();
        }

        [Test]
        public void Filters_should_compare_by_correct_type()
        {
            // Arrange
            const string TypeName = "Type 1";
            const string FieldName = "Field.Name.1";

            var numberFilter =
                new WorkbenchFilter()
                    .ItemsOfType(Resources.String002)
                    .WithField(FieldName)
                    .That(FilterOperatorOption.IsEqualTo, "1");

            var dateFilter =
                new WorkbenchFilter()
                    .ItemsOfType(Resources.String002)
                    .WithField(FieldName)
                    .That(FilterOperatorOption.IsEqualTo, new DateTime(2011, 1, 1));

            var intMatch = CreateDummyWorkbenchItem(TypeName);
            intMatch.Expect(w => w[FieldName]).Return(1).Repeat.Any();

            var doubleMatch = CreateDummyWorkbenchItem(TypeName);
            doubleMatch.Expect(w => w[FieldName]).Return(1.0).Repeat.Any();

            var dateMatch = CreateDummyWorkbenchItem(TypeName);
            dateMatch.Expect(w => w[FieldName]).Return(new DateTime(2011, 1, 1)).Repeat.Any();

            // Act
            var result1 = numberFilter.IsMatch(intMatch);
            var result2 = numberFilter.IsMatch(doubleMatch);
            var result3 = dateFilter.IsMatch(dateMatch);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            result3.ShouldBeTrue();
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <returns>The service instance</returns>
        private static IFilterService GetService()
        {
            var service = ServiceManager.Instance.GetService<IFilterService>();

            if (service == null)
            {
                throw new NullReferenceException("Service instance does not exist");
            }

            return service;
        }

        /// <summary>
        /// Creates the dummy project data.
        /// </summary>
        /// <param name="items">The items.</param>
        private static IProjectData CreateDummyProjectData(IEnumerable<IWorkbenchItem> items = null)
        {
            var workbenchItemRepository = ServiceManager.Instance.GetService<IWorkbenchItemRepository>();
            workbenchItemRepository.AddRange(CreateItems(items));
            var projectData = MockRepository.GenerateStub<IProjectData>();
            projectData.Expect(pd => pd.WorkbenchItems).Return(workbenchItemRepository).Repeat.Any();

            return projectData;
        }

        /// <summary>
        /// Creates the items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        private static IEnumerable<IWorkbenchItem> CreateItems(IEnumerable<IWorkbenchItem> items = null)
        {
            return items ?? new List<IWorkbenchItem>
                {
                    CreateDummyWorkbenchItem("Type 1"),
                    CreateDummyWorkbenchItem("Type 1"),
                    CreateDummyWorkbenchItem("Type 2"),
                    CreateDummyWorkbenchItem("Type 2")
                };
        }

        /// <summary>
        /// Creates the dummy workbench item.
        /// </summary>
        /// <param name="workItemTypeName">Name of the work item type.</param>
        /// <returns>A dummy workbench item.</returns>
        private static IWorkbenchItem CreateDummyWorkbenchItem(string workItemTypeName)
        {
            var item = MockRepository.GenerateMock<IWorkbenchItem>();

            item.Expect(w => w[Settings.Default.TypeFieldName]).Return(workItemTypeName).Repeat.Any();

            return item;
        }

        /// <summary>
        /// Generates the dependency services.
        /// </summary>
        private void GenerateDependencyServices()
        {
            var projectDataService = MockRepository.GenerateMock<IProjectDataService>();

            projectDataService
                .Expect(pds => pds.CurrentProjectData)
                .WhenCalled(mi => mi.ReturnValue = this.projectData)
                .Return(null)
                .Repeat.Any();

            ServiceManagerHelper.MockServiceManager(projectDataService);
            ServiceManagerHelper.RegisterServiceInstance((ILinkManagerService)new LinkManagerService());
            ServiceManagerHelper.RegisterServiceInstance((IFilterService)new FilterService());
            ServiceManagerHelper.RegisterServiceInstance((IWorkbenchItemRepository)new WorkbenchItemRepository());
        }
    }
}

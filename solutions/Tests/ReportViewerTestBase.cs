// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportViewerTestsBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ReportViewerTestsBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using Rhino.Mocks;

    using TfsWorkbench.ReportViewer;

    /// <summary>
    /// The report viewer test fixture base class.
    /// </summary>
    public class ReportViewerTestBase
    {
        /// <summary>
        /// Generates the catalog item.
        /// </summary>
        /// <param name="name">The item name.</param>
        /// <param name="path">The item path.</param>
        /// <param name="isReport">If set to <c>true</c> [is report].</param>
        /// <param name="isFolder">If set to <c>true</c> [is folder].</param>
        /// <param name="isHidden">If set to <c>true</c> [is hidden].</param>
        /// <returns>A new instance of the catalog item base.</returns>
        protected CatalogItemBase GenerateCatalogItem(string name = "Report 01", string path = "Folder/Path/Report 01", bool isReport = true, bool isFolder = false, bool isHidden = false)
        {
            var item = MockRepository.GenerateMock<CatalogItemBase>();
            item.Expect(ci => ci.IsFolder).Return(isFolder);
            item.Expect(ci => ci.IsReport).Return(isReport);
            item.Hidden = isHidden;
            item.Path = path;
            item.Name = name;

            return item;
        }
    }
}
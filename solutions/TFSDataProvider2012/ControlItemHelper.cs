// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlItemHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initializes instance of ControlItemHelper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.Core.Interfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsWorkbench.Core.DataObjects;
using TfsWorkbench.TFSDataProvider2012.Helpers;
using TfsWorkbench.TFSDataProvider2012.Properties;

namespace TfsWorkbench.TFSDataProvider2012
{
    /// <summary>
    /// Initializes instance of ControlItemHelper
    /// </summary>
    internal class ControlItemHelper
    {
        /// <summary>
        /// The control item collections map.
        /// </summary>
        private static readonly Dictionary<string, ControlItemGroup> controlItemMap = new Dictionary<string, ControlItemGroup>();

        /// <summary>
        /// The internal xsl transform instance.
        /// </summary>
        private static XslCompiledTransform internalXslTransform;

        /// <summary>
        /// The serializer intance.
        /// </summary>
        private static SerializerInstance<ControlItemGroup> serializerIntance;

        /// <summary>
        /// Gets the XSL transform.
        /// </summary>
        /// <value>The XSL transform.</value>
        private static XslCompiledTransform XslTransform
        {
            get
            {
                if (internalXslTransform == null)
                {
                    internalXslTransform = new XslCompiledTransform();

                    var assembly = Assembly.GetExecutingAssembly();

                    var assemblyName = assembly.GetName().Name;

                    var streamName = string.Concat(assemblyName, ".Resources.WitdToControlItem.xslt");
                    var stream = assembly.GetManifestResourceStream(streamName);

                    if (stream == null)
                    {
                        throw new FileNotFoundException(string.Concat(Resources.String008, streamName));
                    }

                    internalXslTransform.Load(new XmlTextReader(stream));
                }

                return internalXslTransform;
            }
        }

        /// <summary>
        /// Gets the serializer instance.
        /// </summary>
        /// <value>The serializer instance.</value>
        private static SerializerInstance<ControlItemGroup> SerializerInstance
        {
            get
            {
                return serializerIntance = serializerIntance ?? new SerializerInstance<ControlItemGroup>();
            }
        }

        /// <summary>
        /// Gets the control item colelction.
        /// </summary>
        /// <param name="workbenchItem">The task board item.</param>
        /// <returns>A collection of control item objects.</returns>
        public static ControlItemGroup GetControlItemGroup(IWorkbenchItem workbenchItem)
        {
            if (workbenchItem == null)
            {
                throw new ArgumentNullException("workbenchItem");
            }

            ControlItemGroup collection;

            var valueProvider = workbenchItem.ValueProvider as WorkItemValueProvider;
            if (valueProvider == null)
            {
                throw new ArgumentException(Resources.String013);
            }

            var compoundKey = GenerateCompondKey(valueProvider.WorkItem.Project, valueProvider.WorkItem.Type.Name);

            if (!controlItemMap.TryGetValue(compoundKey, out collection))
            {
                collection = CreateCollection(valueProvider.WorkItem.Type.Export(false));

                controlItemMap.Add(compoundKey, collection);
            }

            // Clone the collection in order to allow multiple instances.
            collection = collection.Clone();
            collection.WorkbenchItem = workbenchItem;

            return collection;
        }

        /// <summary>
        /// Gets the control item collection.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>A control item collection without an associated work item reference.</returns>
        public static IControlItemGroup GetControlItemGroup(Project project, string typeName)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            ControlItemGroup collection;

            var compoundKey = GenerateCompondKey(project, typeName);

            if (!controlItemMap.TryGetValue(compoundKey, out collection))
            {
                var workItemType =
                    project.WorkItemTypes.OfType<WorkItemType>().FirstOrDefault(w => w.Name.Equals(typeName));

                if (workItemType == null)
                {
                    return null;
                }

                collection = CreateCollection(workItemType.Export(false));

                controlItemMap.Add(compoundKey, collection);
            }

            // Clone the collection in order to allow multiple instances.
            return collection.Clone();
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public static void ClearCache()
        {
            foreach (var value in controlItemMap.Values)
            {
                value.WorkbenchItem = null;
            }

            controlItemMap.Clear();
        }

        /// <summary>
        /// Creates the collection.
        /// </summary>
        /// <param name="witd">The work item template document.</param>
        /// <returns>A control item collection instance.</returns>
        private static ControlItemGroup CreateCollection(IXPathNavigable witd)
        {
            var sb = new StringBuilder();

            using (var writer = XmlWriter.Create(sb, XslTransform.OutputSettings))
            {
                var navigator = witd.CreateNavigator();

                if (navigator == null)
                {
                    throw new ArgumentException(Resources.String014);
                }

                using (var reader = navigator.ReadSubtree())
                {
                    reader.MoveToContent();
                    XslTransform.Transform(reader, writer);
                    reader.Close();
                }

                writer.Close();
            }

            return SerializerInstance.Deserialize(sb.ToString());
        }

        /// <summary>
        /// Generates the compond key.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>The compond key for the specfied arguments.</returns>
        private static string GenerateCompondKey(Project project, string typeName)
        {
            return project == null 
                ? null 
                : string.Concat(project.Uri, " - ", typeName);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlItemHelper.cs" company="EMC Consulting">
//   EMC Consulting 2009
// </copyright>
// <summary>
//   Initializes instance of ControlItemHelper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.TeamSystem.TaskBoard.TFSDataProvider.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    using Core.DataObjects;
    using Core.Helpers;
    using Core.Interfaces;

    /// <summary>
    /// Initializes instance of ControlItemHelper
    /// </summary>
    internal class ControlItemHelper
    {
        /// <summary>
        /// The control item collections map.
        /// </summary>
        private static readonly Dictionary<string, ControlItemCollection> controlItemMap = new Dictionary<string, ControlItemCollection>();

        /// <summary>
        /// The internal xsl transform instance.
        /// </summary>
        private static XslCompiledTransform internalXslTransform;

        /// <summary>
        /// The serializer intance.
        /// </summary>
        private static SerializerInstance<ControlItemCollection> serializerIntance;

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
                        throw new FileNotFoundException(string.Concat("Unable to load the xslt resource file: ", streamName));
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
        private static SerializerInstance<ControlItemCollection> SerializerInstance
        {
            get
            {
                return serializerIntance = serializerIntance ?? new SerializerInstance<ControlItemCollection>();
            }
        }

        /// <summary>
        /// Gets the control item colelction.
        /// </summary>
        /// <param name="taskBoardItem">The task board item.</param>
        /// <returns>A collection of control item objects.</returns>
        public static ControlItemCollection GetControlItemCollection(ITaskBoardItem taskBoardItem)
        {
            ControlItemCollection collection;

            var valueProvider = taskBoardItem.ValueProvider as WorkItemValueProvider;
            if (valueProvider == null)
            {
                throw new ArgumentException("The specfied value provider is not of the expected type.");
            }

            var workItemTypeName = valueProvider.WorkItem.Type.Name;

            if (!controlItemMap.TryGetValue(workItemTypeName, out collection))
            {
                collection = CreateCollection(valueProvider.WorkItem.Type.Export(false));

                controlItemMap.Add(workItemTypeName, collection);
            }

            collection.TaskBoardItem = taskBoardItem;

            return collection;
        }

        /// <summary>
        /// Creates the collection.
        /// </summary>
        /// <param name="witd">The work item template document.</param>
        /// <returns>A control item collection instance.</returns>
        private static ControlItemCollection CreateCollection(IXPathNavigable witd)
        {
            var sb = new StringBuilder();

            using (var writer = XmlWriter.Create(sb, XslTransform.OutputSettings))
            {
                var navigator = witd.CreateNavigator();

                if (writer == null || navigator == null)
                {
                    throw new ArgumentException("Witd xml is not valid");
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
    }
}
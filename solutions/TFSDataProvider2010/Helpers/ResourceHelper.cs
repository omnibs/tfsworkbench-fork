// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ResourceHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using TfsWorkbench.TFSDataProvider2010.Properties;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace TfsWorkbench.TFSDataProvider2010.Helpers
{
    /// <summary>
    /// Initializes instance of ResourceHelper
    /// </summary>
    internal static class ResourceHelper
    {
        /// <summary>
        /// The internal transform map.
        /// </summary>
        private static readonly IDictionary<string, XslCompiledTransform> transformMaps = new Dictionary<string, XslCompiledTransform>();

        /// <summary>
        /// The projet match collection.
        /// </summary>
        private static ProjectMatchCollection projectMatchCollection;

        /// <summary>
        /// Gets the transformer.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>An instance of the specified xslt transformer.</returns>
        public static XslCompiledTransform GetTransform(string resourceName)
        {
            XslCompiledTransform transform;
            if (!transformMaps.TryGetValue(resourceName, out transform))
            {
                transform = new XslCompiledTransform();

                var assembly = Assembly.GetExecutingAssembly();

                var assemblyName = assembly.GetName().Name;

                var streamName = string.Concat(assemblyName, ".Resources.", resourceName);
                var stream = assembly.GetManifestResourceStream(streamName);

                if (stream == null)
                {
                    throw new FileNotFoundException(string.Concat(Resources.String008, streamName));
                }

                transform.Load(new XmlTextReader(stream));

                transformMaps.Add(resourceName, transform);
            }

            return transform;
        }

        /// <summary>
        /// Gets the project matchers.
        /// </summary>
        /// <returns>The project match collection instance.</returns>
        public static ProjectMatchCollection GetProjectMatcher()
        {
            if (projectMatchCollection == null)
            {
                var assembly = Assembly.GetExecutingAssembly();

                var assemblyName = assembly.GetName().Name;

                var streamName = string.Concat(assemblyName, ".Resources.ProjectMatchCollection.xml");
                var stream = assembly.GetManifestResourceStream(streamName);

                if (stream == null)
                {
                    throw new FileNotFoundException(string.Concat(Resources.String009, streamName));
                }

                var serialiser = new Core.Helpers.SerializerInstance<ProjectMatchCollection>();

                using (var sr = new StreamReader(stream))
                {
                    projectMatchCollection = serialiser.Deserialize(sr.ReadToEnd());

                    sr.Close();
                }
            }

            return projectMatchCollection;
        }
    }
}

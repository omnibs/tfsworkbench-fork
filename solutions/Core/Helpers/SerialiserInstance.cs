// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialiserInstance.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the SerializerInstance type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the SerializerInstance&lt;T&gt; type.
    /// </summary>
    /// <typeparam name="T">The serializable type.</typeparam>
    public class SerializerInstance<T>
    {
        /// <summary>
        /// The serializer field.
        /// </summary>
        private XmlSerializer serializer;

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        /// <value>The serializer.</value>
        public XmlSerializer Serializer
        {
            get
            {
                return this.serializer = this.serializer ?? new XmlSerializer(typeof(T));
            }
        }

        /// <summary>
        /// Deserilizes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The deserilized insrtance of the object.</returns>
        public T Deserialize(string source)
        {
            T output;

            using (var sr = new StringReader(source))
            {
                output = (T)this.Serializer.Deserialize(sr);
            }

            return output;
        }

        /// <summary>
        /// Serializes the specified source.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <returns>The object as a serialized string.</returns>
        public string Serialize(T source)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                this.Serializer.Serialize(sw, source);
                sw.Close();
            }

            return sb.ToString().Replace("encoding=\"utf-16\"", "encoding=\"utf-8\"");
        }
    }
}
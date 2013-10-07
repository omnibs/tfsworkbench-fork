// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlValidationHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the XmlValidationHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// Initializes instance of XmlValidationHelper
    /// </summary>
    public static class XmlValidationHelper
    {
        /// <summary>
        /// Validates the source against the specified schema.
        /// </summary>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="schemaStream">The schema stream.</param>
        /// <exception cref="XmlSchemaValidationException"></exception>
        public static void ValidateSourceStream(Stream sourceStream, Stream schemaStream)
        {
            if (sourceStream == null)
            {
                throw new ArgumentNullException("sourceStream");
            }

            if (schemaStream == null)
            {
                throw new ArgumentNullException("schemaStream");
            }

            var failures = new List<string>();

            var readerSettings = new XmlReaderSettings();

            readerSettings.Schemas.Add(null, XmlReader.Create(schemaStream));
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.ValidationEventHandler +=
                (sender, e) =>
                    {
                        var message = string.Empty;

                        if (e.Severity == XmlSeverityType.Warning)
                        {
                            message = string.Concat("Warning: ", e.Message);
                        }

                        if (e.Severity == XmlSeverityType.Error)
                        {
                            message = string.Concat("Error: ", e.Message);
                        }

                        failures.Add(
                            string.Concat(
                                message, " - Line: ", e.Exception.LineNumber, " Position: ", e.Exception.LinePosition));
                    };

            using (var reader = XmlReader.Create(sourceStream, readerSettings))
            {
                while (reader.Read())
                {
                }
            }

            if (failures.Count() != 0)
            {
                var concatFailures = string.Concat(failures.Select(s => string.Concat(s, Environment.NewLine, Environment.NewLine)).ToArray());

                throw new XmlSchemaValidationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Schema validation failed:\r\n{0}",
                        concatFailures));
            }

            sourceStream.Position = 0;
        }
    }
}
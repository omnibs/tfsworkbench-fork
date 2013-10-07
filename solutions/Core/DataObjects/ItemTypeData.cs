// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemTypeData.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkbenchItemTypeData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Initializes instance of WorkbenchItemTypeData
    /// </summary>
    public class ItemTypeData 
    {
        /// <summary>
        /// The fields collection.
        /// </summary>
        private readonly Collection<FieldTypeData> fields = new Collection<FieldTypeData>();

        /// <summary>
        /// The states collection.
        /// </summary>
        private readonly Collection<string> states = new Collection<string>();

        /// <summary>
        /// The context fields collection.
        /// </summary>
        private readonly Collection<string> contextFields = new Collection<string>();

        /// <summary>
        /// The additional fields to copy when duplicating items of this type.
        /// </summary>
        private readonly Collection<string> duplicationFields = new Collection<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTypeData"/> class.
        /// </summary>
        public ItemTypeData() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTypeData"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        public ItemTypeData(string typeName)
        {
            this.TypeName = typeName;
        }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        [XmlAttribute(AttributeName = "type")]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the caption field.
        /// </summary>
        /// <value>The caption field.</value>
        [XmlAttribute(AttributeName = "caption")]
        public string CaptionField { get; set; }

        /// <summary>
        /// Gets or sets the body field.
        /// </summary>
        /// <value>The body field.</value>
        [XmlAttribute(AttributeName = "body")]
        public string BodyField { get; set; }

        /// <summary>
        /// Gets or sets the numeric field.
        /// </summary>
        /// <value>The numeric field.</value>
        [XmlAttribute(AttributeName = "numeric")]
        public string NumericField { get; set; }

        /// <summary>
        /// Gets or sets the owner field.
        /// </summary>
        /// <value>The owner field.</value>
        [XmlAttribute(AttributeName = "owner")]
        public string OwnerField { get; set; }

        /// <summary>
        /// Gets the context fields.
        /// </summary>
        /// <value>The context fields.</value>
        [XmlArrayItem(ElementName = "Field")]
        public Collection<string> ContextFields
        {
            get { return this.contextFields; }
        }

        /// <summary>
        /// Gets the duplication fields.
        /// </summary>
        /// <value>The duplication fields.</value>
        [XmlArrayItem(ElementName = "Field")]
        public Collection<string> DuplicationFields
        {
            get { return this.duplicationFields; }
        }

        [XmlAttribute(AttributeName = "colour")]
        public string DefaultColour { get; set; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>The fields.</value>
        [XmlIgnore]
        public Collection<FieldTypeData> Fields
        {
            get
            {
                return this.fields;
            }
        }

        /// <summary>
        /// Gets the states.
        /// </summary>
        /// <value>The states.</value>
        [XmlIgnore]
        public Collection<string> States
        {
            get
            {
                return this.states;
            }
        }
    }
}

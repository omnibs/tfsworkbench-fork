// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchFilter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Filter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.FilterService.Converters;
    using TfsWorkbench.FilterService.Properties;

    /// <summary>
    /// The filter instance.
    /// </summary>
    [XmlRoot(ElementName = "Filter", Namespace = "http://schemas.workbench/Filter")]
    public class WorkbenchFilter : NotifierBase, ICloneable
    {
        /// <summary>
        /// The item type property name.
        /// </summary>
        public const string ItemTypeNamePropertyName = "ItemTypeName";

        /// <summary>
        /// The field name property name.
        /// </summary>
        public const string FieldNamePropertyName = "FieldName";

        /// <summary>
        /// The operator to string converter intance.
        /// </summary>
        private static OperatorToStringConverter operatorToStringConverter = new OperatorToStringConverter();

        /// <summary>
        /// The filter action.
        /// </summary>
        private FilterActionOption filterAction;

        /// <summary>
        /// The field name.
        /// </summary>
        private string fieldName;

        /// <summary>
        /// The filter operator.
        /// </summary>
        private FilterOperatorOption filterOperator;

        /// <summary>
        /// The value.
        /// </summary>
        private string value;

        /// <summary>
        /// The item type name.
        /// </summary>
        private string itemTypeName;

        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchFilter"/> class.
        /// </summary>
        public WorkbenchFilter()
            : this(FilterActionOption.Include)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchFilter"/> class.
        /// </summary>
        /// <param name="filterAction">The filter action.</param>
        public WorkbenchFilter(FilterActionOption filterAction)
        {
            this.FilterAction = filterAction;
            this.ItemTypeName = Resources.String002;
        }

        /// <summary>
        /// Gets or sets the filter action.
        /// </summary>
        /// <value>The filter action.</value>
        [XmlAttribute(AttributeName = "action")]
        public FilterActionOption FilterAction
        {
            get
            {
                return this.filterAction;
            }

            set
            {
                this.UpdateWithNotification("FilterAction", value, ref this.filterAction);
                this.UpdateDescription();
            }
        }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        /// <value>The type of the item.</value>
        [XmlAttribute(AttributeName = "typename")]
        public string ItemTypeName
        {
            get
            {
                return this.itemTypeName;
            }

            set
            {
                this.UpdateWithNotification(ItemTypeNamePropertyName, value, ref this.itemTypeName);
                this.UpdateDescription();
            }
        }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        [XmlAttribute(AttributeName = "fieldname")]
        public string FieldName
        {
            get
            {
                return this.fieldName;
            }

            set
            {
                this.UpdateWithNotification(FieldNamePropertyName, value, ref this.fieldName);
                this.UpdateDescription();
            }
        }

        /// <summary>
        /// Gets or sets the filter operator.
        /// </summary>
        /// <value>The filter operator.</value>
        [XmlAttribute(AttributeName = "operator")]
        public FilterOperatorOption FilterOperator
        {
            get
            {
                return this.filterOperator;
            }

            set
            {
                this.UpdateWithNotification("FilterOperator", value, ref this.filterOperator);
                this.UpdateDescription();
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.UpdateWithNotification("Value", value, ref this.value);
                this.UpdateDescription();
            }
        }

        /// <summary>
        /// Gets the descriptipn.
        /// </summary>
        /// <value>The descriptipn.</value>
        [XmlIgnore]
        public string Description
        {
            get
            {
                return this.description;
            }

            private set
            {
                this.UpdateWithNotification("Description", value, ref this.description);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid filter.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is valid filter; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidFilter
        {
            get
            {
                return !string.IsNullOrEmpty(this.ItemTypeName) && !string.IsNullOrEmpty(this.FieldName);
            }
        }

        /// <summary>
        /// Match items of the specfied type.
        /// </summary>
        /// <param name="workbenchItemTypeName">Name of the workbench item type.</param>
        /// <returns>The filter instance including the specfied item type name.</returns>
        public WorkbenchFilter ItemsOfType(string workbenchItemTypeName)
        {
            this.ItemTypeName = workbenchItemTypeName;

            return this;
        }

        /// <summary>
        /// Matches the specfied field name.
        /// </summary>
        /// <param name="targetFieldName">Name of the field.</param>
        /// <returns>The the filter instance including the specfied field name.</returns>
        public WorkbenchFilter WithField(string targetFieldName)
        {
            this.FieldName = targetFieldName;

            return this;
        }

        /// <summary>
        /// Add the specified filter operator.
        /// </summary>
        /// <param name="operator">The filter operator.</param>
        /// <param name="comparisonValue">The comparison value.</param>
        /// <returns>
        /// The filter instance including the specified opertator.
        /// </returns>
        public WorkbenchFilter That(FilterOperatorOption @operator, object comparisonValue)
        {
            this.FilterOperator = @operator;
            this.Value = comparisonValue.ToString();

            return this;
        }

        /// <summary>
        /// Determines whether the specified workbench item is match.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is match; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public bool IsMatch(IWorkbenchItem workbenchItem)
        {
            return FilterMatchHelper.IsMatch(this, workbenchItem);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Description;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return
                new WorkbenchFilter(this.FilterAction)
                    .ItemsOfType(this.ItemTypeName)
                    .WithField(this.FieldName)
                    .That(this.FilterOperator, this.Value);
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            var operatorAsString = (string)operatorToStringConverter.Convert(this.FilterOperator, typeof(string), null, CultureInfo.CurrentCulture);

            this.Description = string.Format(
                CultureInfo.InvariantCulture,
                Resources.String001,
                this.FilterAction == FilterActionOption.Include ? Resources.String014 : Resources.String015,
                this.ItemTypeName,
                this.FieldName,
                operatorAsString,
                this.Value ?? "Null");
        }
    }
}

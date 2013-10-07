// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterMatchHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the FilterMatchHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System;
    using System.Collections;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.FilterService.Properties;

    /// <summary>
    /// The filter operator action option.
    /// </summary>
    public enum FilterOperatorOption
    {
        /// <summary>
        /// The value equals.
        /// </summary>
        IsEqualTo = 0,

        /// <summary>
        /// The value does not equal.
        /// </summary>
        IsNotEqualTo = 1,

        /// <summary>
        /// The value is greater than.
        /// </summary>
        IsGreaterThan = 2,

        /// <summary>
        /// The value is less than.
        /// </summary>
        IsLessThan = 3,

        /// <summary>
        /// The value is greater than equal to.
        /// </summary>
        IsGreaterThanEqualTo = 4,

        /// <summary>
        /// The value is less than equal to.
        /// </summary>
        IsLessThanEqualTo = 5,

        /// <summary>
        /// The value starts with.
        /// </summary>
        StartsWith = 6,

        /// <summary>
        /// The value ends with.
        /// </summary>
        EndsWith = 7,

        /// <summary>
        /// The value contains.
        /// </summary>
        Contains = 8,

        /// <summary>
        /// The value does not start with.
        /// </summary>
        DoesNotStartWith = 9,

        /// <summary>
        /// The value does not end with.
        /// </summary>
        DoesNotEndWith = 10,

        /// <summary>
        /// The value does not contain.
        /// </summary>
        DoesNotContain = 11,
    }

    /// <summary>
    /// The filter action option enum.
    /// </summary>
    public enum FilterActionOption
    {
        /// <summary>
        /// Include work items that match this filter.
        /// </summary>
        Include = 0,

        /// <summary>
        /// Exclude work items that match this fitler.
        /// </summary>
        Exclude = 1
    }

    /// <summary>
    /// The filter matcher helper class.
    /// </summary>
    internal static class FilterMatchHelper
    {
        /// <summary>
        /// Determines whether the specified filter is match.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified filter is match; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMatch(WorkbenchFilter filter, IWorkbenchItem workbenchItem)
        {
            if (workbenchItem == null)
            {
                throw new ArgumentNullException("workbenchItem");
            }

            if (!filter.IsValidFilter)
            {
                throw new ArgumentException(Resources.String016);
            }

            var hasMatched = false;

            if (filter.ItemTypeName.Equals(Resources.String002) || filter.ItemTypeName.Equals(workbenchItem.GetTypeName()))
            {
                var fieldValue = GetFieldValue(filter.FieldName, workbenchItem);

                if (fieldValue == null)
                {
                    return filter.Value == null;
                }

                bool compareAsStrings;
                object localValueAsType;
                if (!TryGetComparibleValues(filter.Value, fieldValue, out localValueAsType, out compareAsStrings))
                {
                    return false;
                }

                switch (filter.FilterOperator)
                {
                    case FilterOperatorOption.IsEqualTo:
                        hasMatched = compareAsStrings
                            ? string.Equals((string)fieldValue, filter.Value, StringComparison.OrdinalIgnoreCase)
                            : Comparer.Default.Compare(fieldValue, localValueAsType) == 0;
                        break;
                    case FilterOperatorOption.IsNotEqualTo:
                        hasMatched = compareAsStrings
                            ? !string.Equals((string)fieldValue, filter.Value, StringComparison.OrdinalIgnoreCase)
                            : Comparer.Default.Compare(fieldValue, localValueAsType) != 0;
                        break;
                    case FilterOperatorOption.IsGreaterThan:
                        hasMatched = compareAsStrings
                            ? string.Compare((string)fieldValue, filter.Value, StringComparison.OrdinalIgnoreCase) > 0
                            : Comparer.Default.Compare(fieldValue, localValueAsType) > 0;
                        break;
                    case FilterOperatorOption.IsLessThan:
                        hasMatched = compareAsStrings
                            ? string.Compare((string)fieldValue, filter.Value, StringComparison.OrdinalIgnoreCase) < 0
                            : Comparer.Default.Compare(fieldValue, localValueAsType) < 0;
                        break;
                    case FilterOperatorOption.IsGreaterThanEqualTo:
                        hasMatched = compareAsStrings
                            ? string.Compare((string)fieldValue, filter.Value, StringComparison.OrdinalIgnoreCase) >= 0
                            : Comparer.Default.Compare(fieldValue, localValueAsType) >= 0;
                        break;
                    case FilterOperatorOption.IsLessThanEqualTo:
                        hasMatched = compareAsStrings
                            ? string.Compare((string)fieldValue, filter.Value, StringComparison.OrdinalIgnoreCase) <= 0
                            : Comparer.Default.Compare(fieldValue, localValueAsType) <= 0;
                        break;
                    case FilterOperatorOption.StartsWith:
                        if (compareAsStrings)
                        {
                            hasMatched = ((string)fieldValue).StartsWith(filter.Value, StringComparison.OrdinalIgnoreCase);
                        }

                        break;
                    case FilterOperatorOption.EndsWith:
                        if (compareAsStrings)
                        {
                            hasMatched = ((string)fieldValue).EndsWith(filter.Value, StringComparison.OrdinalIgnoreCase);
                        }

                        break;
                    case FilterOperatorOption.Contains:
                        if (compareAsStrings)
                        {
                            hasMatched = ((string)fieldValue).Contains(filter.Value);
                        }

                        break;
                    case FilterOperatorOption.DoesNotStartWith:
                        if (compareAsStrings)
                        {
                            hasMatched = !((string)fieldValue).Contains(filter.Value);
                        }

                        break;
                    case FilterOperatorOption.DoesNotEndWith:
                        if (compareAsStrings)
                        {
                            hasMatched = !((string)fieldValue).EndsWith(filter.Value, StringComparison.OrdinalIgnoreCase);
                        }

                        break;
                    case FilterOperatorOption.DoesNotContain:
                        if (compareAsStrings)
                        {
                            hasMatched = !((string)fieldValue).Contains(filter.Value);
                        }

                        break;

                    default:
                        break;
                }
            }

            return hasMatched;
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The field value</returns>
        private static object GetFieldValue(string fieldName, IWorkbenchItem workbenchItem)
        {
            if (fieldName == Resources.String048)
            {
                return workbenchItem.GetCaption();
            }

            if (fieldName == Resources.String049)
            {
                return workbenchItem.GetBody();
            }

            if (fieldName == Resources.String050)
            {
                return workbenchItem.GetMetric();
            }

            if (fieldName == Resources.String051)
            {
                return workbenchItem.GetOwner();
            }

            return workbenchItem[fieldName];
        }

        /// <summary>
        /// Tries to get comparible values.
        /// </summary>
        /// <param name="localValue">The local value.</param>
        /// <param name="valueToTestAgainst">The value to test against.</param>
        /// <param name="localValueAsType">Type of the local value as.</param>
        /// <param name="compareAsStrings">if set to <c>true</c> [is string].</param>
        /// <returns><c>True</c> if a type comparion is found; otherwise <c>false</c>.</returns>
        private static bool TryGetComparibleValues(string localValue, object valueToTestAgainst, out object localValueAsType, out bool compareAsStrings)
        {
            if (valueToTestAgainst is string)
            {
                localValueAsType = localValue;
                compareAsStrings = true;

                return true;
            }

            localValueAsType = null;
            compareAsStrings = false;

            int comparisonAsInt;
            if ((valueToTestAgainst is int) && int.TryParse(localValue, out comparisonAsInt))
            {
                localValueAsType = comparisonAsInt;
                return true;
            }

            double comparisonAsDouble;
            if ((valueToTestAgainst is double) && double.TryParse(localValue, out comparisonAsDouble))
            {
                localValueAsType = comparisonAsDouble;
                return true;
            }

            DateTime comparisonAsDateTime;
            if ((valueToTestAgainst is DateTime) && DateTime.TryParse(localValue, out comparisonAsDateTime))
            {
                localValueAsType = comparisonAsDateTime;
                return true;
            }

            return false;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueProvider.cs" company="None">
//   None
// </copyright>
// <summary>
//   The i value provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The IValueProvider.
    /// </summary>
    public interface IValueProvider
    {
        /// <summary>
        /// Gets or sets the state changed action method.
        /// </summary>
        /// <value>The state changed.</value>
        Action<string, string> StateChanged { get; set; }

        /// <summary>
        /// Gets or sets the workbench item.
        /// </summary>
        /// <value>The workbench item.</value>
        IWorkbenchItem WorkbenchItem { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        bool IsDirty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        bool IsNew { get; }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>The get validation errors.</value>
        IEnumerable<string> ValidationErrors { get; }

        /// <summary>
        /// Return the specified value.
        /// </summary>
        /// <param name="valueName">The value name.</param>
        /// <returns>The value if the specified name exists; otherwise null.</returns>
        object GetValue(string valueName);

        /// <summary>
        /// The set specified value.
        /// </summary>
        /// <param name="valueName">The value name.</param>
        /// <param name="newValue">The new value.</param>
        void SetValue(string valueName, object newValue);

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>The corresponding display name.</returns>
        string GetDisplayName(string valueName);

        /// <summary>
        /// Gets the allowed values.
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>A list of allowed values if specified; otherwise null.</returns>
        IEnumerable<object> GetAllowedValues(string valueName);

        /// <summary>
        /// Saves this instance or returns a list of validation errors.
        /// </summary>
        /// <returns>A list of validation errors if validation fails; otherwise null.</returns>
        IEnumerable<string> Save();

        /// <summary>
        /// Syncs to latest work item revision.
        /// </summary>
        void SyncToLatest();

        /// <summary>
        /// Determines whether [the specified value name] [is limited to allowed values].
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>
        /// <c>true</c> if [the specified value name] [is limited to allowed values]; otherwise, <c>false</c>.
        /// </returns>
        bool IsLimitedToAllowedValues(string valueName);

        /// <summary>
        /// Determines whether [the specified value name] [has allowed values].
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>
        /// <c>true</c> if [the specified value name] [has allowed values]; otherwise, <c>false</c>.
        /// </returns>
        bool HasAllowedValues(string valueName);
    }
}
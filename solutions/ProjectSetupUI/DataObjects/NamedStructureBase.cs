// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedStructureBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the NamedStructureBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.DataObjects
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Core.DataObjects;

    /// <summary>
    /// Initialises and instance of TfsWorkbench.ProjectSetupUI.DataObjects.NamedStructureBase
    /// </summary>
    internal abstract class NamedStructureBase : NotifierBase, INamedItem, ICloneable
    {
        /// <summary>
        /// The name field.
        /// </summary>
        private string name;

        /// <summary>
        /// The iteration path field.
        /// </summary>
        private string iterationPath;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The release name.</value>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.UpdateWithNotification("Name", value, ref this.name);
            }
        }

        /// <summary>
        /// Gets or sets the iteration path.
        /// </summary>
        /// <value>The iteration path.</value>
        public string IterationPath
        {
            get
            {
                return this.iterationPath;
            }

            set
            {
                this.UpdateWithNotification("IterationPath", value, ref this.iterationPath);
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var contextType = this.GetType();
            var output = Activator.CreateInstance(contextType);

            foreach (var propertyInfo in contextType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.CanRead && pi.CanWrite))
            {
                propertyInfo.SetValue(output, propertyInfo.GetValue(this, null), null);
            }

            return output;
        }
    }
}
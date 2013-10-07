// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewMap.cs" company="None">
//   None
// </copyright>
// <summary>
//   The parent to child relationship config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;

    /// <summary>
    /// The ViewMap data.
    /// </summary>
    public class ViewMap : NotifierBase
    {
        /// <summary>
        /// The states that fall into the backet category.
        /// </summary>
        private readonly Collection<string> bucketStates = new Collection<string>();

        /// <summary>
        /// The states that will be used in the swim lane.
        /// </summary>
        private readonly Collection<string> swimLaneStates = new Collection<string>();

        /// <summary>
        /// The state item colours.
        /// </summary>
        private readonly Collection<StateColour> stateItemColours = new Collection<StateColour>();

        /// <summary>
        /// The parent types collection.
        /// </summary>
        private readonly ObservableCollection<string> parentTypes = new ObservableCollection<string>();

        /// <summary>
        /// The title field
        /// </summary>
        private string title;

        /// <summary>
        /// The description field.
        /// </summary>
        private string description;

        /// <summary>
        /// The child type field.
        /// </summary>
        private string childType;

        /// <summary>
        /// The link name field.
        /// </summary>
        private string linkName;

        /// <summary>
        /// The display order.
        /// </summary>
        private int displayOrder;

        /// <summary>
        /// The swim lane view flag.
        /// </summary>
        private bool isNotSwimLane;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMap"/> class.
        /// </summary>
        public ViewMap()
        {
            this.ParentSorter = new RowSorter { FieldName = Settings.Default.IdFieldName, Direction = SortDirection.Ascending };
            this.ChildSorter = new ItemSorter { FieldName = Settings.Default.IdFieldName, Direction = SortDirection.Ascending };
        }

        /// <summary>
        /// Occurs when [layout updated].
        /// </summary>
        public event EventHandler<ViewMapEventArgs> LayoutUpdated;

        /// <summary>
        /// Gets or sets the view map title.
        /// </summary>
        /// <value>The view map title.</value>
        [XmlAttribute(AttributeName = "title")]
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.UpdateWithNotification("Title", value, ref this.title);
            }
        }

        /// <summary>
        /// Gets or sets the display order.
        /// </summary>
        /// <value>The display order.</value>
        [XmlAttribute(AttributeName = "position")]
        public int DisplayOrder
        {
            get
            {
                return this.displayOrder;
            }

            set
            {
                this.UpdateWithNotification("DisplayOrder", value, ref this.displayOrder);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is swim lane view.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is swim lane view; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute(AttributeName = "notswimlane")]
        public bool IsNotSwimLane
        {
            get
            {
                return this.isNotSwimLane;
            }

            set
            {
                this.UpdateWithNotification("IsNotSwimLane", value, ref this.isNotSwimLane);
            }
        }

        /// <summary>
        /// Gets or sets the view map Description.
        /// </summary>
        /// <value>The view map description.</value>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.UpdateWithNotification("Description", value, ref this.description);
            }
        }

        /// <summary>
        /// Gets the type of the parent.
        /// </summary>
        /// <value>The type of the parent.</value>
        [XmlArrayItem(ElementName = "type")]
        public ObservableCollection<string> ParentTypes
        {
            get
            {
                return this.parentTypes;
            }
        }

        /// <summary>
        /// Gets or sets the type of the child.
        /// </summary>
        /// <value>The type of the child.</value>
        [XmlAttribute(AttributeName = "child")]
        public string ChildType
        {
            get
            {
                return this.childType;
            }

            set
            {
                this.UpdateWithNotification("ChildType", value, ref this.childType);
            }
        }

        /// <summary>
        /// Gets or sets the name of the link.
        /// </summary>
        /// <value>The name of the link.</value>
        [XmlAttribute(AttributeName = "link")]
        public string LinkName
        {
            get
            {
                return this.linkName;
            }

            set
            {
                this.UpdateWithNotification("LinkName", value, ref this.linkName);
            }
        }

        /// <summary>
        /// Gets or sets the parent sorter.
        /// </summary>
        /// <value>The parent sorter.</value>
        [XmlElement(ElementName = "ParentSort")]
        public RowSorter ParentSorter { get; set; }

        /// <summary>
        /// Gets or sets the child sorter.
        /// </summary>
        /// <value>The child sorter.</value>
        [XmlElement(ElementName = "ChildSort")]
        public ItemSorter ChildSorter { get; set; }

        /// <summary>
        /// Gets the swim lane states.
        /// </summary>
        /// <value>The swim lane states.</value>
        [XmlArrayItem(ElementName = "state")]
        public Collection<string> SwimLaneStates
        {
            get
            {
                return this.swimLaneStates;
            }
        }

        /// <summary>
        /// Gets the bucket states.
        /// </summary>
        /// <value>The bucket states.</value>
        [XmlArrayItem(ElementName = "state")]
        public Collection<string> BucketStates
        {
            get
            {
                return this.bucketStates;
            }
        }

        /// <summary>
        /// Gets the state item colours.
        /// </summary>
        /// <value>The state item colours.</value>
        [XmlArrayItem(ElementName = "state")]
        public Collection<StateColour> StateItemColours
        {
            get
            {
                return this.stateItemColours;
            }
        }

        /// <summary>
        /// Called when [layout updated].
        /// </summary>
        public void OnLayoutUpdated()
        {
            if (this.LayoutUpdated == null)
            {
                return;
            }

            this.LayoutUpdated(this, new ViewMapEventArgs(this));
        }

        /// <summary>
        /// Determines whether [the specified link item] [is view link].
        /// </summary>
        /// <param name="linkItem">The link item.</param>
        /// <returns>
        /// <c>true</c> if [the specified link item] [is view link]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsViewLink(ILinkItem linkItem)
        {
            if (linkItem == null || linkItem.Child == null || linkItem.Parent == null)
            {
                return false;
            }

            return this.ParentTypes.Contains(linkItem.Parent.GetTypeName())
                && this.ChildType.Equals(linkItem.Child.GetTypeName())
                && this.LinkName.Equals(linkItem.LinkName);
        }
    }
}
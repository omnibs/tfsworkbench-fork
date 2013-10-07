<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns="http://schemas.workbench/ProjectData">
  <xsl:output method="xml" indent="yes" encoding="utf-8"/>

  <xsl:template match="/">
    <ProjectData>
      <xsl:apply-templates select="//WORKITEMTYPE" />
    </ProjectData>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Product Backlog Item']">
    <ViewMaps>
      <ViewMap
        title="Product Backlog - Tasks"
        child="Sprint Backlog Task"
        link="Scrum.ImplementedBy"
        position="0">
        <Description>The product backlog with implementing tasks:</Description>
        <ParentTypes>
          <type>Product Backlog Item</type>
        </ParentTypes>
        <ParentSort field="Delivery Order (Scrum v3)" direction="Ascending" />
        <ChildSort field="Scrum.v3.TaskPriority" direction="Ascending" />
        <SwimLaneStates>
          <state>Not Started</state>
          <state>In Progress</state>
          <state>Done</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Deleted</state>
          <state>Descoped</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FF008000">Done</state>
          <state colour="#FFFFFF00">Not Started</state>
          <state colour="#FFFFA500">In Progress</state>
          <state colour="#FFDCDCDC">Deleted</state>
          <state colour="#FF800080">Descoped</state>
        </StateItemColours>
      </ViewMap>

      <ViewMap
        title="Product Backlog - Acceptance Tests"
        child="Acceptance Test"
        link="Microsoft.VSTS.Common.TestedBy"
        position="1">
        <Description>The product backlog with related acceptance tests:</Description>
        <ParentTypes>
          <type>Product Backlog Item</type>
        </ParentTypes>
        <ParentSort field="Delivery Order (Scrum v3)" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Not Implemented</state>
          <state>Ready for Test</state>
          <state>Failed</state>
          <state>Passed</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Deleted</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FFDCDCDC">Deleted</state>
          <state colour="#FFFFFF00">Not Implemented</state>
          <state colour="#FFFFA500">Ready for Test</state>
          <state colour="#FFFF0000">Failed</state>
          <state colour="#FF008000">Passed</state>
        </StateItemColours>
      </ViewMap>

      <ViewMap
        title="Product Backlog - Impediments"
        child="Impediment"
        link="Scrum.ImpededBy"
        position="2">
        <Description>The product backlog with related impediments:</Description>
        <ParentTypes>
          <type>Product Backlog Item</type>
        </ParentTypes>
        <ParentSort field="Delivery Order (Scrum v3)" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Open</state>
          <state>Resolved</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Deleted</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FFDCDCDC">Deleted</state>
          <state colour="#FF008000">Resolved</state>
          <state colour="#FFFF0000">Open</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData
        type="Product Backlog Item"
        caption="System.Title"
        body="System.Description"
        numeric="Microsoft.VSTS.Scheduling.StoryPoints"
        owner="">
        <ContextFields>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Scheduling.StoryPoints</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Scrum.v3.WorkRemaining</Field>
          <Field>Microsoft.VSTS.Scheduling.StoryPoints</Field>
          <Field>Scrum.v3.ReturnOnInvestment</Field>
          <Field>Scrum.v3.BusinessValue</Field>
          <Field>Scrum.v3.DeliveryOrder</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Sprint Backlog Task']">
    <ItemTypes>
      <ItemTypeData type="Sprint Backlog Task" body="System.Description" caption="System.Title" numeric="Scrum.v3.WorkRemaining" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Scrum.v3.WorkRemaining</Field>
          <Field>Scrum.v3.TaskPriority</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Scrum.v3.EstimatedEffort</Field>
          <Field>Scrum.v3.WorkRemaining</Field>
          <Field>Scrum.v3.TaskPriority</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Sprint Retrospective']">
    <ItemTypes>
      <ItemTypeData type="Sprint Retrospective" body="System.Description" caption="System.IterationPath" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Impediment']">
    <ItemTypes>
      <ItemTypeData type="Impediment" body="System.Description" caption="System.Title" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Bug']">
    <ItemTypes>
      <ItemTypeData type="Bug" body="System.Description" caption="System.Title" numeric="Scrum.v3.TestingImpact" owner="">
        <ContextFields>
          <Field>System.State</Field>
          <Field>Scrum.v3.TestingImpact</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.TCM.SystemInfo</Field>
          <Field>Microsoft.VSTS.TCM.ReproSteps</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Scrum.v3.TestingImpact</Field>
          <Field>Scrum.v3.ReplicationActionDetail</Field>
          <Field>Scrum.v3.BugOrigin</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>
  
  <xsl:template match="WORKITEMTYPE[@name='Acceptance Test']">
    <ViewMaps>
      <ViewMap
        title="Acceptance Test - Bugs"
        child="Bug"
        link="Scrum.FailedBy"
        position="3">
        <Description>Acceptance tests with related bugs:</Description>
        <ParentTypes>
          <type>Acceptance Test</type>
        </ParentTypes>
        <ParentSort field="Priority" direction="Ascending" />
        <ChildSort field="Testing Impact (Scrum v3)" direction="Ascending" />
        <SwimLaneStates>
          <state>Active</state>
          <state>Ready for Test</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Deleted</state>
          <state>Ignored</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FFFF0000">Active</state>
          <state colour="#FFDCDCDC">Deleted</state>
          <state colour="#FFFFA500">Ready for Test</state>
          <state colour="#FF008000">Closed</state>
          <state colour="#FFDCDCDC">Ignored</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>
    <ItemTypes>
      <ItemTypeData type="Acceptance Test" body="System.Description" caption="System.Title" numeric="Microsoft.VSTS.Common.Priority" owner="">
        <ContextFields>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.TCM.Steps</Field>
          <Field>Microsoft.VSTS.TCM.Parameters</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestName</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestStorage</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestId</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestType</Field>
          <Field>Microsoft.VSTS.TCM.LocalDataSource</Field>
          <Field>Microsoft.VSTS.TCM.AutomationStatus</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestId</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Sprint']">
    <ViewMaps>
      <ViewMap
        title="Sprint - Team Sprint"
        child="Team Sprint"
        link="Scrum.ImplementedBy"
        position="4"
        >
        <Description>Sprints with the implementing team sprints:</Description>
        <ParentTypes>
          <type>Sprint</type>
        </ParentTypes>
        <ParentSort field="Iteration Path" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Not Started</state>
          <state>In Progress</state>
          <state>Done</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Deleted</state>
          <state>Aborted</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FFFFFF00">Not Started</state>
          <state colour="#FFFFA500">In Progress</state>
          <state colour="#FF008000">Done</state>
          <state colour="#FFDCDCDC">Aborted</state>
          <state colour="#FFDCDCDC">Deleted</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData type="Sprint" body="System.Description" caption="System.IterationPath" numeric="Scrum.v3.Capacity" owner="">
        <ContextFields>
          <Field>System.State</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>Scrum.v3.SprintStart</Field>
          <Field>Scrum.v3.SprintEnd</Field>
          <Field>Scrum.v3.Capacity</Field>
          <Field>Scrum.v3.ActualWorkStarts</Field>
          <Field>Scrum.v3.ActualWorkEnd</Field>
          <Field>Scrum.v3.PlannedWork</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Team Sprint']">
    <ViewMaps>
      <ViewMap
        title="Team Sprint - Retrospective"
        child="Sprint Retrospective"
        link="System.LinkTypes.Hierarchy"
        position="5">
        <Description>The team sprint and related retrospective:</Description>
        <ParentTypes>
          <type>Team Sprint</type>
        </ParentTypes>
        <ParentSort field="Iteration Path" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Active</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FF008000">Active</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData type="Team Sprint" body="System.Description" caption="System.IterationPath" numeric="Scrum.v3.Capacity" owner="">
        <ContextFields>
          <Field>System.State</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>Scrum.v3.SprintStart</Field>
          <Field>Scrum.v3.SprintEnd</Field>
          <Field>Scrum.v3.Capacity</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Release']">
    <ViewMaps>
      <ViewMap
        title="Release - Sprint"
        child="Sprint"
        link="Scrum.ImplementedBy"
        position="6"
        notswimlane="true"
        >
        <Description>The release with implementing sptints:</Description>
        <ParentTypes>
          <type>Release</type>
        </ParentTypes>
        <ParentSort field="Iteration Path" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Not Started</state>
          <state>In Progress</state>
          <state>Done</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Deleted</state>
          <state>Aborted</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FFFFFF00">Not Started</state>
          <state colour="#FFFFA500">In Progress</state>
          <state colour="#FF008000">Done</state>
          <state colour="#FFDCDCDC">Aborted</state>
          <state colour="#FFDCDCDC">Deleted</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData type="Release" body="System.Description" caption="System.IterationPath" numeric="Scrum.v3.PlannedVelocity" owner="">
        <ContextFields>
          <Field>System.State</Field>
          <Field>Scrum.v3.ReleaseAudience</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Scrum.v3.PlannedWorkStarts</Field>
          <Field>Scrum.v3.PlannedWorkEnds</Field>
          <Field>Scrum.v3.GoLive</Field>
          <Field>Scrum.v3.ActualWorkStart</Field>
          <Field>Scrum.v3.ActualWorkEnds</Field>
          <Field>Scrum.v3.EstimatedVelocity</Field>
          <Field>Scrum.v3.ActualCumulativeVelocity</Field>
          <Field>Scrum.v3.PlannedVelocity</Field>
          <Field>Scrum.v3.ReleaseAudience</Field>
          <Field>Scrum.v3.Capacity</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>
  
</xsl:stylesheet>

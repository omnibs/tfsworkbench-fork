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

  <xsl:template match="WORKITEMTYPE" />
  
  <xsl:template match="WORKITEMTYPE[@name='Product Backlog Item']">
    <ViewMaps>
      <ViewMap title="Backlog and Bug - Tasks" child="Task" link="System.LinkTypes.Hierarchy" position="0">
        <Description>The product backlog, bugs and related tasks:</Description>
        <ParentTypes>
          <type>Product Backlog Item</type>
          <type>Bug</type>
        </ParentTypes>
        <ParentSort field="Backlog Priority" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>To Do</state>
          <state>In Progress</state>
          <state>Done</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Removed</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FF008000">Done</state>
          <state colour="#FFDCDCDC">Removed</state>
          <state colour="#FFFFA500">In Progress</state>
          <state colour="#FFFFFF00">To Do</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="Backlog and Bug - Tests" child="Test Case" link="Microsoft.VSTS.Common.TestedBy" position="1">
        <Description>The product backlog, bugs and related acceptance criteria:</Description>
        <ParentTypes>
          <type>Product Backlog Item</type>
          <type>Bug</type>
        </ParentTypes>
        <ParentSort field="Backlog Priority" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Design</state>
          <state>Ready</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFA500">Ready</state>
          <state colour="#FF008000">Closed</state>
          <state colour="#FFFFFF00">Design</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="Backlog - Impediments" child="Impediment" link="System.LinkTypes.Related" position="2">
        <Description>The product backlog and related impediments:</Description>
        <ParentTypes>
          <type>Product Backlog Item</type>
        </ParentTypes>
        <ParentSort field="Backlog Priority" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Open</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FF008000">Closed</state>
          <state colour="#FFFF0000">Open</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData type="Product Backlog Item" caption="System.Title" body="Microsoft.VSTS.Common.DescriptionHtml" numeric="Microsoft.VSTS.Scheduling.Effort" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.BacklogPriority</Field>
          <Field>Microsoft.VSTS.Common.BusinessValue</Field>
          <Field>Microsoft.VSTS.Scheduling.Effort</Field>
          <Field>Microsoft.VSTS.Common.AcceptanceCriteria</Field>
          <Field>Microsoft.VSTS.Common.DescriptionHtml</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Bug']">
    <ItemTypes>
      <ItemTypeData type="Bug" caption="System.Title" body="Microsoft.VSTS.TCM.ReproSteps" numeric="Microsoft.VSTS.Scheduling.Effort" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.TCM.SystemInfo</Field>
          <Field>Microsoft.VSTS.TCM.ReproSteps</Field>
          <Field>Microsoft.VSTS.Common.BacklogPriority</Field>
          <Field>Microsoft.VSTS.Scheduling.Effort</Field>
          <Field>Microsoft.VSTS.Common.AcceptanceCriteria</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Sprint']">
    <ViewMaps>
      <ViewMap title="Sprint - Backlog" child="Product Backlog Item" link="System.LinkTypes.Hierarchy" position="5">
        <Description>The sprint and related backlog items:</Description>
        <ParentTypes>
          <type>Sprint</type>
        </ParentTypes>
        <ParentSort field="Iteration Path" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>New</state>
          <state>Approved</state>
          <state>Committed</state>
          <state>Done</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Removed</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FF008000">Done</state>
          <state colour="#FFADD8E6">New</state>
          <state colour="#FFDCDCDC">Removed</state>
          <state colour="#FFFFFF00">Approved</state>
          <state colour="#FFFFA500">Committed</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="Sprint - Bugs" child="Bug" link="System.LinkTypes.Hierarchy" position="6">
        <Description>The sprint and related bugs:</Description>
        <ParentTypes>
          <type>Sprint</type>
        </ParentTypes>
        <ParentSort field="Iteration Path" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>New</state>
          <state>Approved</state>
          <state>Committed</state>
          <state>Done</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Removed</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FF008000">Done</state>
          <state colour="#FFADD8E6">New</state>
          <state colour="#FFDCDCDC">Removed</state>
          <state colour="#FFFFFF00">Approved</state>
          <state colour="#FFFFA500">Committed</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData type="Sprint" caption="System.IterationPath" body="Microsoft.VSTS.Common.DescriptionHtml" numeric="" owner="">
        <ContextFields />
        <DuplicationFields>
          <Field>System.IterationPath</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Scheduling.StartDate</Field>
          <Field>Microsoft.VSTS.Scheduling.FinishDate</Field>
          <Field>Microsoft.VSTS.Common.Retrospective</Field>
          <Field>Microsoft.VSTS.Common.DescriptionHtml</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Task']">
    <ItemTypes>
      <ItemTypeData type="Task" caption="System.Title" body="Microsoft.VSTS.Common.DescriptionHtml" numeric="Microsoft.VSTS.Scheduling.RemainingWork" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Activity</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Scheduling.RemainingWork</Field>
          <Field>Microsoft.VSTS.Common.BacklogPriority</Field>
          <Field>Microsoft.VSTS.Common.Activity</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
          <Field>Microsoft.VSTS.Common.DescriptionHtml</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Test Case']">
    <ItemTypes>
      <ItemTypeData type="Test Case" caption="System.Title" body="Microsoft.VSTS.Common.DescriptionHtml" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.State</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.TCM.Steps</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestName</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestStorage</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestId</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestType</Field>
          <Field>Microsoft.VSTS.TCM.Parameters</Field>
          <Field>Microsoft.VSTS.TCM.LocalDataSource</Field>
          <Field>Microsoft.VSTS.TCM.AutomationStatus</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestId</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Common.DescriptionHtml</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Impediment']">
    <ItemTypes>
      <ItemTypeData type="Impediment" caption="System.Title" body="Microsoft.VSTS.Common.DescriptionHtml" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Resolution</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Common.DescriptionHtml</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

</xsl:stylesheet>

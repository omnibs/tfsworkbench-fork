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

  <xsl:template match="WORKITEMTYPE[@name='User Story']">
    <ViewMaps>
      <ViewMap title="Implementation - Tasks" child="Task" link="System.LinkTypes.Hierarchy" position="0">
        <Description>User stories and bugs with related tasks:</Description>
        <ParentTypes>
          <type>User Story</type>
          <type>Bug</type>
        </ParentTypes>
        <ParentSort field="Stack Rank" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Active</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFFF00">Active</state>
          <state colour="#FF008000">Closed</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="User Stories - Hierarchy" child="User Story" link="System.LinkTypes.Hierarchy" position="1" notswimlane="true">
        <Description>User stories and the related sub user stories:</Description>
        <ParentTypes>
          <type>User Story</type>
        </ParentTypes>
        <ParentSort field="Stack Rank" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Active</state>
          <state>Resolved</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFFF00">Active</state>
          <state colour="#FFFFA500">Resolved</state>
          <state colour="#FF008000">Closed</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="User Stories - Issues" child="Issue" link="System.LinkTypes.Related" position="2">
        <Description>Users stories and the related issues:</Description>
        <ParentTypes>
          <type>User Story</type>
        </ParentTypes>
        <ParentSort field="Stack Rank" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Active</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFF0000">Active</state>
          <state colour="#FF008000">Closed</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="User Stories, Bugs - Test Cases" child="Test Case" link="Microsoft.VSTS.Common.TestedBy" position="3">
        <Description>User stories and bugs with related test cases:</Description>
        <ParentTypes>
          <type>User Story</type>
          <type>Bug</type>
        </ParentTypes>
        <ParentSort field="Stack Rank" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Design</state>
          <state>Ready</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFFF00">Ready</state>
          <state colour="#FF008000">Closed</state>
          <state colour="#FFFFA500">Design</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData
        type="User Story"
        caption="System.Title"
        body="System.Description"
        numeric="Microsoft.VSTS.Scheduling.StoryPoints"
        owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Risk</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Scheduling.StoryPoints</Field>
          <Field>Microsoft.VSTS.Common.Risk</Field>
          <Field>Microsoft.VSTS.Common.StackRank</Field>
          <Field>Microsoft.VSTS.Scheduling.StartDate</Field>
          <Field>Microsoft.VSTS.Scheduling.FinishDate</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
    
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Issue']">
    <ViewMaps />

    <ItemTypes>
      <ItemTypeData
        type="Issue"
        caption="System.Title"
        body="System.Description"
        numeric="Microsoft.VSTS.Common.Priority"
        owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
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
          <Field>Microsoft.VSTS.Common.StackRank</Field>
          <Field>Microsoft.VSTS.Scheduling.DueDate</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Test Case']">
    <ViewMaps />

    <ItemTypes>
      <ItemTypeData
        type="Test Case"
        caption="System.Title"
        body="System.Description"
        numeric="Microsoft.VSTS.Common.Priority"
        owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.TCM.AutomationStatus</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestName</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestStorage</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestId</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestType</Field>
          <Field>Microsoft.VSTS.TCM.Parameters</Field>
          <Field>Microsoft.VSTS.TCM.LocalDataSource</Field>
          <Field>Microsoft.VSTS.TCM.AutomationStatus</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>


  <xsl:template match="WORKITEMTYPE[@name='Task']">
    <ViewMaps />

    <ItemTypes>
      <ItemTypeData
        type="Task"
        caption="System.Title"
        body="System.Description"
        numeric="Microsoft.VSTS.Scheduling.RemainingWork"
        owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Activity</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Activity</Field>
          <Field>Microsoft.VSTS.Scheduling.RemainingWork</Field>
          <Field>Microsoft.VSTS.Scheduling.OriginalEstimate</Field>
          <Field>Microsoft.VSTS.Scheduling.CompletedWork</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.StackRank</Field>
          <Field>Microsoft.VSTS.Scheduling.StartDate</Field>
          <Field>Microsoft.VSTS.Scheduling.FinishDate</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Bug']">
    <ItemTypes>
      <ItemTypeData
        type="Bug"
        caption="System.Title"
        body="Microsoft.VSTS.TCM.ReproSteps"
        numeric="Microsoft.VSTS.Common.Severity"
        owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>System.Reason</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.TCM.SystemInfo</Field>
          <Field>Microsoft.VSTS.TCM.ReproSteps</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
          <Field>Microsoft.VSTS.Common.StackRank</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>  
  </xsl:template>
  
</xsl:stylesheet>

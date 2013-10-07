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
      <ViewMap title="Backlog, Bugs and Tasks" position="0" child="Sprint Backlog Item" link="">
        <Description>The product backlog and bugs with related tasks:</Description>
        <ParentTypes>
          <type>Product Backlog Item</type>
          <type>Bug</type>
        </ParentTypes>
        <ParentSort field="Business Priority (Scrum)" direction="Descending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Not Done</state>
          <state>In Progress</state>
          <state>Ready For Test</state>
          <state>Done</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Deleted</state>
          <state>Deferred</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FF008000">Done</state>
          <state colour="#FFFFFF00">Not Done</state>
          <state colour="#FFFFA500">In Progress</state>
          <state colour="#FF800080">Deferred</state>
          <state colour="#FFDCDCDC">Deleted</state>
          <state colour="#FFADD8E6">Ready For Test</state>
        </StateItemColours>
      </ViewMap>

      <ViewMap title="Backlog, Bugs and Impediments" position="1" child="Impediment" link="">
        <Description>The product backlog and bugs with related impediments:</Description>
        <ParentTypes>
          <type>Product Backlog Item</type>
          <type>Bug</type>
        </ParentTypes>
        <ParentSort field="Business Priority (Scrum)" direction="Descending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Not Done</state>
          <state>In Progress</state>
          <state>Ready For Test</state>
          <state>Done</state>
        </SwimLaneStates>
        <BucketStates>
          <state>Deferred</state>
          <state>Deleted</state>
        </BucketStates>
        <StateItemColours>
          <state colour="#FFFFFF00">Not Done</state>
          <state colour="#FFFFA500">In Progress</state>
          <state colour="#FF008000">Done</state>
          <state colour="#FF800080">Deferred</state>
          <state colour="#FFDCDCDC">Deleted</state>
          <state colour="#FFADD8E6">Ready For Test</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData
        type="Product Backlog Item"
        caption="System.Title"
        body="System.Description"
        numeric="Conchango.TeamSystem.Scrum.EstimatedEffort"
        owner="">
        <ContextFields>
          <Field>System.State</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Conchango.TeamSystem.Scrum.BusinessPriority</Field>
          <Field>Conchango.TeamSystem.Scrum.DeliveryOrder</Field>
          <Field>Conchango.TeamSystem.Scrum.EstimatedEffort</Field>
          <Field>Conchango.TeamSystem.Scrum.WorkRemaining</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
          <Field>Conchango.TeamSystem.Scrum.ConditionsOfAcceptance</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Sprint Backlog Item']">
    <ItemTypes>
      <ItemTypeData
        type="Sprint Backlog Item"
        caption="System.Title"
        body="System.Description"
        numeric="Conchango.TeamSystem.Scrum.WorkRemaining"
        owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Conchango.TeamSystem.Scrum.TaskPriority</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Conchango.TeamSystem.Scrum.EstimatedEffort</Field>
          <Field>Conchango.TeamSystem.Scrum.WorkRemaining</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
          <Field>Conchango.TeamSystem.Scrum.TaskPriority</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Impediment']">
    <ItemTypes>
      <ItemTypeData
        type="Impediment"
        caption="System.Title"
        body="System.Description"
        numeric=""
        owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Bug']">
    <ItemTypes>
      <ItemTypeData
        type="Bug"
        caption="System.Title"
        body="System.Description"
        numeric="Conchango.TeamSystem.Scrum.EstimatedEffort"
        owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Conchango.TeamSystem.Scrum.TaskPriority</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Conchango.TeamSystem.Scrum.EstimatedEffort</Field>
          <Field>Conchango.TeamSystem.Scrum.WorkRemaining</Field>
          <Field>Conchango.TeamSystem.Scrum.Build.Environment</Field>
          <Field>Conchango.TeamSystem.Scrum.TestingImpact</Field>
          <Field>Conchango.TeamSystem.Scrum.DateDiscovered</Field>
          <Field>Conchango.TeamSystem.Scrum.DateClosed</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
          <Field>Conchango.TeamSystem.Scrum.BusinessPriority</Field>
          <Field>Conchango.TeamSystem.Scrum.DeliveryOrder</Field>
          <Field>Conchango.TeamSystem.Scrum.ReplicationActionDetail</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Sprint']">
    <ViewMaps>
      <ViewMap title="Sprint - Retrospective" position="2" child="Sprint Retrospective" link="">
        <Description>The sprints and related retrospectives:</Description>
        <ParentTypes>
          <type>Sprint</type>
        </ParentTypes>
        <ParentSort field="Iteration Path" direction="Ascending" />
        <ChildSort field="System.Id" direction="Ascending" />
        <SwimLaneStates>
          <state>Not Done</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFFF00">Not Done</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData
        type="Sprint"
        caption="System.IterationPath"
        body="System.Description"
        numeric="Conchango.TeamSystem.Scrum.Capacity"
        owner="">
        <ContextFields>
          <Field>System.State</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Conchango.TeamSystem.Scrum.SprintStart</Field>
          <Field>Conchango.TeamSystem.Scrum.SprintEnd</Field>
          <Field>Conchango.TeamSystem.Scrum.Capacity</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>


  <xsl:template match="WORKITEMTYPE[@name='Sprint Retrospective']">
    <ItemTypes>
      <ItemTypeData
        type="Sprint Retrospective"
        caption="System.Title"
        body="System.Description"
        numeric=""
        owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Conchango.TeamSystem.Scrum.Team</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>
  
</xsl:stylesheet>

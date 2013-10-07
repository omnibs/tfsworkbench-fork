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

  <xsl:template match="WORKITEMTYPE[@name='Scenario']">
    <ViewMaps>
      <ViewMap title="Scenario, Bug and QoS - Tasks" position="0" child="Task" link="">
        <Description>The project scenarios, bugs and quality of service requirements with related tasks.</Description>
        <ParentTypes>
          <type>Bug</type>
          <type>Quality of Service Requirement</type>
          <type>Scenario</type>
        </ParentTypes>
        <ParentSort field="Rank" direction="Ascending" />
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
      <ViewMap title="Scenario, Bug and QoS - Risks" position="1" child="Risk" link="">
        <Description>The project scenarios, bugs and quality of service requirements with related risks.</Description>
        <ParentTypes>
          <type>Bug</type>
          <type>Quality of Service Requirement</type>
          <type>Scenario</type>
        </ParentTypes>
        <ParentSort field="Rank" direction="Ascending" />
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
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData type="Scenario" caption="System.Title" body="System.Description" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
          <Field>Microsoft.VSTS.Common.RoughOrderOfMagnitude</Field>
          <Field>Microsoft.VSTS.Common.ExitCriteria</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
          <Field>Microsoft.VSTS.Common.ExitCriteria</Field>
          <Field>Microsoft.VSTS.Common.Rank</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Common.RoughOrderOfMagnitude</Field>
          <Field>Microsoft.VSTS.Scheduling.StartDate</Field>
          <Field>Microsoft.VSTS.Scheduling.FinishDate</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
    
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Bug']">
    <ItemTypes>
      <ItemTypeData type="Bug" caption="System.Title" body="System.Description" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.Common.Rank</Field>
          <Field>Microsoft.VSTS.Test.TestName</Field>
          <Field>Microsoft.VSTS.Test.TestId</Field>
          <Field>Microsoft.VSTS.Test.TestPath</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Task']">
    <ItemTypes>
      <ItemTypeData type="Task" caption="System.Title" body="System.Description" numeric="Microsoft.VSTS.Scheduling.RemainingWork" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Discipline</Field>
          <Field>Microsoft.VSTS.Common.ExitCriteria</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
          <Field>Microsoft.VSTS.Common.ExitCriteria</Field>
          <Field>Microsoft.VSTS.Common.Rank</Field>
          <Field>Microsoft.VSTS.Common.Discipline</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Scheduling.RemainingWork</Field>
          <Field>Microsoft.VSTS.Scheduling.CompletedWork</Field>
          <Field>Microsoft.VSTS.Scheduling.BaselineWork</Field>
          <Field>Microsoft.VSTS.Scheduling.StartDate</Field>
          <Field>Microsoft.VSTS.Scheduling.FinishDate</Field>
          <Field>Microsoft.VSTS.Scheduling.TaskHierarchy</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>
  
  <xsl:template match="WORKITEMTYPE[@name='Quality of Service Requirement']">
    <ItemTypes>
      <ItemTypeData type="Quality of Service Requirement" caption="System.Title" body="System.Description" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.ExitCriteria</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
          <Field>Microsoft.VSTS.Common.QualityOfServiceType</Field>
          <Field>Microsoft.VSTS.Common.RoughOrderOfMagnitude</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
          <Field>Microsoft.VSTS.Common.ExitCriteria</Field>
          <Field>Microsoft.VSTS.Common.Rank</Field>
          <Field>Microsoft.VSTS.Common.QualityOfServiceType</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Common.RoughOrderOfMagnitude</Field>
          <Field>Microsoft.VSTS.Scheduling.StartDate</Field>
          <Field>Microsoft.VSTS.Scheduling.FinishDate</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>
  
  <xsl:template match="WORKITEMTYPE[@name='Risk']">
    <ItemTypes>
      <ItemTypeData type="Risk" caption="System.Title" body="System.Description" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
          <Field>Microsoft.VSTS.Common.ExitCriteria</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Issue</Field>
          <Field>Microsoft.VSTS.Common.ExitCriteria</Field>
          <Field>Microsoft.VSTS.Common.Rank</Field>
          <Field>Microsoft.VSTS.Test.TestName</Field>
          <Field>Microsoft.VSTS.Test.TestId</Field>
          <Field>Microsoft.VSTS.Test.TestPath</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>
</xsl:stylesheet>

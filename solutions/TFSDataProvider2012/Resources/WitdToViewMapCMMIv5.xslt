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

  <xsl:template match="WORKITEMTYPE[@name='Requirement']">
    <ViewMaps>
      <ViewMap title="Requirement and Bug - Tasks" position="0" child="Task" link="System.LinkTypes.Hierarchy">
        <Description>The project requirements and related tasks.</Description>
        <ParentTypes>
          <type>Requirement</type>
          <type>Bug</type>
        </ParentTypes>
        <ParentSort field="Stack Rank" direction="Ascending" />
        <ChildSort field="Stack Rank" direction="Ascending" />
        <SwimLaneStates>
          <state>Proposed</state>
          <state>Active</state>
          <state>Resolved</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFFF00">Proposed</state>
          <state colour="#FFFFA500">Active</state>
          <state colour="#FFADD8E6">Resolved</state>
          <state colour="#FF008000">Closed</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="Requirement and Bug - Risks" position="1" child="Risk" link="System.LinkTypes.Hierarchy">
        <Description>The project requirements with related risks</Description>
        <ParentTypes>
          <type>Requirement</type>
          <type>Bug</type>
        </ParentTypes>
        <ParentSort field="Stack Rank" direction="Ascending" />
        <ChildSort field="Stack Rank" direction="Ascending" />
        <SwimLaneStates>
          <state>Proposed</state>
          <state>Active</state>
          <state>Resolved</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFFF00">Proposed</state>
          <state colour="#FFFFA500">Active</state>
          <state colour="#FFADD8E6">Resolved</state>
          <state colour="#FF008000">Closed</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="Requirement and Bug - Test Cases" position="2" child="Test Case" link="Microsoft.VSTS.Common.TestedBy">
        <Description>The project requirements and bugs with related test cases.</Description>
        <ParentTypes>
          <type>Bug</type>
          <type>Requirement</type>
        </ParentTypes>
        <ParentSort field="Stack Rank" direction="Ascending" />
        <ChildSort field="Stack Rank" direction="Ascending" />
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
      <ViewMap title="Requirement and Bug - Issues" position="3" child="Issue" link="System.LinkTypes.Hierarchy">
        <Description>The project requirements and bugs with related issues.</Description>
        <ParentTypes>
          <type>Requirement</type>
          <type>Bug</type>
        </ParentTypes>
        <ParentSort field="Stack Rank" direction="Ascending" />
        <ChildSort field="Stack Rank" direction="Ascending" />
        <SwimLaneStates>
          <state>Proposed</state>
          <state>Active</state>
          <state>Resolved</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFFF00">Proposed</state>
          <state colour="#FFFFA500">Active</state>
          <state colour="#FFADD8E6">Resolved</state>
          <state colour="#FF008000">Closed</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="Requirement Hierarchy" position="4" child="Requirement" link="System.LinkTypes.Hierarchy" notswimlane="true">
        <Description>The project requirements with releated sub requirements.</Description>
        <ParentTypes>
          <type>Requirement</type>
        </ParentTypes>
        <ParentSort field="Stack Rank" direction="Ascending" />
        <ChildSort field="Stack Rank" direction="Ascending" />
        <SwimLaneStates>
          <state>Proposed</state>
          <state>Active</state>
          <state>Resolved</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFFF00">Proposed</state>
          <state colour="#FFFFA500">Active</state>
          <state colour="#FFADD8E6">Resolved</state>
          <state colour="#FF008000">Closed</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData type="Requirement" caption="System.Title" body="System.Description" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
          <Field>Microsoft.VSTS.CMMI.Committed</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.CMMI.RequirementType</Field>
          <Field>Microsoft.VSTS.Scheduling.StartDate</Field>
          <Field>Microsoft.VSTS.Common.ActivatedDate</Field>
          <Field>Microsoft.VSTS.Common.ResolvedReason</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.StackRank</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Scheduling.OriginalEstimate</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
          <Field>Microsoft.VSTS.CMMI.Committed</Field>
          <Field>Microsoft.VSTS.CMMI.ImpactAssessmentHtml</Field>
          <Field>Microsoft.VSTS.CMMI.UserAcceptanceTest</Field>
          <Field>Microsoft.VSTS.CMMI.SubjectMatterExpert1</Field>
          <Field>Microsoft.VSTS.CMMI.SubjectMatterExpert2</Field>
          <Field>Microsoft.VSTS.CMMI.SubjectMatterExpert3</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
    
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Change Request']">
    <ViewMaps>
      <ViewMap title="Change Request - Requirements" position="5" child="Requirement" link="Microsoft.VSTS.Common.Affects">
        <Description>Change requests and the affected requirements.</Description>
        <ParentTypes>
          <type>Change Request</type>
        </ParentTypes>
        <ParentSort field="Priority" direction="Ascending" />
        <ChildSort field="Stack Rank" direction="Ascending" />
        <SwimLaneStates>
          <state>Proposed</state>
          <state>Active</state>
          <state>Resolved</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FFFFFF00">Proposed</state>
          <state colour="#FFFFA500">Active</state>
          <state colour="#FFADD8E6">Resolved</state>
          <state colour="#FF008000">Closed</state>
        </StateItemColours>
      </ViewMap>
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData type="Change Request" caption="System.Title" body="System.Description" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Scheduling.OriginalEstimate</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
          <Field>Microsoft.VSTS.CMMI.Justification</Field>
          <Field>Microsoft.VSTS.CMMI.ImpactOnArchitecture</Field>
          <Field>Microsoft.VSTS.CMMI.ImpactOnUserExperience</Field>
          <Field>Microsoft.VSTS.CMMI.ImpactOnTest</Field>
          <Field>Microsoft.VSTS.CMMI.ImpactOnDevelopment</Field>
          <Field>Microsoft.VSTS.CMMI.ImpactOnTechnicalPublications</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Bug']">
    <ItemTypes>
      <ItemTypeData type="Bug" caption="System.Title" body="Microsoft.VSTS.TCM.ReproSteps" numeric="Microsoft.VSTS.Common.Severity" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
          <Field>Microsoft.VSTS.Common.StackRank</Field>
          <Field>Microsoft.VSTS.TCM.SystemInfo</Field>
          <Field>Microsoft.VSTS.TCM.ReproSteps</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Scheduling.OriginalEstimate</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
          <Field>Microsoft.VSTS.CMMI.Symptom</Field>
          <Field>Microsoft.VSTS.CMMI.ProposedFix</Field>
          <Field>Microsoft.VSTS.CMMI.FoundInEnvironment</Field>
          <Field>Microsoft.VSTS.CMMI.RootCause</Field>
          <Field>Microsoft.VSTS.CMMI.HowFound</Field>
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
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
          <Field>Microsoft.VSTS.CMMI.TaskType</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.StackRank</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.Common.Discipline</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.CMMI.TaskType</Field>
          <Field>Microsoft.VSTS.CMMI.RequiresReview</Field>
          <Field>Microsoft.VSTS.CMMI.RequiresTest</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
          <Field>Microsoft.VSTS.Scheduling.OriginalEstimate</Field>
          <Field>Microsoft.VSTS.Scheduling.RemainingWork</Field>
          <Field>Microsoft.VSTS.Scheduling.CompletedWork</Field>
          <Field>Microsoft.VSTS.Scheduling.StartDate</Field>
          <Field>Microsoft.VSTS.Scheduling.FinishDate</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Issue']">
    <ItemTypes>
      <ItemTypeData type="Issue" caption="System.Title" body="System.Description" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.CMMI.Escalate</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Triage</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.CMMI.Analysis</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
          <Field>Microsoft.VSTS.CMMI.CorrectiveActionPlan</Field>
          <Field>Microsoft.VSTS.CMMI.CorrectiveActionActualResolution</Field>
          <Field>Microsoft.VSTS.CMMI.TargetResolveDate</Field>
          <Field>Microsoft.VSTS.CMMI.Escalate</Field>
          <Field>Microsoft.VSTS.Scheduling.OriginalEstimate</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>  
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Risk']">
    <ItemTypes>
      <ItemTypeData type="Risk" caption="System.Title" body="System.Description" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>Microsoft.VSTS.Common.StackRank</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Scheduling.OriginalEstimate</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
          <Field>Microsoft.VSTS.CMMI.MitigationTriggers</Field>
          <Field>Microsoft.VSTS.CMMI.MitigationPlan</Field>
          <Field>Microsoft.VSTS.CMMI.ContingencyPlan</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Review']">
    <ItemTypes>
      <ItemTypeData type="Review" caption="System.Title" body="Microsoft.VSTS.CMMI.Purpose" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.CMMI.MeetingType</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.CMMI.Purpose</Field>
          <Field>Microsoft.VSTS.CMMI.Comments</Field>
          <Field>Microsoft.VSTS.CMMI.Minutes</Field>
          <Field>Microsoft.VSTS.CMMI.MeetingType</Field>
          <Field>Microsoft.VSTS.CMMI.CalledDate</Field>
          <Field>Microsoft.VSTS.CMMI.CalledBy</Field>
          <Field>Microsoft.VSTS.CMMI.RequiredAttendee1</Field>
          <Field>Microsoft.VSTS.CMMI.RequiredAttendee2</Field>
          <Field>Microsoft.VSTS.CMMI.RequiredAttendee3</Field>
          <Field>Microsoft.VSTS.CMMI.RequiredAttendee4</Field>
          <Field>Microsoft.VSTS.CMMI.RequiredAttendee5</Field>
          <Field>Microsoft.VSTS.CMMI.RequiredAttendee6</Field>
          <Field>Microsoft.VSTS.CMMI.RequiredAttendee7</Field>
          <Field>Microsoft.VSTS.CMMI.RequiredAttendee8</Field>
          <Field>Microsoft.VSTS.CMMI.OptionalAttendee1</Field>
          <Field>Microsoft.VSTS.CMMI.OptionalAttendee2</Field>
          <Field>Microsoft.VSTS.CMMI.OptionalAttendee3</Field>
          <Field>Microsoft.VSTS.CMMI.OptionalAttendee4</Field>
          <Field>Microsoft.VSTS.CMMI.OptionalAttendee5</Field>
          <Field>Microsoft.VSTS.CMMI.OptionalAttendee6</Field>
          <Field>Microsoft.VSTS.CMMI.OptionalAttendee7</Field>
          <Field>Microsoft.VSTS.CMMI.OptionalAttendee8</Field>
          <Field>Microsoft.VSTS.CMMI.ActualAttendee1</Field>
          <Field>Microsoft.VSTS.CMMI.ActualAttendee2</Field>
          <Field>Microsoft.VSTS.CMMI.ActualAttendee3</Field>
          <Field>Microsoft.VSTS.CMMI.ActualAttendee4</Field>
          <Field>Microsoft.VSTS.CMMI.ActualAttendee5</Field>
          <Field>Microsoft.VSTS.CMMI.ActualAttendee6</Field>
          <Field>Microsoft.VSTS.CMMI.ActualAttendee7</Field>
          <Field>Microsoft.VSTS.CMMI.ActualAttendee8</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Test Case']">
    <ItemTypes>
      <ItemTypeData type="Test Case" caption="System.Title" body="System.Description" numeric="" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.TCM.AutomationStatus</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
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
          <Field>Microsoft.VSTS.TCM.AutomatedTestId</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>
</xsl:stylesheet>

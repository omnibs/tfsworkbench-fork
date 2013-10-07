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
      <ViewMap title="PBI &amp; Bug - Task" position="0" notswimlane="false" child="Task" link="System.LinkTypes.Hierarchy">
        <Description>Product backlog items and bugs with related tasks</Description>
        <ParentTypes>
          <type>Bug</type>
          <type>Product Backlog Item</type>
        </ParentTypes>
        <ParentSort field="Backlog Priority" direction="Ascending" />
        <ChildSort field="ID" direction="Ascending" />
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
          <state colour="#FFFFA500">In Progress</state>
          <state colour="#FFDCDCDC">Removed</state>
          <state colour="#FFFF0000">To Do</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="PBI &amp; Bug - Test Case" position="1" notswimlane="false" child="Test Case" link="Microsoft.VSTS.Common.TestedBy">
        <Description>Product backlog items and bugs with related test cases</Description>
        <ParentTypes>
          <type>Bug</type>
          <type>Product Backlog Item</type>
        </ParentTypes>
        <ParentSort field="Backlog Priority" direction="Ascending" />
        <ChildSort field="ID" direction="Ascending" />
        <SwimLaneStates>
          <state>Design</state>
          <state>Ready</state>
          <state>Closed</state>
        </SwimLaneStates>
        <BucketStates />
        <StateItemColours>
          <state colour="#FF008000">Closed</state>
          <state colour="#FFFF0000">Design</state>
          <state colour="#FFFFA500">Ready</state>
        </StateItemColours>
      </ViewMap>
      <ViewMap title="PBI &amp; Bug - Impediment" position="2" notswimlane="false" child="Impediment" link="System.LinkTypes.Hierarchy">
        <Description>Product backlog items and bugs with related impediments</Description>
        <ParentTypes>
          <type>Bug</type>
          <type>Product Backlog Item</type>
        </ParentTypes>
        <ParentSort field="System.Id" direction="Ascending" />
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
      <ViewMap title="PBI &amp; Bug - Feedback Response" position="3" notswimlane="false" child="Feedback Response" link="System.LinkTypes.Related">
        <Description>Product backlog items and bugs with related feedback responses</Description>
        <ParentTypes>
          <type>Bug</type>
          <type>Product Backlog Item</type>
        </ParentTypes>
        <ParentSort field="Backlog Priority" direction="Ascending" />
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
    </ViewMaps>

    <ItemTypes>
      <ItemTypeData type="Product Backlog Item" caption="System.Title" body="System.Description" numeric="Microsoft.VSTS.Scheduling.Effort" owner="System.AssignedTo">
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
          <Field>Microsoft.VSTS.Common.BacklogPriority</Field>
          <Field>Microsoft.VSTS.Common.BusinessValue</Field>
          <Field>Microsoft.VSTS.Scheduling.Effort</Field>
          <Field>System.Reason</Field>
          <Field>System.State</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>

  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Bug']">
    <ItemTypes>
      <ItemTypeData type="Bug" caption="System.Title" body="System.Description" numeric="Microsoft.VSTS.Common.Severity" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.AssignedTo</Field>
          <Field>System.Description</Field>
          <Field>Microsoft.VSTS.Scheduling.Effort</Field>
          <Field>Microsoft.VSTS.Build.FoundIn</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.Common.BacklogPriority</Field>
          <Field>Microsoft.VSTS.Common.Severity</Field>
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
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.CMMI.Blocked</Field>
          <Field>Microsoft.VSTS.Common.BacklogPriority</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Scheduling.RemainingWork</Field>
          <Field>System.Reason</Field>
          <Field>Microsoft.VSTS.Common.Activity</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Test Case']">
    <ItemTypes>
      <ItemTypeData type="Test Case" caption="System.Title" body="System.Description" numeric="Microsoft.VSTS.Common.Priority" owner="System.AssignedTo">
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
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestId</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestName</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestStorage</Field>
          <Field>Microsoft.VSTS.TCM.AutomatedTestType</Field>
          <Field>Microsoft.VSTS.TCM.AutomationStatus</Field>
          <Field>Microsoft.VSTS.Build.IntegrationBuild</Field>
          <Field>Microsoft.VSTS.TCM.LocalDataSource</Field>
          <Field>Microsoft.VSTS.TCM.Parameters</Field>
          <Field>Microsoft.VSTS.Common.Priority</Field>
          <Field>System.Reason</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Impediment']">
    <ItemTypes>
      <ItemTypeData type="Impediment" caption="System.Title" body="System.Description" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

  <xsl:template match="WORKITEMTYPE[@name='Feedback Response']">
    <ItemTypes>
      <ItemTypeData type="Feedback Response" caption="System.Title" body="System.Description" numeric="Microsoft.VSTS.Common.Rating" owner="System.AssignedTo">
        <ContextFields>
          <Field>System.AssignedTo</Field>
          <Field>System.State</Field>
          <Field>Microsoft.VSTS.Common.Rating</Field>
        </ContextFields>
        <DuplicationFields>
          <Field>System.Title</Field>
          <Field>System.IterationPath</Field>
          <Field>System.AreaPath</Field>
          <Field>System.Description</Field>
          <Field>System.Description</Field>
          <Field>System.AssignedTo</Field>
          <Field>Microsoft.VSTS.Common.Rating</Field>
          <Field>System.Reason</Field>
        </DuplicationFields>
      </ItemTypeData>
    </ItemTypes>
  </xsl:template>

</xsl:stylesheet>

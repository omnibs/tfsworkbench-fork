<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns="http://schemas.workbench/ProjectData"
>
  <xsl:output method="xml" indent="yes" encoding="utf-8"/>

  <xsl:template match="/">
    <ProjectData>
      <ViewMaps>
        <xsl:apply-templates 
          select="//Control[@Type='LinksControl']/LinksControlOptions[WorkItemLinkFilters/@FilterType='include'][count(WorkItemLinkFilters/Filter) = 1][WorkItemLinkFilters/Filter[1]/@FilterOn = 'forwardname'][WorkItemTypeFilters/@FilterType='include'][count(WorkItemTypeFilters/Filter) = 1]"
          />
      </ViewMaps>
      <ItemTypes>
        <ItemTypeData>
          <xsl:attribute name="type">
            <xsl:value-of select="//WORKITEMTYPE/@name"/>
          </xsl:attribute>

          <!-- Check for a default caption field -->
          <xsl:variable select="//FIELD[@refname = 'System.Title']" name="caption" />
          
          <xsl:if test="$caption">
            <xsl:attribute name="caption">
              <xsl:value-of select="$caption/@refname"/>
            </xsl:attribute>
          </xsl:if>

          <!-- Check for a default body field -->
          <xsl:variable select="//FIELD[@refname = 'System.Description']" name="body" />

          <xsl:if test="$body">
            <xsl:attribute name="body">
              <xsl:value-of select="$body/@refname"/>
            </xsl:attribute>
          </xsl:if>

          <!-- Check for a default numeric field -->
          <xsl:variable select="//FIELD[@refname = 'Microsoft.VSTS.Scheduling.StoryPoints']" name="numeric" />

          <xsl:if test="$numeric">
            <xsl:attribute name="numeric">
              <xsl:value-of select="$numeric/@refname"/>
            </xsl:attribute>
          </xsl:if>

          <!-- Check for a default owner field -->
          <xsl:variable select="//FIELD[@refname = 'System.AssignedTo']" name="owner" />

          <xsl:if test="$owner">
            <xsl:attribute name="owner">
              <xsl:value-of select="$owner/@refname"/>
            </xsl:attribute>
          </xsl:if>

          <ContextFields>
            <xsl:if test="//FIELD[@refname = 'System.AssignedTo']">
              <Field>System.AssignedTo</Field>
            </xsl:if>
            <xsl:if test="//FIELD[@refname = 'System.State']">
              <Field>System.State</Field>
            </xsl:if>
          </ContextFields>
          <!-- Add the default duplication fields -->
          <DuplicationFields>
            <xsl:if test="//FIELD[@refname = 'System.Title']">
              <Field>System.Title</Field>
            </xsl:if>
            <xsl:if test="//FIELD[@refname = 'System.IterationPath']">
              <Field>System.IterationPath</Field>
            </xsl:if>
            <xsl:if test="//FIELD[@refname = 'System.AreaPath']">
              <Field>System.AreaPath</Field>
            </xsl:if>
            <xsl:if test="//FIELD[@refname = 'System.Description']">
              <Field>System.Description</Field>
            </xsl:if>
            <xsl:if test="//FIELD[@refname = 'System.Description']">
              <Field>System.Description</Field>
            </xsl:if>
            <xsl:if test="//FIELD[@refname = 'System.AssignedTo']">
              <Field>System.AssignedTo</Field>
            </xsl:if>
          </DuplicationFields>
        </ItemTypeData>
      </ItemTypes>
    </ProjectData>
  </xsl:template>

  <xsl:template match="LinksControlOptions">
    
    <xsl:variable name="ParentWorkItem" select="//WORKITEMTYPE[1]/@name" />
    <xsl:variable name="ChildWorkItem" select="WorkItemTypeFilters[1]/Filter[1]/@WorkItemType" />

    <ViewMap>
      <xsl:attribute name="title">
        <xsl:value-of select="$ParentWorkItem"/>
        <xsl:text xml:space="preserve"> - </xsl:text>
        <xsl:value-of select="$ChildWorkItem"/>
      </xsl:attribute>
      <xsl:attribute name="child">
        <xsl:value-of select="$ChildWorkItem" />
      </xsl:attribute>
      <xsl:attribute name="link">
        <xsl:value-of select="WorkItemLinkFilters[1]/Filter[1]/@LinkType" />
      </xsl:attribute>
      <ParentTypes>
        <type>
          <xsl:value-of select="$ParentWorkItem" />
        </type>
      </ParentTypes>
      <Description>
        <xsl:value-of select="../@Label" />
      </Description>
      <ParentSort field="ID" direction="Ascending" />
      <ChildSort field="ID" direction="Ascending" />
      <SwimLaneStates />
      <BucketStates />
    </ViewMap>
  </xsl:template>  
</xsl:stylesheet>

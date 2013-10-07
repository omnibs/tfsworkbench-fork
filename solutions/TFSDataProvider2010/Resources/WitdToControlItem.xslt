<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns="http://schemas.workbench/ControlItems"
>
  <xsl:output method="xml" indent="yes" encoding="utf-8"/>

  <xsl:template match="/">
    <ControlItemGroup>
      <WorkItemType>
        <xsl:value-of select="//WORKITEMTYPE[1]/@name"/>
      </WorkItemType>
      <ControlItems>
        <ControlItem controltype="FieldControl" displaytext="Id" fieldname="System.Id" readonly="true" />
        <xsl:apply-templates select="//Control[@Type = 'FieldControl' or @Type = 'WorkItemClassificationControl' or @Type = 'HtmlFieldControl' or @Type = 'DateTimeControl']" />
      </ControlItems>
    </ControlItemGroup>
  </xsl:template>

  <xsl:template match="Control">
    <xsl:variable name="fieldname" select="@FieldName" />
    <ControlItem displaytext="{translate(@Label, '&amp;', '')}" fieldname="{$fieldname}" controltype="{@Type}">
      <xsl:if test="@ReadOnly = 'True'">
        <xsl:attribute name="readonly">true</xsl:attribute>
      </xsl:if>
      <HelpText>
        <xsl:apply-templates select="//FIELD">
          <xsl:with-param name="fieldname" select="$fieldname" />
        </xsl:apply-templates>
      </HelpText>
    </ControlItem>
  </xsl:template>

  <xsl:template match="FIELD">
    <xsl:param name="fieldname" />
    <xsl:if test="@refname = $fieldname">
      <xsl:value-of select="HELPTEXT"/>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>

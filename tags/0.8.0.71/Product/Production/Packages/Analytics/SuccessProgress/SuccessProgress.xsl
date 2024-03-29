<?xml version="1.0"?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:exsl="http://exslt.org/common" exclude-result-prefixes="exsl">

  <xsl:output method="html"/>

  <xsl:variable name="BuildAttemptCount" select="count(/statistics/integration)"/>
  <xsl:variable name="SuccessfulBuildCount" select="count(/statistics/integration[@status='Success'])"/>
  <xsl:variable name="FailedBuildCount" select="count(/statistics/integration[@status='Failure'])"/>
  <xsl:variable name="ExceptionBuildCount" select="count(/statistics/integration[@status='Exception'])"/>

  <xsl:template match="/">
    <number>
      <xsl:value-of select="$BuildAttemptCount"/>
    </number>
    <exsl:document href="..\..\..\Artifacts\SuccessProgressSuccessful.xml" fragment="yes" append="yes" >
      <number>
        <xsl:value-of select="$SuccessfulBuildCount"/>
      </number>
    </exsl:document>
    <exsl:document href="..\..\..\Artifacts\SuccessProgressFailed.xml" fragment="yes" append="yes" >
      <number>
        <xsl:value-of select="$FailedBuildCount"/>
      </number>
    </exsl:document>
    <exsl:document href="..\..\..\Artifacts\SuccessProgressException.xml" fragment="yes" append="yes" >
      <number>
        <xsl:value-of select="$ExceptionBuildCount"/>
      </number>
    </exsl:document>
  </xsl:template>

</xsl:stylesheet> 

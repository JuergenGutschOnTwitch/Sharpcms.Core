<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:include href="..\..\..\..\Custom\Components\Snippets.xslt" />

  <xsl:template match="/">
    <div class="choose">
      <div class="content">
        <xsl:apply-templates select="data/navigationplace/*" mode="choose" />
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>
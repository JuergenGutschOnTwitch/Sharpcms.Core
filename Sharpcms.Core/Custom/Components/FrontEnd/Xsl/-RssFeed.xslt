<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

	<ms:script implements-prefix="inventit" xmlns:inventit="urn:my-scripts" xmlns:ms="urn:schemas-microsoft-com:xslt" language="C#" >
		<![CDATA[ 
		public string ReplaceHtml(string text) {
			return System.Text.RegularExpressions.Regex.Replace(text, @"<([^>]|\s)*>", "");
		}
    
		public string textToHtml(string text) {
			return text.Replace("\n", "<br/>");
		}
		]]>
	</ms:script>

	<xsl:template match="channel">
		<xsl:apply-templates select="image" />
		<h2 style="margin-top:0px;margin-bottom:0px;">
			<xsl:value-of select="title" />
		</h2>
		<p>
			<xsl:if test="not(description = '') and description">
				<xsl:value-of disable-output-escaping="yes" select="description" />
				<br />
				<br />
			</xsl:if>
			<xsl:for-each select="item">
				<xsl:apply-templates select="." />
			</xsl:for-each>
		</p>
	</xsl:template>

	<xsl:template match="item">
		<xsl:if test="not(dc:creator = '') and dc:creator" xmlns:dc="http://purl.org/dc/elements/1.1/">
			<xsl:value-of select="dc:creator" />
			<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
		</xsl:if>
		<xsl:if test="not(pubDate = '') and pubDate">
			<xsl:value-of select="pubDate" />
		</xsl:if>
		<br />
		<a target="_blank" style="color:#000000;">
			<xsl:attribute name="href">
				<xsl:value-of select="link" />
			</xsl:attribute>
			<xsl:value-of select="title" />
		</a>
		<br />
	</xsl:template>

</xsl:stylesheet>


<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template match="element[@type='search']" mode="show">
		<xsl:choose>
			<xsl:when test="contains(//data/attributes/pageroot, 'english')">
				<h1>
          <xsl:text>Search sharpcms web site</xsl:text>
        </h1>
			</xsl:when>
			<xsl:otherwise>
				<h1>
          <xsl:text>sharpcms Website durchsuchen</xsl:text>
        </h1>
			</xsl:otherwise>
		</xsl:choose>
		<div>
			<form name="systemform" method="post" encType="multipart/form-data">
				<!-- primary hidden settings -->
				<input type="hidden" name="event_main" value="search" />
				<input type="hidden" name="event_mainvalue" value="" />
				<input type="hidden" name="process">
					<xsl:attribute name="value">
						<xsl:value-of select="//data/query/other/process" />
					</xsl:attribute>
				</input>
				<input type="hidden" name="data_pageidentifier">
					<xsl:attribute name="value">
						<xsl:value-of select="//data/contentplace/page/attributes/pageidentifier" />
					</xsl:attribute>
				</input>
				<input type="hidden" name="data_start" value="" />
				<br />
				<input type="text" name="data_query" value="{query}" id="s" class="search" />  
				<input type="submit" value="Suchen" class="but" />
			</form>
      <xsl:if test="//data/basedata/currentuser/groups/group[contains(., 'admin')]">
        <form name="systemform" method="post" encType="multipart/form-data">
          <!-- primary hidden settings -->
          <input type="hidden" name="event_main" value="index" />
          <input type="hidden" name="event_mainvalue" value="" />
          <input type="submit" value="Rebuild Index" class="linkbutton" />
        </form>
      </xsl:if>
		</div>
	</xsl:template>

	<xsl:template match="element[@type='summary']" mode="show">
		<p>
			<xsl:value-of select="." disable-output-escaping="yes" />
			<hr />
		</p>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='paging']">
		<p>
			<xsl:choose>
				<xsl:when test="contains(//data/attributes/pageroot, 'english')">
					<xsl:text>Result page: </xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>Ergebnisseiten: </xsl:text>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:value-of select="." disable-output-escaping="yes" />
		</p>
		<p>
			<span>
        <xsl:text>Search Powered by </xsl:text>
      </span>
			<a>
        <xsl:attribute name="href">
          <xsl:text>http://incubator.apache.org/lucene.net/</xsl:text>
        </xsl:attribute>
				<xsl:text>Lucene.Net</xsl:text>
			</a>
		</p>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='result']">
		<p>
			<a class="link">
				<xsl:attribute name="href">
					<xsl:value-of select="path" />
				</xsl:attribute>
				<strong>
					<xsl:value-of select="title" />
				</strong>
			</a>
			<br />
			<span class="sample">
				<xsl:value-of select="sample" disable-output-escaping="yes" />
			</span>
			<br />
			<a class="link">
				<xsl:attribute name="href">
					<xsl:value-of select="path" />
				</xsl:attribute>
				<xsl:value-of select="/data/basepath" />
				<xsl:text>/</xsl:text>
				<xsl:value-of select="path" />
			</a>
			<br />
			<br />
		</p>
	</xsl:template>
</xsl:stylesheet>
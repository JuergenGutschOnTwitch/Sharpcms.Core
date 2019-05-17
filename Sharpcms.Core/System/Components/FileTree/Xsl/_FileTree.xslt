<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template  mode="edit" match="filetree">
    <div class="head tree_head">
      <div class="title">
        <b>
          <xsl:text>Directory</xsl:text>
        </b>
      </div>
      <div class="viewstate">
        <xsl:text> </xsl:text>
      </div>
    </div>
    <div class="menu tree_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlAddFolder</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:text>.</xsl:text>
        </xsl:attribute>
        <xsl:text>Add folder</xsl:text>
      </a>
    </div>
    <div class="tree tree_body">
      <ul id="files" class="filetree">
        <xsl:apply-templates mode="filetree" select="folder/*">
          <xsl:with-param name="current-path" />
        </xsl:apply-templates>
      </ul>
    </div>
	</xsl:template>

  <xsl:template mode="filetree" match="folder">
		<xsl:param name="current-path" />
    <li>
      <a>
        <xsl:attribute name="href">
          <xsl:text>/admin/file/edit/folder/</xsl:text>
          <xsl:value-of select="$current-path" />
          <xsl:value-of select="@name" />
          <xsl:text>/</xsl:text>
        </xsl:attribute>
        <span>
          <xsl:attribute name="class">
            <xsl:text>folder</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="@name" />
        </span>
      </a>
      <xsl:if test="*">
        <ul>
          <xsl:apply-templates mode="filetree" select="*">
            <xsl:with-param name="current-path">
              <xsl:value-of select="$current-path" />
              <xsl:value-of select="@name" />
              <xsl:text>/</xsl:text>
            </xsl:with-param>
          </xsl:apply-templates>
        </ul>
      </xsl:if>
    </li>
	</xsl:template>
	
	<xsl:template mode="filetree" match="file">
    <xsl:param name="current-path" />
    <li>
      <a>
        <xsl:attribute name="href">
          <xsl:text>/admin/file/edit/file/</xsl:text>
          <xsl:value-of select="$current-path" />
          <xsl:value-of select="@name" />
          <xsl:text>/</xsl:text>
        </xsl:attribute>
        <span>
          <xsl:attribute name="class">
            <xsl:text>file</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="@name" />
        </span>
      </a>
    </li>
	</xsl:template>
</xsl:stylesheet>
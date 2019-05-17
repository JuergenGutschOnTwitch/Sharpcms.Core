<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />
  
  <xsl:template mode="choose" match="filetree">
    <div id="chooseFileDialog" class="choose" title="Choose File">
      <ul id="pages" class="filetree">
        <li>
          <a>
            <xsl:attribute name="class">
              <xsl:text>hlCloseDialog</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="data-path">
              <xsl:text>.</xsl:text>
            </xsl:attribute>
            <span>
              <xsl:attribute name="class">
                <xsl:text>folder</xsl:text>
              </xsl:attribute>
              <xsl:text>..</xsl:text>
            </span>
          </a>
        </li>
        <xsl:apply-templates mode="filetreechoose" select="folder/*">
          <xsl:with-param name="current-path" />
        </xsl:apply-templates>
      </ul>
    </div>
  </xsl:template>

  <xsl:template mode="filetreechoose" match="folder">
    <xsl:param name="current-path" />
    <li>
      <xsl:choose>
        <xsl:when test="/data/query/other/process = 'admin/choose/file'">
          <span>
            <xsl:attribute name="class">
              <xsl:text>folder</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="@name" />
          </span>
        </xsl:when>
        <xsl:otherwise>
          <a>
            <xsl:attribute name="class">
              <xsl:text>hlCloseDialog</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="data-path">
              <xsl:value-of select="$current-path" />
              <xsl:value-of select="@name" />
            </xsl:attribute>
            <span>
              <xsl:attribute name="class">
                <xsl:text>folder</xsl:text>
              </xsl:attribute>
              <xsl:value-of select="@name" />
            </span>
          </a>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:if test="*">
        <ul>
          <xsl:apply-templates mode="filetreechoose" select="*">
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

  <xsl:template mode="filetreechoose" match="file">
    <xsl:param name="current-path" />
    <li>
      <xsl:choose>
        <xsl:when test="/data/query/other/process = 'admin/choose/file'">
          <a>
            <xsl:attribute name="class">
              <xsl:text>hlCloseDialog</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="data-path">
              <xsl:value-of select="$current-path" />
              <xsl:value-of select="@name" />
            </xsl:attribute>
            <span>
              <xsl:attribute name="class">
                <xsl:text>file</xsl:text>
              </xsl:attribute>
              <xsl:value-of select="@name" />
            </span>
          </a>
        </xsl:when>
        <xsl:otherwise>
          <span>
            <xsl:attribute name="class">
              <xsl:text>file</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="@name" />
          </span>
        </xsl:otherwise>
      </xsl:choose>
    </li>
  </xsl:template>
</xsl:stylesheet>
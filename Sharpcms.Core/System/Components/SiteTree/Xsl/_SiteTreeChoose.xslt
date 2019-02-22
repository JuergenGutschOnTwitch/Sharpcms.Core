<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template mode="choose" match="sitetree">
    <div id="choosePageDialog" class="choose" title="Choose Page">
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
        <xsl:for-each select="*">
          <xsl:call-template name="SiteTreeElementChoose">
            <xsl:with-param name="current-path">
              <xsl:value-of select="name()" />
            </xsl:with-param>
            <xsl:with-param name="isLanguage">
              <xsl:text>true</xsl:text>
            </xsl:with-param>
          </xsl:call-template>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>
 
  <xsl:template name ="SiteTreeElementChoose">
    <xsl:param name="current-path" />
    <xsl:param name="isLanguage" />
    <li>
      <a>
        <xsl:attribute name="class">
          <xsl:text>hlCloseDialog</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:value-of select="$current-path" />
        </xsl:attribute>
        <xsl:if test="@status = 'hide'">
          <xsl:attribute name="title">
            <xsl:text>This Page is hidden</xsl:text>
          </xsl:attribute>
        </xsl:if>
        <span>
          <xsl:choose>
            <xsl:when test="$isLanguage = 'true'">
              <xsl:attribute name="class">
                <xsl:choose>
                  <xsl:when test="/data/basedata/defaultpage = $current-path">
                    <xsl:text>home</xsl:text>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>folder</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="class">
                <xsl:choose>
                  <xsl:when test="/data/basedata/defaultpage = $current-path">
                    <xsl:text>home</xsl:text>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>file</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:value-of select="@menuname" />
        </span>
        <xsl:if test="@status = 'hide'">
          <img class="icon-hiddenpage" src="/System/Components/Admin/Images/icon-hiddenpage.png" />
        </xsl:if>
      </a>
      <xsl:if test="*">
        <ul>
          <xsl:for-each select="*">
            <xsl:call-template name="SiteTreeElementChoose">
              <xsl:with-param name="current-path">
                <xsl:value-of select="$current-path" />
                <xsl:text>/</xsl:text>
                <xsl:value-of select="name()" />
              </xsl:with-param>
              <xsl:with-param name="isLanguage">
                <xsl:text>false</xsl:text>
              </xsl:with-param>
            </xsl:call-template>
          </xsl:for-each>
        </ul>
      </xsl:if>
    </li>
  </xsl:template>
</xsl:stylesheet>
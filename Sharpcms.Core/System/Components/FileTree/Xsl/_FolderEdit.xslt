<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template mode="edit" match="folder">
    <div class="head folderdata_head">
      <div class="title">
        <b>
          <xsl:text>Path: </xsl:text>
          <xsl:value-of select="@path" />
        </b>
      </div>
      <div class="viewstate">
        <a id="foda_vs" class="button expand">
          <xsl:text>˅</xsl:text>
        </a>
      </div>
    </div>
    <div class="menu folderdata_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlRemoveFolder</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Delete folder</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlRenameFolder</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Rename folder</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlAddFolder</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Add subfolder</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlMoveFolder</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Move folder</xsl:text>
      </a>
    </div>
    <div class="tab-pane" id="foda_body" style="float: left;">
      <div id="usda_body_tabs" class="tabs">
        <ul>
          <li>
            <a href="#ptabs1">
              <xsl:text>Upload</xsl:text>
            </a>
          </li>
        </ul>
        <div id="ptabs1" class="tab-page">
          <div class="tab_page_body">
            <xsl:call-template name="upload" />
          </div>
        </div>
      </div>
    </div>
  </xsl:template>

  <xsl:template name="upload">
    <div class="adminmenu">
      <xsl:text>Upload new file in directory</xsl:text>
      <br />
      <xsl:call-template name="uploadfield">
        <xsl:with-param name="currentlevel">
          <xsl:text>1</xsl:text>
        </xsl:with-param>
        <xsl:with-param name="maxlevel">
          <xsl:text>10</xsl:text>
        </xsl:with-param>
      </xsl:call-template>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlUploadFile</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Upload</xsl:text>
      </a>
    </div>
  </xsl:template>

  <xsl:template name="uploadfield">
    <xsl:param name="currentlevel" />
    <xsl:param name="maxlevel" />
    <div id="file_{$currentlevel}">
      <xsl:attribute name="class">
        <xsl:text>uploadfield</xsl:text>
      </xsl:attribute>
      <xsl:if test="$currentlevel > 1">
        <xsl:attribute name="style">
          <xsl:text>display: none;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <input type="file" size="60" class="upload" name="datafile_{$currentlevel}" />
      <div id="showlink_{$currentlevel + 1}">
        <a>
          <xsl:attribute name="class">
            <xsl:text>hlMoreFiles</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="data-currentlevel">
            <xsl:value-of select ="$currentlevel" />
          </xsl:attribute>
          <xsl:text>More files...</xsl:text>
        </a>
      </div>
    </div>
    <xsl:if test="$currentlevel &lt; $maxlevel">
      <xsl:call-template name="uploadfield">
        <xsl:with-param name="currentlevel" select="$currentlevel + 1" />
        <xsl:with-param name="maxlevel" select="$maxlevel" />
      </xsl:call-template>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
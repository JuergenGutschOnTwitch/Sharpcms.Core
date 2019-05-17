<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template mode="edit" match="file">
    <div class="head filedata_head">
      <div class="title">
        <b>
          <xsl:text>Path: </xsl:text>
          <xsl:value-of select="@path" />
        </b>
      </div>
      <div class="viewstate">
        <xsl:choose>
          <xsl:when test="@mainmimetype='image'">
            <a id="fida_vs" class="button expand">
              <xsl:text>˅</xsl:text>
            </a>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text> </xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </div>
    </div>
    <div class="menu filedata_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlRemoveFile</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Delete file</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlRenameFile</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Rename file</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="href">
          <xsl:text>/?process=download/</xsl:text>
          <xsl:value-of select="@path" />&amp;download=true
        </xsl:attribute>
        <xsl:text>Download file</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlMoveFile</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-path">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Move file</xsl:text>
      </a>
    </div>
    <xsl:if test="@mainmimetype='image'">
      <div class="tab-pane" id="fida_body" style="float: left;">
        <div id="fida_body_tabs" class="tabs">
          <ul>
            <li>
              <a href="#ptabs1">
                <xsl:text>File Data</xsl:text>
              </a>
            </li>
          </ul>
          <div id="ptabs1" class="tab-page">
            <div class="tab_page_body">
              <label>
                <xsl:text>View</xsl:text>
              </label>
              <div class="item">
                <img>
                  <xsl:attribute name="src">
                    <xsl:text>?process=download/</xsl:text>
                    <xsl:value-of select="@path" />
                    <xsl:choose>
                      <xsl:when test="/data/query/other/width">
                        <xsl:text>&amp;width=</xsl:text>
                        <xsl:value-of select="/data/query/other/width" />
                      </xsl:when>
                      <xsl:when test="/data/query/other/height">
                        <xsl:text>&amp;height=</xsl:text>
                        <xsl:value-of select="/data/query/other/height" />
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:text>&amp;width=400&amp;height=400</xsl:text>
                      </xsl:otherwise>
                    </xsl:choose>
                  </xsl:attribute>
                </img>
              </div>
              <label>
                <xsl:text>Resize</xsl:text>
              </label>
              <div class="item">
                <fieldset>
                  <label>
                    <xsl:text>Width</xsl:text>
                  </label>
                  <div class="item">
                    <input type="text" name="width" maxlength="5" />
                  </div>
                  <label>
                    <xsl:text>Height</xsl:text>
                  </label>
                  <div class="item">
                    <input type="text" name="height" maxlength="5" />
                  </div>
                  <a>
                    <xsl:attribute name="class">
                      <xsl:text>button hlResizeFile</xsl:text>
                    </xsl:attribute>
                    <xsl:text>Resize</xsl:text>
                  </a>
                </fieldset>
                <div class="message">
                  <strong>
                    <xsl:text>Info: </xsl:text>
                  </strong>
                  <xsl:text>The resized image will be placed in the thumbs folder.</xsl:text>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
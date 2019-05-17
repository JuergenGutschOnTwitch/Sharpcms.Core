<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template mode="edit" match="groups">
    <div class="head tree_head">
      <div class="title">
        <b>
          <xsl:text>Groups</xsl:text>
        </b>
      </div>
      <div class="viewstate">
        <xsl:text> </xsl:text>
      </div>
    </div>
    <div class="menu tree_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlAddGroup</xsl:text>
        </xsl:attribute>
        <xsl:text>Add group</xsl:text>
      </a>
    </div>
    <div class="tree tree_body">
      <ul id="groups" class="filetree">
        <xsl:for-each select="group">
          <li>
            <a>
              <xsl:attribute name="class">
                <xsl:text>hlDeleteGroup</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="data-groupname">
                <xsl:value-of select="@name" />
              </xsl:attribute>
              <span>
                <xsl:attribute name="class">
                  <xsl:text>group</xsl:text>
                </xsl:attribute>
                <xsl:value-of select="@name" />
                <xsl:text> Delete</xsl:text>
              </span>
            </a>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>
</xsl:stylesheet>
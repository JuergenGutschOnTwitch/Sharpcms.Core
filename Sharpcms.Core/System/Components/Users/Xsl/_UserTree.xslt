<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />
  
  <xsl:template mode="edit" match="users">
    <div class="head tree_head">
      <div class="title">
        <b>
          <xsl:text>Users</xsl:text>
        </b>
      </div>
      <div class="viewstate">
        <xsl:text> </xsl:text>
      </div>
    </div>
    <div class="menu tree_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlAddUser</xsl:text>
        </xsl:attribute>
        <xsl:text>Add user</xsl:text>
      </a>
    </div>
    <div class="tree tree_body">
      <ul id="users" class="filetree">
        <xsl:for-each select="user">
          <li>
            <a>
              <xsl:attribute name="href">
                <xsl:text>/admin/user/</xsl:text>
                <xsl:value-of select="login" />
              </xsl:attribute>
              <span>
                <xsl:attribute name="class">
                  <xsl:text>user</xsl:text>
                </xsl:attribute>
                <xsl:value-of select="login" />
              </span>
            </a>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>
</xsl:stylesheet>
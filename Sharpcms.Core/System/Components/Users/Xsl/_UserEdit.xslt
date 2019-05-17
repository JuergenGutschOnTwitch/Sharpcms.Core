<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template mode="edit" match="user">
    <div class="head userdata_head">
      <div class="title">
        <b>
          <xsl:text>User: </xsl:text>
          <xsl:value-of select="login" />
        </b>
      </div>
      <div class="viewstate">
        <a id="usda_vs" class="button expand">
          <xsl:text>˅</xsl:text>
        </a>
      </div>
    </div>
    <div class="menu userdata_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlSaveUser</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-username">
          <xsl:value-of select="login" />
        </xsl:attribute>
        <xsl:text>Save</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlDeleteUser</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-username">
          <xsl:value-of select="login" />
        </xsl:attribute>
        <xsl:text>Delete user</xsl:text>
      </a>
    </div>
    <div class="tab-pane" id="usda_body" style="float: left;">
      <div id="usda_body_tabs" class="tabs">
        <ul>
          <li>
            <a href="#ptabs1">
              <xsl:text>User Data</xsl:text>
            </a>
          </li>
        </ul>
        <div id="ptabs1" class="tab-page">
          <div class="tab_page_body">
            <label>
              <xsl:text>Login</xsl:text>
            </label>
            <div class="item">
              <input>
                <xsl:attribute name="name">
                  <xsl:text>data_user_login</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="type">
                  <xsl:text>text</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="value">
                  <xsl:value-of select="login" />
                </xsl:attribute>
              </input>
            </div>
            <label>
              <xsl:text>Password</xsl:text>
            </label>
            <div class="item">
              <input>
                <xsl:attribute name="name">
                  <xsl:text>data_user_password</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="type">
                  <xsl:text>password</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="value">
                  <xsl:text>emptystring</xsl:text>
                </xsl:attribute>
              </input>
            </div>
            <label>
              <xsl:text>Groups</xsl:text>
            </label>
            <div class="item">
              <ul class="checkbox_list">
                <xsl:variable select="groups" name="groups" />
                <xsl:for-each select="//data/navigationplace/groups/group">
                  <xsl:variable select="@name" name="name" />
                  <li>
                    <input class="checkbox" type="checkbox" name="data_user_groups">
                      <xsl:attribute name="id">
                        <xsl:text>data_user_groups_</xsl:text>
                        <xsl:value-of select="@name" />
                      </xsl:attribute>
                      <xsl:attribute name="value">
                        <xsl:value-of select="@name" />
                      </xsl:attribute>
                      <xsl:if test="$groups/group[@name=$name]">
                        <xsl:attribute name="checked">
                          <xsl:text>checked</xsl:text>
                        </xsl:attribute>
                      </xsl:if>
                    </input>
                    <label>
                      <xsl:attribute name="for">
                        <xsl:text>data_user_groups_</xsl:text>
                        <xsl:value-of select="@name" />
                      </xsl:attribute>
                      <xsl:value-of select="@name" />
                    </label>
                  </li>
                </xsl:for-each>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>

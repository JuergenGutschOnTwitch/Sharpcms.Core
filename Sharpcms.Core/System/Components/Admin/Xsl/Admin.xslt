<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:include href="..\..\..\..\Custom\Components\Snippets.xslt" />

  <xsl:template match="/">
    <html>
      <head>
        <base>
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath" />
          </xsl:attribute>
        </base>
        <title>
          <xsl:text>Admin - sharpcms</xsl:text>
          <xsl:if test="/data/currentuser/basedata">
            <xsl:text> [</xsl:text>
            <xsl:value-of select="/data/currentuser/basedata" />
            <xsl:text>]</xsl:text>
          </xsl:if>
        </title>
        <link type="text/css" rel="stylesheet" href="/System/Components/Admin/Styles/base.css" />
        <link type="text/css" rel="StyleSheet" href="/System/Components/Admin/Styles/jquery/jquery-ui-1.9.2.custom.css" />
        <link type="text/css" rel="StyleSheet" href="/System/Components/Admin/Styles/jquery/selectmenu/jquery.ui.selectmenu.css" />
        <link type="text/css" rel="StyleSheet" href="/System/Components/Admin/Styles/jquery/treeview/jquery.treeview.css" />
        <link type="text/css" rel="StyleSheet" href="/System/Components/Admin/Styles/jquery/jquery-ui-1.9.2.sharpcms.css" />
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/jquery-1.10.2.min.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/jquery-ui-1.9.2.custom.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/jquery.url.packed.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/selectmenu/jquery.ui.selectmenu.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/tiny_mce/jquery.tinymce.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/treeview/jquery.treeview.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/sharpcms.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/admin.xslt.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/sessvars/sessvars.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/collapseexpand.js">
          <xsl:text> </xsl:text>
        </script>
      </head>
      <body>
        <a class="anchor" name="top">
          <xsl:text> </xsl:text>
        </a>
        <form name="systemform" method="post" encType="multipart/form-data">
          <xsl:attribute name="action">
            <xsl:value-of select="data/query/other/process" />
          </xsl:attribute>
          <div class="page">
            <!-- Begin of primary hidden settings -->
            <input type="hidden" name="event_main" value="" />
            <input type="hidden" name="event_mainvalue" value="" />
            <input type="hidden" name="event_redirect" value="" />
            <input type="hidden" name="process">
              <xsl:attribute name="value">
                <xsl:value-of select="data/query/other/process" />
              </xsl:attribute>
            </input>
            <!-- End of primary hidden settings -->

            <!-- Begin Header -->
            <div class="header">
              <a href="{/data/basepath}/admin/">
                <img>
                  <xsl:attribute name="src">
                    <xsl:text>/System/Components/Admin/Images/logo.gif</xsl:text>
                  </xsl:attribute>
                </img>
              </a>
            </div>
            <!-- End Header -->

            <!-- Begin TopMenu -->
            <div class="mainnavi">
              <ul class="left">
                <xsl:apply-templates mode="topmenu" select="/data/basedata/adminmenu/left/item">
                  <xsl:sort select="@weight" order="ascending" />
                </xsl:apply-templates>
              </ul>
              <ul class="right">
                <li>
                  <a href="{/data/basepath}" target="_blank">
                    <xsl:text>Show website</xsl:text>
                  </a>
                </li>
                <xsl:apply-templates mode="topmenu" select="/data/basedata/adminmenu/right/item">
                  <xsl:sort select="@weight" order="ascending" />
                </xsl:apply-templates>
              </ul>
            </div>
            <!-- End TopMenu -->

            <!-- Begin admin_menu -->
            <xsl:if test="/data/messages">
              <div class="messages">
                  <xsl:for-each select="/data/messages/item">
                    <xsl:choose>
                      <xsl:when test="@messagetype = 'Error'">
                        <div class="ui-state-error ui-corner-all">
                          <p>
                            <span class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;">
                              <xsl:text> </xsl:text>
                            </span>
                            <strong>Alert: </strong>
                            <xsl:value-of select="normalize-space(.)" />
                          </p>
                        </div>
                      </xsl:when>
                      <xsl:otherwise>
                        <div class="ui-state-highlight ui-corner-all">
                          <p>
                            <span class="ui-icon ui-icon-info" style="float: left; margin-right: .3em;">
                              <xsl:text> </xsl:text>
                            </span>
                            <strong>Alert: </strong>
                            <xsl:value-of select="normalize-space(.)" />
                          </p>
                        </div>
                      </xsl:otherwise>
                    </xsl:choose>
                  </xsl:for-each>
              </div>
            </xsl:if>
            <!-- End admin_menu -->

            <!-- Begin Content -->
            <div class="content">
              <div class="left">
                <xsl:apply-templates select="data/navigationplace/*" mode="edit" />
                <xsl:text> </xsl:text>
              </div>
              <div class="right">
                <xsl:apply-templates select="data/contentplace/*" mode="edit" />
                <xsl:text> </xsl:text>
              </div>
              <div class="hidden" id="pagedialog">
                <xsl:text> </xsl:text>
              </div>
              <div class="hidden" id="filedialog">
                <xsl:text> </xsl:text>
              </div>
              <div class="hidden" id="folgerdialog">
                <xsl:text> </xsl:text>
              </div>
              <div class="hidden" id="imagedialog">
                <xsl:text> </xsl:text>
              </div>
              <div class="hidden" id="userdialog">
                <xsl:text> </xsl:text>
              </div>
              <div class="hidden" id="groupdialog">
                <xsl:text> </xsl:text>
              </div>
            </div>
            <!-- End Content -->

          </div>
        </form>
      </body>
    </html>
  </xsl:template>

  <xsl:template mode="topmenu" match="item">
    <li>
      <a>
        <xsl:choose>
          <xsl:when test="@path">
            <xsl:attribute name="href">
              <xsl:value-of select="@path" />
              <xsl:text>/</xsl:text>
            </xsl:attribute>
          </xsl:when>
          <xsl:when test="@javascript">
            <xsl:attribute name="href">
              <xsl:text>javascript:</xsl:text>
              <xsl:value-of select="@javascript" />
            </xsl:attribute>
          </xsl:when>
          <xsl:otherwise>
            <xsl:attribute name="class">
              <xsl:text>hlThrowEvent</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="data-action">
              <xsl:value-of select="@event" />
            </xsl:attribute>
          </xsl:otherwise>
        </xsl:choose>
        <xsl:value-of select="." />
      </a>
    </li>
  </xsl:template>
</xsl:stylesheet>
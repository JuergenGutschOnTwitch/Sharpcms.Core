<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:include href="-PageContent.xslt" />
  <xsl:include href="-SiteTree.xslt" />
  <xsl:include href="-RssFeed.xslt" />
  <xsl:include href="..\..\Search\Xsl\_search.xsl" />
  <xsl:include href="..\..\Snippets.xslt" />

  <xsl:template match="/" xmlns="http://www.w3.org/1999/xhtml">
    <html xml:lang="de" lang="de" xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <base>
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath" />
          </xsl:attribute>
        </base>
        <meta http-equiv="Page-Enter" content="blendTrans(Duration=0.1)" />
        <meta http-equiv="Page-Exit" content="blendTrans(Duration=0.1)" />
        <title>
          <xsl:text>sharpcms - </xsl:text>
          <xsl:value-of select="/data/contentplace/page/attributes/pagename" />
        </title>
        <meta http-equiv="content-type" content="text/html; charset=utf-8" />
        <meta name="keywords">
          <xsl:attribute name="content">
            <xsl:value-of select="/data/contentplace/page/attributes/metakeywords" />
          </xsl:attribute>
        </meta>
        <meta name="description">
          <xsl:attribute name="content">
            <xsl:value-of select="/data/contentplace/page/attributes/metadescription" />
          </xsl:attribute>
        </meta>
        <meta name="author" content="Jürgen I. Gutsch (www.gutsch-online.de)" />
        <meta name="generator" content="sharpcms" />
        <meta name="revisit-after" content="7 days" />
        <meta name="robots" content="index,follow" />
        <link id="styleLink" rel="stylesheet" type="text/css">
          <xsl:attribute name="href">
            <xsl:text>Custom/Components/FrontEnd/css/central.css</xsl:text>
          </xsl:attribute>
        </link>
        <xsl:text disable-output-escaping="yes">
          <![CDATA[<!--[if lte IE 7]><link href="Custom/Components/FrontEnd/css/patch_layout_draft.css" rel="stylesheet" type="text/css" /><![endif]-->]]>
        </xsl:text>
        <script type="text/javascript" src="/Custom/Components/FrontEnd/js/searchhi.js">
          <xsl:text> </xsl:text>
        </script>
        <script src="http://www.google-analytics.com/urchin.js" type="text/javascript">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript">
          <xsl:text disable-output-escaping="yes"><![CDATA[<!-- _uacct = "UA-390207-5"; urchinTracker(); //-->]]></xsl:text>
        </script>
      </head>
      <body>
        <div id="page_margins">
          <div id="page">
            <div id="header">
              <div id="head1">
                <div id="logo">
                  <a>
                    <xsl:attribute name="href">
                      <xsl:text>/</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="title">
                      <xsl:text>to frontpage</xsl:text>
                    </xsl:attribute>
                    <img>
                      <xsl:attribute name="src">
                        <xsl:text>/Custom/Components/FrontEnd/img/slogo.gif</xsl:text>
                      </xsl:attribute>
                    </img>
                  </a>
                </div>
                <div id="topmenu">
                  <ul>
                    <xsl:choose>
                      <xsl:when test="contains(//data/attributes/pageroot, 'english')">
                        <!-- Begin Login (english) -->
                        <xsl:choose>
                          <xsl:when test="//data/basedata/currentuser/groups/group[contains(., 'admin')]">
                            <li>
                              <xsl:call-template name="LogOut" />
                            </li>
                          </xsl:when>
                          <xsl:otherwise>
                            <li>
                              <a>
                                <xsl:attribute name="href">
                                  <xsl:value-of select="/data/basepath" />
                                  <xsl:text>login/</xsl:text>
                                  <xsl:if test="/data/attributes/pagepath != ''">
                                    <xsl:text>?redirect=show/</xsl:text>
                                    <xsl:value-of select="/data/attributes/pagepath" />
                                  </xsl:if>
                                </xsl:attribute>
                                <span>Log in</span>
                                <span class="hide"> | </span>
                              </a>
                            </li>
                          </xsl:otherwise>
                        </xsl:choose>
                        <!-- End Login (english) -->

                        <li>
                          <a href="show/german/about/">
                            <span>German</span>
                            <span class="hide"> | </span>
                          </a>
                        </li>
                        <li>
                          <a href="show/english/imprint/">
                            <span>Imprint</span>
                            <span class="hide"> | </span>
                          </a>
                        </li>
                        <li>
                          <a href="show/english/imprint/sitemap/">
                            <span>Sitemap</span>
                            <span class="hide"> | </span>
                          </a>
                        </li>
                        <li>
                          <a href="show/english/imprint/contact/">
                            <span>Contact</span>
                            <span class="hide"> | </span>
                          </a>
                        </li>
                      </xsl:when>
                      <xsl:otherwise>
                        <!-- Begin Login (german) -->
                        <xsl:choose>
                          <xsl:when test="//data/basedata/currentuser/groups/group[contains(., 'admin')]">
                            <li>
                              <xsl:call-template name="LogOut" />
                            </li>
                          </xsl:when>
                          <xsl:otherwise>
                            <li>
                              <a>
                                <xsl:attribute name="href">
                                  <xsl:value-of select="/data/basepath" />
                                  <xsl:text>login/</xsl:text>
                                  <xsl:if test="/data/attributes/pagepath != ''">
                                    <xsl:text>?redirect=show/</xsl:text>
                                    <xsl:value-of select="/data/attributes/pagepath" />
                                  </xsl:if>
                                </xsl:attribute>
                                <span>Anmelden</span>
                                <span class="hide"> | </span>
                              </a>
                            </li>
                          </xsl:otherwise>
                        </xsl:choose>
                        <!-- End Login (german) -->

                        <li>
                          <a href="show/english/about/">
                            <span>English</span>
                            <span class="hide"> | </span>
                          </a>
                        </li>
                        <li>
                          <a href="show/german/imprint/">
                            <span>Impressum</span>
                            <span class="hide"> | </span>
                          </a>
                        </li>
                        <li>
                          <a href="show/german/imprint/sitemap/">
                            <span>Sitemap</span>
                            <span class="hide"> | </span>
                          </a>
                        </li>
                        <li>
                          <a href="show/german/imprint/contact/">
                            <span>Kontakt</span>
                            <span class="hide"> | </span>
                          </a>
                        </li>
                      </xsl:otherwise>
                    </xsl:choose>
                  </ul>
                </div>
                <div class="clear">
                  <xsl:text> </xsl:text>
                </div>
              </div>
              <div id="head2">
                <xsl:text> </xsl:text>
              </div>
              <div id="head3">
                <div class="teaser">
                  <xsl:choose>
                    <xsl:when test="contains(//data/attributes/pageroot, 'english')">
                      <p>
                        <xsl:text>This website hosts the open source content management system "sharpcms". It is a CMS purely based on XML &amp; XSLT.</xsl:text>
                      </p>
                    </xsl:when>
                    <xsl:otherwise>
                      <p>
                        <xsl:text>Diese Website präsentiert das Open Source Content Management System "sharpcms". Dies ist ein CMS das nur auf XML &amp; XSLT basiert.</xsl:text>
                      </p>
                    </xsl:otherwise>
                  </xsl:choose>
                </div>
              </div>
            </div>
            <xsl:call-template name="TopMenu" />
            <div id="main">
              <div id="col1">
                <div id="col1_content" class="clearfix">
                  <xsl:text> </xsl:text>
                </div>
              </div>
              <div id="col2">
                <div id="col2_content" class="clearfix">
                  <xsl:call-template name="SubMenu" />
                  <xsl:apply-templates mode="show" select="/data/contentplace/page/containers/container[@name='news']" />
                  <xsl:text> </xsl:text>
                </div>
              </div>
              <div id="col3">
                <div id="col3_content" class="clearfix">
                  <div id="contentcontainer">
                    <xsl:if test="/data/messages">
                      <div class="adminmenu" style="background-color:#ffffe1">
                        <ul>
                          <xsl:for-each select="/data/messages/item">
                            <li>
                              <xsl:value-of select="." />
                            </li>
                          </xsl:for-each>
                        </ul>
                      </div>
                    </xsl:if>
                    <xsl:apply-templates mode="show" select="/data/contentplace/page/containers/container[@name='content']" />
                    <xsl:text> </xsl:text>
                  </div>
                </div>
                <div id="ie_clearing">
                  <xsl:text disable-output-escaping="yes"> </xsl:text>
                </div>
              </div>
            </div>
            <div id="footer">
              <div id="fo2">
                <p>
                  <a href="http://validator.w3.org/check?uri=referer">
                    <img style="border:0;width:88px;height:31px" src="http://www.w3.org/Icons/valid-xhtml10" alt="Valid XHTML 1.0 Transitional" />
                  </a>
                  <span class="hide"> | </span>
                  <a href="http://jigsaw.w3.org/css-validator/check/referer/">
                    <img style="border:0;width:88px;height:31px" src="http://jigsaw.w3.org/css-validator/images/vcss" alt="Valid CSS!" />
                  </a>
                </p>
              </div>
              <div id="fo1">
                <p>
                  <xsl:choose>
                    <xsl:when test="contains(//data/attributes/pageroot, 'english')">
                      <a href="show/english/about/">Start</a> | <a href="show/english/imprint/">Imprint</a> | <a href="show/english/imprint/sitemap/">Sitemap</a> | <a href="show/english/imprint/contact/">Contact</a>
                    </xsl:when>
                    <xsl:otherwise>
                      <a href="show/german/about/">Start</a> | <a href="show/german/imprint/">Impressum</a> | <a href="show/german/imprint/sitemap/">Sitemap</a> | <a href="show/german/imprint/contact/">Kontakt</a>
                    </xsl:otherwise>
                  </xsl:choose>
                </p>
                <p>
                  <xsl:text>Powered by </xsl:text>
                  <a href="http://www.gutsch-online.de/" target="_blank">www.gutsch-online.de</a> and <a href="http://www.klickflupp.ch/" target="_blank">www.klickflupp.ch</a>
                </p>
                <div class="clear">
                  <xsl:text> </xsl:text>
                </div>
              </div>
            </div>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
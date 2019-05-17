<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template name="edit">
    <xsl:if test="//data/basedata/currentuser/groups/group[contains(., 'admin')]">
      <div>
        <xsl:attribute name="class">
          <xsl:text>editlink</xsl:text>
        </xsl:attribute>
        <a>
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath" />
            <xsl:text>/admin/page/edit/</xsl:text>
            <xsl:value-of select="/data/contentplace/page/attributes/pageidentifier" />
            <xsl:text>/?c=</xsl:text>
            <xsl:number count="container" />
            <xsl:text disable-output-escaping="no">&amp;e=</xsl:text>
            <xsl:number count="element" />
          </xsl:attribute>
          <span>
            <xsl:text>Edit</xsl:text>
          </span>
        </a>
      </div>
    </xsl:if>
  </xsl:template>

  <xsl:template name="LogOut">
    <form name="systemform" id="systemform" method="post" enctype="multipart/form-data">
      <input type="hidden" name="event_main" value="logout" />
      <input type="hidden" name="event_mainvalue" value="" />
      <input type="submit" value="Log out">
        <xsl:attribute name="class">
          <xsl:text>linkbutton bold</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:choose>
            <xsl:when test="contains(//data/attributes/pageroot, 'english')">
              <xsl:text>Log out</xsl:text>
            </xsl:when>
            <xsl:otherwise>
              <xsl:text>Abmelden</xsl:text>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
      </input>
    </form>
  </xsl:template>
  
	<xsl:template mode="show" match="element[@type='sitemap']">
    <xsl:variable name="language">
			<xsl:choose>
				<xsl:when test="contains(//data/attributes/pageroot, 'english')">
					<xsl:text>english</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>german</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<div id="sitemap">
			<xsl:for-each select="/data/navigationplace/sitetree/*[@pageidentifier=$language and @status='open']">
				<ul>
					<xsl:for-each select="*[@status='open']">
						<li>
							<a>
								<xsl:attribute name="href">
									<xsl:text>show/</xsl:text>
									<xsl:value-of select="@pageidentifier" />
									<xsl:text>/</xsl:text>
								</xsl:attribute>
								<xsl:value-of select="@pagename" />
							</a>
							<br />
							<span>
								<xsl:value-of select="@metadescription" />
							</span>
							<xsl:if test="*[@status='open']">
								<ul>
									<xsl:for-each select="*[@status='open']">
										<li>
											<a>
												<xsl:attribute name="href">
													<xsl:text>show/</xsl:text>
													<xsl:value-of select="@pageidentifier" />
													<xsl:text>/</xsl:text>
												</xsl:attribute>
												<xsl:value-of select="@pagename" />
											</a>
											<br />
											<span>
												<xsl:value-of select="@metadescription" />
											</span>
											<xsl:if test="*[@status='open']">
												<ul>
													<xsl:for-each select="*[@status='open']">
														<li>
															<a>
																<xsl:attribute name="href">
																	<xsl:text>show/</xsl:text>
																	<xsl:value-of select="@pageidentifier" />
																	<xsl:text>/</xsl:text>
																</xsl:attribute>
																<xsl:value-of select="@pagename" />
															</a>
															<br />
															<span>
																<xsl:value-of select="@metadescription" />
															</span>
															<xsl:if test="*[@status='open']">
																<ul>
																	<xsl:for-each select="*[@status='open']">
																		<li>
																			<a>
																				<xsl:attribute name="href">
																					<xsl:text>show/</xsl:text>
																					<xsl:value-of select="@pageidentifier" />
																					<xsl:text>/</xsl:text>
																				</xsl:attribute>
																				<xsl:value-of select="@pagename" />
																			</a>
																			<br />
																			<span>
																				<xsl:value-of select="@metadescription" />
																			</span>
																		</li>
																	</xsl:for-each>
																</ul>
															</xsl:if>
														</li>
													</xsl:for-each>
												</ul>
											</xsl:if>
										</li>
									</xsl:for-each>
								</ul>
							</xsl:if>
						</li>
					</xsl:for-each>
				</ul>
			</xsl:for-each>
		</div>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='block']">
    <dl>
			<xsl:if test="title and not(title='')">
				<dt>
					<xsl:choose>
						<xsl:when test="substring(link, 1, 1) = '#'">
							<a>
								<xsl:attribute name="id">
									<xsl:value-of select="link" />
								</xsl:attribute>
							</a>
						</xsl:when>
					</xsl:choose>
					<xsl:choose>
						<xsl:when test="link and not(link='') and not(substring(link, 1, 1) = '#')">
							<a>
								<xsl:attribute name="href">
									<xsl:value-of select="link" />
								</xsl:attribute>
								<xsl:value-of select="title" />
							</a>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="title" />
						</xsl:otherwise>
					</xsl:choose>
				</dt>
			</xsl:if>

			<xsl:if test="picture and not(picture='')">
				<dd class="img">
					<xsl:choose>
						<xsl:when test="link and not(link='') and not(substring(link, 1, 1) = '#')">
							<a>
								<xsl:attribute name="href">
									<xsl:value-of select="link" />
								</xsl:attribute>
								<img>
									<xsl:attribute name="src">
										<xsl:text>/?process=download/</xsl:text>
										<xsl:value-of select="picture" />
										<xsl:text>&amp;width=150</xsl:text>
									</xsl:attribute>
									<xsl:attribute name="alt">
										<xsl:value-of select="title" />
									</xsl:attribute>
								</img>
							</a>
						</xsl:when>
						<xsl:otherwise>
							<img>
								<xsl:attribute name="src">
									<xsl:text>/?process=download/</xsl:text>
									<xsl:value-of select="picture" />
									<xsl:text>&amp;height=150</xsl:text>
								</xsl:attribute>
								<xsl:attribute name="alt">
									<xsl:value-of select="title" />
								</xsl:attribute>
							</img>
						</xsl:otherwise>
					</xsl:choose>
				</dd>
			</xsl:if>
			<dd>
				<xsl:value-of select="text" disable-output-escaping="yes" />
			</dd>
		</dl>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='gallery']">
    <xsl:variable select="folder/@path" name="path" />
		<xsl:if test="//data/query/data/picture">
			<img>
				<xsl:attribute name="src">
					<xsl:text>/?process=download/</xsl:text>
					<xsl:value-of select="$path" />
					<xsl:text>/</xsl:text>
					<xsl:value-of select="//data/query/data/picture" />
					<xsl:text>&amp;width=250</xsl:text>
				</xsl:attribute>
			</img>
			<br />
		</xsl:if>
		<xsl:for-each select="folder/folder/file[@extension='.jpg']">
			<a>
				<xsl:attribute name="href">
					<xsl:text>/?process=</xsl:text>
					<xsl:value-of select="//data/query/other/process" />
					<xsl:text>&amp;data_picture=</xsl:text>
					<xsl:value-of select="@name" />
				</xsl:attribute>
				<img border="0">
					<xsl:attribute name="src">
						<xsl:text>/?process=download/</xsl:text>
						<xsl:value-of select="$path" />
						<xsl:text>/</xsl:text>
						<xsl:value-of select="@name" />
						<xsl:text>&amp;width=50&amp;height=50</xsl:text>
					</xsl:attribute>
				</img>
			</a>
		</xsl:for-each>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='paragraph']">
    <xsl:if test="picture and not(picture='')">
			<img>
				<xsl:attribute name="src">
					<xsl:text>/?process=download/</xsl:text>
					<xsl:value-of select="picture" />
				</xsl:attribute>
				<xsl:attribute name="style">
					<xsl:choose>
						<xsl:when test="picturepos='Left'">
							<xsl:text>float:left;margin-top:0;margin-bottom:10px;margin-right:10px;</xsl:text>
						</xsl:when>
						<xsl:when test="picturepos='Right'">
							<xsl:text>float:right;margin-top:0;margin-bottom:10px;margin-left:10px;</xsl:text>
						</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</img>
		</xsl:if>
		<xsl:value-of select="text" disable-output-escaping="yes" />
		<xsl:if test="not(link = '') and link">
			<a>
				<xsl:choose>
					<xsl:when test="substring(link, 1, 7) = 'http://'">
						<xsl:attribute name="href">
							<xsl:value-of select="link" />
						</xsl:attribute>
						<xsl:attribute name="target">
							<xsl:text>_blank</xsl:text>
						</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="href">
							<xsl:text>/?process=show/</xsl:text>
							<xsl:value-of select="link" />
						</xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:text>Read more</xsl:text>
			</a>
		</xsl:if>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='header']">
    <xsl:if test="text and not(text='')">
			<xsl:choose>
				<xsl:when test="headerstyle='Header1'">
					<h1>
						<xsl:value-of select="text" />
					</h1>
				</xsl:when>
				<xsl:when test="headerstyle='Header2'">
					<h2>
						<xsl:value-of select="text" />
					</h2>
				</xsl:when>
				<xsl:when test="headerstyle='Header3'">
					<h3>
						<xsl:value-of select="text" />
					</h3>
				</xsl:when>
			</xsl:choose>
		</xsl:if>
		<xsl:choose>
			<xsl:when test="offset='1 line'">
				<p>
					<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
				</p>
			</xsl:when>
			<xsl:when test="offset='2 lines'">
				<p>
					<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
				</p>
				<p>
					<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
				</p>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='picture']">
    <xsl:if test="picture and not(picture='')">
			<p>
				<img style="border:none 0px;">
					<xsl:attribute name="src">
						<xsl:text>/?process=download/</xsl:text>
						<xsl:value-of select="picture" />
					</xsl:attribute>
					<xsl:attribute name="alt">
						<xsl:value-of select="alttext" />
					</xsl:attribute>
				</img>
				<xsl:if test="not(text = '') and text">
					<br />
					<xsl:value-of select="text" />
				</xsl:if>
			</p>
		</xsl:if>
		<p>
			<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
		</p>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='iframe']">
    <iframe>
			<xsl:if test="width and not(width='')">
				<xsl:attribute name="width">
					<xsl:value-of select="width" />
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="height and not(height='')">
				<xsl:attribute name="height">
					<xsl:value-of select="height" />
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="border and not(border='')">
				<xsl:attribute name="style">
					<xsl:text>border:solid </xsl:text>
					<xsl:value-of select="border" />
					<xsl:text>px #333333;</xsl:text>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="url and not(url='')">
				<xsl:attribute name="src">
					<xsl:value-of select="url" />
				</xsl:attribute>
			</xsl:if>
		</iframe>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='code']">
    <pre>
			<xsl:if test="not(text = '') and text">
				<xsl:value-of disable-output-escaping="yes" select="text" />
			</xsl:if>
		</pre>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='form']">
    <form name="systemform" id="systemform" method="post" enctype="multipart/form-data">
			<xsl:attribute name="action">
				<xsl:value-of select="/data/query/other/process" />
				<xsl:text disable-output-escaping="yes">/</xsl:text>
			</xsl:attribute>

			<script type="text/javascript">
				<xsl:text disable-output-escaping="yes"><![CDATA[
				<!--
					function validate()
					{
						//if (document.getElementById("data_form_Vorname_").value=="")
						//{
						//	alert("Bitte geben Sie Ihren \"Vornamen\" an!");
						//	document.getElementById("data_form_Vorname_").focus();
						//	return false;
						//}
						//if (document.getElementById("data_form_Nachname_").value=="")
						//{
						//	alert("Bitte geben Sie Ihren \"Nachnamen\" an!");
						//	document.getElementById("data_form_Nachname_").focus();
						//	return false;
						//}
						//if (document.getElementById("data_form_Email_").value=="")
						//{
						//	alert("Ihre \"Emailadresse\" fehlt!");
						//	document.getElementById("data_form_Email_").focus();
						//	return false;
						//}
						//if (document.getElementById("data_form_Kommentar").value=="" || document.getElementById("data_form_Kommentar").value==" ")
						//{
						//	alert("Ohne \"Kommentar\" macht das Formular keinen Sinn!");
						//	document.getElementById("data_form_Kommentar").focus();
						//	return false;
						//}
						return true;
					}
				//-->
				]]></xsl:text>
			</script>
			<!-- primary hidden settings -->
			<input type="hidden" name="event_main" value="submitform" />
			<input type="hidden" name="event_mainvalue" value="" />
			<input type="hidden" name="process">
				<xsl:attribute name="value">
					<xsl:value-of select="data/query/other/process" />
				</xsl:attribute>
			</input>
			<input type="hidden" name="data_pageidentifier">
				<xsl:attribute name="value">
					<xsl:value-of select="/data/contentplace/page/attributes/pageidentifier" />
				</xsl:attribute>
			</input>
			<dl>
				<dt>
					<xsl:choose>
						<xsl:when test="contains(//data/attributes/pageroot, 'english')">
							<label for="data_form_Anrede" accesskey="A">
								<xsl:text>Title</xsl:text>
							</label>
						</xsl:when>
						<xsl:otherwise>
							<label for="data_form_Anrede" accesskey="A">
								<xsl:text>Anrede</xsl:text>
							</label>
						</xsl:otherwise>
					</xsl:choose>
				</dt>
				<dd>
					<select id="data_form_Anrede" name="data_form_Anrede">
						<xsl:for-each select="category-list/*" >
							<option>
								<xsl:attribute name="value">
									<xsl:value-of select="." />
								</xsl:attribute>
								<xsl:value-of select="." />
							</option>
						</xsl:for-each>
					</select>
				</dd>
				<xsl:for-each select="element-list/*">
					<dt>
						<label>
							<xsl:attribute name="for">
								<xsl:text>data_form_</xsl:text>
								<xsl:value-of select="@id" />
							</xsl:attribute>
							<xsl:value-of select="." />
						</label>
					</dt>
					<dd>
						<input type="text" value="">
							<xsl:attribute name="name">
								<xsl:text>data_form_</xsl:text>
								<xsl:value-of select="@id" />
							</xsl:attribute>
							<xsl:attribute name="id">
								<xsl:text>data_form_</xsl:text>
								<xsl:value-of select="@id" />
							</xsl:attribute>
						</input>
					</dd>
				</xsl:for-each>
				<dt>
					<xsl:choose>
						<xsl:when test="contains(//data/attributes/pageroot, 'english')">
							<label for="data_form_Kommentar">
								<xsl:text>Message</xsl:text>
							</label>
						</xsl:when>
						<xsl:otherwise>
							<label for="data_form_Kommentar">
								<xsl:text>Kommentar</xsl:text>
							</label>
						</xsl:otherwise>
					</xsl:choose>
				</dt>
				<dd>
					<textarea name="data_form_Kommentar" id="data_form_Kommentar" cols="50" rows="10">
						<xsl:text> </xsl:text>
					</textarea>
				</dd>
				<dt>
					<xsl:text disable-output-escaping="yes"> </xsl:text>
				</dt>
				<dd>
					<input type="submit" value="Senden" name="sub" class="but" />
				</dd>
			</dl>
		</form>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='freetext']">
    <xsl:if test="not(text = '') and text">
			<xsl:value-of disable-output-escaping="yes" select="text" />
		</xsl:if>
	</xsl:template>

  <xsl:template mode="show" match="element[@type='rssfeed']">
    <xsl:if test="not(url = '') and url">
			<xsl:if test="document(url)">
				<xsl:for-each select="document(url)/rss">
					<xsl:apply-templates />
				</xsl:for-each>
			</xsl:if>
		</xsl:if>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='faqhead']">
    <xsl:value-of select="text" disable-output-escaping="yes" />
		<ul>
			<xsl:for-each select="//element[@type='faq']">
				<li>
					<xsl:value-of select="position()" />.
					<a>
						<xsl:attribute name="href">
							<xsl:value-of select="/data/query/other/process" />
							<xsl:text>/#</xsl:text>
							<xsl:value-of select="position()" />
						</xsl:attribute>
						<xsl:value-of select="question" />
					</a>
				</li>
				<xsl:value-of select="text" disable-output-escaping="yes" />
			</xsl:for-each>
		</ul>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='faq']">
    <p class="contentHeader" style="margin-bottom:0px;">
			<b>
				<xsl:value-of select="position()-2" />.
				<xsl:value-of select="question" />
			</b>
			<a>
				<xsl:attribute name="name">
					<xsl:value-of select="position()-1" />
				</xsl:attribute>
				<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
			</a>
		</p>
		<xsl:value-of select="answer" disable-output-escaping="yes" />
	</xsl:template>

	<xsl:template mode="show" match="element[@type='overview']">
    <xsl:for-each select="/data/navigationplace/sitetree/*[@inpath='true' and @status='open']/*[@inpath='true' and @status='open']/*[@inpath='true' and @status='open']">
			<ul class="overview">
				<xsl:apply-templates mode="overviewlistitems" select="*[@status='open']" />
			</ul>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="*" mode="overviewlistitems">
    <li>
			<xsl:if test="not(@lastedited = '') and @lastedited">
				<xsl:value-of select="@lastedited" />
				<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
			</xsl:if>
			<a>
				<xsl:attribute name="href">
					<xsl:value-of select="/data/query/other/process" />
					<xsl:text>/</xsl:text>
					<xsl:value-of select="name()" />
					<xsl:text>/</xsl:text>
				</xsl:attribute>
				<xsl:value-of select="@pagename" />
			</a>
			<xsl:text disable-output-escaping="yes">&lt;br/&gt;</xsl:text>
			<xsl:if test="not(@metadescription = '') and @metadescription">
				<xsl:value-of select="@metadescription" />
				<xsl:text disable-output-escaping="yes">&lt;br/&gt;</xsl:text>
			</xsl:if>
			<a>
				<xsl:attribute name="href">
					<xsl:value-of select="/data/query/other/process" />
					<xsl:text>/</xsl:text>
					<xsl:value-of select="name()" />
					<xsl:text>/</xsl:text>
				</xsl:attribute>
				Read more
			</a>
			<xsl:text disable-output-escaping="yes">&lt;br/&gt;</xsl:text>
			<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
		</li>
	</xsl:template>
  
  <xsl:template mode="show" match="*">
    <xsl:for-each select="elements/element">
      <xsl:call-template name="edit" />
      <xsl:if test="@publish = '' or @publish = 'true' or //data/basedata/currentuser/groups/group[contains(., 'admin')]">
        <xsl:apply-templates mode="show" select="." />
      </xsl:if>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>
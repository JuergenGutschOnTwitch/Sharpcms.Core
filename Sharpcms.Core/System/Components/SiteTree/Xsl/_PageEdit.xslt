<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />
  
  <xsl:template mode="edit" match="page">
    <input type="hidden" name="data_pageidentifier">
      <xsl:attribute name="value">
        <xsl:value-of select="attributes/pageidentifier" />
      </xsl:attribute>
    </input>
    <input type="hidden" name="page">
      <xsl:attribute name="value">
        <xsl:value-of select="attributes/pageidentifier" />
      </xsl:attribute>
    </input>
    <div class="head pagedata_head">
      <div class="title">
        <b>
          <xsl:text>Page data - </xsl:text>
          <xsl:value-of select="attributes/menuname" />
        </b>
      </div>
      <div class="viewstate">
        <a id="pada_vs" class="button expand">
          <xsl:text>˅</xsl:text>
        </a>
      </div>
    </div>
    <div class="menu pagedata_menu top">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlSave</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="style">
          <xsl:text>width: 29px;</xsl:text>
        </xsl:attribute>
        <xsl:text>Save</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlSaveAndShow</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="style">
          <xsl:text>width: 87px;</xsl:text>
        </xsl:attribute>
        <xsl:text>Save and show</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlShow</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="style">
          <xsl:text>width: 31px;</xsl:text>
        </xsl:attribute>
        <xsl:text>Show</xsl:text>
      </a>
      <select id="adminmoreactions">
        <option data-action="" value="">
          <xsl:text>More actions...</xsl:text>
        </option>
        <option data-action="setstandardpage" value="">
          <xsl:text>&#160;&#160;Set as Start Page</xsl:text>
        </option>
        <option data-action="removepage" value="{attributes/pageidentifier}">
          <xsl:text>&#160;&#160;Delete Page</xsl:text>
        </option>
        <option data-action="addpage" value="{attributes/pageidentifier}">
          <xsl:text>&#160;&#160;Add Subpage</xsl:text>
        </option>
        <option data-action="pagecreatcontainer" value="">
          <xsl:text>&#160;&#160;Add Container</xsl:text>
        </option>
        <option disabled="disabled" />
        <option disabled="disabled">Move / Copy</option>
        <option data-action="pagemoveup" value="{attributes/pageidentifier}">
          <xsl:text>&#160;&#160;Move up</xsl:text>
        </option>
        <option data-action="pagemovedown" value="{attributes/pageidentifier}">
          <xsl:text>&#160;&#160;Move down</xsl:text>
        </option>
        <option data-action="pagemovetop" value="{attributes/pageidentifier}">
          <xsl:text>&#160;&#160;Move Top</xsl:text>
        </option>
        <option data-action="pagemovebottom" value="{attributes/pageidentifier}">
          <xsl:text>&#160;&#160;Move Bottom</xsl:text>
        </option>
        <option data-action="pagemove" value="">
          <xsl:text>&#160;&#160;Move To</xsl:text>
        </option>
        <option data-action="pagecopyto">
          <xsl:attribute name="value">
            <xsl:value-of select="attributes/pageidentifier" />
            <xsl:text>;;</xsl:text>
            <xsl:value-of select="/data/contentplace/page/attributes/pagename" />
          </xsl:attribute>
          <xsl:text>&#160;&#160;Copy To</xsl:text>
        </option>
      </select>
    </div>
    <div class="tab-pane" id="pada_body" style="float: left;">
      <div id="pada_body_tabs" class="tabs">
        <ul>
          <li>
            <a href="#ptabs1">
              <xsl:text>Page Data</xsl:text>
            </a>
          </li>
          <li>
            <a href="#ptabs2">
              <xsl:text>Editor Security</xsl:text>
            </a>
          </li>
          <li>
            <a href="#ptabs3">
              <xsl:text>Extranet Security</xsl:text>
            </a>
          </li>
        </ul>
        <div id="ptabs1" class="tab-page">
          <div class="tab_page_body">
            <xsl:call-template name="topelements" />
          </div>
        </div>
        <div id="ptabs2" class="tab-page">
          <div class="tab_page_body">
            <xsl:call-template name="security">
              <xsl:with-param name="view">
                <xsl:text>editors</xsl:text>
              </xsl:with-param>
            </xsl:call-template>
          </div>
        </div>
        <div id="ptabs3" class="tab-page">
          <div class="tab_page_body">
            <xsl:call-template name="security">
              <xsl:with-param name="view">
                <xsl:text>visitors</xsl:text>
              </xsl:with-param>
            </xsl:call-template>
          </div>
        </div>
      </div>
    </div>
    <div id="tabpagecontent" class="tab-pane" style="float: left;">
      <div class="tabs">
        <ul>
          <xsl:apply-templates mode="tab" select="containers/container" />
        </ul>
        <xsl:apply-templates mode="panel" select="containers/container" />
      </div>
    </div>
    <div class="menu pagedata_menu bottom">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlSave</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="style">
          <xsl:text>width: 29px;</xsl:text>
        </xsl:attribute>
        <xsl:text>Save</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlSaveAndShow</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="style">
          <xsl:text>width: 87px;</xsl:text>
        </xsl:attribute>
        <xsl:text>Save and show</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlShow</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="style">
          <xsl:text>width: 31px;</xsl:text>
        </xsl:attribute>
        <xsl:text>Show</xsl:text>
      </a>
    </div>
  </xsl:template>

  <xsl:template mode="tab" match="container">
    <li>
      <a>
        <xsl:attribute name="href">
          <xsl:text>#tabs-</xsl:text>
          <xsl:number level="any" />
        </xsl:attribute>
        <xsl:value-of select="@name" />
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>delete hlRemoveContrainer</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="title">
          <xsl:text>This will remove the container</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-containerid">
          <xsl:number count="container" />
        </xsl:attribute>
        <xsl:text>X</xsl:text>
      </a>
    </li>
  </xsl:template>

  <xsl:template mode="panel" match="container">
    <div class="tab-page">
      <xsl:attribute name="id">
        <xsl:text>tabs-</xsl:text>
        <xsl:number level="any" />
      </xsl:attribute>
      <div class="menu container_menu">
        <select>
          <xsl:attribute name="class">
            <xsl:text>addelement</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="data-name">
            <xsl:text>data_container_</xsl:text>
            <xsl:number count="container" />
          </xsl:attribute>
          <option>
            <xsl:text>Add element...</xsl:text>
          </option>
          <xsl:for-each select="//data/basedata/elementlist/*[not(name()='top') and (@name=@filter or not(@filter)) ]">
            <option>
              <xsl:attribute name="value">
                <xsl:value-of select="name()" />
              </xsl:attribute>
              <xsl:text>&#160;&#160;</xsl:text>
              <xsl:value-of select="name()" />
            </option>
          </xsl:for-each>
        </select>
      </div>
      <xsl:apply-templates mode="form" select="elements/element" />
    </div>
  </xsl:template>

  <xsl:template mode="form" match="element">
    <xsl:call-template name="elementhead" />
    <xsl:call-template name="elementbody" />
  </xsl:template>

  <xsl:template name="security">
    <xsl:param name="view" />
    <label>
      <xsl:text>Allow Groups or Users to </xsl:text>
      <xsl:choose>
        <xsl:when test="$view='editors'">
          <xsl:text>Edit</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>View</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:text> the current Page</xsl:text>
    </label>
    <div class="item">
      <ul class="checkbox_list">
        <xsl:for-each select="//data/basedata/security/groups/*">
          <li>
            <input type="checkbox" value="user1" class="checkbox">
              <xsl:attribute name="id">
                <xsl:text>checkbox_</xsl:text>
                <xsl:value-of select="$view"/>
                <xsl:text>_group_</xsl:text>
                <xsl:value-of select="."/>
              </xsl:attribute>
              <xsl:attribute name="name">
                <xsl:text>checkbox_</xsl:text>
                <xsl:value-of select="$view"/>
                <xsl:text>_group_</xsl:text>
                <xsl:value-of select="."/>
              </xsl:attribute>
            </input>
            <label>
              <xsl:attribute name="for">
                <xsl:text>checkbox_</xsl:text>
                <xsl:value-of select="$view"/>
                <xsl:text>_group_</xsl:text>
                <xsl:value-of select="."/>
              </xsl:attribute>
              <xsl:text>&lt;</xsl:text>
              <xsl:value-of select="." />
              <xsl:text>&gt;</xsl:text>
            </label>
          </li>
        </xsl:for-each>
        <xsl:for-each select="//data/basedata/security/users/*">
          <li>
            <input type="checkbox" value="user1" class="checkbox">
              <xsl:attribute name="id">
                <xsl:text>checkbox_</xsl:text>
                <xsl:value-of select="$view"/>
                <xsl:text>_user_</xsl:text>
                <xsl:value-of select="."/>
              </xsl:attribute>
              <xsl:attribute name="name">
                <xsl:text>checkbox_</xsl:text>
                <xsl:value-of select="$view"/>
                <xsl:text>_user_</xsl:text>
                <xsl:value-of select="."/>
              </xsl:attribute>
            </input>
            <label>
              <xsl:attribute name="for">
                <xsl:text>checkbox_</xsl:text>
                <xsl:value-of select="$view"/>
                <xsl:text>_user_</xsl:text>
                <xsl:value-of select="."/>
              </xsl:attribute>
              <xsl:value-of select="." />
            </label>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>

  <xsl:template name="topelements">
    <xsl:variable name="currentelement" select="." />
    <xsl:variable name="type" select="@type" />
    <xsl:apply-templates mode="formelement" select="//data/basedata/elementlist/top/*[name(//*[@currentpage='true']/..)=@filter or not(@filter) ]">
      <xsl:with-param name="currentelement" select="//data/contentplace/page/attributes" />
      <xsl:with-param name="id">
        <xsl:text>data_attribute</xsl:text>
      </xsl:with-param>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='tinymce']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of  select="@name" />
    </label>
    <div class="item">
      <textarea class="mceeditor" name="{$id}_{@attribute}">
        <xsl:if test="@height">
          <xsl:attribute name="style">
            <xsl:text>width:618px; height:</xsl:text>
            <xsl:value-of select="@height" />
            <xsl:text>px;</xsl:text>
          </xsl:attribute>
        </xsl:if>
        <xsl:value-of select="$currentelement/*[name()=$attribute]" />
      </textarea>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='textarea']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <textarea name="{$id}_{@attribute}">
        <xsl:if test="@height or $id='data_attribute'">
          <xsl:attribute name="class">
            <xsl:text>simple</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="style">
            <xsl:if test="@height and not(@height='')">
              <xsl:text>height:</xsl:text>
              <xsl:value-of select="@height" />
              <xsl:text>px;</xsl:text>
            </xsl:if>
          </xsl:attribute>
        </xsl:if>
        <xsl:value-of select="$currentelement/*[name()=$attribute]" />
        <xsl:text> </xsl:text>
      </textarea>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='text']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <input type="text" name="{$id}_{@attribute}" value="{$currentelement/*[name()=$attribute]}" />
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='statuslist']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <xsl:variable name="status" select="$currentelement/*[name()=$attribute]" />
      <select>
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />
          <xsl:text>_</xsl:text>
          <xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:for-each select="//data/basedata/pagestatus/*">
          <option >
            <xsl:attribute name="value">
              <xsl:value-of select="name()" />
            </xsl:attribute>
            <xsl:if test="$status=name()">
              <xsl:attribute name="selected">
                <xsl:text>selected</xsl:text>
              </xsl:attribute>
            </xsl:if>
            <xsl:value-of select="." />
          </option>
        </xsl:for-each>
      </select>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='listtemplate']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <xsl:variable name="status" select="$currentelement/*[name()=$attribute]" />
      <select>
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />
          <xsl:text>_</xsl:text>
          <xsl:value-of select="@attribute" />
        </xsl:attribute>
        <option value="">
          <xsl:text>Default</xsl:text>
        </option>
        <xsl:for-each select="/data/templates/*">
          <option >
            <xsl:attribute name="value">
              <xsl:value-of select="name()" />
            </xsl:attribute>
            <xsl:if test="$status=name()">
              <xsl:attribute name="selected">
                <xsl:text>selected</xsl:text>
              </xsl:attribute>
            </xsl:if>
            <xsl:value-of select="name()" />
          </option>
        </xsl:for-each>
      </select>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='hidden']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <input type="hidden">
      <xsl:attribute name="name">
        <xsl:value-of select="$id" />
        <xsl:text>_</xsl:text>
        <xsl:value-of select="@attribute" />
      </xsl:attribute>
      <xsl:attribute name="value">
        <xsl:value-of select="@value" />
      </xsl:attribute>
    </input>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='date']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <input class="date_day" type="text" size="2">
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />
          <xsl:text>_</xsl:text>
          <xsl:value-of select="concat(@attribute, 'day')" />
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="$currentelement/*[name() = concat($attribute, 'day')]" />
        </xsl:attribute>
      </input>
      <xsl:text>. </xsl:text>
      <input class="date_month" type="text" size="2">
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />
          <xsl:text>_</xsl:text>
          <xsl:value-of select="concat(@attribute, 'month')" />
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="$currentelement/*[name() = concat($attribute, 'month')]" />
        </xsl:attribute>
      </input>
      <xsl:text> </xsl:text>
      <input class="date_year" type="text" size="4">
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />
          <xsl:text>_</xsl:text>
          <xsl:value-of select="concat(@attribute, 'year')" />
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="$currentelement/*[name() = concat($attribute, 'year')]" />
        </xsl:attribute>
      </input>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='choosepage']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <input type="text">
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />
          <xsl:text>_</xsl:text>
          <xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="$currentelement/*[name()=$attribute]" />
        </xsl:attribute>
      </input>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlChoosePage</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-value">
          <xsl:value-of select="$id" />
          <xsl:text>;;</xsl:text>
          <xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:text>Choose</xsl:text>
      </a>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='choosefile']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <input type="text">
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />
          <xsl:text>_</xsl:text>
          <xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="$currentelement/*[name()=$attribute]" />
        </xsl:attribute>
      </input>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlChooseFile</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-value">
          <xsl:value-of select="$id" />
          <xsl:text>;;</xsl:text>
          <xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:text>Choose</xsl:text>
      </a>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='chooseimage']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <input type="text">
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />
          <xsl:text>_</xsl:text>
          <xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="$currentelement/*[name()=$attribute]" />
        </xsl:attribute>
      </input>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlChooseImage</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-value">
          <xsl:value-of select="$id" />
          <xsl:text>;;</xsl:text>
          <xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:text>Choose</xsl:text>
      </a>
    </div>
  </xsl:template>
  
  <xsl:template mode="formelement" match="item[@type='choosefolder']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <input type="text" name="{$id}_{@attribute}" value="{$currentelement/*[name()=$attribute]}" />
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlChooseFolder</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="data-value">
          <xsl:value-of select="$id" />
          <xsl:text>;;</xsl:text>
          <xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:text>Choose</xsl:text>
      </a>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='list']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <select>
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:for-each select="*">
          <option>
            <xsl:if test="($currentelement/*[name()=$attribute]) = .">
              <xsl:attribute name="selected">
                <xsl:text>selected</xsl:text>
              </xsl:attribute>
            </xsl:if>
            <xsl:attribute name="value">
              <xsl:value-of select="." />
            </xsl:attribute>
            <xsl:value-of select="." />
          </option>
        </xsl:for-each>
      </select>
    </div>
  </xsl:template>

  <xsl:template name="elementhead">
    <div>
      <xsl:attribute name="class">
        <xsl:text>menu element_head</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="id">
        <xsl:text>element_</xsl:text>
        <xsl:number count="container" />
        <xsl:text>_</xsl:text>
        <xsl:number count="element" />
        <xsl:text>_anchor</xsl:text>
      </xsl:attribute>
      <div>
        <xsl:attribute name="class">
          <xsl:text>name</xsl:text>
        </xsl:attribute>
        <label>
          <xsl:attribute name="class">
            <xsl:text>title</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="for">
            <xsl:text>data_element_</xsl:text>
            <xsl:number count="container" />
            <xsl:text>_</xsl:text>
            <xsl:number count="element" />
            <xsl:text>_elementtitle</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="@type" />
          <xsl:text>:</xsl:text>
        </label>
        <input>
          <xsl:attribute name="type">
            <xsl:text>text</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="name">
            <xsl:text>data_element_</xsl:text>
            <xsl:number count="container" />
            <xsl:text>_</xsl:text>
            <xsl:number count="element" />
            <xsl:text>_elementtitle</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="id">
            <xsl:text>data_element_</xsl:text>
            <xsl:number count="container" />
            <xsl:text>_</xsl:text>
            <xsl:number count="element" />
            <xsl:text>_elementtitle</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="value">
            <xsl:value-of select="@name" />
          </xsl:attribute>
        </input>
      </div>
      <div>
        <xsl:attribute name="class">
          <xsl:text>buttons</xsl:text>
        </xsl:attribute>
        <a>
          <xsl:attribute name="class">
            <xsl:text>button expand</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="id">
            <xsl:text>element_</xsl:text>
            <xsl:number count="container" />
            <xsl:text>_</xsl:text>
            <xsl:number count="element" />
          </xsl:attribute>
          <xsl:attribute name="title">
            <xsl:text>expand</xsl:text>
          </xsl:attribute>
          <xsl:text>˅</xsl:text>
        </a>
        <a>
          <xsl:attribute name="class">
            <xsl:text>button hlMoveElementTop</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="data-containerid">
            <xsl:number count="container" />
          </xsl:attribute>
          <xsl:attribute name="data-elementid">
            <xsl:number count="element" />
          </xsl:attribute>
          <xsl:text>Top</xsl:text>
        </a>
        <a>
          <xsl:attribute name="class">
            <xsl:text>button hlMoveElementDown</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="data-containerid">
            <xsl:number count="container" />
          </xsl:attribute>
          <xsl:attribute name="data-elementid">
            <xsl:number count="element" />
          </xsl:attribute>
          <xsl:text>Down</xsl:text>
        </a>
        <a>
          <xsl:attribute name="class">
            <xsl:text>button hlMoveElementUp</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="data-containerid">
            <xsl:number count="container" />
          </xsl:attribute>
          <xsl:attribute name="data-elementid">
            <xsl:number count="element" />
          </xsl:attribute>
          <xsl:text>Up</xsl:text>
        </a>
        <a>
          <xsl:attribute name="class">
            <xsl:text>button hlCopyElement</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="data-containerid">
            <xsl:number count="container" />
          </xsl:attribute>
          <xsl:attribute name="data-elementid">
            <xsl:number count="element" />
          </xsl:attribute>
          <xsl:text>Copy</xsl:text>
        </a>
        <a>
          <xsl:attribute name="class">
            <xsl:text>button delete hlRemoveElement</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="title">
            <xsl:text>delete the element</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="data-containerid">
            <xsl:number count="container" />
          </xsl:attribute>
          <xsl:attribute name="data-elementid">
            <xsl:number count="element" />
          </xsl:attribute>
          <xsl:text>X</xsl:text>
        </a>
      </div>
      <div>
        <xsl:attribute name="class">
          <xsl:text>publish</xsl:text>
        </xsl:attribute>
        <label>
          <xsl:attribute name="for">
            <xsl:text>data_element_</xsl:text>
            <xsl:number count="container" />
            <xsl:text>_</xsl:text>
            <xsl:number count="element" />
            <xsl:text>_elementpublish</xsl:text>
          </xsl:attribute>
          <xsl:text>Published</xsl:text>
        </label>
        <input>
          <xsl:attribute name="class">
            <xsl:text>checkbox</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="type">
            <xsl:text>checkbox</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="name">
            <xsl:text>data_element_</xsl:text>
            <xsl:number count="container" />
            <xsl:text>_</xsl:text>
            <xsl:number count="element" />
            <xsl:text>_elementpublish</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="id">
            <xsl:text>data_element_</xsl:text>
            <xsl:number count="container" />
            <xsl:text>_</xsl:text>
            <xsl:number count="element" />
            <xsl:text>_elementpublish</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="value">
            <xsl:text>Publish</xsl:text>
          </xsl:attribute>
          <xsl:if test="@publish='true'">
            <xsl:attribute name="checked">
              <xsl:text>true</xsl:text>
            </xsl:attribute>
          </xsl:if>
        </input>
      </div>
    </div>
  </xsl:template>

  <xsl:template name="elementbody">
    <div>
      <xsl:attribute name="class">
        <xsl:text>element_body</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="id">
        <xsl:text>element_</xsl:text>
        <xsl:number count="container" />
        <xsl:text>_</xsl:text>
        <xsl:number count="element" />
        <xsl:text>_body</xsl:text>
      </xsl:attribute>
      <div>
        <xsl:attribute name="class">
          <xsl:text>element_info</xsl:text>
        </xsl:attribute>
        <xsl:text> </xsl:text>
      </div>
      <div>
        <xsl:attribute name="class">
          <xsl:text>element_edit</xsl:text>
        </xsl:attribute>
        <xsl:variable name="currentelement" select="." />
        <xsl:variable name="type" select="@type" />
        <xsl:apply-templates mode="formelement" select="//data/basedata/elementlist/*[name()=$type]/*">
          <xsl:with-param name="currentelement" select="$currentelement" />
          <xsl:with-param name="id">
            <xsl:text>data_element_</xsl:text>
            <xsl:number count="container" />_<xsl:number count="element" />
          </xsl:with-param>
        </xsl:apply-templates>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>
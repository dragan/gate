﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Gate.Helpers
{
    public partial class ShowExceptions
    {
        static void ErrorPage(IDictionary<string,object> env, Response response, Exception ex)
        {
            var request = new Request(env);
            var path = request.PathBase + request.Path;

            // adapted from Django <djangoproject.com>
            // Copyright (c) 2005, the Lawrence Journal-World
            // Used under the modified BSD license:
            // http://www.xfree86.org/3.3.6/COPYRIGHT2.html#5
            response.Write(@"
<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
<html lang=""en"">
<head>
  <meta http-equiv=""content-type"" content=""text/html; charset=utf-8"" />
  <meta name=""robots"" content=""NONE,NOARCHIVE"" />
  <title>");
response.Write(h(ex.GetType().Name));
response.Write(@" at ");
response.Write(h(path));
response.Write(@"</title>
  <style type=""text/css"">
    html * { padding:0; margin:0; }
    body * { padding:10px 20px; }
    body * * { padding:0; }
    body { font:small sans-serif; }
    body>div { border-bottom:1px solid #ddd; }
    h1 { font-weight:normal; }
    h2 { margin-bottom:.8em; }
    h2 span { font-size:80%; color:#666; font-weight:normal; }
    h3 { margin:1em 0 .5em 0; }
    h4 { margin:0 0 .5em 0; font-weight: normal; }
    table {
        border:1px solid #ccc; border-collapse: collapse; background:white; }
    tbody td, tbody th { vertical-align:top; padding:2px 3px; }
    thead th {
        padding:1px 6px 1px 3px; background:#fefefe; text-align:left;
        font-weight:normal; font-size:11px; border:1px solid #ddd; }
    tbody th { text-align:right; color:#666; padding-right:.5em; }
    table.vars { margin:5px 0 2px 40px; }
    table.vars td, table.req td { font-family:monospace; }
    table td.code { width:100%;}
    table td.code div { overflow:hidden; }
    table.source th { color:#666; }
    table.source td {
        font-family:monospace; white-space:pre; border-bottom:1px solid #eee; }
    ul.traceback { list-style-type:none; }
    ul.traceback li.frame { margin-bottom:1em; }
    div.context { margin: 10px 0; }
    div.context ol {
        padding-left:30px; margin:0 10px; list-style-position: inside; }
    div.context ol li {
        font-family:monospace; white-space:pre; color:#666; cursor:pointer; }
    div.context ol.context-line li { color:black; background-color:#ccc; }
    div.context ol.context-line li span { float: right; }
    div.commands { margin-left: 40px; }
    div.commands a { color:black; text-decoration:none; }
    #summary { background: #ffc; }
    #summary h2 { font-weight: normal; color: #666; }
    #summary ul#quicklinks { list-style-type: none; margin-bottom: 2em; }
    #summary ul#quicklinks li { float: left; padding: 0 1em; }
    #summary ul#quicklinks>li+li { border-left: 1px #666 solid; }
    #explanation { background:#eee; }
    #template, #template-not-exist { background:#f6f6f6; }
    #template-not-exist ul { margin: 0 0 0 20px; }
    #traceback { background:#eee; }
    #requestinfo { background:#f6f6f6; padding-left:120px; }
    #summary table { border:none; background:transparent; }
    #requestinfo h2, #requestinfo h3 { position:relative; margin-left:-100px; }
    #requestinfo h3 { margin-bottom:-1em; }
    .error { background: #ffc; }
    .specific { color:#cc3300; font-weight:bold; }
  </style>
  <script type=""text/javascript"">
  //<!--
    function getElementsByClassName(oElm, strTagName, strClassName){
        // Written by Jonathan Snook, http://www.snook.ca/jon;
        // Add-ons by Robert Nyman, http://www.robertnyman.com
        var arrElements = (strTagName == ""*"" && document.all)? document.all :
        oElm.getElementsByTagName(strTagName);
        var arrReturnElements = new Array();
        strClassName = strClassName.replace(/\-/g, ""\\-"");
        var oRegExp = new RegExp(""(^|\\s)"" + strClassName + ""(\\s|$$)"");
        var oElement;
        for(var i=0; i<arrElements.length; i++){
            oElement = arrElements[i];
            if(oRegExp.test(oElement.className)){
                arrReturnElements.push(oElement);
            }
        }
        return (arrReturnElements)
    }
    function hideAll(elems) {
      for (var e = 0; e < elems.length; e++) {
        elems[e].style.display = 'none';
      }
    }
    window.onload = function() {
      hideAll(getElementsByClassName(document, 'table', 'vars'));
      hideAll(getElementsByClassName(document, 'ol', 'pre-context'));
      hideAll(getElementsByClassName(document, 'ol', 'post-context'));
    }
    function toggle() {
      for (var i = 0; i < arguments.length; i++) {
        var e = document.getElementById(arguments[i]);
        if (e) {
          e.style.display = e.style.display == 'none' ? 'block' : 'none';
        }
      }
      return false;
    }
    function varToggle(link, id) {
      toggle('v' + id);
      var s = link.getElementsByTagName('span')[0];
      var uarr = String.fromCharCode(0x25b6);
      var darr = String.fromCharCode(0x25bc);
      s.innerHTML = s.innerHTML == uarr ? darr : uarr;
      return false;
    }
    //-->
  </script>
</head>
<body>

<div id=""summary"">
  <h1>");
response.Write(h(ex.GetType().Name));
response.Write(@" at ");
response.Write(h(path));
response.Write(@"</h1>
  <h2>");
response.Write(h(ex.Message));
response.Write(@"</h2>
  <table><tr>
    <th>Ruby</th>
    <td>
{% if first = frames.first %}
      <code>{%=h first.filename %}</code>: in <code>{%=h first.function %}</code>, line {%=h frames.first.lineno %}
{% else %}
      unknown location
{% end %}
    </td>
  </tr><tr>
    <th>Web</th>
    <td><code>");
response.Write(h(request.Method ));
response.Write(@" ");
response.Write(h( request.Host + path ));
response.Write(@" </code></td>
  </tr></table>

  <h3>Jump to:</h3>
  <ul id=""quicklinks"">
    <li><a href=""#get-info"">GET</a></li>
    <li><a href=""#post-info"">POST</a></li>
    <li><a href=""#cookie-info"">Cookies</a></li>
    <li><a href=""#env-info"">ENV</a></li>
  </ul>
</div>

<div id=""traceback"">
  <h2>Traceback <span>(innermost first)</span></h2>
  <ul class=""traceback"">
{% frames.each { |frame| %}
      <li class=""frame"">
        <code>{%=h frame.filename %}</code>: in <code>{%=h frame.function %}</code>

          {% if frame.context_line %}
          <div class=""context"" id=""c{%=h frame.object_id %}"">
              {% if frame.pre_context %}
              <ol start=""{%=h frame.pre_context_lineno+1 %}"" class=""pre-context"" id=""pre{%=h frame.object_id %}"">
                {% frame.pre_context.each { |line| %}
                <li onclick=""toggle('pre{%=h frame.object_id %}', 'post{%=h frame.object_id %}')"">{%=h line %}</li>
                {% } %}
              </ol>
              {% end %}

            <ol start=""{%=h frame.lineno %}"" class=""context-line"">
              <li onclick=""toggle('pre{%=h frame.object_id %}', 'post{%=h frame.object_id %}')"">{%=h frame.context_line %}<span>...</span></li></ol>

              {% if frame.post_context %}
              <ol start='{%=h frame.lineno+1 %}' class=""post-context"" id=""post{%=h frame.object_id %}"">
                {% frame.post_context.each { |line| %}
                <li onclick=""toggle('pre{%=h frame.object_id %}', 'post{%=h frame.object_id %}')"">{%=h line %}</li>
                {% } %}
              </ol>
              {% end %}
          </div>
          {% end %}
      </li>
{% } %}
  </ul>
</div>

<div id=""requestinfo"">
  <h2>Request information</h2>

  <h3 id=""get-info"">GET</h3>
  ");
 if (request.Query.Any()) { 
response.Write(@"
    <table class=""req"">
      <thead>
        <tr>
          <th>Variable</th>
          <th>Value</th>
        </tr>
      </thead>
      <tbody>
          ");
 foreach(var kv in request.Query.OrderBy(kv => kv.Key)) { 
response.Write(@"
          <tr>
            <td>");
response.Write(h(kv.Key));
response.Write(@"</td>
            <td class=""code""><div>");
response.Write(h(kv.Value));
response.Write(@"</div></td>
          </tr>
          ");
 } 
response.Write(@"
      </tbody>
    </table>
  ");
 } else  { 
response.Write(@"
    <p>No GET data.</p>
  ");
 } 
response.Write(@"

  <h3 id=""post-info"">POST</h3>
  {% unless req.POST.empty? %}
    <table class=""req"">
      <thead>
        <tr>
          <th>Variable</th>
          <th>Value</th>
        </tr>
      </thead>
      <tbody>
          {% req.POST.sort_by { |k, v| k.to_s }.each { |key, val| %}
          <tr>
            <td>{%=h key %}</td>
            <td class=""code""><div>{%=h val.inspect %}</div></td>
          </tr>
          {% } %}
      </tbody>
    </table>
  {% else %}
    <p>No POST data.</p>
  {% end %}


  <h3 id=""cookie-info"">COOKIES</h3>
  {% unless req.cookies.empty? %}
    <table class=""req"">
      <thead>
        <tr>
          <th>Variable</th>
          <th>Value</th>
        </tr>
      </thead>
      <tbody>
        {% req.cookies.each { |key, val| %}
          <tr>
            <td>{%=h key %}</td>
            <td class=""code""><div>{%=h val.inspect %}</div></td>
          </tr>
        {% } %}
      </tbody>
    </table>
  {% else %}
    <p>No cookie data.</p>
  {% end %}

  <h3 id=""env-info"">Rack ENV</h3>
    <table class=""req"">
      <thead>
        <tr>
          <th>Variable</th>
          <th>Value</th>
        </tr>
      </thead>
      <tbody>
        ");
 foreach(var kv in env.OrderBy(kv=>kv.Key)) { 
response.Write(@"
          <tr>
            <td>");
response.Write(h(kv.Key));
response.Write(@"</td>
            <td class=""code""><div>");
response.Write(h(kv.Value));
response.Write(@"</div></td>
          </tr>
          ");
 } 
response.Write(@"
      </tbody>
    </table>

</div>

<div id=""explanation"">
  <p>
    You're seeing this error because you use <code>Gate.Helpers.ShowExceptions</code>.
  </p>
</div>

</body>
</html>
            ");
        }
    }
}


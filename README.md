# searchtosql
Search string to SQL query converter

Algorithms adapted from an experimental advertising arbitrage search engine implemented in C#.

Designed for use as a microservice under Docker.


# Search Help

Searches are normally performed with "must contain" words. A match requires that all of the words entered be present on the page. You can search for a specific phrase by enclosing it in double quotes (").

You can explicitly search for pages which contain a specific word or phrase by prefixing it with an AND (+) sign. Only pages which contain that word or phrase will be shown.

You can search for pages that may contain a specific word or phrase by prefixing it with an OR (|) sign. Pages which contain that word or phrase will optionally be shown.

You can ignore all pages which contain a specific word or phrase by prefixing it with a NOT or minus (-) sign. Pages that contains that word or phrase will not be displayed in the search results. Note that negated phrases imply the presence of both words, but not as a phrase.

You can limit your search to a particular site by using the "SITE:" prefix before the full domain name.

All search words are case **insensitive**. Matches will be found in the page title, keywords, description and body text.

<TABLE cellpadding=3 border=1 width="90%">
<TR>
<TH>Search&nbsp;String</TH>
<TH>Results pages will contain:</TH>
</TR>
<tr>
<TD>Online AND Security</TD>
<TD>both the word <i>Online</i> and the word <i>Security</i></TD>
</tr>
<tr>
<TD>Online + Security</TD>
<TD>both the word <i>Online</i> and the word <i>Security</i></TD>
</tr>
<TR>
<TD>Online OR Security</TD>
<TD>either the word <i>Online</i> or the word <i>Security (or both)</i></TD>
</TR>
<tr>
<TD>Online | Security</TD>
<TD>either the word <i>Online</i> or the word <i>Security (or both)</i></TD>
</tr>
<tr>
<TD>Online - Security</TD>
<TD>the word <i>Online</i> and not the word <i>Security</i></TD>
</tr>
<tr>
<TD>- (Online | Security)</TD>
<TD>neither the word <i>Online</i> nor the word <i>Security</i></TD>
</tr>
<tr>
<TD>&quot;Online Security&quot;</TD>
<TD>the exact phrase <i>Online Security</i></TD>
</tr>
<tr>
<TD>Online - &quot;Online Security&quot;</TD>
<TD>the word <i>Online</i>, but not the exact phrase <i>Online Security</i></TD>
</tr>
<tr>
<TD>Online SITE:<%Response.Write(osbSite.wwwGlobal.SiteNameURL);%></TD>
<TD>the word <i>Online</i> from the site <%Response.Write(osbSite.wwwGlobal.SiteNameURL);%></TD>
</tr>
<tr>
<TD nowrap>Online - SITE:<%Response.Write(osbSite.wwwGlobal.SiteNameURL);%></TD>
<TD>the word <i>Online</i>, excluding pages from the site <%Response.Write(osbSite.wwwGlobal.SiteNameURL);%></TD>
</tr>
<tr>
<TD nowrap>Internet - "Online Technology"<br>SITE:<%Response.Write(osbSite.wwwGlobal.SiteNameURL);%></TD>
<TD>the word <i>Internet</i>, and not the phrase <i>Online Technology</i>, from the site <%Response.Write(osbSite.wwwGlobal.SiteNameURL);%></TD>
</tr>
</TABLE>



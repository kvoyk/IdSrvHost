# IdSrvHost
Identity servers with two core configurations and Cosign/Google/Facebook security providers

Configuration for the Identity server host with two configuration
  -core1 authentication with Cosign security provider
  -core2 authenticaton with Cosing/Google/Facebook/Github security providers
  
  Cosign security provider implementation can be found in this project https://github.com/kvoyk/OwinOAuthProviders
  Information about Cosign can be found here http://weblogin.org/
  
  In order to make cosign to work you need to set use rewrite module on IIS server or set custom handler that will redirect all request coming
  from Cosign server to your application.
    
    
  Sample of rewrite entries
  <rewrite>
    <rules>
        <clear />
        <rule name="Cosign-RedirectCore1" enabled="true" stopProcessing="true">
            <match url="cosign/valid?" negate="false" />
            <conditions logicalGrouping="MatchAll" trackAllCaptures="false">
                <add input="{QUERY_STRING}" pattern="core=core1" />
            </conditions>
            <action type="Redirect" url="https://yourserver/host/path/signin-cosign" redirectType="SeeOther" />
        </rule>
        <rule name="Cosign-RedirectCore2" enabled="true" stopProcessing="true">
            <match url="cosign/valid?" negate="false" />
            <conditions logicalGrouping="MatchAll" trackAllCaptures="false">
                <add input="{QUERY_STRING}" pattern="core=core2" />
            </conditions>
            <action type="Redirect" url="https://yourserver/host/path/signin-cosign" redirectType="SeeOther" />
        </rule>
    </rules>
</rewrite>

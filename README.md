# Todo List Web  

This is a sample that I build so that I could get more familiar with OpenID Connect, OAuth and specifically in combination with AD FS on Server 2016 TP5.

## Todo List Contents

There are two projects in this solution: TodoListWebApp and TodoListService
* TodoListWebapp: a web application where you can sign in and manage a personal todo List.
* TodoListService: an API that stores the users their todo list. 

## Remarks:

**REMARK #1: the todo list content is forgotten when the API is restarted. Thís is a simple educational example.
**REMARK #1: the UserProfileController has not been touched/ported and has no use for the current state of the TodoListWeb application
**REMARK #2: I'm not a developer. You can read, copy and use my code, but I'm not responsible for whatever happens. This sample is mainly focussed on understanding the nuances of OAuth/OpenIDConnect specifically with AD FS 2016. That's where I've put most of the effort (time) in. That also means there are probably several examples of how not to do MVC, or CSS or ... I'm always keen to learn and I appreciate feedback.

## Additional Information

TodoListWebapp web.config:

* ida:AppKey: the shared secret you generated in the AD FS application group wizard for this application
* ida:ClientId: the client id you generated in the AD FS application group wizard for this Application
* ida:RedirectUri: the base URL of the application where AD FS should redirect to. Example: https://todolistweb.contoso.com/
* todo:TodoListResourceid: the identifier of the API (resource). This should match up with what you configure on the Application Group. Example: https://todolistapi.contoso.com
* todo:TodoListBaseAddress: the base address for the API. This is where the code will send it GET and POST requests too. Example: https://todolistapi.contoso.com
* ida:ADFSService: the AD FS instance you want to use. Example: https://adfs.contoso.com/adfs
* ida:ADFSDiscoveryDoc: the OpenID-Configuration endpoint on the AD FS instance you want to use: Example: https://adfs.contoso.com/adfs/.well-known/openid-configuration

TodoListService web.config:

* ida:ADFSDiscoveryDoc: the AD FS Federation Metadata URL. Even do we're doing OAuth we still need to use the FederationMetadata.xml here. A colleague told me that this is probably due to backwards compatibility of ActiveDirectoryFederationServicesBearerAuthenticationOptions. Example: https://adfs.contoso.com/FederationMetadata/2007-06/FederationMetadata.xml
* ida:Audience: the identifier of the API (resource). This should match up with what you configure on the Application Group. Example: https://todolistapi.contoso.com
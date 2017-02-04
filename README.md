# Todo List Web  

This is a sample that I built so that I could get more familiar with OpenID Connect, OAuth and how they are implemented with AD FS on Server 2016 TP5.

This sample is based on https://github.com/Azure-Samples/active-directory-dotnet-webapp-webapi-openidconnect

## Todo List Web Contents

There are two projects in this solution: TodoListWebApp and TodoListService
* TodoListWebapp: a web application where you can sign in and manage a personal todo List. There's also a page which shows you the claims for the ID Token and the API Access Token.
* TodoListService: an API that stores the users their todo list.
* TodoHelpWeb: a simple web application where you can sign in. I use this sample to demonstrate the AD FS 2016 capabilities of customizing the ID token.

## Remarks:

* **REMARK #1**: the todo list content is forgotten when the API is restarted. This is a simple educational example.
* **REMARK #1**: the UserProfileController has not been touched/ported and has no use for the current state of the TodoListWeb application
* **REMARK #2: I'm not a developer. You can read, copy and use my code, but I'm not responsible for whatever happens. This sample is mainly focussed on understanding the nuances of OAuth/OpenIDConnect specifically with AD FS 2016. That's where I've put most of the effort (time) in. That also means there are probably several examples of how not to do MVC, or CSS or ... I'm always keen to learn and I appreciate feedback.**

## Additional Information

TodoListWebapp web.config:

* **ida:AppKey**: the shared secret you generated in the AD FS application group wizard for this application
* **ida:ClientId**: the client id you generated in the AD FS application group wizard for this Application
* **ida:RedirectUri**: the base URL of the application where AD FS should redirect to. Example: https://todolistweb.contoso.com/
* **todo:TodoListResourceid**: the identifier of the API (resource). This should match up with what you configure on the Application Group. Example: https://todolistapi.contoso.com
* **todo:TodoListBaseAddress**: the base address for the API. This is where the code will send it GET and POST requests too. Example: https://todolistapi.contoso.com
* **ida:ADFSService**: the AD FS instance you want to use. Example: https://adfs.contoso.com/adfs
* **ida:ADFSDiscoveryDoc**: the OpenID-Configuration endpoint on the AD FS instance you want to use: Example: https://adfs.contoso.com/adfs/.well-known/openid-configuration

TodoListService web.config:

* **ida:ADFSDiscoveryDoc**: the AD FS Federation Metadata URL. Even do we're doing OAuth we still need to use the FederationMetadata.xml here. A colleague told me that this is probably due to backwards compatibility of ActiveDirectoryFederationServicesBearerAuthenticationOptions. Example: https://adfs.contoso.com/FederationMetadata/2007-06/FederationMetadata.xml
* **ida:Audience**: the identifier of the API (resource). This should match up with what you configure on the Application Group. Example: https://todolistapi.contoso.com

TodoHelpWeb web.config:

* **ida:ClientId**: the client id you generated in the AD FS application group wizard for this Application
* **ida:RedirectUri**: the base URL of the application where AD FS should redirect to. Example: https://todolistweb.contoso.com/
* **ida:ADFSService**: the AD FS instance you want to use. Example: https://adfs.contoso.com/adfs
* **ida:ADFSDiscoveryDoc**: the OpenID-Configuration endpoint on the AD FS instance you want to use: Example: https://adfs.contoso.com/adfs/.well-known/openid-configuration
* **ida:ADFSUserInfo**"the AD FS user info endpoint. For future use.

AD FS Application Group Configuration:

Make sure to use the correct identifiers, keys and URL's as specified in the web.configs. You'll also need to configure the following Claim Issuance Rule for the API:

**Transform an Incoming Claim:**
* Incoming claim type: **UPN**
* Outgoing claim type: **Name ID**
* Outgoing name ID format: **unspecified**

The API relies on this so that each todo list is only available to the user the list belongs too.

## Some Screenshots

* Landing page

![Alt text](/Img/1LandingPage.png?raw=true)

* After signing in

![Alt text](/Img/2AfterSignIn.png?raw=true)

* Manage to do list

![Alt text](/Img/3TodoList.png?raw=true)

* Show my claims

![Alt text](/Img/4ShowClaims.png?raw=true)

Here you can see that for the Access Token I got a http://foo/bar "HardCodedValue" claim that was configured in the Claims Issuance Rules for the API. The name claim in the Access Token contains the sAMAccountName, but that was just a  test. And the nameidentifier claims contains the UPN. The API relies on this claim to make sure todo lists are keyed off uniquely to each user.


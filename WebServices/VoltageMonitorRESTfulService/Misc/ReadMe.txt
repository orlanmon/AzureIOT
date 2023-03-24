

Azure Log In

https://portal.azure.com


UN:  monacos@monacos.us
PW:  GoWestYoungMan_123

Private Account /  Not Work Account!!!



Swagger

https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?tabs=visual-studio%2Cvisual-studio-xml



RESTful Service Reference

https://www.c-sharpcorner.com/UploadFile/dacca2/http-request-methods-get-post-put-and-delete/



// ASP.NET Core (MVC) and ASP.NET MVC 5

http://www.mithunvp.com/difference-between-asp-net-mvc6-asp-net-mvc5/

https://docs.microsoft.com/en-us/aspnet/core/migration/webapi?view=aspnetcore-2.1

https://docs.microsoft.com/en-us/aspnet/core/index?view=aspnetcore-2.1

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/choose-aspnet-framework?view=aspnetcore-2.1

http://stephenwalther.com/archive/2015/02/24/top-10-changes-in-asp-net-5-and-mvc-6

https://docs.microsoft.com/en-us/dotnet/standard/choosing-core-framework-server

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1&tabs=aspnetcore1x#azure-app-service-provider


Build Instructions

ASP.NET CORE Web Application with .NET Framework

Install Microsoft.AspNetCore.Authentication.JwtBearer 1.1.0 from Nuget
Install Swashbuckle.AspNetCore 3.0.0 from Nuget
Install  Microsoft.Extensions.Logging.AzureAppServices 1.0.0 from Nuget








KeyVault Configuration

https://www.c-sharpcorner.com/article/getting-started-with-azure-key-vault/

https://www.netiq.com/communities/cool-solutions/creating-application-client-id-client-secret-microsoft-azure-new-portal/

https://docs.microsoft.com/en-us/azure/key-vault/key-vault-get-started




Azure

Adding App Service to Azure Active Directory


https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal

https://docs.microsoft.com/en-us/azure/architecture/multitenant-identity/web-api


*****************************************************************************
** !!!Best Resources Yet!!!
**Azure Registering Applicaiton in AD and Setting Permissions For Key Vault**
*****************************************************************************

https://docs.microsoft.com/en-us/azure/key-vault/key-vault-get-started

https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-scenarios


// Potentially good resources.

// !!!
https://docs.microsoft.com/en-us/azure/architecture/multitenant-identity/web-api


https://docs.microsoft.com/en-us/azure/architecture/multitenant-identity/run-the-app#update-the-application-manifests


!!!
https://docs.microsoft.com/en-us/sharepoint/dev/spfx/web-parts/guidance/connect-to-api-secured-with-aad


 !!Microsoft Authentication Library (MSAL)!!

// Active Directory Authentication Library (ADAL)  Being Replaced by Microsoft Authentication Library (MSAL)

// Outdated/Obsolete
https://contos.io/protecting-a-net-core-api-with-azure-active-directory-59bbcd5b3429


New Authentication End Point V2  MSAL ( Azure AD + MSA accounts )





// New Version
https://contos.io/working-with-identity-in-net-core-2-0-d235a7bf9cbe



*****************************************************************************
// Steps To Building App Service
*****************************************************************************


Create App Service

micro-service-one

Create Application Slots

dev/uat/prod


Create Key Vaults for Each Environment

https://dev-env-settings.vault.azure.net/
https://uat-env-settings.vault.azure.net/
https://prod-env-settings.vault.azure.net/


// Register Existing Web API App Service With Azure AD  ( Registered Application )

Applicaiton/Client ID = e028aded-e586-4385-bb88-d358dd5b7052

Register in Active Directory App Registration

Goto Keys Create New One 

Application Key Name: app-service-key

Application Key Secret:  s9Rf8f0lQsb0IagmWv3zIKQ3GlfXi5G5Io7F5FPobAA=


 "ApplicationSettings": {
    "VaultUrl": "https://dev-env-settings.vault.azure.net",
    "ClientId": "e028aded-e586-4385-bb88-d358dd5b7052",
    "ClientSecret": "s9Rf8f0lQsb0IagmWv3zIKQ3GlfXi5G5Io7F5FPobAA=",
    "Environment": "Development"
  },



Now Grant ADD Application Access to Each Key Vault

For each Key Vault Goto Access Policies

Selected Azure AD App and assign key and secret permissions

*****************************************************************************


Monacos.us Azure Active Directory App Registration


App ID/ClientID:  4c7f365d-85e0-4297-9ec1-975a1bb3e276 
Secret Key:      BLz7/ANWX1cDvwSQCgcvpZtZ723QcPU+GPiLukg/dz0=


Web API Manifest ( Add www.monacos.us AAD App Client ID )

 "knownClientApplications": ["4c7f365d-85e0-4297-9ec1-975a1bb3e276"],





//*****************************************************************************
// Application Settings for Instalation Slots
//*****************************************************************************

ApplicationSettings:Environment    Select Slot Settings Toggled On




// Swagger Interface

http://micro-service-one-prod-env.azurewebsites.net/swagger/

// Test/Demo URLs  ( Pulls Value From Key Vault ) using Secret Key Name

http://micro-service-one-dev-env.azurewebsites.net/api/configuration/GetKeyVaultSetting/UserName

// Slot Specific Application Settings

http://micro-service-one-dev-env.azurewebsites.net/api/configuration/GetConfigurationSetting/Environment









/**********************************************************************************************************************************/


/***********************************************************************************
// OLD Key Vault Creation Method ** KEEP FOR Reference
/***********************************************************************************

Login-AzureRmAccount


// Create New Resource Group If Necessary ( Not Needed One Alreay Exists )
 
New-AzureRmResourceGroup –Name 'monacoswebapplication' –Location 'East US'


// Create Key Vault

New-AzureRmKeyVault -VaultName 'ApplicationSettings' -ResourceGroupName 'monacoswebapplication' -Location 'East US'


// Response

Vault Name                       : ApplicatonSettings
Resource Group Name              : monacoswebapplication
Location                         : eastus
Resource ID                      : /subscriptions/0dc78152-04ff-4995-9813-29db62f1aab2/resourceGroups/monacoswebapplica
                                   tion/providers/Microsoft.KeyVault/vaults/ApplicatonSettings
Vault URI                        : https://ApplicatonSettings.vault.azure.net
Tenant ID                        : 02ab6bc4-9edc-4e50-b34f-f804fb5ff6fa
SKU                              : Standard
Enabled For Deployment?          : False
Enabled For Template Deployment? : False
Enabled For Disk Encryption?     : False
Access Policies                  :
                                   Tenant ID                   : 02ab6bc4-9edc-4e50-b34f-f804fb5ff6fa
                                   Object ID                   : 605db21f-d706-49de-83d5-0a3afbf5ba72
                                   Application ID              :
                                   Display Name                : Orlando Monaco
                                   (monacos_monacos.us#EXT#@monacosmonacos.onmicrosoft.com)
                                   Permissions to Keys         : get, create, delete, list, update, import, backup,
                                   restore
                                   Permissions to Secrets      : all
                                   Permissions to Certificates : all



// Add Permission to KeyVault
Set-AzureRmKeyVaultAccessPolicy -VaultName 'ApplicatonSettings' -UserPrincipalName 'monacos@monacos.us' -PermissionsToKeys all -PermissionsToSecrets all -ResourceGroupName 'monacoswebapplication'




// Set a Key Vault Secret Key and Secret Value
// Secret Key is 'ApplicatonSettingsPrimaryKey'
// Note: Secret Value is 'genesis19682=='

// Set New Key Vault Secret Value Seting.  Value =  'genesis19682=='

$secretvalue=ConvertTo-SecureString 'genesis19682==' -AsPlainText -Force 

Set-AzureKeyVaultSecret -VaultName 'ApplicatonSettings' -Name 'ApplicatonSettingsPrimaryKey' -SecretValue $secretvalue




Vault Name   : applicatonsettings
Name         : ApplicatonSettingsPrimaryKey
Version      : 57967890eba74f67956c0fb85cd3f698
Id           : https://applicatonsettings.vault.azure.net:443/secrets/ApplicatonSettingsPrimaryKey/57967890eba74f67956c
               0fb85cd3f698
Enabled      : True
Expires      :
Not Before   :
Created      : 3/24/2018 9:55:03 AM
Updated      : 3/24/2018 9:55:03 AM
Content Type :
Tags         :

Note that we are converting the access key to secure string and then storing it in key vault using above cmdlets.

Active Directory Application ID =  Client ID!

Application ID is Client ID :  0b3c92ca-a2e1-40d5-a5dd-2d4c1f43fd6f

Active Directory Application Client Secret!

Active Directory App Registration Key:   MonacosWebAppKey    Value/Client Secret:  bJCHs2+HghpRdMkh1gYfZMP9qIF5TDasdv4gVbo85yo=


Grant Web Application Access to Key Vault

-ServicePrincipalName = Client ID

Set-AzureRmKeyVaultAccessPolicy -VaultName 'applicatonsettings' -ServicePrincipalName ‘0b3c92ca-a2e1-40d5-a5dd-2d4c1f43fd6f' -PermissionsToKeys all -PermissionsToSecrets all –ResourceGroupName 'monacoswebapplication'

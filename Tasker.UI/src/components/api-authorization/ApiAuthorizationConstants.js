import { ApiPrefix } from '../../common/TaskerUrls';
export const ApplicationName = 'Tasker';

// TODO Delete not neccesary

export const QueryParameterNames = {
  ReturnUrl: 'returnUrl',
  Message: 'message'
};

export const LogoutActions = {
  LogoutCallback: 'logout-callback',
  Logout: 'logout',
  LoggedOut: 'logged-out'
};

export const LoginActions = {
  Login: 'login',
//   LoginCallback: 'login-callback',
//   LoginFailed: 'login-failed',
//   Profile: 'profile',
  Register: 'register'
};

const authPrefix = '/tasker/authentication';

export const ApplicationPaths = {
//   DefaultLoginRedirectPath: '/',
  ApiAuthorizationClientConfigurationUrl: `${ApiPrefix}/OpenIdConnect/${ApplicationName}`,
  ApiAuthorizationPrefix: `${authPrefix}:`,
  Login: `${authPrefix}/${LoginActions.Login}`,
//   LoginFailed: `${prefix}/${LoginActions.LoginFailed}`,
//   LoginCallback: `${prefix}/${LoginActions.LoginCallback}`,
  Register: `${authPrefix}/${LoginActions.Register}`,
//   Profile: `${prefix}/${LoginActions.Profile}`,
//   LogOut: `${prefix}/${LogoutActions.Logout}`,
//   LoggedOut: `${prefix}/${LogoutActions.LoggedOut}`,
//   LogOutCallback: `${prefix}/${LogoutActions.LogoutCallback}`,
//   IdentityRegisterPath: '/Identity/Account/Register',
//   IdentityManagePath: '/Identity/Account/Manage'
};
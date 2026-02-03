using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Authorization;
// This is both the Attribute you put on Controllers AND the Requirement logic
public class UserInSameLocation : IAuthorizationRequirement { }

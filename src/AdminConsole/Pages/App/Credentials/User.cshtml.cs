﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Passwordless.AdminConsole.Pages.Shared;
using Passwordless.AdminConsole.Services;

namespace Passwordless.AdminConsole.Pages.App.Credentials;

public class UserModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IScopedPasswordlessClient _passwordlessClient;

    public CredentialsModel Credentials { get; set; } = new();
    public IReadOnlyCollection<AliasPointer> Aliases { get; set; }

    [BindProperty(SupportsGet = true)]
    public string UserId { get; set; }

    public string RegisterToken { get; set; }

    public UserModel(ILogger<IndexModel> logger, IScopedPasswordlessClient api)
    {
        _logger = logger;
        _passwordlessClient = api;
    }

    public async Task OnGet()
    {
        Credentials.Items = await _passwordlessClient.ListCredentialsAsync(UserId);
        Credentials.HideDetails = false;
        Aliases = await _passwordlessClient.ListAliasesAsync(UserId);
    }

    public async Task<IActionResult> OnPost(string token)
    {
        var res = await _passwordlessClient.VerifyTokenAsync(token);
        return new JsonResult(res);
    }

    public async Task<IActionResult> OnPostRemoveCredential(string credentialId)
    {
        await _passwordlessClient.DeleteCredentialAsync(credentialId);
        return RedirectToPage();
    }
}
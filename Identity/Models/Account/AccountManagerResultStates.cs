namespace Identity.Models.Account
{
    public enum RegisterResultState
    {
        InvalidInput,
        UserNotCreated,
        TokenNotCreated,
        EmailSent,
        SignedIn,
        NotSignedIn
    }

    public enum ConfirmUserResultState
    {
        EmailNotConfirmed,
        UserConfirmed
    }

    public enum SignInResultState
    {
        UserNotFound,
        EmailNotConfirmed,
        SignedIn,
        NotSignedIn
    }

    public enum SignOutResultState
    {
        SignedOut,
        NotSignedOut
    }

    public enum ForgotPasswordResultState
    {
        UserNotFound,
        EmailNotConfirmed,
        TokenNotCreated,
        EmailSent,
        EmailNotSent
    }

    public enum ResetPasswordResultState
    {
        InvalidToken,
        TokenVerified,
        PasswordNotReset,
        PasswordReset
    }
}
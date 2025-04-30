using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace GestorMEI.Identity
{
    public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
    {
        private readonly IStringLocalizer<LocalizedIdentityErrorDescriber> _localizer;

        public LocalizedIdentityErrorDescriber(IStringLocalizer<LocalizedIdentityErrorDescriber> localizer)
        {
            _localizer = localizer;
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = "EmailDuplicado",
                Description = string.Format(_localizer["Email {0} já está em uso."], email)
            };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = "TokenInválido",
                Description = string.Format(_localizer["Token inválido."])
            };
        }

        public override IdentityError InvalidEmail(string? email)
        {
            return new IdentityError
            {
                Code = "EmailInválido",
                Description = string.Format(_localizer["Email '{0}' é inválido."], email)
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = "UsuarioDuplicado",
                Description = string.Format(_localizer["Usuário '{0}' já está em uso."], userName)
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = "SenhaRequerDígito",
                Description = string.Format(_localizer["A senha requer pelo menos um dígito."])
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = "SenhaRequerLetraMinúscula",
                Description = string.Format(_localizer["A senha requer pelo menos uma letra minúscula."])
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = "SenhaRequerLetraMaiúscula",
                Description = string.Format(_localizer["A senha requer pelo menos uma letra maiúscula"])
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = "SenhaCurta",
                Description = string.Format(_localizer["A senha precisa ter no mínimo '{0}' caracteres."], length)
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = "SenhaRequerCaracterEspecial",
                Description = string.Format(_localizer["A senha requer pelo menos um caracter especial."])
            };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = "UsuárioPossuiSenha",
                Description = string.Format(_localizer["O Usuário já possui uma senha."])
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = "SenhaNãoCoincide",
                Description = string.Format(_localizer["As senhas não coincidem."])
            };
        }

        public override IdentityError InvalidUserName(string? userName)
        {
            return new IdentityError
            {
                Code = "UsuárioInválido",
                Description = string.Format(_localizer["Nome de usuário inválido."], userName)
            };
        }

        
    }
}

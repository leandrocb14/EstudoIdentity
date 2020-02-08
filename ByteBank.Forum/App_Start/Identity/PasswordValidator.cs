using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ByteBank.Forum.App_Start.Identity
{
    public class PasswordValidator : IIdentityValidator<string>
    {
        public int MinLengthPassword { get; set; }
        public bool SpecialCharacterRequired { get; set; }
        public bool LowerCaseRequired { get; set; }
        public bool UpperCaseRequired { get; set; }
        public bool DigitsRequired { get; set; }
        public async Task<IdentityResult> ValidateAsync(string item)
        {
            var erros = new List<string>();

            if (!HaveMinLengthPassword(item))
                erros.Add($"A senha deve conter no mínimo {MinLengthPassword} caracteres.");
            if (SpecialCharacterRequired && !HaveSpecialCharacter(item))
                erros.Add("A senha deve conter caracteres especiais.");
            if (LowerCaseRequired && !HaveLowerCase(item))
                erros.Add("A senha deve conter no mínimo um caracter minusculo.");
            if (UpperCaseRequired && !HaveUpperCase(item))
                erros.Add("A senha deve conter no mínimo um caracter maiusculo.");
            if(DigitsRequired && !HaveDigit(item))
                erros.Add("A senha deve conter no mínimo um dígito numérico.");

            if (erros.Any())
                return IdentityResult.Failed(erros.ToArray());
            else
                return IdentityResult.Success;
        }

        private bool HaveMinLengthPassword(string pPassword) =>
            pPassword?.Length >= MinLengthPassword;

        private bool HaveSpecialCharacter(string pPassword) =>
            Regex.IsMatch(pPassword, @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]");

        private bool HaveLowerCase(string pPassword) =>
            pPassword.Any(char.IsLower);

        private bool HaveUpperCase(string pPassword) =>
            pPassword.Any(char.IsUpper);

        private bool HaveDigit(string pPassword) =>
            pPassword.Any(char.IsDigit);
    }
}
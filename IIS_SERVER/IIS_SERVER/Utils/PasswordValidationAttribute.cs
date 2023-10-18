using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PasswordValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is string password)
        {
            if (password.Length < 8)
            {
                return false;
            }

            if (!Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*\d)(?=.*\W).+$"))
            {
                return false;
            }

            return true;
        }

        return false;
    }
}
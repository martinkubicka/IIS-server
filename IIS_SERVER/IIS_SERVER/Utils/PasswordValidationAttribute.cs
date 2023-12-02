/**
* @file PasswordValidationAttribute.cs
* author { Martin Kubicka (xkubic45) }
* @date 17.12.2023
* @brief Validation of password attribute
*/


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
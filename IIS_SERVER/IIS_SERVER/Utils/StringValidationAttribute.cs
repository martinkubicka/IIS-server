/**
* @file StringValidationAttribute.cs
* author { Martin Kubicka (xkubic45) }
* @date 17.12.2023
* @brief Validation of string attribute
*/


using System.ComponentModel.DataAnnotations;

namespace IIS_SERVER.Utils;

public class StringValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is string stringValue)
        {
            return !string.IsNullOrWhiteSpace(stringValue);
        }

        return true;
    }
}
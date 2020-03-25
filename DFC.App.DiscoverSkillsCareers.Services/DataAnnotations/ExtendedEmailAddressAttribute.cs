using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DFC.App.DiscoverSkillsCareers.Services.DataAnnotations
{
    public class ExtendedEmailAddressAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                return Regex.IsMatch(value.ToString(), @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$");
            }

            return false;
        }
    }
}

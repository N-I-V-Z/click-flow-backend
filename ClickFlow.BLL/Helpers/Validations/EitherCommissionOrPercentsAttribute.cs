using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.Helpers.Validations
{
    public class EitherCommissionOrPercentsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var commissionProperty = validationContext.ObjectType.GetProperty("Commission");
            var percentsProperty = validationContext.ObjectType.GetProperty("Percents");

            var commissionValue = commissionProperty.GetValue(validationContext.ObjectInstance);
            var percentsValue = percentsProperty.GetValue(validationContext.ObjectInstance);

            if (commissionValue != null && percentsValue != null)
            {
                return new ValidationResult("Chỉ được nhập một trong hai: Commission hoặc Percents.");
            }

            if (commissionValue == null && percentsValue == null)
            {
                return new ValidationResult("Cần nhập một trong hai: Commission hoặc Percents.");
            }

            return ValidationResult.Success;
        }
    }
}

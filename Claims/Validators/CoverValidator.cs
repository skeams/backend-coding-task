using Claims.Models;
using FluentValidation;

namespace Claims.Validators
{
    public class CoverValidator : AbstractValidator<Cover>
    {
        public CoverValidator()
        {
            RuleFor(x => x.StartDate).Must(date => date > DateOnly.FromDateTime(DateTime.Today));
            RuleFor(x => x).Must(x => x.StartDate.AddYears(1) > x.EndDate).WithMessage("Period must not exceed 1 year");
        }
    }
}


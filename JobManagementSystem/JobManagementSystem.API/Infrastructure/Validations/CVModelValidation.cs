using FluentValidation;
using System.Text.RegularExpressions;

namespace JobManagementSystem.API.Infrastructure
{
    public class CVModelValidation : AbstractValidator<CVModel>
    {
        public CVModelValidation()
        {
            RuleFor((CVModel cv) => cv.Skills).NotNull();


            RuleForEach((CVModel cv) => cv.Skills)
                .Must((SkillDescriptor skill) =>
                {
                    int SkillsMaxValue = Enum.GetValues(typeof(Skill)).Cast<int>().Max();
                    int SkillsMinValue = Enum.GetValues(typeof(Skill)).Cast<int>().Min();
                    return skill.skill <= SkillsMaxValue && skill.skill >= SkillsMinValue;
                })
                .WithMessage("there is not skill with this id")
                .Must((SkillDescriptor skill) => skill.expirience > 0)
                .WithMessage("Skill's expirience must be more than 0");

            RuleFor(cv => cv.Education)
                .Must((int education) =>
                {
                    int EducationMaxVal = Enum.GetValues(typeof(Education)).Cast<int>().Max();
                    int EducationMinVal = Enum.GetValues(typeof(Education)).Cast<int>().Min();
                    return education <= EducationMaxVal && education >= EducationMinVal;
                })
                .WithMessage("There is not educational institution with this id");

            RuleFor(cv => cv.Degree)
                .Must((int degree) =>
                {
                    int DegreeMaxVal = Enum.GetValues(typeof(Degree)).Cast<int>().Max();
                    int DegreeMinVal = Enum.GetValues(typeof(Degree)).Cast<int>().Min();
                    return degree <= DegreeMaxVal && degree >= DegreeMinVal;
                })
                .WithMessage("There is not Degree with this id");

            RuleFor(cv => cv.Profession)
                .Must((int profession) =>
                {
                    int ProfMaxVal = Enum.GetValues(typeof(Profession)).Cast<int>().Max();
                    int ProfMinVal = Enum.GetValues(typeof(Profession)).Cast<int>().Min();
                    return profession <= ProfMaxVal && profession >= ProfMinVal;
                })
                .WithMessage("there is not proffession with this id");

            RuleFor(cv => cv)
                .Must((CVModel cv) => cv.EducationStarted != null && cv.EducationEnded != null &&
                cv.EducationStarted < cv.EducationEnded)
                .WithMessage("Start date must be less then end date");

            RuleFor(cv => cv.PhoneNumber)
                .NotNull()
                .Must((string phoneNumber) => phoneNumber != null && phoneNumber.Length == 18)
                .WithMessage("Phone number length must be equal to 18")
                .Matches(new Regex(@"^\(\d{3}\) \d{3}-\d{2}-\d{2}-\d{2}$"))
                .WithMessage("Invalid phone number");
        }
    }
}

using FluentValidation;

namespace JobManagementSystem.API.Infrastructure
{
    public class JobValidation : AbstractValidator<JobModel>
    {
        public JobValidation()
        {
            RuleFor((JobModel job) => job.JobSkills)
                .Must(reqSkills => reqSkills != null && reqSkills.Count >= 5)
                .WithMessage("Job descriptions requires minimum 5 skills");


            RuleForEach((JobModel job) => job.JobSkills)
                .Must((JobSkillDescriptor skill) =>
                {
                    int SkillsMaxValue = Enum.GetValues(typeof(Skill)).Cast<int>().Max();
                    int SkillsMinValue = Enum.GetValues(typeof(Skill)).Cast<int>().Min();
                    return skill.skill <= SkillsMaxValue && skill.skill >= SkillsMinValue;
                })
                .WithMessage("there is not skill with this id")
                .Must((JobSkillDescriptor skill) => skill.weight > 0)
                .WithMessage("Skill's weight must be more than 0")
                .Must(skill => skill.requestedExpirience >= 0)
                .WithMessage("Skill's request exp must be more than 0");


            RuleFor(job => job.ExpireDate)
                .Must(expDate => expDate > DateTime.Now.Date)
                .WithMessage("Exp date must be in the future");

            RuleFor(job => job.JobTitle)
                .Must(title => title != null && title.Length < 100)
                .WithMessage("title is requried");
        }
    }
}

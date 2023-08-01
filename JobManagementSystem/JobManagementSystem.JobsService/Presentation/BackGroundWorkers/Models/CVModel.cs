namespace JobManagementSystem.JobsService.Presentation
{
    public class SkillDescriptorForDeletion
    {
        public string skill { get; set; }
        public int expirience { get; set; }
    }
    public class SkillDescriptor
    {
        public int skill { get; set; }
        public int expirience { get; set; }
        public override bool Equals(object? obj)
        {
            if (!(obj is SkillDescriptor)) return false;
            SkillDescriptor vs = (SkillDescriptor)obj;
            return this.skill == vs.skill;
        }
        public override int GetHashCode()
        {
            return skill.GetHashCode();
        }
    }

    public class CVModel
    {
        public HashSet<SkillDescriptor> Skills { get; set; }
        public string PhoneNumber { get; set; }
        public int Education { get; set; }
        public int Degree { get; set; }
        public int Profession { get; set; }
        public DateTime EducationStarted { get; set; }
        public DateTime EducationEnded { get; set; }
    }
}

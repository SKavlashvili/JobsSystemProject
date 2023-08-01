namespace JobManagementSystem.API.Infrastructure
{
    public class JobSkillDescriptor
    {
        public int skill { get; set; }
        public int requestedExpirience { get; set; }
        public int weight { get; set; }
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
    public class JobModel
    {
        private DateTime _expDate;
        public string JobTitle { get; set; }
        public HashSet<JobSkillDescriptor> JobSkills { get; set; }
        public DateTime ExpireDate
        {
            get { return _expDate; }
            set { _expDate = value.Date; }
        }
    }
}

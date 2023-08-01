using Microsoft.AspNetCore.Server.IIS.Core;
using System.Collections.Generic;


namespace JobManagementSystem.API.Infrastructure
{
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
        private DateTime _universityStarted;
        private DateTime _universityEnded;
        public HashSet<SkillDescriptor> Skills { get; set; }
        public string PhoneNumber { get; set; }
        public int Education { get; set; }
        public int Degree { get; set; }
        public int Profession { get; set; }
        public DateTime EducationStarted
        {
            get
            {
                return _universityStarted;
            }
            set
            {
                _universityStarted = value.Date;
            }
        }
        public DateTime EducationEnded
        {
            get
            {
                return _universityEnded;
            }
            set
            {
                _universityEnded = value.Date;
            }
        }
    }
}

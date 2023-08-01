namespace JobManagementSystem.API.Infrastructure
{
    public class PredefinedDataResponseModel
    {
        public List<KeyValuePair<string, int>> Skills { get; set; }
        public List<KeyValuePair<string, int>> Proffesions { get; set; }
        public List<KeyValuePair<string, int>> Degrees { get; set; }
        public List<KeyValuePair<string, int>> Education { get; set; }
        public PredefinedDataResponseModel()
        {
            this.Skills = GetDataFromEnum<Skill>();
            this.Proffesions = GetDataFromEnum<Profession>();
            this.Degrees = GetDataFromEnum<Degree>();
            this.Education = GetDataFromEnum<Education>();
        }
        private List<KeyValuePair<string, int>> GetDataFromEnum<T>() where T : Enum
        {
            List<KeyValuePair<string,int>> Result = new List<KeyValuePair<string, int>>();
            int MaxValueOfGenericEnum = Enum.GetValues(typeof(T)).Length - 1;
            for(int i = 0; i <= MaxValueOfGenericEnum; i++)
            {
                Result.Add(new KeyValuePair<string, int>(((T)Enum.ToObject(typeof(T),i)).ToString(), i));
            }
            return Result;
        }

    }
}

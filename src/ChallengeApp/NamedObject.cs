using System;

namespace ChallengeApp
{
    public delegate void GradeAddedDelegate(object sender, EventArgs args);
    public delegate void GradeDeletedDelegate(object sender, EventArgs args); 
    public class NamedObject
    {
        public NamedObject(string name)
        {
            this.Name = name;
        }
        public NamedObject(string name, string sex)
        {
            this.Name = name;
            this.Sex = sex;
        }
        public NamedObject(IFileWrapper fileWrapper, string name, string sex)
        {
            this.Name = name;
            this.Sex = sex;
        }
        public string Name { get; set; }
        public string Sex { get; set; }
    }
}
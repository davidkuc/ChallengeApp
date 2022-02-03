using System.Collections.Generic;
using System.IO;

namespace ChallengeApp
{
    public abstract class EmployeeBase : NamedObject, IEmployee
    {
        protected IFileWrapper _fileWrapper;
        protected IEmployee _employee;
        public const string firm = "firm";
        protected string CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;  /* filepath to  .\Challenge\src\ChallengeApp\bin\Debug\net5.0 */
        public bool EmployeeFilesExist { get; protected set; }

        public EmployeeBase(string name) : base(name)
        { }
        public EmployeeBase(string name, string empsex) : base(name, empsex)
        { }

        public EmployeeBase(IFileWrapper fileWrapper, string name, string empsex) : base(fileWrapper, name, empsex)
        { }

        public abstract event GradeAddedDelegate GradeAdded;
        public abstract event GradeAddedDelegate GradeAddedBelow30;
        public abstract event GradeDeletedDelegate GradeDeleted;

        public abstract string AddGrade(string grade);

        public abstract List<string> DeleteGrade(string index);

        public abstract string[] ShowEmployeeGrades();

        public abstract Statistics GetStatistics();
    }
}
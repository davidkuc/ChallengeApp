using System.Collections.Generic;
using System.IO;

namespace ChallengeApp
{
    public interface IEmployee
    {
        string Name { get; }
        string Sex { get; }
        bool EmployeeFilesExist { get; }

        string AddGrade(string grade);

        List<string> DeleteGrade(string index);

        string[] ShowEmployeeGrades();
        
        Statistics GetStatistics();
    }
}
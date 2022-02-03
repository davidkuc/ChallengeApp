using System;
using System.Collections.Generic;
using System.IO;

namespace ChallengeApp
{
    public class TestSavedEmployee : EmployeeBase
    {
        public override event GradeAddedDelegate GradeAdded;
        public override event GradeAddedDelegate GradeAddedBelow30;
        public override event GradeDeletedDelegate GradeDeleted;

        public TestSavedEmployee(string name) : base(name)
        {}
        
        public TestSavedEmployee(string name, string sex) : base(name, sex)
        { 
            using (var writerToAudit = File.AppendText(@$"{this.CurrentDirectory}\{this.Name}Audit.txt"))
            using (var writerToEmpGrades = File.AppendText(@$"{this.CurrentDirectory}\{this.Name}.txt"))
            using (var writerToEmployeeList = File.AppendText(@$"{this.CurrentDirectory}\EmployeeList.txt"))
            {
                Console.WriteLine($"New employee |{this.Name}| created.");
                writerToEmployeeList.WriteLine($"Joined: {DateTime.UtcNow}, Employee name: {this.Name}, sex: {this.Sex}");
            }

            if (
            File.Exists(@$"{this.CurrentDirectory}\{this.Name}Audit.txt")
            &
            File.Exists(@$"{this.CurrentDirectory}\{this.Name}.txt")
            &
            File.Exists(@$"{this.CurrentDirectory}\EmployeeList.txt")
               )
            {
                EmployeeFilesExist = true;
            }
            else
            {
                EmployeeFilesExist = false;
            }
        }

        public TestSavedEmployee(IFileWrapper fileWrapper, string name, string sex) : base(fileWrapper, name, sex)
        {
            _fileWrapper = fileWrapper;
            
            using (var writerToAudit = File.AppendText(@$"path"))
            using (var rwriterToEmpGrades = File.AppendText(@$"path1"))
            using (var writerToEmployeeList = File.AppendText(@$"path2"))
            {
                Console.WriteLine($"New employee |{this.Name}| created.");
                writerToEmployeeList.WriteLine($"Joined: {DateTime.UtcNow}, Employee name: {this.Name}, sex: {this.Sex}");
            }

            if (
             _fileWrapper.FileExists("path")
            &
             _fileWrapper.FileExists("path1")
            &
             _fileWrapper.FileExists("path2")
               )
            {
                EmployeeFilesExist = true;
            }
            else
            {
                EmployeeFilesExist = false;
            }
        }

        public override string AddGrade(string grade)
        {
            double result;
            bool canParse = double.TryParse(grade, out result);

            if (canParse)
            {
                using (var writerToEmpGrades = _fileWrapper.StreamWriterToEmployeeGrades("path"))
                using (var writerToAudit = _fileWrapper.StreamWriterToAuditFile("path"))
                {
                    if (0 <= result && result <= 100)
                    {
                        writerToEmpGrades.WriteLine(result);
                        writerToAudit.WriteLine($"Time: {DateTime.UtcNow}, Grade: {grade}");

                        if (GradeAdded != null)
                        {
                            GradeAdded(this, new EventArgs());
                        }
                        if (GradeAddedBelow30 != null && result < 30)
                        {
                            GradeAddedBelow30(this, new EventArgs());
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Wrong value of ({nameof(grade)}). Please input string from 0 to 100");
                    }
                }
            }
            else
            {
                throw new FormatException($"Invalid format of {nameof(grade)}");
            }
            return grade;
        }


        public override List<string> DeleteGrade(string gradeIndex)
        {
            int inputIndex;
            List<string> newGradesList = new List<string>();
            bool canParse = int.TryParse(gradeIndex, out inputIndex);

            if (canParse)
            {
                string tempFile = Path.GetTempFileName();
                var tempArr = _fileWrapper.ReturnStringArray();

                var deletedGrade = tempArr[inputIndex];
                tempArr[inputIndex] = "grade deleted";


                using (var writerToAudit = _fileWrapper.StreamWriterToAuditFile("path"))
                using (var readerFromTextFile = _fileWrapper.StreamReaderFromTextFile("path"))
                using (var writerToEmpGrades = _fileWrapper.StreamWriterToEmployeeGrades("path"))
                {
                    string line;
                    int index = 0;

                    writerToAudit.WriteLine($"Time: {DateTime.UtcNow}, Grade deleted: {deletedGrade} at index: {inputIndex}");
                    while ((line = readerFromTextFile.ReadLine()) != null)
                    {
                        writerToEmpGrades.WriteLine(tempArr[index]);
                        newGradesList.Add(tempArr[index]);
                        index++;
                    }
                }
                File.Delete("path");
                File.Move(tempFile, "path");

                if (GradeDeleted != null)
                {
                    GradeDeleted(this, new EventArgs());
                }
                Console.WriteLine();
            }
            else
            {
                throw new FormatException($"Invalid format of {nameof(gradeIndex)}");
            }
            return newGradesList;
        }



        public override string[] ShowEmployeeGrades()
        {
            Console.WriteLine();
            Console.WriteLine($"Grades of |{this.Name}|");
            Console.WriteLine();
            Console.WriteLine("Index  |  Grade");
            var grades = _fileWrapper.ReturnStringArray();

            WriteGradesToConsole(grades);

            return grades;
        }

        public void WriteGradesToConsole(string[] grades)
        {
            for (int i = 0; i < grades.Length; i++)
            {
                Console.WriteLine($"{i}  |  {grades[i]}");
            }
        }

        public List<string> ReturnEmployeeGradesAsList()
        {
            var tempList = new List<string>();

            using (var reader = _fileWrapper.StreamReaderFromTextFile("path"))
            {
                var line = reader.ReadLine();

                while (line != null)
                {
                    tempList.Add(line);
                    line = reader.ReadLine();
                }
            }
            return tempList;
        }


        public override Statistics GetStatistics()
        {
            var result = new Statistics();
            if (_fileWrapper.FileExists("path"))
            {
                using (var reader = _fileWrapper.StreamReaderFromTextFile("path"))
                {
                    var line = reader.ReadLine();

                    while (line != null)
                    {
                        bool canParse;
                        double parseResult;
                        if (canParse = double.TryParse(line, out parseResult))
                        {
                            result.Add(parseResult);
                        }
                        line = reader.ReadLine();
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("File with this name does not exist.");
            }
            return result;
        }

        public void ChangeThisEmployeeName(string newname)
        {
            var nameToCheck = newname;

            foreach (var ch in nameToCheck)
            {
                if (char.IsDigit(ch))
                {
                    throw new ArgumentException($"Name cannot contain numbers.");
                }
            }

            var oldName = this.Name;
            this.Name = newname;

            ReplaceOldNameInEmployeeListFile(oldName, this.Name);

        }

        public string ReplaceOldNameInEmployeeListFile(string oldName, string currentName)
        {
            string text = _fileWrapper.Text;
            text = text.Replace(oldName, currentName);
            return text;
        }
    }    
}
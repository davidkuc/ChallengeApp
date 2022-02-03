using System;
using ChallengeApp;
using System.Collections.Generic;
using Xunit;
using System.IO;
using System.Linq;
using Moq;
using System.Text;

namespace Challenge.Tests
{

    public class SavedEmployeeTests
    {
        [Fact]
        public void SavedEmployeeConstructorCreatesTXTFiles()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            fileWrapperMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");

            var empFilesExist = emp.EmployeeFilesExist;

            Assert.True(empFilesExist);
        }

        [Fact]
        public void GetStatisticsThrowsExceptionWhenEmployeeGradesFileDoesntExist()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            fileWrapperMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");

            Assert.Throws<FileNotFoundException>(() => emp.GetStatistics());
        }

        [Fact]
        public void AddGradeReturnsCorrectGrade()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");
            var memoryStreamGrades = new MemoryStream();
            var memoryStreamAudit = new MemoryStream();
            fileWrapperMock.Setup(x => x.StreamWriterToEmployeeGrades(It.IsAny<string>())).Returns(() => new StreamWriter(memoryStreamGrades));
            fileWrapperMock.Setup(x => x.StreamWriterToAuditFile(It.IsAny<string>())).Returns(() => new StreamWriter(memoryStreamAudit));

            var grade = emp.AddGrade("75");

            Assert.Equal("75", grade);
        }

        [Fact]
        public void AddGradeWritesToTextFiles()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");
            var memoryStreamGrades = new MemoryStream();
            var memoryStreamAudit = new MemoryStream();
            fileWrapperMock.Setup(x => x.StreamWriterToEmployeeGrades(It.IsAny<string>())).Returns(() => new StreamWriter(memoryStreamGrades));
            fileWrapperMock.Setup(x => x.StreamWriterToAuditFile(It.IsAny<string>())).Returns(() => new StreamWriter(memoryStreamAudit));
            var streamReaderFromStreamGrades = new StreamReader(memoryStreamGrades);
            var streamReaderFromStreamAudit = new StreamReader(memoryStreamAudit);

            emp.AddGrade("75");
            var actualGrade = Encoding.UTF8.GetString(memoryStreamGrades.ToArray()).Replace("\r\n", "");
            var actualAuditText = Encoding.UTF8.GetString(memoryStreamAudit.ToArray()).Replace("\r\n", "");

            Assert.Equal("75", actualGrade);
            Assert.Equal($"Time: {DateTime.UtcNow}, Grade: 75", actualAuditText);
        }

        [Fact]
        public void DeleteGradeReturnsNewGradesList()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");
            var expectedGrades = new List<string>();
            expectedGrades.Add("grade deleted");
            expectedGrades.Add("43");
            expectedGrades.Add("25");
            var gradesToMethod = new List<string>();
            gradesToMethod.Add("75");
            gradesToMethod.Add("43");
            gradesToMethod.Add("25");
            var gradesToMethodAsString = string.Join("\n", gradesToMethod);
            byte[] fakeFileBytes = Encoding.UTF8.GetBytes(gradesToMethodAsString);
            var fakeFileBytesMemoryStream = new MemoryStream(fakeFileBytes);
            var auditMemoryStream = new MemoryStream();
            var gradesMemoryStream = new MemoryStream();
            fileWrapperMock.Setup(x => x.ReturnStringArray()).Returns(gradesToMethod.ToArray());
            fileWrapperMock.Setup(x => x.StreamReaderFromTextFile(It.IsAny<string>())).Returns(() => new StreamReader(fakeFileBytesMemoryStream));
            fileWrapperMock.Setup(x => x.StreamWriterToEmployeeGrades(It.IsAny<string>())).Returns(() => new StreamWriter(gradesMemoryStream));
            fileWrapperMock.Setup(x => x.StreamWriterToAuditFile(It.IsAny<string>())).Returns(() => new StreamWriter(auditMemoryStream));

            var actualGrades = emp.DeleteGrade("0");

            Assert.Equal(expectedGrades, actualGrades);
        }

        [Fact]
        public void ShowEmployeeGradesReturnsGradesAsArray()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            var gradesToMethod = new string[3];
            gradesToMethod[0] = "75";
            gradesToMethod[1] = "43";
            gradesToMethod[2] = "32";
            var expectedGrades = new string[3];
            expectedGrades[0] = "75";
            expectedGrades[1] = "43";
            expectedGrades[2] = "32";
            fileWrapperMock.Setup(x => x.ReturnStringArray()).Returns(gradesToMethod);
            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");

            var grades = emp.ShowEmployeeGrades();

            Assert.Equal(grades, expectedGrades);
        }

        [Fact]
        public void WriteGradesToConsoleTest()
        {
            var gradesToMethod = new string[3];
            gradesToMethod[0] = "75 ";
            gradesToMethod[1] = "43 ";
            gradesToMethod[2] = "32 ";
            var expectedGrades = new string[3];
            expectedGrades[0] = "75 ";
            expectedGrades[1] = "43 ";
            expectedGrades[2] = "32 ";
            var emp = new TestSavedEmployee("empName", "M");
            var output = new StringWriter();
            Console.SetOut(output);

            emp.WriteGradesToConsole(gradesToMethod);
            
            Assert.Equal(expectedGrades, gradesToMethod);
        }

        [Fact]
        public void ReturnEmployeeGradesAsListTest()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");
            var expectedGrades = new List<string>();
            expectedGrades.Add("75");
            expectedGrades.Add("43");
            expectedGrades.Add("25");
            var gradesToMethod = new List<string>();
            gradesToMethod.Add("75 ");
            gradesToMethod.Add("43 ");
            gradesToMethod.Add("25 ");
            var gradesToMethodAsString = string.Join("", gradesToMethod);
            byte[] fakeFileBytes = Encoding.UTF8.GetBytes(gradesToMethodAsString.Replace(' ', '\n'));
            var memoryStream = new MemoryStream(fakeFileBytes);
            fileWrapperMock.Setup(x => x.StreamReaderFromTextFile(It.IsAny<string>())).Returns(() => new StreamReader(memoryStream));

            var result = emp.ReturnEmployeeGradesAsList();

            Assert.Equal(expectedGrades, result);
        }

        [Fact]
        public void GetStatisticsTest()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");
            var expectedGrades = new List<string>();
            expectedGrades.Add("75");
            expectedGrades.Add("43");
            expectedGrades.Add("25");
            var gradesToMethod = new List<string>();
            gradesToMethod.Add("75 ");
            gradesToMethod.Add("43 ");
            gradesToMethod.Add("25 ");
            var gradesToMethodAsString = string.Join("", gradesToMethod);
            byte[] fakeFileBytes = Encoding.UTF8.GetBytes(gradesToMethodAsString.Replace(' ', '\n'));
            var memoryStream = new MemoryStream(fakeFileBytes);
            fileWrapperMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            fileWrapperMock.Setup(x => x.StreamReaderFromTextFile(It.IsAny<string>())).Returns(() => new StreamReader(memoryStream));

            var stats = emp.GetStatistics();

            Assert.Equal(75, stats.Maxgrade);
            Assert.Equal(25, stats.Mingrade);
            Assert.Equal(47.7, Math.Round(stats.Average, 1));
            Assert.Equal('D', stats.Letter);
        }

        [Fact]
        public void ChangeThisEmployeeNameTest()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");
            var oldText = "empName";
            fileWrapperMock.Setup(x => x.Text).Returns(oldText);

            emp.ChangeThisEmployeeName("newEmpName");

            Assert.NotEqual("empName", emp.Name);
            Assert.Equal("newEmpName", emp.Name);
        }

        [Fact]
        public void ReplaceOldNameInEmployeeListFileTest()
        {
            var fileWrapperMock = new Mock<IFileWrapper>();
            var emp = new TestSavedEmployee(fileWrapperMock.Object, "empName", "M");
            var oldText = $"Joined: {DateTime.UtcNow}, Employee name: empName, sex: M";
            var expectedText = $"Joined: {DateTime.UtcNow}, Employee name: newEmpName, sex: M";
            fileWrapperMock.Setup(x => x.Text).Returns(oldText);
            var oldName = "empName";
            var newName = "newEmpName";

            var actualText = emp.ReplaceOldNameInEmployeeListFile(oldName, newName);

            Assert.Equal(expectedText, actualText);
        }
    }
}
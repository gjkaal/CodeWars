using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.CompilerServices;

namespace MsTestCodeWars;

[TestClass]
public class StringHandlingTests
{
    public static ReadOnlySpan<char> GetExtensionWithSpan(string value)
    {
        return value.AsSpan(value.LastIndexOf('.') + 1);
    }

    public static string GetExtensionWithSubstring(string value)
    {
        return value.Split('.').Last();
    }

    [DataTestMethod]
    [DataRow("filename.txt", "txt")]
    public void FindExtensionInString(string input, string expected)
    {
        var findWithSpan = new string(GetExtensionWithSpan(input));
        Assert.AreEqual(expected, findWithSpan);
        var findWithSubstring = new string(GetExtensionWithSubstring(input));
        Assert.AreEqual(expected, findWithSubstring);
    }

    [DataTestMethod]
    [DataRow("filename.txt")]
    public void FindExtensionIsComparable(string input)
    {
        Assert.IsTrue(GetExtensionWithSpan(input) == GetExtensionWithSubstring(input));
    }
}

[TestClass]
public class FileSystemServicesTests
{
    const string BaseFolder = "C:\\Users\\gjkaa\\Downloads\\data\\attachments";
    public FileSystemServicesTests()
    {
        
    }

    [TestMethod]
    public void CanReadAllFiles()
    {
        foreach (var file in FindAllFiles(BaseFolder))
        {
            Assert.IsTrue(file.Exists);
        }
    }

    [TestMethod]
    public void CanRenameAllFiles()
    {
        foreach (var file in FindAllFiles(BaseFolder))
        {
            var fileName = file.Name;
            var filePath = file.FullName;
            filePath = filePath.Substring(0, filePath.LastIndexOfAny(new char[] { '/', '\\' }));

            if (!fileName.All(char.IsLower))
            {
                Console.WriteLine($"Renaming {fileName}");
                File.Move(
                    Path.Combine(filePath, fileName), 
                    Path.Combine(filePath, fileName.ToLowerInvariant()));
            }
        }
        Assert.IsTrue(true, "Rename completed");
    }

    private IEnumerable<FileInfo> FindAllFiles(string pathName) => FindAllFiles(pathName, 6);

    private IEnumerable<FileInfo> FindAllFiles(string pathName, int depth)
    {
        if (!Directory.Exists(pathName)) throw new Exception("Invalid folder");
        if (depth==0) throw new Exception($"Too much folders at path: {pathName}");

        foreach (var d in Directory.EnumerateDirectories(pathName))
        {
            foreach(var f in FindAllFiles(d, depth - 1))
            {
                yield return f;
            }
        }
        foreach (var f in Directory.EnumerateFiles(pathName))
        {
            yield return new FileInfo(f);
        }
    }
}
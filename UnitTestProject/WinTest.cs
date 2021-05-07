using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject
{
    public class WinTest
    {
        static Dictionary<int, Kemorave.Win.RegistryTools.ProgramInfo> Programes = new Dictionary<int, Kemorave.Win.RegistryTools.ProgramInfo>();
        [STAThread]
        public static void Main()
        {
           
            var programes = Kemorave.Win.RegistryTools.RegistryHelper.GetAllInstalledPrograms().OrderBy(it=>it.ToString());
            int index = 0;
            foreach (var item in programes)
            {
                index++;
                Writing.Write($"{index} - {item} ({item.EstimatedSize??0} bytes)", ConsoleColor.Red);
                Programes[index] = item;
            }
        A:
            Writing.Write("\n========== Input an application index number to uninstall ===========".ToUpper());
            var line = Console.ReadLine();
            if (int.TryParse(line, out index))
            {
                Kemorave.Win.RegistryTools.RegistryHelper.UninstallApplication(Programes[index].UninstallString);
            }
            else
            {
                Writing.Write("\nWrong Input".ToUpper());
                goto A;
            }
        }

    }
}
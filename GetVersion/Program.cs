

using GetVersion.DbContexts;
using GetVersion.Model;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class Program
{

    public static async Task<int> Main(string[] args)
    {

        string? ip = "";
        int? port = null;

        //Ввод исходных данных - IP и номер порта хоста Apache 
        Regex regexIp = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");

        while (!regexIp.IsMatch(ip ?? ""))
        {
            Console.WriteLine("Enter correct IP adress of apache host or key 'e' for exit:");
            ip = Console.ReadLine();

            if (ip.ToLower() == "e")
            {
                Console.WriteLine("Bye!");
                return 0;
            }
        }



        while (port == null)
        {
            Console.WriteLine("Enter port number or key 'e' for exit:");
            string? tempstr = Console.ReadLine();
            if (int.TryParse(tempstr, out int r))
            {
                port = r;
            }
            else if ( (tempstr?.ToLower() ?? "") == "e")
            {
                Console.WriteLine("Bye!");
                return 0;
            }
        }


        //создаем процесс с требуемыми параметрами (адрес страницы для опроса версии, IP и порт) и перенаправляем его данные в текущий процесс.  
        ProcessStartInfo procInfo = new ProcessStartInfo();
        procInfo.FileName = "nmap";
        procInfo.Arguments = "--script "+$"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Scripts{Path.DirectorySeparatorChar}Httpget" + @" --script-args http-get.path=/testpage " + ip + " -p" + port.ToString();
        procInfo.RedirectStandardOutput = true;

        Process? GetVersion = Process.Start(procInfo);
        string? response = await GetVersion?.StandardOutput.ReadToEndAsync();
       
        //Пытаемся найти подстроку Server Version:version
        Regex getRawVersion = new Regex(@"Server Version:((\.|\s|\d)*)");
        var m = getRawVersion.Match(response);
        string rawVersion = m.Value;

        //Если скрипт успешно вернул версию, достанем ее
        string? version = null; 
        if (!string.IsNullOrEmpty(rawVersion))
        {
            Regex getVersion = new Regex(@"\d(\d|\.)*");
            var m1 = getVersion.Match(rawVersion);
            version = m1.Value;
        }


        //Если достали версию сохраним ее в бд иначе запишем в бд аларм. Для записи использую Entity framework, который сам создаст базу, если ее нет, иначе подключется к существующей согласно настроек в datacontext
        if (!string.IsNullOrEmpty(version))
        {
            Console.WriteLine($"Your apache server version is {version}");

            //write to database
            using (var context = new PosgresqlContext())
            {
                await context.Versions.AddAsync(new VersionData() { Version = version, Timestamp=DateTime.UtcNow });
                context.SaveChanges();
            }

            Console.WriteLine($"Data saved in database successfully");
        }
        else
        {
            Console.WriteLine($"Сouldn't read the version");

            //write to database
            using (var context = new PosgresqlContext())
            {
                await context.Versions.AddAsync(new VersionData() { Version = "Error! Can't read version!", Timestamp=DateTime.UtcNow });
                context.SaveChanges();
            }

            Console.WriteLine($"Data saved in database successfully");

        }
        
        Console.ReadLine();

        return 0;
    }

}
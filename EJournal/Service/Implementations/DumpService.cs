using EJournal.Data;
using EJournal.Response;
using EJournal.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySQLBackupNetCore;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Drawing;
using System.IO;

namespace EJournal.Service.Implementations
{
    public class DumpService : IDumpService
    {
        private readonly IConfiguration _configuration;
        private readonly JournalDbContext _dbContext;
        public DumpService(IConfiguration appConfig, JournalDbContext dbContext) 
        {
            _configuration = appConfig;
            _dbContext = dbContext;
        }

        public async Task<BaseResponse<string[]>> GetBackUpsName()
        {
            return await Task<BaseResponse<string[]>>.Run(() =>
            {
                BaseResponse<string[]> response = new BaseResponse<string[]>();
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(_configuration.GetValue<string>("DumpDatabase:backupDir"));
                    List<string> myList = new List<string>(); //ваш лист с Bitmap
                    foreach (var file in directoryInfo.GetFiles()) //проходим по файлам
                    {
                        //получаем расширение файла и проверяем подходит ли оно нам 
                        if (Path.GetExtension(file.FullName) == ".sql")
                            myList.Add(file.Name); //если расширение подошло, создаём Bitmap
                    }
                    response.StatusCode = StatusCodeEnum.OK;
                    response.Data = myList.Order().ToArray();
                    return response;
                }
                catch
                {
                    return new BaseResponse<string[]>
                    {
                        StatusCode = StatusCodeEnum.InternalServerError
                    };
                }
            });
		}
		public async Task DeleteDump(string dumpName)
        {
            await Task.Run(() =>
            {
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(_configuration.GetValue<string>("DumpDatabase:backupDir"));
                    List<string> myList = new List<string>(); //ваш лист с Bitmap
                    FileInfo? file = directoryInfo.GetFiles().FirstOrDefault(f => f.Name == dumpName);
                    if (file != null)
                    {
                        file.Delete();
                    }
                }
                catch
                {

                }
            });
		}


		public async Task CreateDump()
        {
            
            await Task.Run(() =>
            {
                string backupDir = _configuration.GetValue<string>("DumpDatabase:backupDir");
                string connectionString = _configuration.GetValue<string>("DumpDatabase:connectionString");

                string file = backupDir + "backup_" + DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss") + ".sql";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            try
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                mb.ExportToFile(file);
                                conn.Close();
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            });
        }
        public async Task<BaseResponse<string>> JumpDump(string dumpName)
        {
			return await Task<BaseResponse<string>>.Run(() =>
			{
				string backupDir = _configuration.GetValue<string>("DumpDatabase:backupDir");
				string connectionString = _configuration.GetValue<string>("DumpDatabase:connectionString");

				string file = backupDir+dumpName;

				using (MySqlConnection conn = new MySqlConnection(connectionString))
				{
					using (MySqlCommand cmd = new MySqlCommand())
					{
						using (MySqlBackup mb = new MySqlBackup(cmd))
						{
                            try
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                mb.ImportFromFile(file);
                                conn.Close();
                            }
                            catch
                            {
                                return new BaseResponse<string>
                                {
                                    Data = "Ошибка востановления!",
                                    StatusCode = StatusCodeEnum.InternalServerError
                                };
                            }
						}
					}
				}
                return new BaseResponse<string>
                {
                    Data = "Резервная копия \""+ dumpName + "\" успешно применена!",
                    StatusCode = StatusCodeEnum.OK
				};
			});
		}
	}
}

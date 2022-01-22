using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NCLCore
{
    public class SDK
    {
        private static readonly ILog log = LogManager.GetLogger("SDK");
        public  enum DownloadSource
        {
            Official,
            MCBBS,
            BMCLAPI,
            Custom
        }
        public string GetDownloadSoure(DownloadSource ds)
        {
            switch (ds)
            {
                case DownloadSource.Official:
                    return null;

                case DownloadSource.MCBBS:
                    return "https://download.mcbbs.net/";
                case DownloadSource.BMCLAPI:
                    return "https://bmclapi2.bangbang93.com";
                case DownloadSource.Custom:
                    return "custom";
                default:
                    return null;
            }       
        }
        public List<Client> GetALLClient(string dir)
        {
            List<Client> clients = new List<Client>();
            try
            {
                DirectoryInfo root = new DirectoryInfo(dir);
               
                foreach(DirectoryInfo directory in root.GetDirectories()){
                    if(directory.Name== "versions")
                    {
                        root=new DirectoryInfo(directory.FullName);
                        foreach(DirectoryInfo file in directory.GetDirectories())
                        { 
                            Client client = new Client();
                            foreach(FileInfo fileInfo in file.GetFiles())
                            {
                               
                                if (fileInfo.Name == file.Name + ".json")
                                {
                                    client.Name=file.Name;
                                    try
                                    {
                                        using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(fileInfo.FullName))
                                        {
                                            using (JsonTextReader reader = new JsonTextReader(jsonfile))
                                            {
                                            JObject jObject = (JObject)JToken.ReadFrom(reader);
                                                if (jObject["inheritsFrom"] != null)
                                                {
                                                    client.McVer = jObject["inheritsFrom"].ToString();
                                                }
                                                else
                                                {
                                                    client.McVer = jObject["id"].ToString();
                                                }
                                               
                                            }
                                        }
                                    }catch (Exception ex){}
                                }
                                if (fileInfo.Name == file.Name + ".ncharge")
                                {
                                    client.Ncharge = true;
                                    try
                                    {
                                        using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(fileInfo.FullName))
                                        {
                                            using (JsonTextReader reader = new JsonTextReader(jsonfile))
                                            {
                                                JObject jObject = (JObject)JToken.ReadFrom(reader);
                                                try
                                                {
                                                    client.NchargeVer = jObject["ver"].ToString();
                                                }
                                                catch (Exception ex)
                                                {
                                                    
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex) { }
                                }

                            }
                            if (client.isNotNull())
                            {
                                clients.Add(client);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                log.Error(ex);
               return clients;
            }

            return clients;
        }
    }
    
}

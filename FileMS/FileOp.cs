using MediaInfo.DotNetWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace FileMS
{
    [Description("FileOp")]
    public class FileOp
    {
        
        private string sPath;     
        private string sExt;
        private string dExt;
        private string dPath;

        [Description("Action 操作")]
        public string Action { get; set; }

        [Description("SPath 原路径")]
        public string SPath { get => sPath; set => sPath = value; }

        [Description("SExt 原扩展")]
        [DefaultValue("*.mp4")]
        public string SExt { get => sExt; set => sExt = value; }

        [Description("DExt 目标扩展")]
        public string DExt { get => dExt; set => dExt = value; }

        [Description("DPath 目标路径")]
        public string DPath { get => dPath; set => dPath = value; }

        public void MoveJoin() {
            var sMap = new Dictionary<string, FileInfo>();
            var sFiles = new DirectoryInfo(sPath).EnumerateFiles(sExt, SearchOption.TopDirectoryOnly);
            foreach (var file in sFiles)
            {
                sMap.Add(Path.GetFileNameWithoutExtension(file.Name), file);
            }

            var dFiles = new DirectoryInfo(dPath).EnumerateFiles(dExt, SearchOption.AllDirectories);
            foreach (var file in dFiles)
            {
                var key = Path.GetFileNameWithoutExtension(file.Name);
                if (sMap.ContainsKey(key))
                {
                    var sFile = sMap[key];
                    var dpath = file.DirectoryName + @"\" + sFile.Name;

                    try
                    {
                        Console.WriteLine(string.Format("move {0} to {1}", sFile.FullName, dpath));
                        sFile.MoveTo(dpath);
                        file.Delete();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }


                }
            }
        }

        public void FindMp4()
        {
            var dPath = sPath + @"\mp4\";
            if (!Directory.Exists(dPath))
            {
                Directory.CreateDirectory(dPath);
            }
            var sFiles = new DirectoryInfo(sPath).EnumerateFiles("*", SearchOption.TopDirectoryOnly);
            foreach (var file in sFiles)
            {
                var info = FileMediaInfo(file.FullName);
                if (info.VideoCodec.StartsWith("AVC"))
                {
                    Console.WriteLine("file:{0}\tv:{1}\ta:{2}", file.Name, info.VideoCodec, info.AudioCodec);
                    file.MoveTo(dPath + Path.GetFileNameWithoutExtension(file.Name) + ".mp4");
                }
                // Console.WriteLine("file:{0}\tv:{1}\ta:{2}",file.Name,info.VideoCodec,info.AudioCodec);
            }
        }

        private static MediaInfoWrapper FileMediaInfo(string file)
        {
            var mInfo = new MediaInfoWrapper(file, Environment.CurrentDirectory);
            //Console.WriteLine(mInfo.VideoCodec);
            return mInfo;
        }

        public void MoveSum()
        {
            var sMap = new Dictionary<string, List<FileInfo>>();
            var sFiles = new DirectoryInfo(sPath).EnumerateFiles("*", SearchOption.TopDirectoryOnly);
            foreach (var file in sFiles)
            {
                var key = Path.GetFileNameWithoutExtension(file.Name);
                if (!sMap.ContainsKey(key))
                {
                    sMap.Add(key, new List<FileInfo>());
                }
                sMap[key].Add(file);
            }

            if (!Directory.Exists(dPath))
                Directory.CreateDirectory(dPath);

            foreach (var kv in sMap)
            {
                if (kv.Value.Count > 1)
                {
                    foreach (var file in kv.Value)
                    {
                        var dFilePath = dPath +@"\"+ file.Name;
                        try
                        {
                            Console.WriteLine(string.Format("move {0} to {1}", file.FullName, dFilePath));
                            file.MoveTo(dFilePath);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            var props = this.GetType().GetProperties();
            var sb = new StringBuilder();
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                sb.AppendFormat("{0}={1}", prop.Name, prop.GetValue(this));
                if (i < props.Length - 1) {
                    sb.Append(" | ");
                }
            }
            return sb.ToString();
        }
    }
}

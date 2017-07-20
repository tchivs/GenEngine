using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenEngine
{
    internal class Engine : FileOperations
    {
        private string _OldPCI;

        ///旧的PCI，十六进制的
        public string OldPCI
        {
            get => _OldPCI;
            set { _OldPCI = HexConvert.TenToHex(value); }
        }

        private string _NewPCI;
        ///新的PCI
        public string NewPCI
        {
            get => _NewPCI;
            set { _NewPCI = HexConvert.TenToHex(value); }
        }

        public string[] _NewNames { get; set; }

        /// <summary>
        /// 设置新的文件名
        /// </summary>
        /// <param name="oldpci"></param>
        /// <param name="newpci"></param>
        private void SetNewNames(string oldpci, string newpci)
        {
            for (int i = 0; i < FileNames.Length; i++)
            {

                //判断文件名是否有PCI在里面
                if (this[i].Contains(oldpci))
                {
                    //如果有，就把旧的PCI换成新的
                    _NewNames[i] = filePath + @"\" + this[i].Replace(oldpci, newpci);
                }
                else
                {
                    //如果没有，那就在文件名前加上PCI
                    _NewNames[i] = filePath + "PCI-" + newpci + this[i];
                }
            }

        }


        /// <summary>
        /// 索引器，用来得到正在操作的文件名
        /// </summary>
        /// <param name="index"></param>
        /// <returns>返回一个字符串，文件名</returns>
        public string this[int index]
        {
            get { return Path.GetFileName(FileNames[index]); }
        }

        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="FileNames">要操作的文件名</param>
        /// <param name="oldpci">旧的PCI</param>
        /// <param name="newpci">新的PCI</param>
        public Engine(string[] FileNames, string oldpci, string newpci)
        {
            this.FileNames = FileNames;
            this._NewNames = new string[FileNames.Length];
            this.OldPCI = oldpci;
            this.NewPCI = newpci;
            SetNewNames(oldpci, newpci);
        }

        /// <summary>
        /// 文件名返回PCI
        /// </summary>
        /// <param name="str">要返回PCI的文件名</param>
        /// <returns>PCI</returns>
        public static string GetOldPci(string str)
        {
            // string str = FileNameWithoutPath[0];
            string pattern = @"PCI(?<grp0>[^\D]+)-";
            Regex reg = new Regex(pattern);
            Match m = reg.Match(str);
            str = m.ToString().Replace("PCI", "");
            str = str.Replace("-", "");
            if (m.Success)
            {
                return str;

            }
            return null;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        public void ReadFile()
        {
            for (int i = 0; i < FileNames.Length; i++)
            {
                using (FileStream fs = new FileStream(FileNames[i], FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        //
                        StringBuilder data = new StringBuilder();

                        try
                        {
                            while (true)
                            {
                                data.Append(HexConvert.ByteToHex(br.ReadByte()));
                            }

                        }
                        catch (Exception)
                        {
                            GetNewDate(data);
                            // data.Replace("00"+ OldPCI, "00"+ NewPCI);
                            //把修改好的十六进制文本转换成字节数组
                            byte[] byteArray = HexConvert.HexStringToByteArray(data.ToString());
                            using (FileStream newFs = File.Open(_NewNames[i], FileMode.Create))
                            {
                                newFs.Write(byteArray, 0, byteArray.Length);
                            }
                        }




                    }
                }

            }

        }
        /// <summary>
        /// 去重复数组
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string[] DelRepeatData(List<string> a)
        {
            return a.GroupBy(p => p).Select(p => p.Key).ToArray();
        }

        private void GetNewDate(StringBuilder data)
        {
            data.Replace("F04E" + OldPCI, "F04E" + NewPCI);
            //data.Replace("0030" + OldPCI, "0030" + NewPCI);没有用
            // data.Replace("00" + OldPCI, "00" + NewPCI);不行
            //  data.Replace("3A" + OldPCI, "3A" + NewPCI);
            // data.Replace("1300" + OldPCI, "1300" + NewPCI);没用
            //data.Replace("114C" + OldPCI, "114C" + NewPCI);
            //data.Replace("115C" + OldPCI, "115C" + NewPCI);
            //存放数据
            List<string> list = new List<string>();
            Regex reg = new Regex(@"11[0-9a-fA-F]." + OldPCI);
           MatchCollection matches = reg.Matches(data.ToString());
            foreach (var item in matches)
            {
                list.Add(item.ToString());
            }
           var TEMP=  DelRepeatData(list);
            list.Clear();
            list.AddRange(TEMP);
            foreach (var item in list)
            {
                data.Replace(item, item.Substring(0, 4) + NewPCI);
            }
            //MatchEvaluator evaluator = new MatchEvaluator(this.ConvertToData);
            //for (int i = 0; i <matches.Count; i++)
            //{
            //    reg.Replace(data, evaluator(matches[i]));
            //}

            //foreach (var item in matches)
            //{
            //    list.Add( item.ToString());
            //}

            //for (int j = 10; j <= 99; j++)
            //{
            //    data.Replace("11" + j + OldPCI, "11" + j + NewPCI);
            //}
            //修复遗漏PCI
            //for (int i = 0; i <= 9; i++)
            //{

            //    data.Replace("11" + i + "A" + OldPCI, "11" + i + "A" + NewPCI);
            //    data.Replace("11" + i + "B" + OldPCI, "11" + i + "B" + NewPCI);
            //    data.Replace("11" + i + "C" + OldPCI, "11" + i + "C" + NewPCI);
            //    data.Replace("11" + i + "D" + OldPCI, "11" + i + "D" + NewPCI);
            //    data.Replace("11" + i + "E" + OldPCI, "11" + i + "E" + NewPCI);
            //    data.Replace("11" + i + "F" + OldPCI, "11" + i + "F" + NewPCI);
            //    data.Replace("11" + "A" + i + OldPCI, "11" + "A" + i + NewPCI);
            //    data.Replace("11" + "B" + i + OldPCI, "11" + "B" + i + NewPCI);
            //    data.Replace("11" + "C" + i + OldPCI, "11" + "C" + i + NewPCI);
            //    data.Replace("11" + "D" + i + OldPCI, "11" + "D" + i + NewPCI);
            //    data.Replace("11" + "E" + i + OldPCI, "11" + "E" + i + NewPCI);
            //    data.Replace("11" + "F" + i + OldPCI, "11" + "F" + i + NewPCI);
            //}
            // 0000 - 0500的才换，不然有的LOG会没有轨迹
            for (int k = 0; k <= 5; k++)
            {
                data.Replace("0" + k + "00" + OldPCI, "0" + k + "00" + NewPCI);
            }

        }

        private string ConvertToData(Match match)
        {

            return match.Value.Substring(0, 4);
        }
    }
}

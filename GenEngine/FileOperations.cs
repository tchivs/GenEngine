using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenEngine
{
    #region Documentation -----------------------------------------------------------------------------
    /// Class:  FileOperations
    ///
    /// Summary:  文件操作类
    ///
    /// Author: Topiv
    ///
    /// Date:   2017-06-25
    #endregion

  public abstract  class FileOperations
    {
        /// <summary>
        /// 要操作的文件名
        /// </summary>
        public string[] FileNames { get; set; }



        /// <summary>
        /// 文件目录
        /// </summary>
        public string filePath { get=>Path.GetDirectoryName(FileNames[0])+@"\"; }

        
    }
}

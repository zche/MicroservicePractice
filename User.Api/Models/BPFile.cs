using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Api.Models
{
    public class BPFile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        /// <summary>
        /// 上传的源文件地址
        /// </summary>
        public string OriginFilePath { get; set; }
        /// <summary>
        /// 格式转换后的文件地址
        /// </summary>
        public string FormatFilePath { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}

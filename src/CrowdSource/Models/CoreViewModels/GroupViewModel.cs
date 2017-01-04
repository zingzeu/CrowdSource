using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreViewModels
{
    public class GroupViewModel
    {
        public int GroupId { get; set; }
        
        public string TextBUC { get; set; }
        public string TextChinese { get; set; }
        public string TextEnglish { get; set; }

        /// <summary>
        /// "俗"
        /// </summary>
        public bool IsOral { get; set; }
        /// <summary>
        /// "文"
        /// </summary>
        public bool IsLiterary { get; set; }

        /// <summary>
        /// 是否是关键行
        /// </summary>
        public bool IsPivotRow { get; set; }
        /**
         * IF IT IS A PIVOT ROW 
         **/

       
        public string BoPoMoFo { get; set; }

        public string Radical { get; set; }

        /**
         * END IF
         **/
        
        /// <summary>
        /// 异常标记
        /// If flagged, a group will be removed from the queue.
        /// </summary>
        public bool Flagged { get; set; }

        public FlagEnum FlagType { get; set; }
    }

    public enum FlagEnum
    {
        SegmentationError, //图片切割错误
        OtherError
    }
}

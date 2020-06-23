﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 创建孔特征
    /// </summary>
    public class HoleBuilder
    {
        private CircularFaceList list;

        public HoleBuilder(CircularFaceList cir)
        {
            this.list = cir;
        }
        public AbstractHoleFeater GetHoleFeater()
        {

        }
        /// <summary>
        /// 判断是否是盲孔
        /// </summary>
        /// <returns></returns>
        public bool GetAskBlindHole()
        {
            Vector3d vec1 = this.list.CircleFaceList[0].Direction;
            Vector3d vec2 = new Vector3d(-vec1.X, -vec1.Y, -vec1.Z);
            int k1 = TraceARay.AskTraceARay(this.list.CircleFaceList[0].Data.Face.GetBody(), this.list.CircleFaceList[0].StartPt, vec1);
            int k2 = TraceARay.AskTraceARay(this.list.CircleFaceList[0].Data.Face.GetBody(), this.list.CircleFaceList[0].StartPt, vec1);
            return k1 != 0 || k2 != 0;
        }
    }
}

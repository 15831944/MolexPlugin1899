using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MolexPlugin.Model
{
    [Serializable]
    public class ElectrodeInfo11 : ISetAttribute,ICloneable
    {
        /// <summary>
        /// 电极名
        /// </summary>
        public string EleName { get; set; }
        /// <summary>
        /// X向PH
        /// </summary>
        public double PitchX { get; set; } = 0;
        /// <summary>
        /// X向个数
        /// </summary>
        public int PitchXNum { get; set; } = 0;
        /// <summary>
        /// Y向PH
        /// </summary>
        public double PitchY { get; set; } = 0;
        /// <summary>
        /// Y向个数
        /// </summary>
        public int PitchYNum { get; set; } = 0;
        /// <summary>
        /// 粗放
        /// </summary>
        public double CrudeInter { get; set; } = 0;
        /// <summary>
        /// 粗放个数
        /// </summary>
        public int CrudeNum { get; set; } = 0;
        /// <summary>
        /// 中放
        /// </summary>
        public double DuringInter { get; set; } = 0;
        /// <summary>
        /// 中放个数
        /// </summary>
        public int DuringNum { get; set; } = 0;
        /// <summary>
        /// 精放
        /// </summary>
        public double FineInter { get; set; } = 0;
        /// <summary>
        /// 精放个数
        /// </summary>
        public int FineNum { get; set; } = 0;
        /// <summary>
        /// 材料
        /// </summary>
        public string Material { get; set; }
        /// <summary>
        /// 电极类型
        /// </summary>
        public string EleType { get; set; }
        /// <summary>
        /// 放电条件
        /// </summary>
        public string Condition { get; set; }
        /// <summary>
        /// 拉升值
        /// </summary>
        public double Extrudewith { get; set; }
        /// <summary>
        /// CH值
        /// </summary>
        public string Ch { get; set; }
        /// <summary>
        /// 借用电极
        /// </summary>
        public string BorrowName { get; set; }
        /// <summary>
        /// 电极设定参数
        /// </summary>
        public double[] EleSetValue { get; set; } = new double[3];
        /// <summary>
        /// 备料值
        /// </summary>
        public int[] Preparation { get; set; } = new int[3];
        /// <summary>
        /// 标准料
        /// </summary>
        public bool IsPreparation { get; set; }
        /// <summary>
        /// 电极描述
        /// </summary>
        public string ElePresentation { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 工艺
        /// </summary>
        public string Technology { get; set; }
        /// <summary>
        /// 加工模板
        /// </summary>
        public string CamTemplate { get; set; }
        /// <summary>
        /// 放电面积
        /// </summary>
        public double Area { get; set; }
        /// <summary>
        /// 电极编号
        /// </summary>
        public int EleNumber { get; set; }
        /// <summary>
        /// 跑位
        /// </summary>
        public string Positioning { get; set; }
        /// <summary>
        /// /Z向基准
        /// </summary>
        public bool ZDatum { get; set; }
        /// <summary>
        /// 基准台宽度
        /// </summary>
        public double DatumWidth { get; set; }
        /// <summary>
        /// 电极单齿外形
        /// </summary>
        public double[] EleHeadDis { get; set; } = new double[2] { 0, 0 };
        /// <summary>
        /// 电极单齿最小距离
        /// </summary>
        public double EleMinDim { get; set; } = 9999;
        /// <summary>
        /// 设置属性
        /// </summary>
        public void SetAttribute(Part obj)
        {
            AttributeUtils.AttributeOperation("EleName", this.EleName, obj);
            AttributeUtils.AttributeOperation("BorrowName", this.BorrowName, obj);

            AttributeUtils.AttributeOperation("PitchX", this.PitchX, obj);
            AttributeUtils.AttributeOperation("PitchXNum", this.PitchXNum, obj);
            AttributeUtils.AttributeOperation("PitchY", this.PitchY, obj);
            AttributeUtils.AttributeOperation("PitchYNum", this.PitchYNum, obj);

            AttributeUtils.AttributeOperation("CrudeInter", this.CrudeInter, obj);
            AttributeUtils.AttributeOperation("CrudeNum", this.CrudeNum, obj);
            AttributeUtils.AttributeOperation("DuringInter", this.DuringInter, obj);
            AttributeUtils.AttributeOperation("DuringNum", this.DuringNum, obj);
            AttributeUtils.AttributeOperation("FineInter", this.FineInter, obj);
            AttributeUtils.AttributeOperation("FineNum", this.FineNum, obj);

            AttributeUtils.AttributeOperation("Material1", this.Material, obj);
            AttributeUtils.AttributeOperation("EleType", this.EleType, obj);
            AttributeUtils.AttributeOperation("Condition", this.Condition, obj);
            AttributeUtils.AttributeOperation("Extrudewith", this.Extrudewith, obj);
            AttributeUtils.AttributeOperation("CH", this.Ch, obj);

            AttributeUtils.AttributeOperation("IsPreparation", this.IsPreparation, obj);
            AttributeUtils.AttributeOperation("Remarks", this.Remarks, obj);
            AttributeUtils.AttributeOperation("Technology", this.Technology, obj);
            AttributeUtils.AttributeOperation("CamTemplate", this.CamTemplate, obj);

            AttributeUtils.AttributeOperation("EleSetValue", this.EleSetValue, obj);
            AttributeUtils.AttributeOperation("Preparation", this.Preparation, obj);
            AttributeUtils.AttributeOperation("ElePresentation", this.ElePresentation, obj);
            AttributeUtils.AttributeOperation("Area", this.Area, obj);
            AttributeUtils.AttributeOperation("EleNumber", this.EleNumber, obj);
            AttributeUtils.AttributeOperation("Positioning", this.Positioning, obj);

            AttributeUtils.AttributeOperation("DatumWidth", this.DatumWidth, obj);
            AttributeUtils.AttributeOperation("EleHeadDis", this.EleHeadDis, obj);
            AttributeUtils.AttributeOperation("EleMinDim", this.EleMinDim, obj);
        }
        /// <summary>
        /// 读取属性
        /// </summary>
        public void GetAttribute(Part obj)
        {
            this.EleName = AttributeUtils.GetAttrForString(obj, "EleName");
            this.BorrowName = AttributeUtils.GetAttrForString(obj, "BorrowName");

            this.PitchX = AttributeUtils.GetAttrForDouble(obj, "PitchX");
            this.PitchXNum = AttributeUtils.GetAttrForInt(obj, "PitchXNum");
            this.PitchY = AttributeUtils.GetAttrForDouble(obj, "PitchY");
            this.PitchYNum = AttributeUtils.GetAttrForInt(obj, "PitchYNum");

            this.CrudeInter = AttributeUtils.GetAttrForDouble(obj, "CrudeInter");
            this.CrudeNum = AttributeUtils.GetAttrForInt(obj, "CrudeNum");
            this.DuringInter = AttributeUtils.GetAttrForDouble(obj, "DuringInter");
            this.DuringNum = AttributeUtils.GetAttrForInt(obj, "DuringNum");
            this.FineInter = AttributeUtils.GetAttrForDouble(obj, "FineInter");
            this.FineNum = AttributeUtils.GetAttrForInt(obj, "FineNum");

            this.Material = AttributeUtils.GetAttrForString(obj, "Material1");
            this.EleType = AttributeUtils.GetAttrForString(obj, "EleType");
            this.Condition = AttributeUtils.GetAttrForString(obj, "Condition");
            this.Extrudewith = AttributeUtils.GetAttrForDouble(obj, "Extrudewith");
            this.Ch = AttributeUtils.GetAttrForString(obj, "CH");

            this.IsPreparation = AttributeUtils.GetAttrForBool(obj, "IsPreparation");
            this.Remarks = AttributeUtils.GetAttrForString(obj, "Remarks");
            this.Technology = AttributeUtils.GetAttrForString(obj, "Technology");
            this.CamTemplate = AttributeUtils.GetAttrForString(obj, "CamTemplate");

            for (int i = 0; i < 3; i++)
            {
                this.EleSetValue[i] = AttributeUtils.GetAttrForDouble(obj, "EleSetValue", i);
            }
            for (int i = 0; i < 3; i++)
            {
                this.Preparation[i] = AttributeUtils.GetAttrForInt(obj, "Preparation", i);
            }

            this.Positioning = AttributeUtils.GetAttrForString(obj, "Positioning");
            this.ElePresentation = AttributeUtils.GetAttrForString(obj, "ElePresentation");
            this.Area = AttributeUtils.GetAttrForDouble(obj, "Area");
            this.EleNumber = AttributeUtils.GetAttrForInt(obj, "EleNumber");
            this.DatumWidth = AttributeUtils.GetAttrForDouble(obj, "DatumWidth");
            this.EleMinDim = AttributeUtils.GetAttrForDouble(obj, "EleMinDim");
            for (int i = 0; i < 2; i++)
            {
                this.EleHeadDis[i] = AttributeUtils.GetAttrForDouble(obj, "EleHeadDis", i);
            }
        }
        /// <summary>
        /// 转换为DataTable
        /// </summary>
        /// <returns></returns>
        private static DataTable CreateDataTable()
        {
            DataTable dataTable = new DataTable(typeof(ElectrodeInfo11).Name);
            foreach (PropertyInfo propertyInfo in typeof(ElectrodeInfo11).GetProperties())
            {
                dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
            }

            dataTable.Columns.Add("EleSetValueX", Type.GetType("System.Double"));
            dataTable.Columns.Add("EleSetValueY", Type.GetType("System.Double"));
            dataTable.Columns.Add("EleSetValueZ", Type.GetType("System.Double"));

            dataTable.Columns.Add("PreparationX", Type.GetType("System.Double"));
            dataTable.Columns.Add("PreparationY", Type.GetType("System.Double"));
            dataTable.Columns.Add("PreparationZ", Type.GetType("System.Double"));
            return dataTable;
        }
        /// <summary>
        /// Info转换为DataTable
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(List<ElectrodeInfo11> infos)
        {
            DataTable dt = CreateDataTable();
            foreach (ElectrodeInfo11 ei in infos)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(ElectrodeInfo11).GetProperties())
                {
                    row[propertyInfo.Name] = propertyInfo.GetValue(ei, null);
                }
                row["EleSetValueX"] = ei.EleSetValue[0];
                row["EleSetValueY"] = ei.EleSetValue[1];
                row["EleSetValueZ"] = ei.EleSetValue[2];

                row["PreparationX"] = ei.Preparation[0];
                row["PreparationY"] = ei.Preparation[1];
                row["PreparationZ"] = ei.Preparation[2];

                dt.Rows.Add(row);
            }

            return dt;
        }
        /// <summary>
        /// DataRow转化ElectrodeInfo
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static ElectrodeInfo11 GetEleInfo(DataRow dr)
        {
            ElectrodeInfo11 model = new ElectrodeInfo11();
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {

                PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                if (propertyInfo != null && dr[i] != DBNull.Value)
                    propertyInfo.SetValue(model, dr[i], null);
            }
            model.EleSetValue[0] = Convert.ToDouble(dr["EleSetValueX"]);
            model.EleSetValue[1] = Convert.ToDouble(dr["EleSetValueY"]);
            model.EleSetValue[2] = Convert.ToDouble(dr["EleSetValueZ"]);

            model.Preparation[0] = Convert.ToInt32(dr["PreparationX"]);
            model.Preparation[1] = Convert.ToInt32(dr["PreparationY"]);
            model.Preparation[2] = Convert.ToInt32(dr["PreparationZ"]);

            return model;
        }
        /// <summary>
        /// 写入属性
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public bool SetAttribute(params NXObject[] objs)
        {
            try
            {
                AttributeUtils.AttributeOperation("EleName", this.EleName, objs);
                AttributeUtils.AttributeOperation("BorrowName", this.BorrowName, objs);

                AttributeUtils.AttributeOperation("PitchX", this.PitchX, objs);
                AttributeUtils.AttributeOperation("PitchXNum", this.PitchXNum, objs);
                AttributeUtils.AttributeOperation("PitchY", this.PitchY, objs);
                AttributeUtils.AttributeOperation("PitchYNum", this.PitchYNum, objs);

                AttributeUtils.AttributeOperation("CrudeInter", this.CrudeInter, objs);
                AttributeUtils.AttributeOperation("CrudeNum", this.CrudeNum, objs);
                AttributeUtils.AttributeOperation("DuringInter", this.DuringInter, objs);
                AttributeUtils.AttributeOperation("DuringNum", this.DuringNum, objs);
                AttributeUtils.AttributeOperation("FineInter", this.FineInter, objs);
                AttributeUtils.AttributeOperation("FineNum", this.FineNum, objs);

                AttributeUtils.AttributeOperation("Material1", this.Material, objs);
                AttributeUtils.AttributeOperation("EleType", this.EleType, objs);
                AttributeUtils.AttributeOperation("Condition", this.Condition, objs);
                AttributeUtils.AttributeOperation("Extrudewith", this.Extrudewith, objs);
                AttributeUtils.AttributeOperation("CH", this.Ch, objs);

                AttributeUtils.AttributeOperation("IsPreparation", this.IsPreparation, objs);
                AttributeUtils.AttributeOperation("Remarks", this.Remarks, objs);
                AttributeUtils.AttributeOperation("Technology", this.Technology, objs);
                AttributeUtils.AttributeOperation("CamTemplate", this.CamTemplate, objs);

                AttributeUtils.AttributeOperation("EleSetValue", this.EleSetValue, objs);
                AttributeUtils.AttributeOperation("Preparation", this.Preparation, objs);
                AttributeUtils.AttributeOperation("ElePresentation", this.ElePresentation, objs);
                AttributeUtils.AttributeOperation("Area", this.Area, objs);
                AttributeUtils.AttributeOperation("EleNumber", this.EleNumber, objs);
                AttributeUtils.AttributeOperation("Positioning", this.Positioning, objs);

                AttributeUtils.AttributeOperation("DatumWidth", this.DatumWidth, objs);
                AttributeUtils.AttributeOperation("EleHeadDis", this.EleHeadDis, objs);
                AttributeUtils.AttributeOperation("EleMinDim", this.EleMinDim, objs);
                return true;
            }
            catch (NXException ex)
            {
                ClassItem.WriteLogFile("写入属性错误！" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {           
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            ms.Seek(0, SeekOrigin.Begin);
            return bf.Deserialize(ms);
        }
    }
}


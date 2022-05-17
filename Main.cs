using Newtonsoft.Json;
using Power.Configure;
using Power.Controls.Action;
using Power.Controls.PMS;
using Power.Global;
using Power.IBaseCore;
using Power.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using XCode;
using XCode.DataAccessLayer;
using Power.Systems.StdPlan.StdPlan;
using static Power.Service.PlanService.Plan.PlanService;
using System.Data.SqlClient;

namespace Power.JJ.APP
{
    public class SqlDataBase
    {
        private const int MaxPool = 512;//最大连接数
        private const int MinPool = 5;//最小连接数
        private const bool Asyn_Process = true;//设置异步访问数据库
                                               //在单个连接上得到和管理多个、仅向前引用和只读的结果集(ADO.NET2.0)
        private const bool Mars = true;
        private const int Conn_Timeout = 5000;//设置连接等待时间
        private const int Conn_Lifetime = 15000;//设置连接的生命周期
        private SqlConnection con = null;//连接对象
        private SqlTransaction dbTran = null;
        public Dictionary<string, object> paramList = new Dictionary<string, object>();

        public SqlDataBase()
        {
            //server=47.101.200.39;database=JJPowerPMDBTest;uid=sa;pwd=Power3506
            string sConnectionString = @"server=47.101.200.39;database=JJPowerPMDBTest;uid=sa;pwd=Power3506;"
            //string sConnectionString = @"server=192.168.1.16;database=EPC_PowerPMDB;uid=sa;pwd=`1qaz2wsx3edc;"
                                     + "Max Pool Size=" + MaxPool + ";"
                                     + "Min Pool Size=" + MinPool + ";"
                                     + "Connect Timeout=" + Conn_Timeout + ";"
                                     + "Connection Lifetime=" + Conn_Lifetime + ";"
                                     + "Asynchronous Processing=" + Asyn_Process + ";";
            con = new SqlConnection(sConnectionString);
        }

        public SqlDataBase(string sLJCS)
        {
            string sConnectionString = sLJCS + ";";
            sConnectionString += @"Max Pool Size=" + MaxPool + ";"
                               + "Min Pool Size=" + MinPool + ";"
                               + "Connect Timeout=" + Conn_Timeout + ";"
                               + "Connection Lifetime=" + Conn_Lifetime + ";"
                               + "Asynchronous Processing=" + Asyn_Process + ";";
            con = new SqlConnection(sConnectionString);
        }

        public void beginTran()
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            dbTran = con.BeginTransaction();
        }

        public void commitTran()
        {
            try
            {
                dbTran.Commit();
            }
            catch
            {
                dbTran.Rollback();
            }
            finally
            {
                con.Close();
            }
        }

        public void rollback()
        {
            dbTran.Rollback();
            con.Close();
        }

        public DataSet getDataSet(string sSQL)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            SqlCommand cmd = new SqlCommand(sSQL, con);
            cmd.Parameters.Clear();
            foreach (KeyValuePair<string, object> dic in paramList)
                cmd.Parameters.AddWithValue(dic.Key, dic.Value);
            if (dbTran != null)
                cmd.Transaction = dbTran;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            clearParam();
            if (dbTran == null)
                if (dbTran == null)
                    con.Close();
            return ds;
        }

        public void doSQL(string sSQL)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            SqlCommand cmd = new SqlCommand(sSQL, con);
            cmd.Parameters.Clear();
            foreach (KeyValuePair<string, object> dic in paramList)
                cmd.Parameters.AddWithValue(dic.Key, dic.Value);
            if (dbTran != null)
                cmd.Transaction = dbTran;
            cmd.ExecuteNonQuery();
            clearParam();
            if (dbTran == null)
                con.Close();
        }

        public void addParam(string param, object value)
        {
            paramList.Add(param, value);
        }

        public void clearParam()
        {
            paramList.Clear();
        }
    }
    public class Main : BaseControl
    {

        [Action(Authorize = false)]
        public string middletable()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return "ok";
        }








        /// <summary>
        /// 跳转中间页面前 生成认证文件
        /// </summary>
        /// <returns></returns>
        [Action(Authorize = false)]
        public string SetPass(string UserCode)
        {
            //设置密文
            try
            {
                if (!Directory.Exists(@"C:/PowerPMSAuth"))
                {
                    Directory.CreateDirectory(@"C:/PowerPMSAuth");
                }
                if (!File.Exists("C:\\PowerPMSAuth\\" + UserCode + ".txt"))
                {
                    FileStream fs1 = new FileStream("C:\\PowerPMSAuth\\" + UserCode + ".txt", FileMode.Create, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.WriteLine(UserCode);//开始写入值
                    sw.Close();
                    fs1.Close();
                }
            }
            catch
            {
                //吞掉异常
            }

            return "ok";
        }

        /// <summary>
        /// 模拟登陆 同时校验认证文件
        /// </summary>
        /// <returns></returns>
        [Action(Authorize = false)]
        public string Login(string UserCode, string Language)
        {
            //try
            //{
            //    if (!File.Exists("C:\\PowerPMSAuth\\" + UserCode + ".txt"))
            //    {
            //        ViewResultModel objResult = new ViewResultModel();
            //        objResult.success = false;
            //        objResult.message = "认证不通过";
            //        return objResult.ToJson();
            //    }
            //    else
            //    {
            //        File.SetAttributes("C:\\PowerPMSAuth\\" + UserCode + ".txt", FileAttributes.Normal);
            //        File.Delete("C:\\PowerPMSAuth\\" + UserCode + ".txt");
            //    }
            //}
            //catch
            //{
            //    //吞掉异常
            //}

            APIControl aPIControl = new APIControl();
            ViewResultModel viewResultModel = aPIControl.Login(UserCode, Language);
            return viewResultModel.ToJson();
        }



        /// <summary>
        /// 模拟超级用户登陆登陆
        /// </summary>
        /// <returns></returns>
        [Action(Authorize = false)]
        public string LoginNoParam(string UserCode, string Language)
        {
            //string sResult = LoginNo("admin");
            //return sResult;

            AccountControl entity = new AccountControl();
            return entity.Login("PowerEncode", "%C2%B2%10%04%0E%11", "%C2%A9%1CD%241", "");
        }


        [Action(Authorize = false)]
        public string LoginNo(string UserId)
        {
            string text = string.Concat(new string[]
            {
                "select  Code,PassWord from pb_user where Code='",
                UserId,
                "'  or Replace(Code,'  ','')='",
                UserId,
                "'"
            });
            DataTable dataTable = DAL.QuerySQL(text);
            string result;
            if (dataTable.Rows.Count > 0)
            {
                PowerConfig.init("shPower");
                MessageCache.init();
                DAL.SetDefaultConn("PowerPMDB");
                PublicUtil.SessionID = new LoginAction().Login(dataTable.Rows[0]["Code"].ToString(), dataTable.Rows[0]["PassWord"].ToString(), "zh-CN").data["sessionid"].ToString();
                ISession session = Business.Factory.ObjectFactory<ISessionUtil>.Create(PowerConfig.DefaultPublicUtil).getSession(false);
                session.BizAreaId = "00000000-0000-0000-0000-00000000000A";
                session.EpsProjId = "00000000-0000-0000-0000-0000000000a4";
                PowerGlobal.SaveSession(session);
                result = dataTable.Rows[0]["PassWord"].ToString();
            }
            else if (dataTable.Rows.Count == 0)
            {
                result = "未找到此账号";
            }
            else
            {
                result = "error";
            }
            return result;
        }

        [Action(Authorize = false)]
        public string GetPlanStatusControlData(string plan_guid, string pageIndex, string pageSize, string sortField, string sortOrder)
        {
            ViewResultModel val = ViewResultModel.Create(false, "");
            int num = Convert.ToInt32(pageIndex);
            int num2 = Convert.ToInt32(pageSize);
            string arg = (DAL.ConnStrs[DAL.DefaultConnName].DataBaseType != DatabaseType.Oracle) ? $"SELECT c.project_shortname,c.project_name,b.plan_short_name,b.plan_name,a.task_id,a.task_guid,a.task_code,a.task_name,a.task_type,a.plan_guid,a.driving_path_flag,\r\na.target_start_date,a.target_end_date,a.act_start_date,a.act_end_date,a.rsrc_guid,a.rsrc_name,a.status_code ,row_number()over(order by a.target_start_date desc) as rowNumber  FROM pln_task a INNER JOIN pln_project_plan b \r\nON a.plan_guid=b.proj_plan_guid INNER JOIN pln_project c ON a.proj_guid=c.project_guid WHERE a.plan_guid='{plan_guid}'  and b.isHistoryVersion=0" : $"SELECT c.project_shortname as \"project_shortname\",c.project_name as \"project_name\" ,b.plan_short_name as \"plan_short_name\",b.plan_name as \"plan_name\",a.task_id as \"task_id\",\r\na.task_guid as \"task_guid\",a.task_code as \"task_code\",a.task_name as \"task_name\",a.task_type as \"task_type\",a.plan_guid as \"plan_guid\",a.driving_path_flag as \"driving_path_flag\",\r\na.target_start_date as \"target_start_date\",a.target_end_date as \"target_end_date\",a.act_start_date as \"act_start_date\",a.act_end_date as \"act_end_date\",a.rsrc_guid as \"rsrc_guid\",a.rsrc_name as \"rsrc_name\",a.status_code as \"status_code\" ,row_number() over (order by a.target_start_date desc) as rowNumber  FROM pln_task a INNER JOIN pln_project_plan b \r\nON a.plan_guid=b.proj_plan_guid INNER JOIN pln_project c ON a.proj_guid=c.project_guid WHERE a.plan_guid='{plan_guid}'  and b.isHistoryVersion=0";
            string sql = $"Select count(1) as num from ({arg}) XCode_T1";
            string text = "";
            if (num > -1)
            {
                text = $"Select XCode_T1.* from ({arg}) XCode_T1 where rowNumber Between {num * num2 + 1} and {(num + 1) * num2}";
                if (sortField != "" && sortOrder != "")
                {
                    text = text + " order by XCode_T1." + sortField + " " + sortOrder;
                }
            }
            else
            {
                text = $"Select XCode_T1.* from ({arg}) XCode_T1 ";
            }

            DAL dAL = DAL.Create();
            try
            {
                DataTable value = dAL.Select(sql).Tables[0];
                DataTable dataTable = dAL.Select(text).Tables[0];
                List<Hashtable> list = new List<Hashtable>();
                if (dataTable.Rows.Count > 0)
                {
                    dataTable.Columns.Add("taskstatusClass", Type.GetType("System.String"));
                    dataTable.Columns.Add("taskstatusTitle", Type.GetType("System.String"));
                    TaskDateEntity taskDateEntity = new TaskDateEntity();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        row["target_start_date"] = ((row["target_start_date"].ToString() == "") ? ((object)taskDateEntity.date) : row["target_start_date"]);
                        row["target_end_date"] = ((row["target_end_date"].ToString() == "") ? ((object)taskDateEntity.date) : row["target_end_date"]);
                        row["act_start_date"] = ((row["act_start_date"].ToString() == "") ? ((object)taskDateEntity.date) : row["act_start_date"]);
                        row["act_end_date"] = ((row["act_end_date"].ToString() == "") ? ((object)taskDateEntity.date) : row["act_end_date"]);
                        DateTime act_start_date = Convert.ToDateTime(DateTime.Parse(row["act_start_date"].ToString()));
                        DateTime act_end_date = Convert.ToDateTime(DateTime.Parse(row["act_end_date"].ToString()));
                        DateTime planStart = Convert.ToDateTime(DateTime.Parse(row["target_start_date"].ToString()));
                        DateTime planEnd = Convert.ToDateTime(DateTime.Parse(row["target_end_date"].ToString()));
                        string task_type = row["task_type"].ToString();
                        row["taskstatusClass"] = getTaskStatusIcon(act_start_date, act_end_date, planStart, planEnd, task_type);
                        row["taskstatusTitle"] = getTaskIconTitle(row["taskstatusClass"].ToString());
                        if (list.Count == 0)
                        {
                            Hashtable hashtable = new Hashtable();
                            hashtable.Add("name", row["taskstatusTitle"]);
                            hashtable.Add("num", 1);
                            list.Add(hashtable);
                            continue;
                        }

                        List<Hashtable> list2 = new List<Hashtable>(list);
                        int num3 = 1;
                        bool flag = false;
                        foreach (Hashtable item in list2)
                        {
                            if (item["name"].ToString() == row["taskstatusTitle"].ToString())
                            {
                                int num4 = int.Parse(item["num"].ToString());
                                num4++;
                                list[num3 - 1]["num"] = num4;
                                flag = true;
                            }
                            else if (!flag && num3 == list2.Count)
                            {
                                Hashtable hashtable2 = new Hashtable();
                                hashtable2.Add("name", row["taskstatusTitle"]);
                                hashtable2.Add("num", 1);
                                list.Add(hashtable2);
                            }

                            num3++;
                        }
                    }
                }

                Hashtable hashtable3 = new Hashtable();
                hashtable3.Add("PlanSomeStatusControlData", GetPlanSomeStatusControl(plan_guid));
                hashtable3.Add("StartTaskAndEndTaskPct", GetStartTaskAndEndTaskPct(plan_guid));
                hashtable3.Add("totalCount", value);
                hashtable3.Add("data", dataTable);
                hashtable3.Add("taskStatusList", list);
                val.data=hashtable3;
                val.success=true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dAL.Session.AutoClose();
            }

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(val, s_setting);

        }

        [Action(Authorize = false)]
        public string GridPageLoadProjectPlan(string proj_guid)
        {
            ArrayList arrayList = new ArrayList();

            string Sql_One = @"
            Select XCode_T1.* From (Select A.proj_plan_guid,A.plan_name,A.data_date, row_number() over(Order By  A.proj_plan_id) as rowNumber From  PLN_project_plan A Where   (0=0)  and (A.proj_guid='" + proj_guid + @"' and A.parent_plan_id=0) and  A.proj_guid='" + proj_guid + @"' ) XCode_T1 Where rowNumber Between 1 And 1 ";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayList.Add(Dt_One);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayList, s_setting);
        }

        [Action(Authorize = false)]
        public string GridPageLoadEx()
        {
            ArrayList arrayList = new ArrayList();

            string Sql_One = @"
            select Id,Code,Title,StartDay,StartType,EndDay,EndType,IsNatureMonth,Actived,Memo,seq_num,UpdHumId,UpdHumName,UpdDate,RegDate,RegHumName,RegHumId,OwnProjName,OwnProjId,EpsProjId,ApprHumId,ApprHumName,ApprDate,Status,WeekStartDay from PS_PLN_AccountingPeriod where Actived ='1' order by seq_num desc ";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayList.Add(Dt_One);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayList, s_setting);
        }

        [Action(Authorize = false)]
        public string GetWBSEVM(string plan_guid, string task_guid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("plan_guid='{0}' and task_guid='{1}'", plan_guid, task_guid);
            EntityList<TaskBCWSDO> entityList = Entity<TaskBCWSDO>.FindAll(stringBuilder.ToString(), "periodstrat", "period_guid,periodstrat,periodend,plan_complete_pct,act_complete_pct");
            List<Hashtable> list = new List<Hashtable>();
            double num = 0.0;
            decimal num2 = default(decimal);
            for (int i = 0; i < entityList.Count; i++)
            {
                TaskBCWSDO val = entityList[i];
                Hashtable hashtable = new Hashtable();
                hashtable.Add("startdate", ((TaskBCWSDO<TaskBCWSDO>)(object)val).periodstrat);//get_periodstrat
                hashtable.Add("enddate", ((TaskBCWSDO<TaskBCWSDO>)(object)val).periodend);
                hashtable.Add("period_guid", ((TaskBCWSDO<TaskBCWSDO>)(object)val).period_guid);
                decimal num3 = ((TaskBCWSDO<TaskBCWSDO>)(object)val).act_complete_pct;
                double num4 = ((TaskBCWSDO<TaskBCWSDO>)(object)val).plan_complete_pct;
                decimal num5 = ((TaskBCWSDO<TaskBCWSDO>)(object)val).act_complete_pct;
                double num6 = ((TaskBCWSDO<TaskBCWSDO>)(object)val).plan_complete_pct;
                if (i != 0)
                {
                    if (num3 != 0m)
                    {
                        num5 = num3 - num2;
                    }
                    else
                    {
                        num3 = num2;
                    }

                    if (num4 == 0.0)
                    {
                        num4 = num;
                    }

                    num6 = num4 - num;
                }

                hashtable.Add("actsumpct", num3);
                hashtable.Add("plnsumpct", num4);
                hashtable.Add("actcurpct", num5);
                hashtable.Add("plncurpct", num6);
                num = num4;
                num2 = num3;
                list.Add(hashtable);
            }
            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(list, s_setting);

        }

        [Action(Authorize = false)]
        public string GetV_PlanOverallProgress(string plan_name)
        {
            ArrayList arrayList = new ArrayList();

            string Sql_One = @"Select proj_guid,proj_plan_guid,proj_plan_id,parent_plan_guid,plan_short_name,plan_name,data_date,plan_start_date,plan_end_date,plan_complete_pct,act_complete_pct,lastfeedbackdate,bcws_cost,bsws,bswp,sv,receive_user From V_PS_PlN_PlanOverallProgress PPPOP Where  (1 = 1 and LongCode is not null and LongCode like '%1.5%' and PPPOP.plan_name = '"+ plan_name + "' and PPPOP.proj_guid = '6fddf002-fc9d-494c-b615-1827796b27be') Order By  plan_short_name ";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayList.Add(Dt_One);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayList, s_setting);
        }

        [Action(Authorize = false)]
        public string GetV_TaskAndWBS(string TaskWbsNameOld)
        {
            ArrayList arrayList = new ArrayList();
            string Sql_Four = @"
            Select * From (select
	        distinct
	        case when CHARINDEX('标段,', (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, ''))) > '0' then SUBSTRING((isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, '')), 1, CHARINDEX('标段,', (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, ''))) + 1) else (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, '')) end TaskWbsNameNew,
	        (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, '')) TaskWbsNameOld,
	        a.*
        from 
        (
	        select
		        case when IsPass = '延期未完成' then (datediff(DAY, format(aa.reend_date, 'yyyy-MM-dd'), format(GETDATE(), 'yyyy-MM-dd')) + 1) else (datediff(DAY, format(GETDATE(), 'yyyy-MM-dd'), format(aa.reend_date, 'yyyy-MM-dd')) + 1) end HowManyDays,
		        aa.*
	        from
	        (
		        select 
			        case when format(GETDATE(), 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd') and (a.act_start_date is null or format(a.act_start_date, 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd')) then '延期开始'
			        when format(GETDATE(), 'yyyy-MM-dd') > format(a.reend_date, 'yyyy-MM-dd') and (a.act_end_date is null or a.act_end_date = '') then '延期未完成' else '合格' end IsPass,
			        a.restart_date,
			        a.reend_date,
			        a.act_start_date,
			        a.act_end_date,
			        (isnull(a.task_name, '') + ',' + isnull(b.wbs_name, '')) TaskWbsName,
			        b.parent_wbs_guid,
			        a.proj_guid
		        from PLN_task a 

		        left join PLN_PROJWBS b on a.wbs_guid = b.wbs_guid
		        where a.restart_date is not null and a.reend_date is not null and format(a.reend_date, 'yyyy-MM-dd') >= '2000-01-01'
	        )aa where aa.IsPass = '延期开始' or aa.IsPass = '延期未完成'
        )a

        left join PLN_PROJWBS c on a.parent_wbs_guid = c.wbs_guid

        left join PLN_PROJWBS d on c.parent_wbs_guid = d.wbs_guid

        left join PLN_PROJWBS e on d.parent_wbs_guid = e.wbs_guid

        left join PLN_PROJWBS f on e.parent_wbs_guid = f.wbs_guid

        left join PLN_PROJWBS g on f.parent_wbs_guid = g.wbs_guid 

        left join PLN_PROJWBS h on g.parent_wbs_guid = h.wbs_guid

        left join PLN_PROJWBS m on h.parent_wbs_guid = m.wbs_guid

        left join PLN_PROJWBS n on m.parent_wbs_guid = n.wbs_guid)dd Where  
        (proj_guid = '1cc767c9-ec0f-403c-ad37-616487cc5139' and parent_wbs_guid != '00000000-0000-0000-0000-000000000000' and TaskWbsNameOld like '%"+ TaskWbsNameOld + "%') Order By  HowManyDays desc";
            DataTable Dt_Four = XCode.DataAccessLayer.DAL.QuerySQL(Sql_Four);
            arrayList.Add(Dt_Four);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayList, s_setting);
        }

        [Action(Authorize = false)]
        public string GetPS_MonitorNotifySetting(string EpsProjId)
        {
            ArrayList arrayList = new ArrayList();
            string Sql_One = @"
            Select A.* From  PS_PLN_MonitorNotifySetting A Where   (0=0)  and (A.ConfigLevel='Project' and A.EpsProjId='"+ EpsProjId + "') and 1=1 ";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayList.Add(Dt_One);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayList, s_setting);
        }


        [Action(Authorize = false)]
        public string GetPS_MonitorNotifySettingbyGolbal(string EpsProjId)
        {
            ArrayList arrayList = new ArrayList();
            string Sql_One = @"
            Select A.* From  PS_PLN_MonitorNotifySetting A Where   (0=0)  and (A.ConfigLevel='Golbal') and 1=1 ";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayList.Add(Dt_One);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayList, s_setting);
        }

        [Action(Authorize = false)]
        public string GetA1NPS_BOQQD(string EpsProjId, string Plan_guid)
        {
            ArrayList arrayList = new ArrayList();

            string Sql_One = @"select top 1 YSHQD,SHZQD from NPS_BOQ where FID  in (select proc_guid from PLN_taskproc 
                           where plan_guid='" + Plan_guid + @"' and proj_guid = '" + EpsProjId + @"')";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayList.Add(Dt_One);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayList, s_setting);
        }

        [Action(Authorize = false)]
        public string GetA2NPS_BOQQD(string EpsProjId, string Plan_guid)
        {
            ArrayList arrayList = new ArrayList();

            string Sql_One = @"select top 1 YSHQD,SHZQD from NPS_BOQ where FID  in (select proc_guid from PLN_taskproc 
                           where plan_guid='" + Plan_guid + @"' and proj_guid = '" + EpsProjId + @"')";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayList.Add(Dt_One);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayList, s_setting);
        }

        [Action(Authorize = false)]
        public string ViewScreenLeft(string EpsProjId, string EpsProjLongCode)
        {
            ArrayList arrayList = new ArrayList();

            string Sql_One = @"
        select proj_guid,proj_plan_guid,proj_plan_id,parent_plan_guid,plan_short_name,plan_name,data_date,plan_start_date,plan_end_date,plan_complete_pct,act_complete_pct,lastfeedbackdate,bcws_cost,bsws,bswp,sv,receive_user from V_PS_PlN_PlanOverallProgress PPPOP where LongCode is not null and LongCode like '%" + EpsProjLongCode + @"%' and plan_name = 'A1标段' and proj_guid = '" + EpsProjId + @"'
    ";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayList.Add(Dt_One);


            string Sql_Two = @"
        select proj_guid,proj_plan_guid,proj_plan_id,parent_plan_guid,plan_short_name,plan_name,data_date,plan_start_date,plan_end_date,plan_complete_pct,act_complete_pct,lastfeedbackdate,bcws_cost,bsws,bswp,sv,receive_user from V_PS_PlN_PlanOverallProgress PPPOP where LongCode is not null and LongCode like '%" + EpsProjLongCode + @"%' and plan_name = 'A2标段' and proj_guid = '" + EpsProjId + @"'
    ";

            DataTable Dt_Two = XCode.DataAccessLayer.DAL.QuerySQL(Sql_Two);
            arrayList.Add(Dt_Two);


            string Sql_Three = @"
        select
            dd.*
        from
        (
            select
                case when MMM.IsPass = '未启动' then '01'
                when MMM.IsPass = '提前开始' then '02'
                when MMM.IsPass = '延期开始' then '03'
                when MMM.IsPass = '已开始' then '04'
                when MMM.IsPass = '已完成' then '05'
                when MMM.IsPass = '延期完成' then '06'
                when MMM.IsPass = '延期未完成' then '07' end SortNum,
                MMM.*
            from
            (
                select
                    MM.TaskWbsName,
                    MM.IsPass,
                    MM.proj_guid,
                    Count(*) WhichCount
                from
                (
                    select
                        M.TaskWbsNameNew,
                        reverse(substring(reverse(M.TaskWbsNameNew), 1, (case when charindex(',', reverse(M.TaskWbsNameNew)) > '1' then charindex(',', reverse(M.TaskWbsNameNew)) - 1 else charindex(',', reverse(M.TaskWbsNameNew)) end))) TaskWbsName,
                        M.IsPass,
                        M.proj_guid
                    from
                    (
                        select
                            case when CHARINDEX('标段,', (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, ''))) > '0' then SUBSTRING((isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, '')), 1, CHARINDEX('标段,', (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, ''))) + 1) else (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, '')) end TaskWbsNameNew,
                            a.IsPass,
                            a.proj_guid
                        from
                        (
                            select
                                case when format(GETDATE(), 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd') then '未启动'
                                when ((format(GETDATE(), 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd') and a.act_start_date is not null) or (format(GETDATE(), 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd') and (a.act_start_date is null or format(a.act_start_date, 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd'))) or (format(GETDATE(), 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd') and a.act_start_date is not null and format(a.act_start_date, 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd'))) and a.act_end_date is not null and format(a.act_end_date, 'yyyy-MM-dd') <= format(a.reend_date, 'yyyy-MM-dd') then '已完成'
                                when format(GETDATE(), 'yyyy-MM-dd') > format(a.reend_date, 'yyyy-MM-dd') and (a.act_end_date is null or a.act_end_date = '') then '延期未完成'
                                when format(GETDATE(), 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd') and (a.act_start_date is null or format(a.act_start_date, 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd')) then '延期开始'
                                when format(GETDATE(), 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd') and a.act_start_date is not null then '提前开始'
                                when format(GETDATE(), 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd') and a.act_start_date is not null and format(a.act_start_date, 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd') then '已开始'
                                when format(GETDATE(), 'yyyy-MM-dd') > format(a.reend_date, 'yyyy-MM-dd') and a.act_end_date is not null then '延期完成' end IsPass,
                                (isnull(a.task_name, '') + ',' + isnull(b.wbs_name, '')) TaskWbsName,
                                b.parent_wbs_guid,
                                a.proj_guid
                            from PLN_task a

                            left join PLN_PROJWBS b on a.wbs_guid = b.wbs_guid
                            where a.restart_date is not null and a.reend_date is not null and format(a.reend_date, 'yyyy-MM-dd') >= '2000-01-01'
                        )a

                        left join PLN_PROJWBS c on a.parent_wbs_guid = c.wbs_guid

                        left join PLN_PROJWBS d on c.parent_wbs_guid = d.wbs_guid

                        left join PLN_PROJWBS e on d.parent_wbs_guid = e.wbs_guid

                        left join PLN_PROJWBS f on e.parent_wbs_guid = f.wbs_guid

                        left join PLN_PROJWBS g on f.parent_wbs_guid = g.wbs_guid

                        left join PLN_PROJWBS h on g.parent_wbs_guid = h.wbs_guid

                        left join PLN_PROJWBS m on h.parent_wbs_guid = m.wbs_guid

                        left join PLN_PROJWBS n on m.parent_wbs_guid = n.wbs_guid
                    )M
                )MM group by MM.TaskWbsName,MM.IsPass,MM.proj_guid
            )MMM
        )dd where dd.proj_guid = '" + EpsProjId + @"' and dd.TaskWbsName = 'A1标段'";

            DataTable Dt_Three = XCode.DataAccessLayer.DAL.QuerySQL(Sql_Three);
            arrayList.Add(Dt_Three);


            string Sql_Four = @"
        select
            dd.*
        from
        (
            select
                case when MMM.IsPass = '未启动' then '01'
                when MMM.IsPass = '提前开始' then '02'
                when MMM.IsPass = '延期开始' then '03'
                when MMM.IsPass = '已开始' then '04'
                when MMM.IsPass = '已完成' then '05'
                when MMM.IsPass = '延期完成' then '06'
                when MMM.IsPass = '延期未完成' then '07' end SortNum,
                MMM.*
            from
            (
                select
                    MM.TaskWbsName,
                    MM.IsPass,
                    MM.proj_guid,
                    Count(*) WhichCount
                from
                (
                    select
                        M.TaskWbsNameNew,
                        reverse(substring(reverse(M.TaskWbsNameNew), 1, (case when charindex(',', reverse(M.TaskWbsNameNew)) > '1' then charindex(',', reverse(M.TaskWbsNameNew)) - 1 else charindex(',', reverse(M.TaskWbsNameNew)) end))) TaskWbsName,
                        M.IsPass,
                        M.proj_guid
                    from
                    (
                        select
                            case when CHARINDEX('标段,', (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, ''))) > '0' then SUBSTRING((isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, '')), 1, CHARINDEX('标段,', (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, ''))) + 1) else (isnull(a.TaskWbsName, '') + ',' + isnull(c.wbs_name, '') + ',' + isnull(d.wbs_name, '') + ',' + isnull(e.wbs_name, '') + ',' + isnull(f.wbs_name, '') + ',' + isnull(g.wbs_name, '') + ',' + isnull(h.wbs_name, '') + ',' + isnull(m.wbs_name, '') + ',' + isnull(n.wbs_name, '')) end TaskWbsNameNew,
                            a.IsPass,
                            a.proj_guid
                        from
                        (
                            select
                                case when format(GETDATE(), 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd') then '未启动'
                                when ((format(GETDATE(), 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd') and a.act_start_date is not null) or (format(GETDATE(), 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd') and (a.act_start_date is null or format(a.act_start_date, 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd'))) or (format(GETDATE(), 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd') and a.act_start_date is not null and format(a.act_start_date, 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd'))) and a.act_end_date is not null and format(a.act_end_date, 'yyyy-MM-dd') <= format(a.reend_date, 'yyyy-MM-dd') then '已完成'
                                when format(GETDATE(), 'yyyy-MM-dd') > format(a.reend_date, 'yyyy-MM-dd') and (a.act_end_date is null or a.act_end_date = '') then '延期未完成'
                                when format(GETDATE(), 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd') and (a.act_start_date is null or format(a.act_start_date, 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd')) then '延期开始'
                                when format(GETDATE(), 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd') and a.act_start_date is not null then '提前开始'
                                when format(GETDATE(), 'yyyy-MM-dd') > format(a.restart_date, 'yyyy-MM-dd') and a.act_start_date is not null and format(a.act_start_date, 'yyyy-MM-dd') <= format(a.restart_date, 'yyyy-MM-dd') then '已开始'
                                when format(GETDATE(), 'yyyy-MM-dd') > format(a.reend_date, 'yyyy-MM-dd') and a.act_end_date is not null then '延期完成' end IsPass,
                                (isnull(a.task_name, '') + ',' + isnull(b.wbs_name, '')) TaskWbsName,
                                b.parent_wbs_guid,
                                a.proj_guid
                            from PLN_task a

                            left join PLN_PROJWBS b on a.wbs_guid = b.wbs_guid
                            where a.restart_date is not null and a.reend_date is not null and format(a.reend_date, 'yyyy-MM-dd') >= '2000-01-01'
                        )a

                        left join PLN_PROJWBS c on a.parent_wbs_guid = c.wbs_guid

                        left join PLN_PROJWBS d on c.parent_wbs_guid = d.wbs_guid

                        left join PLN_PROJWBS e on d.parent_wbs_guid = e.wbs_guid

                        left join PLN_PROJWBS f on e.parent_wbs_guid = f.wbs_guid

                        left join PLN_PROJWBS g on f.parent_wbs_guid = g.wbs_guid

                        left join PLN_PROJWBS h on g.parent_wbs_guid = h.wbs_guid

                        left join PLN_PROJWBS m on h.parent_wbs_guid = m.wbs_guid

                        left join PLN_PROJWBS n on m.parent_wbs_guid = n.wbs_guid
                    )M
                )MM group by MM.TaskWbsName,MM.IsPass,MM.proj_guid
            )MMM
        )dd where dd.proj_guid = '" + EpsProjId + @"' and dd.TaskWbsName = 'A2标段'
    ";

            DataTable Dt_Four = XCode.DataAccessLayer.DAL.QuerySQL(Sql_Four);
            arrayList.Add(Dt_Four);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayList, s_setting);
        }

        [Action(Authorize = false)]
        public string ViewScreenRight(string EpsProjId, string EpsProjLongCode)
        {
            ArrayList arrayListTwo = new ArrayList();

            string Sql_One = @"
        select
            dd.*
        from
        (select
            BB.BYJHGCJE,
            BB.BYSJJE,
            BB.KGLJJE,
            BB.plan_name,
            format(BB.period_enddate, 'yyyy-MM') period_enddate
        from
        (
            select
                B.plan_name,
                Max(B.period_enddate) period_enddate
            from
            (
                select
                    b.plan_name,
                    b.period_enddate
                from NPS_BOQZQ a

                left join PS_PLN_FeedBackRecord b on a.MasterId = b.Id
            )B group by B.plan_name,format(B.period_enddate, 'yyyy-MM')
        )AA

        left join

        (
            select
                Sum(A.BYJHGCJE) BYJHGCJE,
                Sum(A.BYSJJE) BYSJJE,
                Sum(A.KGLJJE) KGLJJE,
                A.plan_name,
                A.period_enddate
            from
            (
                select
                    a.BYJHGCJE,
                    a.BYSJJE,
                    a.KGLJJE,
                    b.plan_name,
                    b.period_enddate
                from NPS_BOQZQ a

                left join PS_PLN_FeedBackRecord b on a.MasterId = b.Id
            )A group by A.plan_name,A.period_enddate
        )BB on AA.plan_name = BB.plan_name and AA.period_enddate = BB.period_enddate)dd order by dd.period_enddate desc
    ";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One); 
            arrayListTwo.Add(Dt_One);


            string Sql_Two = @"
        select
            dd.*
        from
        (
            select
                BB.BYJHGCJE,
                BB.BYSJJE,
                BB.KGLJJE,
                BB.plan_name,
                format(BB.period_enddate, 'yyyy-MM') period_enddate
            from
            (
                select
                    B.plan_name,
                    Max(B.period_enddate) period_enddate
                from
                (
                    select
                        b.plan_name,
                        b.period_enddate
                    from NPS_BOQZQ a

                    left join PS_PLN_FeedBackRecord b on a.MasterId = b.Id
                )B group by B.plan_name,format(B.period_enddate, 'yyyy-MM')
            )AA

            left join

            (
                select
                    Sum(A.BYJHGCJE) BYJHGCJE,
                    Sum(A.BYSJJE) BYSJJE,
                    Sum(A.KGLJJE) KGLJJE,
                    A.plan_name,
                    A.period_enddate
                from
                (
                    select
                        a.BYJHGCJE,
                        a.BYSJJE,
                        a.KGLJJE,
                        b.plan_name,
                        b.period_enddate
                    from NPS_BOQZQ a

                    left join PS_PLN_FeedBackRecord b on a.MasterId = b.Id
                )A group by A.plan_name,A.period_enddate
            )BB on AA.plan_name = BB.plan_name and AA.period_enddate = BB.period_enddate
        )dd where dd.plan_name = 'A1标段' order by dd.period_enddate desc
    ";

            DataTable Dt_Two = XCode.DataAccessLayer.DAL.QuerySQL(Sql_Two); 
            arrayListTwo.Add(Dt_Two);


            string Sql_Three = @"
        select
            dd.*
        from
        (
            select
                BB.BYJHGCJE,
                BB.BYSJJE,
                BB.KGLJJE,
                BB.plan_name,
                format(BB.period_enddate, 'yyyy-MM') period_enddate
            from
            (
                select
                    B.plan_name,
                    Max(B.period_enddate) period_enddate
                from
                (
                    select
                        b.plan_name,
                        b.period_enddate
                    from NPS_BOQZQ a

                    left join PS_PLN_FeedBackRecord b on a.MasterId = b.Id
                )B group by B.plan_name,format(B.period_enddate, 'yyyy-MM')
            )AA

            left join

            (
                select
                    Sum(A.BYJHGCJE) BYJHGCJE,
                    Sum(A.BYSJJE) BYSJJE,
                    Sum(A.KGLJJE) KGLJJE,
                    A.plan_name,
                    A.period_enddate
                from
                (
                    select
                        a.BYJHGCJE,
                        a.BYSJJE,
                        a.KGLJJE,
                        b.plan_name,
                        b.period_enddate
                    from NPS_BOQZQ a

                    left join PS_PLN_FeedBackRecord b on a.MasterId = b.Id
                )A group by A.plan_name,A.period_enddate
            )BB on AA.plan_name = BB.plan_name and AA.period_enddate = BB.period_enddate
        )dd where dd.plan_name = 'A2标段' order by dd.period_enddate desc
    ";

            DataTable Dt_Three = XCode.DataAccessLayer.DAL.QuerySQL(Sql_Three);
            arrayListTwo.Add(Dt_Three);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayListTwo, s_setting);
        }

        [Action(Authorize = false)]
        public string GetProjectSchedule(string EpsProjId = "", string EpsProjLongCode = "")
        {
            ViewResultModel viewResultModel = ViewResultModel.Create(success: false, "");

            DAL dAL = DAL.Create();
            try
            {
                string format = $"Select proj_guid,proj_plan_guid,proj_plan_id,parent_plan_guid,plan_short_name,\r\nplan_name,data_date,plan_start_date,plan_end_date,plan_complete_pct,act_complete_pct,\r\nlastfeedbackdate,bcws_cost,bsws,bswp,sv,receive_user From V_PS_PlN_PlanOverallProgress PPPOP \r\nWhere   1=1  and  1=1  and (PPPOP.proj_guid='{EpsProjId}' or LongCode \r\nlike '{EpsProjLongCode}%')";
                if (DAL.ConnStrs[DAL.DefaultConnName].DataBaseType == DatabaseType.Oracle)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Select  PROJ_GUID as \"proj_guid\", PROJ_PLAN_GUID as \"proj_plan_guid\", PROJ_PLAN_ID as \"proj_plan_id\",");
                    stringBuilder.AppendLine("PARENT_PLAN_GUID as \"parent_plan_guid\", PLAN_SHORT_NAME as \"plan_short_name\", PLAN_NAME as \"plan_name\", DATA_DATE as \"data_date\",");
                    stringBuilder.AppendLine("PLAN_START_DATE as \"plan_start_date\",PLAN_END_DATE as \"plan_end_date\", PLAN_COMPLETE_PCT as \"plan_complete_pct\", ACT_COMPLETE_PCT as \"act_complete_pct\",");
                    stringBuilder.AppendLine("LASTFEEDBACKDATE as \"lastfeedbackdate\",BCWS_COST as \"bcws_cost\",BSWS as \"bsws\", BSWP as \"bswp\",SV as \"sv\" ,RECEIVE_USER as  \"receive_user\" From V_PS_PlN_PlanOverallProgress PPPOP ");
                    stringBuilder.AppendFormat("Where   1=1  and  1=1  and (PPPOP.proj_guid='{0}' or LongCode like '{1}%')", EpsProjId, EpsProjLongCode);
                    format = stringBuilder.ToString();
                }

                DataTable value = DAL.QuerySQL(string.Format(format));
                Hashtable hashtable = new Hashtable();
                hashtable.Add("GetProjectSchedule", value);
                viewResultModel.data = hashtable;
                viewResultModel.success = true;
            }
            catch (Exception ex)
            {
                viewResultModel.success = false;
                viewResultModel.message = ex.Message;
            }
            finally
            {
                //dAL.Session.AutoClose();
            }

            return viewResultModel.ToJson();
        }


        [Action(Authorize = false)]
        public string SelA1taskproc()
        {
            ArrayList arrayListTwo = new ArrayList();

            string Sql_One = @"select * from pln_taskproc where proj_guid = '6fddf002-fc9d-494c-b615-1827796b27be' and 
                                plan_guid in ('a2458514-48eb-5111-5e1b-599a7d56c0a5') and 
                                proc_code like '001001001003003001%' and proc_name like '%环%' and proc_name not like '%负%' and proc_name <> '第0环' ";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayListTwo.Add(Dt_One);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayListTwo, s_setting);
        }

        [Action(Authorize = false)]
        public string SelA2taskproc()
        {
            ArrayList arrayListTwo = new ArrayList();

            string Sql_One = @"select * from pln_taskproc where proj_guid = '6fddf002-fc9d-494c-b615-1827796b27be' and 
                                plan_guid in ('3d82fc97-0ca9-3409-9e03-b11e565f5836') and 
                                proc_code like '001001002003003%' and proc_name like '%环%' and proc_name not like '%负%' and proc_name <> '第0环' ";

            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayListTwo.Add(Dt_One);

            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayListTwo, s_setting);
        }




        [Action(Authorize = false)]
        public string SeltaskZQ(string Year,string plan_guid,string proj_guid)
        {
            ArrayList arrayListTwo = new ArrayList();

            //string Sql_One = @"select
            //                     M.period_enddate,
            //                     Sum(M.BY_Plan_complete_pct) BY_Plan_complete_pct,
            //                     Sum(M.BY_act_complete_pct) BY_act_complete_pct,
            //                     Sum(M.plan_complete_pct) plan_complete_pct,
            //                     Sum(M.act_complete_pct) act_complete_pct
            //                    from
            //                    (
            //                     select
            //                      (isnull(a.BY_Plan_complete_pct, 0) * isnull(b.ListingPrice, 0) / 10000) BY_Plan_complete_pct,
            //                      (isnull(a.BY_act_complete_pct, 0) * isnull(b.ListingPrice, 0) / 10000) BY_act_complete_pct,
            //                      (isnull(a.plan_complete_pct, 0) * isnull(b.ListingPrice, 0) / 10000) plan_complete_pct,
            //                      (isnull(a.act_complete_pct, 0) * isnull(b.ListingPrice, 0) / 10000) act_complete_pct,
            //                      period_enddate
            //                     from PLN_taskZQ a

            //                     left join NPS_BOQ_Price b on a.task_guid = b.task_guid where 
            //                     a.period_enddate is not null and year(a.period_enddate) ='" + Year + "' and a.proj_guid = '" + proj_guid + "'and a.plan_guid = '"+ plan_guid + "'" +
            //                        "and period_enddate = (select top 1 x.period_enddate from PLN_taskZQ x where  " +
            //                        " year(x.period_enddate) = '"+ Year + "' and x.proj_guid = '"+ proj_guid + "' and  " +
            //                        " x.plan_guid = '"+ plan_guid + "' and format(x.period_enddate, 'yyyy-MM') = format(a.period_enddate, 'yyyy-MM') order by x.period_enddate desc) )M group by M.period_enddate ";


            string Sql_One = @"select * from  pln_SuperScreenRight where year(period_enddate)='"+ Year + "' and plan_guid='"+ plan_guid + "'";
            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayListTwo.Add(Dt_One);
            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayListTwo, s_setting);
        }


        [Action(Authorize = false)]
        public string SelSHQD(string plan_guid)
        {
            ArrayList arrayListTwo = new ArrayList();
            string Sql_One = @" select top 1 YSHQD,SHZQD from NPS_BOQ where FID  in (select proc_guid from PLN_taskproc where plan_guid='"+ plan_guid + "' and proj_guid = '6fddf002-fc9d-494c-b615-1827796b27be') ";
            DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(Sql_One);
            arrayListTwo.Add(Dt_One);
            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayListTwo, s_setting);
        }



        [Action(Authorize = false)]
        public string SelWBSToTask(string plan_guid,string wbs_short_name,string date)
        {
            SqlDataBase dbSQL = new SqlDataBase();
            ArrayList arrayListTwo = new ArrayList();
            string Sql_One = @"select
	XX.wbs_short_name,
	XX.wbs_name,
	XX.wbs_guid,
	XX.parent_wbs_guid,
	XX.lastyear_complete_pct,
	XX.BN_Plan_complete_pct,
	XX.BY_Plan_complete_pct,
	XX.BY_act_complete_pct,
	XX.BN_act_complete_pct,
	XX.act_complete_pct,
	Sum(XX.ConstProcess) ConstProcess,
	Sum(XX.TaskSumWorks) TaskSumWorks,
	Sum(XX.lastyear_complete_pctMoney) lastyear_complete_pctMoney,
	Sum(XX.lastyearcomplete_sum) lastyearcomplete_sum,
	Sum(XX.BN_Plan_complete_pctMoney) BN_Plan_complete_pctMoney,
	Sum(XX.BY_Plan_complete_pctMoney) BY_Plan_complete_pctMoney,
	Sum(XX.BY_act_complete_pctMoney) BY_act_complete_pctMoney,
	Sum(XX.BN_act_complete_pctMoney) BN_act_complete_pctMoney,
	Sum(XX.act_complete_pctMoney) act_complete_pctMoney
from
(
	select
		A.wbs_short_name,
		A.wbs_name,
		A.wbs_guid,
		A.parent_wbs_guid,
		A.lastyear_complete_pct,
		A.BN_Plan_complete_pct,
		A.BY_Plan_complete_pct,
		A.BY_act_complete_pct,
		A.BN_act_complete_pct,
		A.act_complete_pct,
		(isnull(B.ListingPrice, 0) / 10000) ConstProcess,
		isnull(B.TaskSumWorks, 0) TaskSumWorks,
		((isnull(B.ListingPrice, 0) / 10000) * isnull(B.lastyear_complete_pct, 0) * 0.01) lastyear_complete_pctMoney,
		(isnull(B.lastyear_complete_pct, 0) * isnull(B.TaskSumWorks, 0)) lastyearcomplete_sum,
		((isnull(B.ListingPrice, 0) / 10000) * isnull(B.BN_Plan_complete_pct, 0) * 0.01) BN_Plan_complete_pctMoney,
		((isnull(B.ListingPrice, 0) / 10000) * isnull(B.BY_Plan_complete_pct, 0) * 0.01) BY_Plan_complete_pctMoney,
		((isnull(B.ListingPrice, 0) / 10000) * isnull(B.BY_act_complete_pct, 0) * 0.01) BY_act_complete_pctMoney,
		((isnull(B.ListingPrice, 0) / 10000) * isnull(B.BN_act_complete_pct, 0) * 0.01) BN_act_complete_pctMoney,
		((isnull(B.ListingPrice, 0) / 10000) * isnull(B.act_complete_pct, 0) * 0.01) act_complete_pctMoney
	from PLN_PROJWBSZQ A

	left join 

	(
		select
			a.*,
			b.ListingPrice
		from PLN_taskZQ a

		left join NPS_BOQ_Price b on a.task_guid = b.task_guid
	)B on (B.task_code like (A.wbs_short_name + '0%') or A.wbs_short_name =@wbs_short_name) where A.proj_guid = '6fddf002-fc9d-494c-b615-1827796b27be' and 
	A.MasterId in (select mm.Id from PS_PLN_FeedBackRecord mm where mm.plan_guid = @plan_guid and mm.EpsProjId = '6fddf002-fc9d-494c-b615-1827796b27be' and mm.period_enddate = (select top 1 nn.period_enddate from PS_PLN_FeedBackRecord nn where plan_guid=@plan_guid and format(nn.period_enddate, 'yyyy-MM') = @date and exists(select 1 from PLN_taskZQ A where A.masterid = nn.id) order by nn.period_enddate desc)) and 
	A.wbs_short_name in (select x.Code from PB_BaseDataList x where BaseDataId = (select y.Id from PB_BaseData y where y.DataType = 'MonthReport_A1')) 
	and A.parent_wbs_guid != '00000000-0000-0000-0000-000000000000' and B.MasterId in (select mm.Id from PS_PLN_FeedBackRecord mm where mm.plan_guid=@plan_guid and mm.EpsProjId = '6fddf002-fc9d-494c-b615-1827796b27be' and mm.period_enddate = 
	(
		select top 1 nn.period_enddate from PS_PLN_FeedBackRecord nn where plan_guid=@plan_guid
		and format(nn.period_enddate, 'yyyy-MM'
	) = @date and exists(select 1 from PLN_taskZQ X where X.masterid = nn.id) order by nn.period_enddate desc))
)XX group by XX.wbs_short_name,XX.wbs_name,XX.wbs_guid,XX.parent_wbs_guid,XX.lastyear_complete_pct,XX.BN_Plan_complete_pct,XX.BY_Plan_complete_pct,XX.BY_act_complete_pct,XX.BN_act_complete_pct,XX.act_complete_pct order by XX.wbs_short_name";

            StringBuilder sInsZY = new StringBuilder();
            sInsZY.Clear();
            sInsZY.AppendLine(Sql_One.ToString());
            dbSQL.addParam("plan_guid", plan_guid);
            dbSQL.addParam("wbs_short_name", wbs_short_name);
            dbSQL.addParam("date", date);
            DataSet Dt_One = dbSQL.getDataSet(sInsZY.ToString());
            //DataTable Dt_One = XCode.DataAccessLayer.DAL.QuerySQL(sInsZY.ToString());
            arrayListTwo.Add(Dt_One);
            JsonSerializerSettings s_setting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(arrayListTwo, s_setting);
        }


    }
}

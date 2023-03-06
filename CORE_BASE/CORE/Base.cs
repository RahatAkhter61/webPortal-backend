using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using CORE_BASE.CONTEXT;
using CORE_MODELS;
//using Sy
namespace CORE_BASE.CORE
{
    public class Base<T> where T : class
    {
        //Shahzad Ismail 
        protected COREDBDataContext dc;
        public Base()
        {
            dc = new COREDBDataContext(CoreConnection.ConnectionString);
        }
        protected List<string> Errors = new List<string>();

        protected bool BulkState = false;
        protected T Current { get; set; }
        protected IList<T> CurrentBulk { get; set; }
        protected object PrimaryKeyValue = null;

        protected void New()
        {
            PrimaryKeyValue = null;
            Current = Activator.CreateInstance<T>();
            CurrentBulk = Activator.CreateInstance<List<T>>();
            BulkState = false;
        }

        protected IList<T> ExecuteCommand<T>(string procname, params object[] parameters) where T : class, new()
        {
            IList<T> lst = dc.ExecuteQuery<T>("Exec " + procname, parameters).ToList();
            return lst;
        }

        protected DataTable ExecuteSp(string spName, string[] ParameterName, params object[] parameters)
        {
            try
            {
                SqlConnection conn = new SqlConnection(CoreConnection.ConnectionString);
                SqlDataAdapter cmd = new SqlDataAdapter(spName, conn);
                cmd.SelectCommand.CommandText = spName;
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                int i = 0;
                foreach (var item in ParameterName)
                {
                    cmd.SelectCommand.Parameters.Add(new SqlParameter("@" + item, parameters[i]));
                    i++;
                }
                DataTable dt = new DataTable();
                conn.Open();
                cmd.Fill(dt);


                conn.Close();
                return dt;
            }
            catch (Exception Ex)
            {

                throw Ex;
                //MessageBox.Show(Ex.Message);
            }
        }
        protected DataTable ExecuteStoreProc(string spName, string[] ParameterName, string Connection, params object[] parameters)
        {
            try
            {
                SqlConnection conn = new SqlConnection(Connection);
                SqlDataAdapter cmd = new SqlDataAdapter(spName, conn);
                cmd.SelectCommand.CommandText = spName;
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                int i = 0;
                foreach (var item in ParameterName)
                {
                    cmd.SelectCommand.Parameters.Add(new SqlParameter("@" + item, parameters[i]));
                    i++;
                }
                DataTable dt = new DataTable();
                conn.Open();
                cmd.Fill(dt);


                conn.Close();
                return dt;
            }
            catch (Exception Ex)
            {

                throw Ex;
                //MessageBox.Show(Ex.Message);
            }
        }

        protected virtual bool IsValidBeforeSave()
        {
            return false;
        }


        protected void Save()
        {
            if (IsValidBeforeSave() == false)
            {

                if (PrimaryKeyValue == null)
                {
                    dc.GetTable<T>().InsertOnSubmit(Current);
                }
                dc.SubmitChanges();

            }
            else
                throw new Exception();


        }

        protected void SaveBulk()
        {
            BulkState = true;
            if (IsValidBeforeSave() == false)
            {
                if (PrimaryKeyValue == null)
                {
                    dc.GetTable<T>().InsertAllOnSubmit(CurrentBulk);
                }
                dc.SubmitChanges();
                BulkState = false;
            }
            else
                throw new Exception();
        }

        protected void Delete()
        {
            if (PrimaryKeyValue != null)
            {
                dc.GetTable<T>().DeleteOnSubmit(Current);
                dc.SubmitChanges();
            }
        }

        protected void DeleteBulk()
        {
            if (PrimaryKeyValue != null)
            {
                dc.GetTable<T>().DeleteAllOnSubmit(CurrentBulk);
                dc.SubmitChanges();
            }
        }

        private string PKAttribute { get; set; }
        protected void SetPKValue()
        {
            if (PKAttribute == null)
            {
                var map = dc.Mapping.GetTable(typeof(T)).RowType.DataMembers.SingleOrDefault(s => s.IsPrimaryKey);
                if (map != null)
                    PKAttribute = map.Name.ToString();
            }
            else
                PrimaryKeyValue = null;

        }

        protected void GetByField(Expression<Func<T, bool>> Condition)
        {
            Current = dc.GetTable<T>().Where(Condition).FirstOrDefault();
            this.SetPKValue();
            PrimaryKeyValue = Current != null ? Current.GetType().GetProperty(PKAttribute).GetValue(Current, null) : null;
        }

        protected void GetByPrimaryKey(object val)
        {
            this.SetPKValue();
            Current = dc.GetTable<T>().Where(PKAttribute + " = " + val).FirstOrDefault();
            if (Current != null)
                PrimaryKeyValue = Current.GetType().GetProperty(PKAttribute).GetValue(Current, null);
        }
        protected DTO SuccessResponse(object data)
        {
            return new DTO { isSuccessful = true, data = data };
        }

        protected DTO BadResponse(object error)
        {
            //return new DTO { IsSuccessful = false, Errors = ErrorList };

            return new DTO { isSuccessful = false, errors = error };
        }
    }


}

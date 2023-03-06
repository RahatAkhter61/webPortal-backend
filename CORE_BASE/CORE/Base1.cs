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
    public class Base<T> : IBaseRepository<T> where T : class
    {

        public COREDBDataContext dc;
        public Base()
        {
            //this.SecurityAccess(Assembly.GetCallingAssembly());
            dc = new COREDBDataContext(CoreConnection.ConnectionString);

        }
        private List<string> ErrorsList = new List<string>();

        private bool BulkStateValue = false;
        private object PrimaryKey = null;
        public List<string> Errors
        {
            get
            {
                return ErrorsList;
            }
            set
            {
                ErrorsList = value;
            }
        }

        public bool BulkState
        {
            get
            {
                return BulkStateValue;
            }
            set
            {
                BulkStateValue = value;
            }
        }

        public T Current { get; set; }


        public IList<T> CurrentBulk { get; set; }

        public object PrimaryKeyValue
        {
            get
            {
                return PrimaryKey;
            }
            set
            {
                PrimaryKey = value;
            }
        }

        public void New()
        {
            PrimaryKeyValue = null;
            Current = Activator.CreateInstance<T>();
            CurrentBulk = Activator.CreateInstance<List<T>>();
            BulkState = false;
        }

        //public IList<T> ExecuteCommand<T>(string procname, params object[] parameters) where T : class
        //{
        //    IList<T> lst = dc.ExecuteQuery<T>("Exec " + procname, parameters).ToList();
        //    return lst;
        //}


        public virtual bool IsValidBeforeSave()
        {
            return false;
        }

        public virtual void WithInSave()
        {

        }


        public void Save()
        {
            if (IsValidBeforeSave() == false)
            {
                //FillDefaultEntry();
                if (PrimaryKeyValue == null)
                {
                    WithInSave();

                    dc.GetTable<T>().InsertOnSubmit(Current);
                }
                dc.SubmitChanges();

            }
            else
                throw new Exception();


        }

        public void SaveBulk()
        {
            BulkState = true;
            if (IsValidBeforeSave() == false)
            {
                if (PrimaryKeyValue == null)
                {
                    WithInSave();
                    dc.GetTable<T>().InsertAllOnSubmit(CurrentBulk);
                }
                dc.SubmitChanges();
                BulkState = false;
            }
            else
                throw new Exception();
        }

        public void Delete()
        {
            if (PrimaryKeyValue != null)
            {
                dc.GetTable<T>().DeleteOnSubmit(Current);
                dc.SubmitChanges();
            }
        }

        public void DeleteBulk()
        {
            if (PrimaryKeyValue != null)
            {
                dc.GetTable<T>().DeleteAllOnSubmit(CurrentBulk);
                dc.SubmitChanges();
            }
        }

        public string PKAttribute { get; set; }
        public void SetPKValue()
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

        public void GetByField(Expression<Func<T, bool>> Condition)
        {
            Current = dc.GetTable<T>().Where(Condition).FirstOrDefault();
            this.SetPKValue();
            PrimaryKeyValue = Current != null ? Current.GetType().GetProperty(PKAttribute).GetValue(Current, null) : null;
        }

        public void GetByPrimaryKey(object val)
        {
            this.SetPKValue();
            Current = dc.GetTable<T>().Where(PKAttribute + " = " + val).FirstOrDefault();
            if (Current != null)
                PrimaryKeyValue = Current.GetType().GetProperty(PKAttribute).GetValue(Current, null);
        }
        //public DTO SuccessResponse(object data)
        //{
        //    return new DTO { IsSuccessful = true, Data = data };
        //}

        //public DTO BadResponse(object error)
        //{
        //    //return new DTO { IsSuccessful = false, Errors = ErrorList };

        //    return new DTO { IsSuccessful = false, Errors = error };
        //}


        public DTO SuccessResponse(object data)
        {
            return new DTO { isSuccessful = true, data = data };
        }

        public DTO BadResponse(object data)
        {
            return new DTO { isSuccessful = false, errors = data };
        }
    }
}
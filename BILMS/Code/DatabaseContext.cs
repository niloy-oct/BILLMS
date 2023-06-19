
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BILMS.Models;


namespace BILMS.Code
{


    public class DatabaseContext : DbContext
    {
        private string query;
        private DataTable _dt;
        public DatabaseContext() : base("LocalSqlServer")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180; // seconds
        }

      
        public DatabaseContext Query(string q)
        {

            this.query = q;
            return this;
        }

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryProduct> InventoryProducts { get; set; }
       

        public List<Dictionary<string, object>> ToDynamicList()
        {
            if (this.Database.Connection.State != ConnectionState.Open)
            { this.Database.Connection.Open(); }
            var cmd = this.Database.Connection.CreateCommand();
            cmd.CommandText = query;

            var reader = cmd.ExecuteReader();
            var rows = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var key = reader.GetName(i);
                    //var type = reader.GetType();
                    var value = reader.GetValue(i);
                    row.Add(key, value);
                }

                rows.Add(row);
                row = null;

            }

            reader.Close();

            try
            {
                if (this.Database.Connection.State == ConnectionState.Open)
                { this.Database.Connection.Close(); }
            }
            catch { }

            return rows;

        }

        public async Task<List<Dictionary<string, object>>> ToDynamicListAsync()
        {
            if (this.Database.Connection.State != ConnectionState.Open)
            { await this.Database.Connection.OpenAsync(); }

            var cmd = this.Database.Connection.CreateCommand();
            cmd.CommandText = query;

            var reader = await cmd.ExecuteReaderAsync();
            var rows = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var key = reader.GetName(i);
                    //var type = reader.GetType();
                    var value = reader.GetValue(i);
                    row.Add(key, value);
                }

                rows.Add(row);
                row = null;

            }

            reader.Close();
            try
            {
                if (this.Database.Connection.State == ConnectionState.Open)
                { this.Database.Connection.Close(); }
            }
            catch { }

            return rows;

        }

        public async Task<DataTable> ToDataTableAsync()
        {
            if (this.Database.Connection.State != ConnectionState.Open)
            { await this.Database.Connection.OpenAsync(); }

            var cmd = this.Database.Connection.CreateCommand();
            cmd.CommandText = query;

            var reader = await cmd.ExecuteReaderAsync();
            _dt = new DataTable();
            try
            {
                _dt.Load(reader);
                reader.Close();
            }
            catch { }



            try
            {
                if (this.Database.Connection.State == ConnectionState.Open)
                { this.Database.Connection.Close(); }
            }
            catch { }

            return _dt;

        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Entity<VmarketRegionZones>(
            //eb =>
            //{
            //    eb.HasNoKey();
            //});


            //  modelBuilder.Entity<MZone>().HasNoKey()

        }

    }
}

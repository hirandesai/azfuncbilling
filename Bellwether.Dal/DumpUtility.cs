using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Dal
{
	public class DumpUtility
	{
		private string connectionString;

		public DumpUtility(string ConnectionString)
		{
			connectionString = ConnectionString;
		}
		public void Insert<T>(List<T> Data)
		{
			var dataInTable = ToDataTable(Data);
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var transaction = connection.BeginTransaction();
				try
				{
					using (var sbCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
					{
						sbCopy.BulkCopyTimeout = 0;
						sbCopy.BatchSize = 5000;
						sbCopy.DestinationTableName = CspContext.GetTableName(typeof(T));
						GetColumnsMapping(sbCopy, typeof(T));
						sbCopy.WriteToServer(dataInTable);
					}
					transaction.Commit();
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
				finally
				{
					transaction.Dispose();
					connection.Close();
				}
			}
		}
		private DataTable ToDataTable<T>(IList<T> data)
		{
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
			DataTable table = new DataTable();
			for (int i = 0; i < props.Count; i++)
			{
				PropertyDescriptor prop = props[i];
				table.Columns.Add(prop.Name, prop.PropertyType);
			}
			object[] values = new object[props.Count];
			foreach (T item in data)
			{
				for (int i = 0; i < values.Length; i++)
				{
					values[i] = props[i].GetValue(item);
				}
				table.Rows.Add(values);
			}
			return table;
		}

		private void GetColumnsMapping(SqlBulkCopy sqlBulkCopy, Type tableType)
		{
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(tableType);
			foreach (PropertyDescriptor prop in props)
			{
				if (!prop.Name.Equals("Id"))
				{
					sqlBulkCopy.ColumnMappings.Add(prop.Name, prop.Name);
				}
			}
		}
	}
}

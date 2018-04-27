using Bellwether.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.UsageBilling
{
    public class DbInitializer
    {
		public static void init(string connectionString)
		{
			using (var context = new CspContext(connectionString))
			{
				context.Database.Initialize(false);
			}
		}
    }
}

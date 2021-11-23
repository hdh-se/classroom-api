using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Helpers
{
    public class AuditHelper
    {
        public static void CreateAudit<T>(T entity, string currentUser) where T: Audit
        {
            entity.CreateBy = currentUser;
            entity.CreateOn = DateTime.Now;
            entity.UpdateBy = currentUser;
            entity.UpdateOn = DateTime.Now;
        }
        
        public static void UpdateAudit<T>(T entity, string currentUser) where T: Audit
        {
            entity.UpdateBy = currentUser;
            entity.UpdateOn = DateTime.Now;
        }

        
    }
}

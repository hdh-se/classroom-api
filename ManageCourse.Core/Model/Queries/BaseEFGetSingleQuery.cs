using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Queries
{
    public class BaseEFGetSingleQuery<TEntity>
    {
        private readonly ICollection<Tuple<string, object>> queryValues = new List<Tuple<string, object>>();

        public virtual List<Expression<Func<TEntity, object>>> GetIncludeExpressions()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        public virtual ExpressionStarter<TEntity> GetQueryConditions()
        {
            return PredicateBuilder.New<TEntity>(true);
        }

        public Tuple<string, object>[] GetQueryValues()
        {
            return queryValues.ToArray();
        }

        protected void AppendQueryValue(string columnName, object value)
        {
            queryValues.Add(new Tuple<string, object>(columnName, value));
        }
    }
}

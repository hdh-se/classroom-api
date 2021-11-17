using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Queries
{
    public abstract class BaseEFQuery<TEntity>
    {
        public bool IncludeCount { get; set; }
        public int? StartAt { get; set; }
        public int? MaxResults { get; set; }
        public string SortColumn { get; set; }
        public string Includes { get; set; }

        public virtual List<Expression<Func<TEntity, object>>> GetIncludeExpressions()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        public virtual List<string> GetIncludeNavigationPaths()
        {
            return Includes == null ? new List<string>() : Includes.Split(",").Select(x => x.Trim()).ToList();
        }

        public virtual ExpressionStarter<TEntity> GetQueryConditions()
        {
            return PredicateBuilder.New<TEntity>(true);
        }

        public virtual Dictionary<string, Expression<Func<TEntity, object>>> GetSortColumnMappings()
        {
            return new Dictionary<string, Expression<Func<TEntity, object>>>();
        }
    }
    public class JoinedEntity<TEntity1, TEntity2>
    {
        public TEntity1 Entity1 { get; set; }
        public TEntity2 Entity2 { get; set; }
    }
}

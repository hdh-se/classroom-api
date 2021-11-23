using ManageCourse.Core.Data;
using ManageCourse.Core.Data.Common;
using ManageCourse.Core.Exceptions;
using ManageCourse.Core.Model.Queries;
using ManageCourse.Core.Repositories;
using ManageCourseAPI.Model.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Controllers.Common
{
    public class ApiControllerBase : ControllerBase
    {
        protected IGeneralModelRepository GeneralModelRepository { get; }
        protected DbContextContainer DbContextContainer { get; }

        public ApiControllerBase(IGeneralModelRepository generalModelRepository, DbContextContainer dbContextContainer)
        {
            GeneralModelRepository = generalModelRepository;
            DbContextContainer = dbContextContainer;
        }

        protected async Task<GeneralResultResponse<TResponse>> GetSearchResult<TEntity, TResponse>(
            BaseEFQuery<TEntity> query,
            Func<TEntity, TResponse> converter)
            where TEntity : class
        {
            var context = DbContextContainer.GetContextFor<TEntity>();
            var data = await context.QueryByAsync(query)
                .ConfigureAwait(false);
            var total = await context.CountByAsync(query)
                .ConfigureAwait(false);
            return
                new GeneralResultResponse<TResponse>(total > query.StartAt + query.MaxResults, total,
                    data.Select(converter).ToList());
        }
    }
}


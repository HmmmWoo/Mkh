using System;
using System.Linq.Expressions;
using Mkh.Data.Abstractions.Entities;
using Mkh.Data.Abstractions.Logger;
using Mkh.Data.Abstractions.Pagination;
using Mkh.Data.Abstractions.Queryable.Grouping;
using Mkh.Data.Core.SqlBuilder;

namespace Mkh.Data.Core.Queryable.Grouping
{
    internal class GroupingQueryable<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14> : GroupingQueryable, IGroupingQueryable<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14>
        where TEntity : IEntity, new()
        where TEntity2 : IEntity, new()
        where TEntity3 : IEntity, new()
        where TEntity4 : IEntity, new()
        where TEntity5 : IEntity, new()
        where TEntity6 : IEntity, new()
        where TEntity7 : IEntity, new()
        where TEntity8 : IEntity, new()
        where TEntity9 : IEntity, new()
        where TEntity10 : IEntity, new()
        where TEntity11 : IEntity, new()
        where TEntity12 : IEntity, new()
        where TEntity13 : IEntity, new()
        where TEntity14 : IEntity, new()
    {
        public GroupingQueryable(QueryableSqlBuilder sqlBuilder, DbLogger logger, Expression expression) : base(sqlBuilder, logger, expression)
        {
        }

        public IGroupingQueryable<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14> Having(Expression<Func<IGrouping<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14>, bool>> expression)
        {
            _queryBody.SetHaving(expression);
            return this;
        }

        public IGroupingQueryable<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14> Having(string havingSql)
        {
            _queryBody.SetHaving(havingSql);
            return this;
        }

        public IGroupingQueryable<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14> OrderBy(string field)
        {
            _queryBody.SetSort(field, SortType.Asc);
            return this;
        }

        public IGroupingQueryable<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14> OrderByDescending(string field)
        {
            _queryBody.SetSort(field, SortType.Desc);
            return this;
        }

        public IGroupingQueryable<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14> OrderBy<TResult>(Expression<Func<IGrouping<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14>, TResult>> expression)
        {
            _queryBody.SetSort(expression, SortType.Asc);
            return this;
        }

        public IGroupingQueryable<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14> OrderByDescending<TResult>(Expression<Func<IGrouping<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14>, TResult>> expression)
        {
            _queryBody.SetSort(expression, SortType.Desc);
            return this;
        }

        public IGroupingQueryable<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14> Select<TResult>(Expression<Func<IGrouping<TKey, TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10, TEntity11, TEntity12, TEntity13, TEntity14>, TResult>> expression)
        {
            _queryBody.SetSelect(expression);
            return this;
        }
    }
}

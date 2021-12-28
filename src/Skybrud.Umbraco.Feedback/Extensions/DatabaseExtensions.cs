using NPoco;
using Skybrud.Essentials.Collections;
using Skybrud.Umbraco.Feedback.Services;
using System;
using System.Linq.Expressions;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Feedback.Extensions {

    public static class DatabaseExtensions {

        public static T[] Page<T>(this IUmbracoDatabase database, int page, int itemsPerPage, Sql sql, out int total) {

            var result = database.Page<T>(page, itemsPerPage, sql);

            total = (int)result.TotalItems;

            return result.Items.ToArray();

        }

        public static Sql<ISqlContext> OrderBy<TDto>(this Sql<ISqlContext> sql, Expression<Func<TDto, object>> field, SortOrder sortOrder) {
            return sortOrder == SortOrder.Descending ? sql.OrderByDescending<TDto>(field) : sql.OrderBy<TDto>(field);
        }

        public static Sql<ISqlContext> OrderBy<TDto>(this Sql<ISqlContext> sql, Expression<Func<TDto, object>> field, EntriesSortOrder sortOrder) {
            return sortOrder == EntriesSortOrder.Desc ? sql.OrderByDescending<TDto>(field) : sql.OrderBy<TDto>(field);
        }

    }

}
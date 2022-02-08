using NPoco;
using Skybrud.Essentials.Collections;
using Skybrud.Umbraco.Feedback.Services;
using System;
using System.Linq.Expressions;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Feedback.Extensions {

    /// <summary>
    /// Static class with various database related extension methods.
    /// </summary>
    public static class DatabaseExtensions {

        /// <summary>
        /// Returns a paginated result based on the specified <paramref name="sql"/> command.
        /// </summary>
        /// <typeparam name="T">The type of the DTO.</typeparam>
        /// <param name="database">A reference to the current Umbraco database.</param>
        /// <param name="page">The page to be returned.</param>
        /// <param name="itemsPerPage">The maximum amount of items per page.</param>
        /// <param name="sql">The SQL command to be executed.</param>
        /// <param name="total">When this method returns, holds the total amount of rows matching the SQL command.</param>
        /// <returns>An array of <typeparamref name="T"/> with the paginated results.</returns>
        public static T[] Page<T>(this IUmbracoDatabase database, int page, int itemsPerPage, Sql sql, out int total) {

            Page<T> result = database.Page<T>(page, itemsPerPage, sql);

            total = (int) result.TotalItems;

            return result.Items.ToArray();

        }

        /// <summary>
        /// Appends an ORDER clause to the specified <paramref name="sql"/> command.
        /// </summary>
        /// <typeparam name="TDto">The type of the DTO.</typeparam>
        /// <param name="sql">The SQL command.</param>
        /// <param name="field">An expression indicating the sorting should be based on.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>An instance of <see cref="Sql{ISqlContext}"/> representing the updated SQL command.</returns>
        public static Sql<ISqlContext> OrderBy<TDto>(this Sql<ISqlContext> sql, Expression<Func<TDto, object>> field, SortOrder sortOrder) {
            return sortOrder == SortOrder.Descending ? sql.OrderByDescending(field) : sql.OrderBy(field);
        }
        
        /// <summary>
        /// Appends an ORDER clause to the specified <paramref name="sql"/> command.
        /// </summary>
        /// <typeparam name="TDto">The type of the DTO.</typeparam>
        /// <param name="sql">The SQL command.</param>
        /// <param name="field">An expression indicating the sorting should be based on.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>An instance of <see cref="Sql{ISqlContext}"/> representing the updated SQL command.</returns>
        public static Sql<ISqlContext> OrderBy<TDto>(this Sql<ISqlContext> sql, Expression<Func<TDto, object>> field, EntriesSortOrder sortOrder) {
            return sortOrder == EntriesSortOrder.Desc ? sql.OrderByDescending(field) : sql.OrderBy(field);
        }

    }

}
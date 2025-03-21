﻿using System.Linq.Expressions;

namespace ClickFlow.DAL.Queries
{
    public class QueryOptions<T>
	{
		public Expression<Func<T, bool>>? Predicate { get; set; }
		public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }
		public bool Tracked { get; set; } = true;
        public List<Expression<Func<T, object>>> IncludeProperties { get; set; } = new List<Expression<Func<T, object>>>();
        public List<Expression<Func<object, object>>> ThenIncludeProperties { get; set; } = new List<Expression<Func<object, object>>>();

    }
}

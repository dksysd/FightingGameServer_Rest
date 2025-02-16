using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FightingGameServer_Rest.Repositories;

[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class Repository<T>(GameDbContext context) : IRepository<T>
    where T : class
{
    protected readonly GameDbContext Context = context;

    public virtual async Task<T?> CreateAsync(T entity)
    {
        EntityEntry<T> createdEntity = await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
        return createdEntity.Entity;
    }

    public virtual async Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
    {
        IQueryable<T> query = Context.Set<T>();
        if (includeFunc != null)
        {
            query = includeFunc(query);
        }

        // EF.Property 대신 람다 식과 Expression을 사용하여 동적으로 Where 조건 생성**
        IEntityType entityType = Context.Model.FindEntityType(typeof(T)) ??
                                 throw new InvalidOperationException(
                                     $"Can't find entity type of {typeof(T).Name}"); // 엔티티 타입 메타데이터 가져오기
        IKey primaryKey = entityType.FindPrimaryKey() ??
                          throw new InvalidOperationException(
                              $"Can't find primary key of {entityType.Name}"); // 기본 키 정보 가져오기
        string primaryKeyName = primaryKey.Properties[0].Name; // 기본 키 속성 이름 가져오기 (복합키가 아닌 경우 가정)

        ParameterExpression parameter = Expression.Parameter(typeof(T), "e"); // e => ... 람다식 파라미터 생성
        MemberExpression property = Expression.Property(parameter, primaryKeyName); // e.PrimaryKeyName 속성 접근
        ConstantExpression constant = Expression.Constant(id); // id 상수 값 생성
        BinaryExpression equal = Expression.Equal(property, constant); // e.PrimaryKeyName == id 조건 생성
        Expression<Func<T, bool>>
            predicate = Expression.Lambda<Func<T, bool>>(equal, parameter); // Func<TEntity, bool> 람다 식 생성

        return await query.FirstOrDefaultAsync(predicate); // 동적으로 생성된 predicate를 사용하여 조회
    }

    public virtual async Task<List<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
    {
        IQueryable<T> query = Context.Set<T>(); // 기본 쿼리 시작

        if (includeFunc != null) // includeFunc가 null이 아니면
        {
            query = includeFunc(query); // queryBuilder 함수를 적용하여 쿼리 수정 (Include 등)
        }

        return await query.ToListAsync();
    }

    public virtual async Task<T> UpdateAsync(int id, T entity)
    {
        T? existingEntity = await Context.Set<T>().FindAsync(id);
        if (existingEntity == null)
        {
            throw new InvalidOperationException("Can't find entity");
        }
        Context.Entry(existingEntity).CurrentValues.SetValues(entity);
        await Context.SaveChangesAsync();
        return existingEntity;
    }

    public virtual async Task<T> DeleteAsync(int id, T entity)
    {
        T? existingEntity = await Context.Set<T>().FindAsync(id);
        if (existingEntity == null)
        {
            throw new InvalidOperationException("Can't find entity");
        }
        Context.Set<T>().Remove(existingEntity);
        await Context.SaveChangesAsync();
        return existingEntity;
    }
}